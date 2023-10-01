`timescale 1ns / 1ps
//代码来自：https://github.com/becomequantum  别忘了去看看 /推荐书籍 目录下的内容，《2022人类觉醒指南》是我写的一些无法发出来的内容。
//解读代码的视频：
//https://www.bilibili.com/video/BV1QY411V7Pg  提升自己的独立思考能力比看懂别人写的代码更重要，“无限次元”频道里《没有独立思考能力的人只是可编程机器人！》这些闲谈的视频更重要！
//https://www.youtube.com/watch?v=dE5gn4jkSbw

module Histogram #(
   parameter  DATA_WIDTH = 8,               //像素数据位宽，一般是8位。
   parameter  CNT_WIDTH = 18                //计数数组位宽
   )
   (
    input clk,
    input VSYNC,                            //垂直同步信号，也就是帧有效信号，用于指示一帧结束之后清零计数Ram里的数据，这部分下面代码里没写
    input DataEn,                           //DE,数据有效信号
    input [DATA_WIDTH - 1 : 0] PixelData    //像素数据
    );
    wire [CNT_WIDTH - 1 : 0] Cnt, CntPlus1;
    reg [DATA_WIDTH - 1 : 0] addrb;
    reg web;
    
    always@(posedge clk) begin
      web <= DataEn;
      addrb <= PixelData;
    end
    
    assign CntPlus1 = Cnt + 1;           
    
    CountArray iCountArray (
      .clka(clk),            // input wire clka
      .wea(1'b0),            // a口做为读端口，所以写使能置零。
      .addra({0, PixelData}),// PixelData只有8位，addra是10位，所以前面要加个0补足位数，不然没补足的高位就是悬空的X，会导致仿真出错
      .dina(0),              // 写数据也置为0，不用的输入要置零，不能空着
      .douta(Cnt),           // output wire [17 : 0] douta
      .clkb(clk),            // input wire clkb
      .web(web),             // input wire [0 : 0] web
      .addrb({0,addrb}),     // input wire [9 : 0] addrb   没有写n'b0注明位数时，默认为32位
      .dinb(CntPlus1),       // input wire [17 : 0] dinb
      .doutb()               // 输出没用上空着就行 
    );    
endmodule
