`timescale 1ns / 1ps
//本代码来自：github.com/becomequantum/Kryon
//代码解读视频：www.bilibili.com/video/BV1B3411W7Ht
//该模块用来控制图像数据的缓存. 输入图像数据, 输出OPERATOR_HEIGHT行算子数据, 并控制Block Ram的读写. 乃核心模块.
module LineBuffer
   #(                                                                   //这里写的参数是默认值,在例化该模块的时候可以根据需要修改
   parameter DATA_WIDTH      =  8,                                      //像素数据位宽，一般是8位，也可能是10、12、14位，二值图像就是1位. The pixel data bit width is generally 8 bits, or 10, 12, and 14 bits. The binary image is 1 bits.
   parameter ADDR_WIDTH      = 11,                                      //缓存图像的Block RAM地址位宽,和图像宽度有关,2^11=2048
   parameter OPERATOR_HEIGHT =  3                                       //算子的高度,NxN的算子这里就设为N
   )
   (
    input                                               clk           ,
    input                                               DataEn        , //像素数据的Enable信号
    input  [DATA_WIDTH - 1                         : 0] PixelData     , //像素数据               
    output [ADDR_WIDTH - 1                         : 0] addra         , //以下为用来读写Block Ram的信号,a端口读出,b端口写回    
    input  [(OPERATOR_HEIGHT - 1) * DATA_WIDTH - 1 : 0] douta         , //N行的算子只需要缓存N-1行的数据. The N line operator only needs to buffer N-1 lines' data  
    output                                              web           ,
    output [ADDR_WIDTH - 1                         : 0] addrb         ,
    output [(OPERATOR_HEIGHT - 1) * DATA_WIDTH - 1 : 0] dinb          ,
    output                                              OperatorDataEn, //输出OPERATOR_HEIGHT行算子数据 
    output [OPERATOR_HEIGHT * DATA_WIDTH - 1       : 0] OperatorData    
    );
    
    reg    [ADDR_WIDTH - 1                         : 0] FrogCount = 0  ;//FPGA里的寄存器可以赋初始值,程序加载后就是初始值,所以一般不需要reset信号. Registers in FPGA can be assigned initial values, and the initial values are loaded after programs are loaded, so reset signals are generally not required.      
    reg    [DATA_WIDTH - 1                         : 0] PixelDataReg   ;
    reg    [OPERATOR_HEIGHT * DATA_WIDTH - 1       : 0] OperatorDataReg;
    reg    [1                                      : 0] DataEnReg      ;
    
    assign addra          = FrogCount      ,
           OperatorData   = OperatorDataReg,
           OperatorDataEn = DataEnReg[1]   ,                            //数据延时了两个周期,使能信号也要延时2个周期. The data is delayed by two cycles, so enable signal also needs to be delayed for 2 cycles.
           addrb          = FrogCount - 2  ,                            //因为数据延时了两个周期,所以写回地址要用读地址减二. Because the data has been delayed for two cycles, the read address should minus two.
           web            = DataEnReg[1]   ,
           dinb           = OperatorDataReg[OPERATOR_HEIGHT * DATA_WIDTH - 1 : DATA_WIDTH];  
                                                                        //再存回Ram的数据要把最新的一行移进去,最老的一行移出.低位为旧数据,也就是算子最上面一行的数据. Shift out the oldest data.
    always@(posedge clk)
    begin
      if(DataEn || OperatorDataEn)
        FrogCount     <= FrogCount + 1;                                 //计数用以产生读写Ram的地址. Generate Ram reading address
      else            
        FrogCount     <= 0;
      PixelDataReg    <= PixelData;                                     //由于读取Ram中之前缓存的数据会比新来的数据晚一个周期,所以要把新来的数据寄存一个周期好和读出的数据对齐
      OperatorDataReg <= {PixelDataReg,douta};                          //把读出的之前缓存的数据和新一行的数据合并成N行算子数据输出,这样输出的数据又延时了一个周期,共延时两个周期, 这一级寄存器可以取消不要。
      DataEnReg[0]    <= DataEn;                                        //所以要延时使能信号两个周期。如果OperatorDataReg这一级寄存器不要了，延时就为1个周期。
      DataEnReg[1]    <= DataEnReg[0];
    end 
    
endmodule
//不觉醒只有从韭菜升级为人血馒头命，推荐看看“推荐书籍”目录里的内容。