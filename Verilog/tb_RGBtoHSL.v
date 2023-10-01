`timescale 1ns / 1ps

module tb_RGBtoHSL;

	// Inputs
	reg clk;
	reg RGBEn;
	reg [7:0] R;
	reg [7:0] G;
	reg [7:0] B;

	// Outputs
	wire HSLEn;
	wire [7:0] H;
	wire [7:0] S;
	wire [7:0] L;

	// Instantiate the Unit Under Test (UUT)
	RGBtoHSL uut (
		.clk(clk), 
		.RGBEn(RGBEn), 
		.R(R), 
		.G(G), 
		.B(B), 
		.HSLEn(HSLEn), 
		.H(H), 
		.S(S), 
		.L(L)
	);
	
	always #5 clk = ~clk;

	initial begin
		// Initialize Inputs
		clk = 0;
		RGBEn = 0;
		R = 0;
		G = 0;
		B = 0;

		// Wait 100 ns for global reset to finish
		#10;
        
		// Add stimulus here
		repeat(30) @(posedge clk);     //除法器要初始化几十个周期才能给出正确的仿真结果，前面空的周期少了不行
		R = 1;            //140 120 2  本测试激励在ISE里仿真时，时序是正确的，在Vivado里可能会有问题，需要小改一下。
		G = 2;
		B = 3;            
		@(posedge clk);
		RGBEn = 1;
		R = 255;           //0 0 240
		G = 255;
		B = 255;
		@(posedge clk);
		R = 0;           //120 240 120
		G = 255;
		B = 255;
		@(posedge clk);
		R = 255;           //0 240 120
		G = 0;
		B = 0;
		@(posedge clk);
		R = 0;             //80 240 120
		G = 255;
		B = 0;
		@(posedge clk);
		R = 0;             //160 240 120
		G = 0;
		B = 255;
		@(posedge clk);
		R = 71;              //68 26 70
		G = 82;
		B = 66;
		@(posedge clk);
		R = 170;             //212 76 123
		G = 91;
		B = 146;
		@(posedge clk);
		R = 128;            //200 240 60
		G = 0;
		B = 128;
		@(posedge clk);
		R = 134;            //161 185 174
		G = 131;
		B = 239;
		@(posedge clk);
		R = 255;            //36 240 134
		G = 233;
		B = 30;
		@(posedge clk);
		R = 77;            //26 175 42
		G = 55;
		B = 12;
		@(posedge clk);
		RGBEn = 0;
		R = 66;            //0 0 62
		G = 66;
		B = 66;
      repeat(35) @(posedge clk);   
		$finish;
	end
      
endmodule

