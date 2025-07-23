`timescale 1ns / 1ps
//���������ԣ�github.com/becomequantum/Kryon

//�����뾭���������������û������ģ���δ��������ʵ�ʲ��ԣ�����ѧϰ�о�ʹ�á�
`define DL 3  //��ʹ���ź�Delay�������ڣ���Ҫ������ʹ���źż����㹻�����ڼ��

module CCAL #(         //Connected Component Analysis-Labeling
  parameter Wb = 11,   //2^Wb��bitWidth�� = ͼ���ȣ�11�ʺϿ�С��2048��ͼ��
  parameter Hb = 10,   //2^Hb = ͼ��߶ȡ�����û��֡�������ɨͼ�����룬���ֵ���Ҫ���ڴ�ɨ����ͨ����ܳ��ֵ����߶�
  parameter Nb = 10    //��ͨ���ŵ�λ�����洢��ͨ����Ϣ��Ram�ĵ�ַλ��
  )(
  input  clk,
  input  SRST,         //û�и�λ�ź�FIFO��ISE����治��ȷ����FIFO�ֿ������ó��޸�λ�źţ�����û�и�λ�ź�ҲӦ���ǿ����õ�      
  input  DataEn,
  input  PixelData,    //�����ڣ�0��ǰ���ף�1
  output OEn,
  output [89 - 3 * Wb - 2 * Hb - Nb - 1:0] Count, //��ͨ���ܵ���
  output [Wb - 1:0] XMin, 
  output [Wb - 1:0] XMax,
  output [Hb - 1:0] YMin,
  output [Hb - 1:0] YMax,
  output [Nb - 1:0] RealN
  );
  reg  PrePixel, PreCombCur = 0;
  reg  [2     :0] XStartEn = 0;
  reg  [`DL   :0] DataEn_;
  reg  [Wb - 1:0] x, XStart = 0, XEnd = 0;  //x������forѭ�����ѭ��������XStart���ڻ��浥����ͨ����������
  reg  [Hb - 1:0] y = 0;          //yҪ����ֵ����FPGA����������ֵ����Ч��
  reg  [5     :0] PreState = 6'b1, NextPreState;
  reg  [2     :0] CurState = 3'b1, NextCurState;					 
  reg  [Nb - 1:0] N = 0, InheritN = 0;          //��ͨ����
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
	    XStart <= 0;           //��βXStart�ָ�Ϊ���ֵ���������ֵ�ж���������ͨ�� 
    
	 if(End)
	   XEnd <= x;
	 else if(EnEnd)
	   XEnd <= 0;
		 
    if(Start) 
	   XStartEn[0] <= 1;
	 else if(End) 
	   XStartEn[0] <= 0;
	
	 XStartEn[2:1] <= XStartEn[1:0];
	 
	 if(EnEnd)  y <= y + 1'b1; //ÿ�н�βy��1�������֡��Ч�źţ�yҪ��֡��Ч�ź�����ʱ����
	 
	 if(PreCombCur) //ǰһ�в���ǰ��ʱ���±��
	   InheritN <= RealN; 
	 
	 PreState <= NextPreState;
	 CurState <= NextCurState;
  end
  
  wire Start = PixelData && !PrePixel,   //������ͨ��Ŀ�ʼ�ͽ���
       End   = !PixelData && PrePixel,
		 EnEnd = !DataEn_[`DL - 1] && DataEn_[`DL];         //����Ч�źŽ���
		 
  //������һ����ͨ����Ϣ��״̬��
  parameter PreIDLE = 1,       //One Hot���ȱ���
            PreDONE = 1 << 1,  //û����֮��ͨ�������жϸ���ͨ���Ƿ��Ѿ���ᣬ���������������ͳ�ƽ��
				PreLOOK = 1 << 2,  //��iCCList��ͨ��ͳ�Ʊ�Ҫһ�鵽��			
				PreCCAL = 1 << 3,  //��һ��û�б��ϲ����ϲ�������һ�е�״̬���ϲ��¡�
				PreREAD = 1 << 4,  //��һ�б�����֮��Ҫ��һ���������µ���һ�б��ȥCCList��������
				PreWAIT = 1 << 5;  //����ǰ�кϲ���û����
  
  wire [2:0] yDiff = y[2:0] - {1'b0, PreY};
  reg NotC[1:0];
  always@(posedge clk) begin
    NotC[0] = XStart > PreXEnd + 1;
	 NotC[1] = NotC[0];
  end
 
       //��һ��Ϊ��ʱ�ж�ȷ������������ͨ���ˣ����߶�������һ���ˣ���Ȼ����������ͨ���ˡ�
  wire EmptyAndNotC = CurEmpty && (yDiff[1:0] > 2'b1 || ( 
                      yDiff[1:0] == 2'b1 && x > PreXEnd + 4 && ( (XStartEn[2] &&  NotC[1]) || !XStartEn[2] ) ));
  wire NotEmptyNotC = !CurEmpty && LeftNotC; //�ǿյ�����ͨ����һ�е�ǰ�����������ұ߲���ͨ����ζ����һ���������һ��û����֮��ͨ���ˡ�
  wire NotEmptyDone = !CurEmpty && PreDoneC; //�ǿա���ͨ����������
                                                               //�²���
  wire [5:0]nextPre = RealNEn ? PreLOOK : (PreDoneC ? PreREAD : (CurState[2] ? PreWAIT : PreCCAL)); //RealNEnΪ1������ָ����ʱ����Ҫ�������
  
  always@(*)begin
    case(PreState)
	   PreIDLE:
	     if(!PreEmpty && (yDiff[1:0] != 2'b0)) begin  //��һ��FIFO�ڵ�ǰ�оͻ�ǿգ��������ǵ��ж�Ҫ����һ�вŻῪʼ��
		    if((DataEn_[0] || DataEn_[`DL]) && EmptyAndNotC || NotEmptyNotC)  //����ͨ����             
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
		  else //elseҪд��������ISE�����������
		    NextPreState = PreIDLE;
			 
		PreLOOK: begin
		  NextPreState = nextPre;         //һ�鵽�ס���һ�����¾�ת�����������ܼ������������ͨ��ͼ����鿴
		  PreCombCur = RealNEn || CurState[2] ? 0 : 1;
		end
		
		PreCCAL:begin
		  PreCombCur = !CurEmpty ? 1 : 0; //��һ�кϲ�����һ���µġ���û��ŵ���ͨ��
		  
        if(NotEmptyDone || NotEmptyNotC || EmptyAndNotC)//�ǿ���ͨ�����¡��ǿյ�����ͨ���յ��Ѳ���������ͨ
          NextPreState = PreREAD;
        else
          NextPreState = PreCCAL;
      end	

      PreWAIT: begin                    //����ǰ�кϲ���û����
         NextPreState = PreIDLE;		
			PreCombCur = 0;
		end
	   default: begin
		  NextPreState = PreIDLE;
		  PreCombCur = 0;
		end
	 endcase
  end
  
  wire LeftNotC  = PreXEnd + 1 < CurXStart;         //����ͨ������һ�е������ͨ�������
  wire RightNotC = CurXEnd + 1 < PreXStart;         //����ͨ������һ�е������ͨ�����ұ�
  wire Connect   = !LeftNotC && !RightNotC;         //���ֲ���ͨ������������Ǿ�����ͨ
  wire PreDoneC  = Connect && (CurXEnd >= PreXEnd); //��һ�в�����������һ���ұ�������ͨ����,������
  wire CurDoneC  = Connect && (PreXEnd >= CurXEnd); //��һ�в�����������һ���ұ�������ͨ����
  
  
  
  //����ǰ����ͨ����Ϣ��״̬��
  parameter CurIDLE = 1,  
            CurNEW  = 1 << 1,    //��һ���ǿ��л�û����֮����ͨ�ľͽ����½�״̬
	         CurCCAL = 1 << 2;    //�²���
				
  wire PreEmptyOrNotC = PreEmpty ||                       //��һ���ǿյ�
			            (!PreEmpty && PreY == CurY[1:0]) ||  //�ǿգ�����������һ�и�д��ȥ�ģ���Ҳ��ζ����һ���ǿյ�   
                     (!PreEmpty && RightNotC);            //���֮ǰ����ͨ�������ͨ״̬����IDLE״̬�������������ζ��ǰ������ͨ	 
  //�²������£�     ��һ��Ϊ�ջ�����ͨ �� ����ͨ��������
  wire CurCCALDone = PreEmptyOrNotC || (NextPreState[5:4] && CurDoneC) ;
  
  always@(*)begin
    case(CurState)	 
	   CurIDLE:
		  if(!CurEmpty) begin
		    if(PreEmptyOrNotC)			                   //��һ���ǿյĻ�û����ͨ�ľͽ����½���ͨ��״̬
			   NextCurState = CurNEW;                     //CurNEW״̬���д��һ���½�����ͨ����Ϣ��CCList�Ȼ����N���1
			 else if(/*!PreEmpty && Connect &&*/ !CurDoneC && NextPreState[4])   //��ͨ�����µ�״̬Pre״̬���Ѵ����ˡ���һ�������ˣ���ǰ�л�û����
			   NextCurState = CurCCAL;
			 else
			   NextCurState = CurIDLE;
		  end
		  else
		    NextCurState = CurIDLE;
			 
		CurCCAL:                                         //��һ������ͨ�ͺϲ���һ��
		  if(CurCCALDone)
		    NextCurState = CurIDLE;
		  else
		    NextCurState = CurCCAL;	   
		default: NextCurState = CurIDLE;
	 endcase
  end 
		 
  wire [Wb - 1:0] CurXStart, CurXEnd, PreXStart, PreXEnd, BottomLineXMax;
  wire [Nb - 1:0] CurN, PreN; //��ͨ����
  wire [Hb - 1:0] CurY;
                   //��ǰ���½���ͨ��  ����һ�кϲ������£�           ���ºϲ���һ�С���������ͨ��������ͨ��������
  wire PreWrCurRd = NextCurState[1] || (PreCombCur && CurDoneC) || (CurState[2] && CurCCALDone);    //1��CurNEW 2��CurCCAL;

  //First-Word Fall-Fhroughģʽ����ͬʱ�ӡ�FiFo���Ҫ����ǰ����Ĳ������������Ҫ����ʵ��ͼ��������趨��������Ӧ�ô���һ��������ܳ��ֶ��ٸ���ͨ��
  //�������õ�FIFO������д����������ڻ������dout�ϣ�ͬʱempty�ź����͡���������rd_en����Ƴ�����ݡ�
  FIFO36x512 iCurrentLine(//��ǰ��
    .clk(clk), 
	 .srst(SRST), 
    .din({4'b0, y, XStart, x - 1'b1}), //x - 1����XEnd, ������ͨ��Ľ������꣬�����1Ҫд��1'b1��дx - 1����ᱻ����32λ��
    .wr_en(End),                       //������ͨ�����ʱ��Ϣ��д��FIFO����ʱд�����Ϣ����û����ͨ���ŵ�
    .rd_en(PreWrCurRd),                //��ǰ�ж�һ����ʱ����һ�о�Ҫдһ��
    .dout({CurY, CurXStart, CurXEnd}), // output [35 : 0] dout
    .full(), 
    .empty(CurEmpty) 
  );    
  
  
  wire [1:0] PreY;
  wire CCDone = ~RealNEn && (PreXEnd == BottomLineXMax) && (y != YMax);//��ͨ������ж��ź�,"���޴�Ԫ"C#�������и���ϸ��ע�͡�
  assign OEn = CCDone && NextPreState[1]; //��һ������ͨ�����а�������ͨ������������
               //��һ������ͨ���º�����ͨ���¶���
  wire PreRd = NextPreState[1] || NextPreState[4];       //1:PreDONE 4:READ��5 WAIT״̬����һ�л�û���£�������
  assign CurN =  NextCurState[1] ? N : (PreWrCurRd && PreCombCur ? RealN : InheritN);
  
  FIFO36x512 iPreviousLine(//��һ�С���ǰ����ĵ�����ͨ����Ϣ���ڼ�����һ�е���ͨ�������ȡ���֮��д����һ��FIFO��
    .clk(clk), 
    .srst(SRST), 
    .din({0, CurY[1:0], CurN, CurXStart, CurXEnd}), // input [35 : 0] din
    .wr_en(PreWrCurRd), 
    .rd_en(PreRd), 
    .dout({PreY, PreN, PreXStart, PreXEnd}), // output [35 : 0] dout
    .full(), 
    .empty(PreEmpty) 
  );
  //2��CurCCAL�� ��ǰ�кϲ���һ�е�д�źš�4����һ�б��ϲ�����һ�����¡�5����һ�б��ϲ���û����
  wire CurCombPre = CurState[2] &&  NextPreState[5:4] && (InheritN != RealN);  
  wire CCwea = NextCurState[1] || CurCombPre;//1��CurNEW State �½�һ����ͨ����ϢʱiCCList��д�ź�
               
  always@(posedge clk)
    if(NextCurState[1]) N <= N + 1'b1;    //���ֻ�����½�һ����ͨ����Ϣ���1
  
  wire [89    :0] CCdina  = NextCurState[1] ? {CurCount, CurXStart, CurXEnd, CurY, CurY, CurXEnd, N, 1'b0} 
                                            : {iaCount, iaXMin, iaXMax, iaYMin, iaYMax, oaBottomLineXMax, oaRealN, oaRealNEn};
  wire [Nb - 1:0] CCaddra = NextCurState[1] ? N : InheritN; //�½�:�²���
  wire [89    :0] CCdinb = CurCombPre ? {79'b0,oaRealN,1'b1}
                                      : {bCount, bXMin, bXMax, bYMin, bYMax, bBottomLineXMax, RealN, RealNEn};
  
  //�ϲ��µ�һ��
  wire [89 - 3 * Wb - 2 * Hb - Nb - 1:0] CurCount = CurXEnd - CurXStart + 1'b1;
  wire [89 - 3 * Wb - 2 * Hb - Nb - 1:0] bCount = Count + CurCount;
  wire [Wb - 1:0] bXMin = CurXStart < XMin ? CurXStart : XMin;
  wire [Wb - 1:0] bXMax = CurXEnd > XMax ? CurXEnd : XMax;
  wire [Wb - 1:0] bBottomLineXMax = CurXEnd;
  wire [Hb - 1:0] bYMin = YMin;
  wire [Hb - 1:0] bYMax = CurY;
  wire CCweb = PreCombCur || CurCombPre;            //��һ�кϲ�������һ�е�дʹ���ź�
  wire [Nb - 1:0] CCaddrb = NextPreState[3:2] || CCweb ? RealN : PreN;              //�ϲ��£��²��ϡ�����CCAL״̬��ַ����RealN 2:Look���²���
  
  //�ϲ���һ����ͨ����Ϣ����ʱb�˿ڵ��������һ�е���ͨ���Ǳ��ϲ����ġ�
  wire [89 - 3 * Wb - 2 * Hb - Nb - 1:0] iaCount = oaCount + Count;
  wire [Wb - 1:0] iaXMin = XMin < oaXMin ? XMin : oaXMin;
  wire [Wb - 1:0] iaXMax = XMax > oaXMax ? XMax : oaXMax;
  wire [Hb - 1:0] iaYMin = YMin < oaYMin ? YMin : oaYMin;
  wire [Hb - 1:0] iaYMax = YMax > oaYMax ? YMax : oaYMax ;
  
  //Count�ܵ���, XMin, XMax, YMin, YMax, BottomLineXMax, RealN, RealNEnΪ1����RealN��Ч����Ϣ���ϲ�������ָ���ı��
  BlockRam90x1024 iCCList( //��ͨ��ͳ�Ʊ�
    .clka(clk), 
    .wea(CCwea),                            //�½����²���
    .addra(CCaddra), // input [9 : 0] addra
    .dina(CCdina), // input [89 : 0] dina
    .douta({oaCount, oaXMin, oaXMax, oaYMin, oaYMax, oaBottomLineXMax, oaRealN, oaRealNEn}), // output [89 : 0] douta
    .clkb(clk), 
    .web(CCweb),                            //�ϲ��£��²���ʱ��д�ϵ�RealN
    .addrb(CCaddrb), 
    .dinb(CCdinb), 
    .doutb({Count, XMin, XMax, YMin, YMax, BottomLineXMax, RealN, RealNEn})
  );
																									  
endmodule
