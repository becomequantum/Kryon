`timescale 1ns / 1ps
//本代码来自：github.com/becomequantum/Kryon
//代码解读视频：www.bilibili.com/video/BV1B3411W7Ht
//本模块是9x9二值图像算子的示例,二值算子主要用作图像形态学操作.二值算子可以弄的比较大,能实现一些简单的形状特征识别,目标大小识别等功能
//This module is an example of the 9x9 binary image operator. The binary operator is mainly used in image morphological operations. 
//The binary operator's size can be made very large, and can realize some simple functions such as: shape feature recognition, shape size recognition and so on.
`define DATA_WIDTH        1         //图像数据位宽, 二值算子就是1.     It's binary operator, so pixel data width is 1
`define OPERATOR_HEIGHT   9         //算子高度,本例是9,可根据需要修改. The height of the operator, in this example it's 9, can be modified as needed
`define ADDR_WIDTH        11        //2^11 = 2048可支持一行2048像素.   Maximum 2048 pixel per line, can be modified.

module BinaryOperator9x9(
    input                       clk      ,     
    input                       DataEn   ,                                 
    input [`DATA_WIDTH - 1 : 0] PixelData,                                 
    output                      DataOutEn         //只是示例模块,所以没有写具体结果输出,使用时可自行添加 Add whatever output you want
    );
    
    wire [`OPERATOR_HEIGHT * `DATA_WIDTH - 1 : 0] OperatorData;
    reg  [8 :0] Array0,Array1,Array2,Array3,Array4,Array5,Array6,Array7,Array8; 
    reg  [15:0] Sum;
    
    always@(posedge clk)                          //把9x9个点都加起来. Sum all 9x9 pixels。这样的代码可以写个上位机程序自动生成
      Sum = Array0[0] + Array0[1] + Array0[2] + Array0[3] + Array0[4] + Array0[5] + Array0[6] + Array0[7] + Array0[8] +
            Array1[0] + Array1[1] + Array1[2] + Array1[3] + Array1[4] + Array1[5] + Array1[6] + Array1[7] + Array1[8] +
            Array2[0] + Array2[1] + Array2[2] + Array2[3] + Array2[4] + Array2[5] + Array2[6] + Array2[7] + Array2[8] +
            Array3[0] + Array3[1] + Array3[2] + Array3[3] + Array3[4] + Array3[5] + Array3[6] + Array3[7] + Array3[8] +
            Array4[0] + Array4[1] + Array4[2] + Array4[3] + Array4[4] + Array4[5] + Array4[6] + Array4[7] + Array4[8] +
            Array5[0] + Array5[1] + Array5[2] + Array5[3] + Array5[4] + Array5[5] + Array5[6] + Array5[7] + Array5[8] +
            Array6[0] + Array6[1] + Array6[2] + Array6[3] + Array6[4] + Array6[5] + Array6[6] + Array6[7] + Array6[8] +
            Array7[0] + Array7[1] + Array7[2] + Array7[3] + Array7[4] + Array7[5] + Array7[6] + Array7[7] + Array7[8] +
            Array8[0] + Array8[1] + Array8[2] + Array8[3] + Array8[4] + Array8[5] + Array8[6] + Array8[7] + Array8[8];
    
    assign Dilation  = Sum > 0,                   //9x9算子膨胀结果 The result of 9x9 dilation
           Erosion   = Sum >= (9*9),              //腐蚀结果                          erosion
           InBetween = Sum >= ( 33);              //这个结果和“无限次元”图像处理软件中“阈胀蚀”功能的参数设为：直径9、阈值33时效果一样。这个视频有详细介绍：www.bilibili.com/video/BV1WY411L7Bd
    
    reg    [5:0] DataEnReg;                       //结果延时了5+1个周期,5是因为9x9的算子中心位置是第5; 1是计算Sum时延时的
    assign DataOutEn = DataEnReg[5];              //The output delayed 5+1 clock cycle, 5 is because the 9x9 operator's center position is 5; 1 is because of Sum's calculation delay
    
    always@(posedge clk)                            
    begin
      {Array8[8],Array7[8],Array6[8],Array5[8],Array4[8],Array3[8],Array2[8],Array1[8],Array0[8]} <= OperatorData;
      Array0[7:0]    <= Array0[8:1];              //移位寄存产生9x9二值算子数据阵列,OperatorData中低位对应算子上面     
      Array1[7:0]    <= Array1[8:1];              //Shift Reg to generate 9x9 binary operator data array, Lower bits in 'OperatorData' correspond to operator's upper part
      Array2[7:0]    <= Array2[8:1];
      Array3[7:0]    <= Array3[8:1];
      Array4[7:0]    <= Array4[8:1];
      Array5[7:0]    <= Array5[8:1];
      Array6[7:0]    <= Array6[8:1];
      Array7[7:0]    <= Array7[8:1];
      Array8[7:0]    <= Array8[8:1];
      DataEnReg[0]   <= OperatorDataEn;           //延时使能信号,这里的延时是接着LineBuffer模块输出的使能信号OperatorDataEn之后的
      DataEnReg[5:1] <= DataEnReg[4:0];           //输出的使能信号长度是和输入一样的，相当于加了zero padding
    end   
     
    //LineBuffer和Block Ram例化,这两个模块合起来完成图像数据的缓存和输出N行算子数据的工作
    //Instantiate module LineBuffer and BlockRam, They buffer image data and output N line data of the operator
    wire [`ADDR_WIDTH - 1                          : 0] addra,addrb;
    wire [(`OPERATOR_HEIGHT - 1) * `DATA_WIDTH - 1 : 0] douta, dinb;
    LineBuffer
    #(
      .DATA_WIDTH     (`DATA_WIDTH     ),
      .ADDR_WIDTH     (`ADDR_WIDTH     ),
      .OPERATOR_HEIGHT(`OPERATOR_HEIGHT)
    )
    i3LineBuffer
    (
      .clk           (clk           ),
      .DataEn        (DataEn        ),
      .PixelData     (PixelData     ),               
      .addra         (addra         ),
      .douta         (douta         ),
      .web           (web           ),
      .addrb         (addrb         ),
      .dinb          (dinb          ),                    
      .OperatorDataEn(OperatorDataEn),
      .OperatorData  (OperatorData  )    
    );
    
    //这个Block Ram需要根据你所需的算子大小和图像的宽度来生成,2048为Ram深度,对应图像宽度,如果你的图像宽度大于2048,就需生成更深的Ram。生成时不要勾选任何输出寄存！Vivado里面默认勾上了，要去掉！
    //Ram的宽度需>= (算子高度 - 1) * 数据位宽. 本例所需宽度 >= (9 - 1)  * 1 = 8 
    //You need to generate this Block Ram according to your operator's size and your image's width. 2048 is the depth of the Ram, correspond to image width,
    //if your image width > 2048, then you need to generate a deeper Ram
    //Ram width needs to >= (operator's height - 1) * Data width. In this example, Ram width needs to >= (9 - 1) * 1 = 8
    BlockRam9x2048 iBlockRam9x2048 (
      .clka (clk  ),  // input  wire          clka
      .wea  (0    ),  // input  wire [0  : 0] wea     a端口用作读,所以写使能信号要置零. a is reading port, so wea needs to be 0
      .addra(addra),  // input  wire [10 : 0] addra
      .dina (0    ),  // input  wire [8  : 0] dina
      .douta(douta),  // output wire [8  : 0] douta
      .clkb (clk  ),  // input  wire          clkb
      .web  (web  ),  // input  wire [0  : 0] web
      .addrb(addrb),  // input  wire [10 : 0] addrb
      .dinb (dinb ),  // input  wire [8  : 0] dinb
      .doutb(     )   // output wire [8  : 0] doutb
    );
endmodule
