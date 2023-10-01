`timescale 1ns / 1ps
//本代码来自：github.com/becomequantum/Kryon 
//本代码实现的就是"无限次元"C#代码中"图像.cs"代码里"RGB转HSL"这个函数
//代码讲解视频: www.bilibili.com/video/BV1MS4y1r76o
module RGBtoHSL(
    input clk,
    input RGBEn,
    input [7:0] R,
    input [7:0] G,
    input [7:0] B,
    output HSLEn,
    output reg [7:0] H,
    output reg [7:0] S,
    output reg [7:0] L
    );
	 //后缀_n代表相比最初的RGB输入延迟了n个周期。不是相同延迟的变量不能放在一起运算，所以用后缀标明延时以防出错。
	 reg [7:0] max_1, min_1, diff_2, R_1, G_1, B_1, max_y_2, max_z_2; 
	 reg [8:0] sum_2;
	 reg [1:0] maxIndex_1, maxIndex_2;     
	 
	 //先把R、G、B中哪个最大，哪个最小比出来，再把最大、最小的和：sum_2，差：diff_2算出来
	 wire [7:0] maxRG      = R > G ? R : G,
	            minRG      = R > G ? G : R;
    wire       maxIndexRG = R > G ? 0 : 1;			
	 
	 always@(posedge clk) begin
	   max_1 <= maxRG > B ? maxRG : B;   //RGB中的最大值
		min_1 <= minRG > B ? B : minRG;   //最小值
		
		diff_2 <= max_1 - min_1;          
		sum_2  <= max_1 + min_1;          
		
		maxIndex_1 <= maxRG > B ? {1'b0,maxIndexRG} : 2;  
		maxIndex_2 <= maxIndex_1;         //用于记录RGB谁最大：R最大为0，G最大为1，B最大为2
	 end
	 
	 //---计算L亮度值，L最好算--- 
	 //L_2 = sum_2 * (24 / 51), 24/51 = 0.47 = 0.0_111100001, 需9位精度才够, 直接写 sum_2 * 0.47仿真结果正确，但综合过不了，乘以常数无需DSP乘法器。
	 //除以510再乘以240是要把L的取值区间也统一到0到240之间，原本是0到255*2 = 510
	 wire [17:0] L_2 = (sum_2 << 8)+ (sum_2 << 7) + (sum_2 << 6) + (sum_2 << 5) + sum_2;//写成sum_2 * 9'b111100001 也可以                
	 
	                         //3用来记录差为0，H，S为0这个情况      ，正负符号位，             四舍五入
	 wire [10:0] delayin_2 = {(diff_2 == 0) ? 2'd3 : maxIndex_2, y_z_2[8], L_2[17:10] + L_2[9]};
    wire [10:0] delayout_22;	 
	 
	  Delay20 d20 ( //用于把L值，RGB哪个最大，y_z_2的正负，这三个信号延迟20个周期，和除法器延迟相同。L虽然只需2个周期就能算出，但输出必须和H，S时序对齐，所以需要延迟。
      .d(delayin_2), // input [10 : 0] d
      .clk(clk), // input clk
      .q(delayout_22) // output [10 : 0] q
	  );
	  
	  always@(posedge clk)
	    L <= delayout_22[7:0];     //最终的L输出，延迟了23个周期
	
	 
	 //---计算S饱和度值---
	                        //预防分母等于零导致结果不确定
	 wire [7:0] sdivisor_2 = (sum_2 == 9'd510 || sum_2 == 0) ? 1 : ((sum_2 < 9'd255) ? sum_2 : 9'd510 - sum_2);
	 wire [9:0] sfractional;
	 wire [7:0] squotient;
	 
	 Divider sDivider (//延时20个周期。Divider IP,用于整数除法。配置：除数、被除数都是8位，无符号，小数部分10位。
	  .clk(clk),
	  .rfd(), //用不着
	  .dividend(diff_2), // input [7 : 0] dividend 被除数就是diff_2
	  .divisor(sdivisor_2), // input [7 : 0] divisor
	  .quotient(squotient), // output [7 : 0] quotient
	  .fractional(sfractional)); // output [9 : 0] fractional
	
    wire [17:0] S_22 = {squotient[0],sfractional} * 8'd240;  //squotient商还要取一位是因为它有可能为1。	
	 
	 always@(posedge clk)
	   S <= S_22[17:10] + S_22[9]; //S最终输出，加后面一位是在四舍五入
		

   //---计算H色调值---
	always@(posedge clk) begin
	   R_1 <= R; G_1 <= G; B_1 <= B; //延时一个周期用于下面计算max_1 - RGB_1, 因为max_1延了一个周期，所以也必须把R、G、B延一个周期
		
		if(maxIndex_1 == 2'b0) begin
        max_y_2 <= max_1 - B_1;
		  max_z_2 <= max_1 - G_1;	
   	end
		if(maxIndex_1 == 2'b1) begin
        max_y_2 <= max_1 - R_1;
		  max_z_2 <= max_1 - B_1;	
   	end
		if(maxIndex_1 == 2'd2) begin
        max_y_2 <= max_1 - G_1;
		  max_z_2 <= max_1 - R_1;	
   	end 
	 end
	 
	 wire [8:0] y_z_2 = max_y_2 - max_z_2;
	 
	 wire [9:0] yzfractional;
	 wire [7:0] yzquotient;
	 wire [7:0] yzdividend_2 = y_z_2[8] ? (~y_z_2[7:0] + 1) : y_z_2[7:0]; //取绝对值运算，因为除法器是无符号的，
	  
	 Divider yzDivider ( //配置和前面的除法器一样
	  .clk(clk), // input clk
	  .rfd(), // output rfd
	  .dividend(yzdividend_2), // input [7 : 0] dividend
	  .divisor(diff_2), // input [7 : 0] divisor
	  .quotient(yzquotient), // output [7 : 0] quotient
	  .fractional(yzfractional)); // output [9 : 0] fractional
	 

    wire [1:0] maxIndex_22 = delayout_22[10:9]; //用于记录RGB谁最大：R最大为0，G最大为1，B最大为2，为3时RGB都相等，H，S为零。延迟了20个周期后，总共延迟22个周期。
    wire sign_22 = delayout_22[8];	 
	 wire [7:0] Hbias = 80 * maxIndex_22;        //色调值偏置: 红为0，绿为80，蓝为160
    
    wire [17:0] H_22 = {yzquotient[0],yzfractional} * 40;  //除出来的结果是0到1区间的的，乘以40扩大到0到40区间
	 
	 wire [10:0] Hb_22 = sign_22 ? {Hbias,2'b0} - H_22[17:8] : {Hbias,2'b0} + H_22[17:8]; //把正负号恢复，并加上色调值偏置: 红为0，绿为80，蓝为160
	 
	 wire [7 :0] Hround_22 = Hb_22[9:2] + Hb_22[1];     //四舍五入
	 
	 wire [10:0] H240_22 = {1'b0,8'd240,2'b0} + Hb_22;  //用于下面“小于零就加240”这种情况
	 
	 always@(posedge clk) begin
	   if(maxIndex_22 == 2'd3)   //maxIndex为3时RGB都相等，H等于零
		  H <= 0;
		else if(Hb_22[10] == 1)   //小于零就加240
		  H <= H240_22[9:2] + H240_22[1];
		else if(Hround_22 > 240 && (Hb_22[10] == 0)) //大于240就减240
		  H <= Hround_22 - 240;
		else
		  H <= Hround_22;
	 end
	 
	 //最后把使能信号也延迟相应的周期和HSL输出信号对齐
	 Delay23 EnDelay (//IP: Ram based Shift-Register 
     .d(RGBEn), // input [0 : 0] d
     .clk(clk), // input clk
     .q(HSLEn) // output [0 : 0] q
    );
    
endmodule
