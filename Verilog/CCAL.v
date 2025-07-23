`timescale 1ns / 1ps
//本代码来自：github.com/becomequantum/Kryon

//本代码经过少量仿真测试是没有问题的，并未经过大量实际测试，仅供学习研究使用。
`define DL 3  //把使能信号Delay几个周期，这要求两行使能信号间有足够的周期间隔

module CCAL #(         //Connected Component Analysis-Labeling
  parameter Wb = 11,   //2^Wb（bitWidth） = 图像宽度，11适合宽小于2048的图像
  parameter Hb = 10,   //2^Hb = 图像高度。对于没有帧概念的线扫图像输入，这个值设的要大于待扫描连通域可能出现的最大高度
  parameter Nb = 10    //连通域标号的位宽，即存储连通域信息表Ram的地址位宽
  )(
  input  clk,
  input  SRST,         //没有复位信号FIFO在ISE里仿真不正确，但FIFO又可以设置成无复位信号，按理没有复位信号也应该是可以用的      
  input  DataEn,
  input  PixelData,    //背景黑：0，前景白：1
  output OEn,
  output [89 - 3 * Wb - 2 * Hb - Nb - 1:0] Count, //连通域总点数
  output [Wb - 1:0] XMin, 
  output [Wb - 1:0] XMax,
  output [Hb - 1:0] YMin,
  output [Hb - 1:0] YMax,
  output [Nb - 1:0] RealN
  );
  reg  PrePixel, PreCombCur = 0;
  reg  [2     :0] XStartEn = 0;
  reg  [`DL   :0] DataEn_;
  reg  [Wb - 1:0] x, XStart = 0, XEnd = 0;  //x类似于for循环里的循环变量。XStart用于缓存单行连通域的起点坐标
  reg  [Hb - 1:0] y = 0;          //y要赋初值，在FPGA里这样赋初值是有效的
  reg  [5     :0] PreState = 6'b1, NextPreState;
  reg  [2     :0] CurState = 3'b1, NextCurState;					 
  reg  [Nb - 1:0] N = 0, InheritN = 0;          //连通域标号
  wire [89 - 3 * Wb - 2 * Hb - Nb - 1:0] oaCount; 
  wire [Wb - 1:0] oaXMin, oaXMax, oaBottomLineXMax;
  wire [Hb - 1:0] oaYMin, oaYMax;
  wire [Nb - 1:0] oaRealN;
  
  always@(posedge clk)begin
    PrePixel <= DataEn ? PixelData : 1'b0;
	 x        <= (DataEn || DataEn_[`DL])? x + 1     : 0;
	 DataEn_  <= {DataEn_[`DL - 1:0], DataEn};
	 
	 if(Start)          
	    XStart <= x;
    else if(EnEnd)
	    XStart <= 0;           //行尾XStart恢复为最大值，会用这个值判断哪里有连通域 
    
	 if(End)
	   XEnd <= x;
	 else if(EnEnd)
	   XEnd <= 0;
		 
    if(Start) 
	   XStartEn[0] <= 1;
	 else if(End) 
	   XStartEn[0] <= 0;
	
	 XStartEn[2:1] <= XStartEn[1:0];
	 
	 if(EnEnd)  y <= y + 1'b1; //每行结尾y加1，如果有帧有效信号，y要在帧有效信号升起时清零
	 
	 if(PreCombCur) //前一行并当前行时记下标号
	   InheritN <= RealN; 
	 
	 PreState <= NextPreState;
	 CurState <= NextCurState;
  end
  
  wire Start = PixelData && !PrePixel,   //单行连通域的开始和结束
       End   = !PixelData && PrePixel,
		 EnEnd = !DataEn_[`DL - 1] && DataEn_[`DL];         //行有效信号结束
		 
  //处理上一行连通域信息的状态机
  parameter PreIDLE = 1,       //One Hot独热编码
            PreDONE = 1 << 1,  //没有与之连通的则需判断该连通域是否已经完结，如果完结则输出它的统计结果
				PreLOOK = 1 << 2,  //查iCCList连通域统计表，要一查到底			
				PreCCAL = 1 << 3,  //上一行没有被合并，合并所有下一行的状态。上并下。
				PreREAD = 1 << 4,  //上一行被读出之后要等一个周期让新的上一行标号去CCList里查出数据
				PreWAIT = 1 << 5;  //被当前行合并后还没完事
  
  wire [2:0] yDiff = y[2:0] - {1'b0, PreY};
  reg NotC[1:0];
  always@(posedge clk) begin
    NotC[0] = XStart > PreXEnd + 1;
	 NotC[1] = NotC[0];
  end
 
       //下一行为空时判断确定不会再有连通的了，或者都到下下一行了，自然不会再有连通的了。
  wire EmptyAndNotC = CurEmpty && (yDiff[1:0] > 2'b1 || ( 
                      yDiff[1:0] == 2'b1 && x > PreXEnd + 4 && ( (XStartEn[2] &&  NotC[1]) || !XStartEn[2] ) ));
  wire NotEmptyNotC = !CurEmpty && LeftNotC; //非空但不连通。下一行当前最靠左的数据在右边不连通，意味着上一行这个在下一行没有与之连通的了。
  wire NotEmptyDone = !CurEmpty && PreDoneC; //非空、连通，但已完事
                                                               //下并上
  wire [5:0]nextPre = RealNEn ? PreLOOK : (PreDoneC ? PreREAD : (CurState[2] ? PreWAIT : PreCCAL)); //RealNEn为1代表有指向，这时就需要继续查表
  
  always@(*)begin
    case(PreState)
	   PreIDLE:
	     if(!PreEmpty && (yDiff[1:0] != 2'b0)) begin  //上一行FIFO在当前行就会非空，但对它们的判断要到下一行才会开始。
		    if((DataEn_[0] || DataEn_[`DL]) && EmptyAndNotC || NotEmptyNotC)  //无连通完事             
		      NextPreState = PreDONE;
		    else if(!CurEmpty && Connect) begin
			   NextPreState = nextPre;
				PreCombCur = RealNEn || CurState[2] ? 0 : 1;
			 end
		    else begin
		      NextPreState = PreIDLE;
				PreCombCur = 0;
			 end
		  end
		  else //else要写，否则在ISE里仿真有问题
		    NextPreState = PreIDLE;
			 
		PreLOOK: begin
		  NextPreState = nextPre;         //一查到底。上一行完事就转读出，还可能继续有下面的连通域就继续查看
		  PreCombCur = RealNEn || CurState[2] ? 0 : 1;
		end
		
		PreCCAL:begin
		  PreCombCur = !CurEmpty ? 1 : 0; //上一行合并下面一行新的、还没标号的连通域。
		  
        if(NotEmptyDone || NotEmptyNotC || EmptyAndNotC)//非空连通但完事、非空但不连通、空但已不可能有连通
          NextPreState = PreREAD;
        else
          NextPreState = PreCCAL;
      end	

      PreWAIT: begin                    //被当前行合并后还没完事
         NextPreState = PreIDLE;		
			PreCombCur = 0;
		end
	   default: begin
		  NextPreState = PreIDLE;
		  PreCombCur = 0;
		end
	 endcase
  end
  
  wire LeftNotC  = PreXEnd + 1 < CurXStart;         //不连通，且上一行的这个连通域在左边
  wire RightNotC = CurXEnd + 1 < PreXStart;         //不连通，且上一行的这个连通域在右边
  wire Connect   = !LeftNotC && !RightNotC;         //两种不连通的情况都不是那就是连通
  wire PreDoneC  = Connect && (CurXEnd >= PreXEnd); //上一行不可能再有下一行右边其它连通域了,完事了
  wire CurDoneC  = Connect && (PreXEnd >= CurXEnd); //下一行不可能再有上一行右边其它连通域了
  
  
  
  //处理当前行连通域信息的状态机
  parameter CurIDLE = 1,  
            CurNEW  = 1 << 1,    //上一行是空行或没有与之相连通的就进入新建状态
	         CurCCAL = 1 << 2;    //下并上
				
  wire PreEmptyOrNotC = PreEmpty ||                       //上一行是空的
			            (!PreEmpty && PreY == CurY[1:0]) ||  //非空，但数据是这一行刚写进去的，这也意味着上一行是空的   
                     (!PreEmpty && RightNotC);            //如果之前有连通会进入连通状态，在IDLE状态发现这种情况意味着前后都无连通	 
  //下并上完事：     上一行为空或无连通 或 有连通但完事了
  wire CurCCALDone = PreEmptyOrNotC || (NextPreState[5:4] && CurDoneC) ;
  
  always@(*)begin
    case(CurState)	 
	   CurIDLE:
		  if(!CurEmpty) begin
		    if(PreEmptyOrNotC)			                   //上一行是空的或没有连通的就进入新建连通域状态
			   NextCurState = CurNEW;                     //CurNEW状态里会写入一个新建的连通域信息到CCList里，然后标号N会加1
			 else if(/*!PreEmpty && Connect &&*/ !CurDoneC && NextPreState[4])   //连通且完事的状态Pre状态机已处理了。上一行完事了，当前行还没完事
			   NextCurState = CurCCAL;
			 else
			   NextCurState = CurIDLE;
		  end
		  else
		    NextCurState = CurIDLE;
			 
		CurCCAL:                                         //上一行有连通就合并上一行
		  if(CurCCALDone)
		    NextCurState = CurIDLE;
		  else
		    NextCurState = CurCCAL;	   
		default: NextCurState = CurIDLE;
	 endcase
  end 
		 
  wire [Wb - 1:0] CurXStart, CurXEnd, PreXStart, PreXEnd, BottomLineXMax;
  wire [Nb - 1:0] CurN, PreN; //连通域标号
  wire [Hb - 1:0] CurY;
                   //当前行新建连通域；  被上一行合并且完事；           留下合并上一行、但已无连通，或有连通但完事了
  wire PreWrCurRd = NextCurState[1] || (PreCombCur && CurDoneC) || (CurState[2] && CurCCALDone);    //1：CurNEW 2：CurCCAL;

  //First-Word Fall-Fhrough模式，相同时钟。FiFo宽度要根据前面设的参数来定，深度要根据实际图像情况来设定，理论上应该大于一行中最多能出现多少个连通域。
  //这种配置的FIFO数据在写入后两个周期会出现在dout上，同时empty信号拉低。无需升起rd_en就能瞥见数据。
  FIFO36x512 iCurrentLine(//当前行
    .clk(clk), 
	 .srst(SRST), 
    .din({4'b0, y, XStart, x - 1'b1}), //x - 1就是XEnd, 单行连通域的结束坐标，这里减1要写成1'b1，写x - 1结果会被当成32位！
    .wr_en(End),                       //单行连通域结束时信息被写入FIFO。此时写入的信息里是没有连通域标号的
    .rd_en(PreWrCurRd),                //当前行读一个的时候上一行就要写一个
    .dout({CurY, CurXStart, CurXEnd}), // output [35 : 0] dout
    .full(), 
    .empty(CurEmpty) 
  );    
  
  
  wire [1:0] PreY;
  wire CCDone = ~RealNEn && (PreXEnd == BottomLineXMax) && (y != YMax);//连通域完结判断信号,"无限次元"C#代码里有更详细的注释。
  assign OEn = CCDone && NextPreState[1]; //上一行无连通完事中包含了连通域完结这种情况
               //上一行无连通完事和有连通完事读出
  wire PreRd = NextPreState[1] || NextPreState[4];       //1:PreDONE 4:READ。5 WAIT状态下上一行还没完事，不读出
  assign CurN =  NextCurState[1] ? N : (PreWrCurRd && PreCombCur ? RealN : InheritN);
  
  FIFO36x512 iPreviousLine(//上一行。当前行里的单行连通域信息会在检查和上一行的连通情况、获取标号之后写入上一行FIFO中
    .clk(clk), 
    .srst(SRST), 
    .din({0, CurY[1:0], CurN, CurXStart, CurXEnd}), // input [35 : 0] din
    .wr_en(PreWrCurRd), 
    .rd_en(PreRd), 
    .dout({PreY, PreN, PreXStart, PreXEnd}), // output [35 : 0] dout
    .full(), 
    .empty(PreEmpty) 
  );
  //2：CurCCAL。 当前行合并上一行的写信号。4：上一行被合并且上一行完事。5：上一行被合并且没完事
  wire CurCombPre = CurState[2] &&  NextPreState[5:4] && (InheritN != RealN);  
  wire CCwea = NextCurState[1] || CurCombPre;//1：CurNEW State 新建一个连通域信息时iCCList的写信号
               
  always@(posedge clk)
    if(NextCurState[1]) N <= N + 1'b1;    //标号只会在新建一个连通域信息后加1
  
  wire [89    :0] CCdina  = NextCurState[1] ? {CurCount, CurXStart, CurXEnd, CurY, CurY, CurXEnd, N, 1'b0} 
                                            : {iaCount, iaXMin, iaXMax, iaYMin, iaYMax, oaBottomLineXMax, oaRealN, oaRealNEn};
  wire [Nb - 1:0] CCaddra = NextCurState[1] ? N : InheritN; //新建:下并上
  wire [89    :0] CCdinb = CurCombPre ? {79'b0,oaRealN,1'b1}
                                      : {bCount, bXMin, bXMax, bYMin, bYMax, bBottomLineXMax, RealN, RealNEn};
  
  //合并新的一行
  wire [89 - 3 * Wb - 2 * Hb - Nb - 1:0] CurCount = CurXEnd - CurXStart + 1'b1;
  wire [89 - 3 * Wb - 2 * Hb - Nb - 1:0] bCount = Count + CurCount;
  wire [Wb - 1:0] bXMin = CurXStart < XMin ? CurXStart : XMin;
  wire [Wb - 1:0] bXMax = CurXEnd > XMax ? CurXEnd : XMax;
  wire [Wb - 1:0] bBottomLineXMax = CurXEnd;
  wire [Hb - 1:0] bYMin = YMin;
  wire [Hb - 1:0] bYMax = CurY;
  wire CCweb = PreCombCur || CurCombPre;            //上一行合并下面新一行的写使能信号
  wire [Nb - 1:0] CCaddrb = NextPreState[3:2] || CCweb ? RealN : PreN;              //上并下，下并上、查表和CCAL状态地址等于RealN 2:Look。下并上
  
  //合并另一个连通域信息。此时b端口的输出是上一行的连通域，是被合并掉的。
  wire [89 - 3 * Wb - 2 * Hb - Nb - 1:0] iaCount = oaCount + Count;
  wire [Wb - 1:0] iaXMin = XMin < oaXMin ? XMin : oaXMin;
  wire [Wb - 1:0] iaXMax = XMax > oaXMax ? XMax : oaXMax;
  wire [Hb - 1:0] iaYMin = YMin < oaYMin ? YMin : oaYMin;
  wire [Hb - 1:0] iaYMax = YMax > oaYMax ? YMax : oaYMax ;
  
  //Count总点数, XMin, XMax, YMin, YMax, BottomLineXMax, RealN, RealNEn为1代表RealN有效，信息被合并过，已指向别的编号
  BlockRam90x1024 iCCList( //连通域统计表
    .clka(clk), 
    .wea(CCwea),                            //新建，下并上
    .addra(CCaddra), // input [9 : 0] addra
    .dina(CCdina), // input [89 : 0] dina
    .douta({oaCount, oaXMin, oaXMax, oaYMin, oaYMax, oaBottomLineXMax, oaRealN, oaRealNEn}), // output [89 : 0] douta
    .clkb(clk), 
    .web(CCweb),                            //上并下，下并上时改写上的RealN
    .addrb(CCaddrb), 
    .dinb(CCdinb), 
    .doutb({Count, XMin, XMax, YMin, YMax, BottomLineXMax, RealN, RealNEn})
  );
																									  
endmodule
