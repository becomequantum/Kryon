`timescale 1ns / 1ps

module tb_Histogram();
 reg clk;
 reg DataEn;
 reg [7:0] PixelData;
  
  Histogram #(
      .DATA_WIDTH(8),               
      .CNT_WIDTH(18)  
    )dutHistogram (
    .clk(clk),
    .VSYNC(),
    .DataEn(DataEn),
    .PixelData(PixelData)
  );
  
  always #5 clk = ~clk;
  
  initial begin
    clk = 0;
    DataEn = 0;
    PixelData = 0;
    repeat(3) @(posedge clk);
    #1 DataEn = 1;
    @(posedge clk);
    #1 PixelData = 1;
    @(posedge clk);
    #1 PixelData = 2;
    @(posedge clk);
    #1 PixelData = 3;
    @(posedge clk);
    #1 PixelData = 2;
    @(posedge clk);
    #1 PixelData = 4;
    repeat(2)@(posedge clk);
    #1 PixelData = 5;
    repeat(3)@(posedge clk);
    #1 PixelData = 0;
    @(posedge clk);
    #1 PixelData = 6;
    repeat(4)@(posedge clk);
    #1 PixelData = 1;
    repeat(2)@(posedge clk);
    #1 DataEn = 0;
    
    repeat(10)@(posedge clk);
    $finish;
  end
  
endmodule
