  `timescale 1 ns / 1 ps
//本代码来自：github.com/becomequantum/Kryon
//代码解读视频：https://www.bilibili.com/video/BV1ZS4y1H7oQ
//下面的测试激励能直接读取二进制bmp位图文件做为仿真输入，并把仿真结果也写入到一个位图文件中，这样仿真结束后就可以直接打开位图文件查看处理结果。该测试激励在ISE仿真器里测试是没有问题的。
  module tb_ImageProcess();
	 reg clk,RGBEn;
	 reg [7:0] R, G, B, InBetween;
	 integer InBmpFile, OutBmpFile1, BmpWidth, BmpHeight, BmpByteCount, Stride, w, h, i2;
	 reg [7:0] BmpHeadMem[1:54];    //bmp位图文件头大小是54字节，从1开始是因为$fread似乎在没给读到mem哪个位置这个参数时会读到地址1上。
	 reg [7:0] PixelMem[1:4];
	 
	 BinaryOperator9x9 iBinaryOperator9x9 //二值算子
    (
     .clk      (clk   ),         
     .DataEn   (RGBEn),
     .PixelData(G == 255),               //背景黑：0，前景白：1
     .DataOutEn()    
    );
/*
	  CCAL #(
	   .Wb(11),
		.Hb(10),
		.Nb(10)
	 )
	 iCCAL(
	  .clk      (clk   ),         
	  .SRST     (SRST),
     .DataEn   (RGBEn),
     .PixelData(G == 255),                //背景黑：0，前景白：1
	  .OEn(OEn),
	  .Count(),
	  .XMin(),
	  .XMax(),
	  .YMin(),
	  .YMax(),
	  .RealN()
	 );
	 
	*///这段代码用于测试连通域识别模块CCAL.v
	  
/*
	always@(posedge clk)              //输出连通域识别结果到CCList.txt   
	   if(OEn)
         $fwrite(CCListFile,"%d,%d,%d,%d,%d,%d\n",iCCAL.Count,iCCAL.XMin,iCCAL.XMax,iCCAL.YMin,iCCAL.YMax,iCCAL.RealN);
*///仿真CCAL模块时，需把上面这两块注释的代码取消注释，然后把仿真BinaryOperator9x9这个模块的相关代码注释掉。
	  
	 always #5 clk = ~clk;  //时钟信号
	 
	 initial begin
	  clk = 1; R = 0; G = 0; B = 0; RGBEn = 0; 
		
		InBmpFile = $fopen("input.bmp","r"); 
	  $fread(BmpHeadMem,InBmpFile);                                                //读入位图文件头	
	  
	  BmpWidth = {BmpHeadMem[22],BmpHeadMem[21],BmpHeadMem[20],BmpHeadMem[19]};    //从文件头中获取图片的宽、高
		BmpHeight = {BmpHeadMem[26],BmpHeadMem[25],BmpHeadMem[24],BmpHeadMem[23]};
		
    BmpByteCount = {BmpHeadMem[30],BmpHeadMem[29]} >> 3;		                     //一个像素是3字节还是4字节，24位或32位bmp都适用
    
		if(BmpByteCount == 4 || (BmpWidth % 4 == 0))
		  Stride =  BmpWidth * BmpByteCount;
		else
		  Stride = ((BmpWidth * BmpByteCount) / 4 + 1) * 4;                           //计算实际每行图像数据占用的字节数 
		  
		
		ImageSimInput;                                                               //调用图像仿真输入任务，读取input.bmp数据，生成类似VGA时序波形
		
		repeat(10)@(posedge clk);
		$fclose(InBmpFile);
		$finish;
	 end
	 
	 task ImageSimInput;   //读入BMP位图文件，生存仿真波形
	 begin
		repeat(10)@(posedge clk);
		for(h = BmpHeight - 1; h >= 0; h = h - 1) begin           //最上面第一行数据存在了后面，文件头后面紧接的数据是最下面一行的
		  repeat(20)@(posedge clk);                               //行间空隙。对于连通域识别算法，这个周期数不能太小。
		  $fseek(InBmpFile, 54 + Stride * h,0);                   //把读文件的位置调到每一行数据的开始
		  #1 RGBEn = 1;
		  for(w = 0; w < BmpWidth; w = w + 1) begin
		    $fread(PixelMem, InBmpFile, 1, BmpByteCount);         //$fread(读到哪个Mem, 要读的文件, 读到Mem的哪个地址上（设为0，1都从1开始，设为2会从2开始）, 读几个字节)
		    B = PixelMem[1]; G = PixelMem[2]; R = PixelMem[3]; 
			 @(posedge clk);
			 #1;
		  end
		  RGBEn = 0; R = 0; G = 0; B = 0;
		end
	
		for(h = 0; h < 1; h = h + 1) begin                        //还要再生成几行像素使能信号，算是在Padding，因为算子的输出也会延时若干行。
		  repeat(5)@(posedge clk);  
		  #1 RGBEn = 1;
		  repeat(BmpWidth)@(posedge clk);
		  #1 RGBEn = 0;
		end
		
	   repeat(10)@(posedge clk);
	 end
   endtask
	 
	 
	 
	 //保存9x9二值算子的计算结果.不同的initial块是并行的. 仿真CCAL时可以把下面这块注释掉
    initial begin 
	   OutBmpFile1 = $fopen("output.bmp","w");                    //结果保存到了这里打开的bmp文件中，如果文件已存在则会被覆盖。
		for(i2 = 1; i2 <= 54; i2 = i2 + 1) begin                   //先写入位图54字节的文件头，就是把input.bmp的文件头复制了过来
		  $fwrite(OutBmpFile1,"%c",BmpHeadMem[i2]);
		end
      
      repeat(4) @(posedge iBinaryOperator9x9.DataOutEn);         //9x9的算子结果会比原数据延时4行，因为中心点下面还有4行，所以要跳过4行后再开始采样输出数据，所以前面的ImageSimInput要多输出几行使能信号
		
      for(i2 = BmpHeight - 1; i2 >= 0; i2 = i2 - 1) begin        //如果有多个initial块时，不同块里的循环变量不能相同
        @(posedge iBinaryOperator9x9.DataOutEn)
		  $fseek(OutBmpFile1, 54 + Stride * i2,0);                 //把读文件的位置调到每一行数据的开始
		  
        while(iBinaryOperator9x9.DataOutEn == 1) begin
        	@(posedge clk); 
        	InBetween = iBinaryOperator9x9.InBetween ? 8'hff : 8'h0;//背景黑：0，前景白：1 -> ff，  
        	$fwrite(OutBmpFile1,"%c%c%c",InBetween, InBetween, InBetween);  		
			if(BmpByteCount == 4)
			  $fwrite(OutBmpFile1, "%c", 8'hff);
        end     
			 
      end 
		
		$fclose(OutBmpFile1);  //只有关闭了文件，文件内容才会完整的写入到文件中。
    end  
	 
  endmodule
