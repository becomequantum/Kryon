`timescale 1ns / 1ps
//本代码来自：github.com/becomequantum/Kryon
//代码解读视频：www.bilibili.com/video/BV1B3411W7Ht
//本模块是3x3,8位灰度图像算子的示例,这样的算子可以做很多事情.比如滤波,平滑,边缘检测等,也可用于传感器输出Raw数据的Bayer插值.
//This module is an example of the 3x3, 8 bit gray image operator. Operators like this can do a lot of things, such as filtering, smoothing, edge detection, etc., and can also be used for Bayer interpolation of Raw data from image sensor output.
//以下参数可根据需要修改 The following parameters can be modified as required
`define DATA_WIDTH        8         //图像数据位宽,本模块示例的是8位灰度算子,所以这里设为8  Image data bit width, this example is 8 bit grayscale operator, so here is 8. 
`define OPERATOR_HEIGHT   3         //算子高度,本模块为3x3算子示例,所以这里设为3 The height of the operator, in this example it's 3
`define ADDR_WIDTH        11        //2^11 = 2048可支持一行2048像素,             Maximum 2048 pixel per line, can be modified.

module GrayOperator3x3(
    input                       clk      ,
  //input Vsync,                                  //如果用作Bayer插值的话还需要输入帧有效信号,因为需要知道现在这一行是第几行   If use as Bayer interpolation, you also need to input the frame enable signal, because you need to know which line it is.   
    input                       DataEn   ,                                 
    input [`DATA_WIDTH - 1 : 0] PixelData,                                 
    output                      DataOutEn         //只是示例模块,所以没有写具体结果输出,使用时可自行添加 Add whatever output you want
    );
    
    wire [`OPERATOR_HEIGHT * `DATA_WIDTH - 1 : 0] OperatorData           ; //本例中就是[23:0],三行8位图像数据. This example is [23:0], three line of 8 bit image data.
    reg  [`DATA_WIDTH - 1                    : 0] GaussianBlur           ; 
    reg  [`DATA_WIDTH + 1                    : 0] Gx,Gy                  ; 
    wire [`DATA_WIDTH + 1                    : 0] Left,Right,Up,Down     ;
    reg  [`DATA_WIDTH - 1                    : 0] Array00,Array01,Array02, //3x3算子数据阵列,如果是5x5的算子这里就要写5x5个. 3x3 operator data array, if it is 5x5 operator, here needs to write a 5X5 array
                                                  Array10,Array11,Array12, //Array11为3x3算子的中心点, Array11 is the center
                                                  Array20,Array21,Array22;    
    
    //对算子进行的计算,不同的计算会有不同的效果.注意,如果要在进行高斯平滑之后再进行边缘检测,那就需要两个这样的模块:
    //第一个模块进行平滑,结果再输入到第二个模块进行边缘检测
    //Different calculations have different effects. Note that two such modules are needed if the edge detection is performed after the Gauss smoothing is carried out:
    //The first module dose smooth, and the result inputs to the second module for edge detection.
    always@(posedge clk)
    begin
    	GaussianBlur <= (Array00 >> 4) + (Array01 >> 3) + (Array02 >> 4) +  
    	                (Array10 >> 3) + (Array11 >> 2) + (Array12 >> 3) +
    	                (Array20 >> 4) + (Array21 >> 3) + (Array22 >> 4);     //高斯平滑(模糊)的结果,注意这样的运算要小心结果溢出.Gauss smoothing (blur) results, be careful of overflow.
    	                
    	Gx           <= Right >= Left ? Right - Left : Left - Right;          //计算Sobel算子加权和的绝对值 Calculating the absolute value of weighted sum of the Sobel operator
    	Gy           <= Up    >= Down ? Up    - Down : Down - Up   ;
    end
    
    assign Left  = Array00 + {Array10,1'b0} + Array20,                      //计算Sobel算子加权和. Calculating the weighted sum of the Sobel operator
           Right = Array02 + {Array12,1'b0} + Array22,                      //神经网络里的参数在推理时算是常数，乘以常数不需要DSP乘法器  
           Up    = Array00 + {Array01,1'b0} + Array02,
           Down  = Array20 + {Array21,1'b0} + Array22;
    
    assign Sobel = Gx + Gy >= 400;                                          //Sobel算子边缘检测结果,大概是这么算的吧,仅供参考. 
    
    reg    [2:0] DataEnReg;          //输出使能信号要和输出的计算结果对齐,它具体要延时多少个周期和算子的大小以及进行计算时加了多少级寄存器有关, Output enable signal mast be aligned with the output results, it's delay relates to the size of the operator and calcuation delay
    assign DataOutEn = DataEnReg[2]; //本示例中延时是3个周期, 3列的算子会产生2个周期的延时;再加上计算时的1个. 5列的算子会产生3个,3为算子中心位置 In this example, the delay is 3 cycles, the 3 column operator produces 2 cycles' delay, plus 1 cycle delay in calcuation. 5 column operator will produces 3 cycles' delay, 3 as the center position of the operator.
                                     //如果你修改了算子大小和计算中的寄存器级数,务必跑仿真验证一下延时有没有弄对. If you modified the size of the operator or the register levels in the calculation, you must run simulation to verify if the delay is correct.
        
    always@(posedge clk)
    begin 
    	Array00 <= Array01; Array01 <= Array02;
    	Array10 <= Array11; Array11 <= Array12;
    	Array20 <= Array21; Array21 <= Array22;
    	{Array22,Array12,Array02} <= OperatorData;     //移位寄存产生3x3算子数据阵列,OperatorData中低位对应算子上面. Shift Reg to generate 3x3 gray operator data array, Lower bits in 'OperatorData' correspond to operator's upper part
    	DataEnReg[0]   <=  OperatorDataEn;             //延时使能信号,这里的延时是接着LineBuffer模块输出的使能信号OperatorDataEn之后的
    	DataEnReg[2:1] <=  DataEnReg[1:0];             //Delay output enable signal, this delay is after 'LineBuffer' module's output enable signal 'OperatorDataEn'                                   
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
    
    //这个Block Ram需要根据你所需的算子大小和图像的宽度来生成,2048为Ram深度,对应图像宽度,如果你的图像宽度大于2048,就需生成更深的Ram。注意不要勾选输出寄存器！
    //Ram的宽度需>= (算子高度 - 1) * 数据位宽. 本例所需宽度 >= (3 - 1) * 8 = 16 
    //You need to generate this Block Ram according to your operator's size and your image's width. 2048 is the depth of the Ram, correspond to image width,
    //if your image width > 2048, then you need to generate a deeper Ram
    //Ram width needs to >= (operator's height - 1) * Data width. In this example, Ram width needs to >= (3 - 1) * 8 = 16 
    BlockRam18x2048 iBlockRam18x2048 (
      .clka (clk  ),  // input  wire          clka
      .wea  (0    ),  // input  wire [0  : 0] wea     a端口用作读,所以写使能信号要置零 a is reading port, so wea needs to be 0
      .addra(addra),  // input  wire [10 : 0] addra
      .dina (0    ),  // input  wire [17 : 0] dina
      .douta(douta),  // output wire [17 : 0] douta
      .clkb (clk  ),  // input  wire          clkb
      .web  (web  ),  // input  wire [0  : 0] web
      .addrb(addrb),  // input  wire [10 : 0] addrb
      .dinb (dinb ),  // input  wire [17 : 0] dinb
      .doutb(     )   // output wire [17 : 0] doutb
    );
endmodule
