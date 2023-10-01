#B站无限次元: https://space.bilibili.com/2139404925  https://github.com/becomequantum/Kryon
import std/monotimes
import std/tables
from times import inMicroseconds
import strformat
#import std/re     # std正则库, 这个不好用
import pkg/regex   #用的是这个:https://github.com/nitely/nim-regex 它需要这个:https://github.com/nitely/nim-unicodedb 从Git下载下来之后,在它们的文件目录打开命令行, 运行nimble install即可安装
import std/unicode

echo "---Nim性能测试:\n"

block 数组读写测试:
    const 大小:int32 = 10000_0000  #const 修饰编译时常量 把数组测试放在函数里时,大小到60万就会出bug
    echo "Nim数组读写测试:\n"
    var
        数组:array[大小, int32]     #定义的时候不赋值,以后再赋值要用var,let代表常量
        读:int

    var 开始 = getMonotime()
    for i in low(数组)..high(数组):
        数组[i] = i
    var 耗时 = float64((getMonotime() - 开始).inMicroseconds) / 1000.0  #div除的是整数, /除浮点数
    echo "1亿int数组写耗时: ", 耗时, " ms"   # 开了release 115ms, 写比C慢, C堆数组写耗时大概70ms; 读20ms, 和C差不多. 换成byte 写36ms 读5.2ms,和C差不多. int在64位系统里应该是int64

    开始 = getMonotime()
    for i in low(数组)..high(数组):
        读 = 数组[i]
    耗时 = float64((getMonotime() - 开始).inMicroseconds) / 1000.0
    echo "1亿int数组读耗时: ", 耗时, " ms\n\n"   # 读20ms 和C一样

block 哈希表测试:
    echo "Nim哈希表测试:\n"
    var
        表 = initTable[int,int]()
        读:int
    var 开始 = getMonotime()
    for i in 0..33554432:
        表[i] = i
    var 耗时 = float64((getMonotime() - 开始).inMicroseconds) / 1000.0  
    echo "3千万条目添加耗时: ", 耗时, " ms"   
    开始 = getMonotime()
    for i in 0..33554432:
        读 = 表[i]
    耗时 = float64((getMonotime() - 开始).inMicroseconds) / 1000.0  
    echo "3千万条目读取耗时: ", 耗时, " ms\n\n"   #顺序添加和读取, 添加5276ms,读3052ms,还不如c++ std的unordered_map


proc 正则测速(字串:string, 正则字串:string) =
    let 正则 = re2(正则字串)
    var 开始 = getMonotime()
    let 匹配结果 = 字串.findAll(正则)
    let 结果数 = len(匹配结果)
    var 耗时 = float64((getMonotime() - 开始).inMicroseconds) / 1000.0
    echo "结果数: ", fmt"{结果数:8}", " 耗时: ", 耗时, " ms"


block 正则引擎测试:
    const 容量 = 1000_0000; #一千万
    echo "Nim正则引擎测试:"
    let 一 = "一"
    echo "Nim内部字串是utf-8编码 '一'的码:", fmt"{int(一[0]):#X}", " ", fmt"{int(一[1]):#X}", " ",fmt"{int(一[2]):#X}","\n" # E4 B8 80 utf-8编码, C#内部字串是两字节的Unicode C++还是古老的ANSI
    let 匹配结果 = "a匹配中文行不行呢".findAll(re2(r"中文"))
    echo 匹配结果, " ", "不用Unicode,匹配结果中的起终点是按字节来的,'中'是第三个字符,但起始位置是7,因为a占一个字节,'匹配'占了6个字节(UTF-8)" 
    let U16 = toRunes("一丁的Unicode码: ")  #这个函数能把字串转成Unicode数组,"一丁"是Unicode汉字中最前面的两个字, '一'的码是0x4e00
    echo U16, fmt"{int(U16[0]):#X}", " ",fmt"{int(U16[1]):#X}"
    var #下面要跟多行这里要空行
        字串 = newStringOfCap(容量 + 1)  #创建一个预留大小的字串
        正则字串 = "a"

    for i in 1..4:
        for j in 1..容量 div 10 * i:   # [开始 .. 结束]这是个闭区间
            字串.add(char(96 + i))     # N个a,2N个b,3N个c,4N个d
    字串.add('e')
    echo "\n字串长度: ", 容量 div 10000, "万 耗时正比结果数测试:"
    for i in 'a'..'e':
        正则字串[0] = char(i)
        正则测速(字串, 正则字串) #这个测试同样表现出了结果越多耗时越多的现象,很多结果时速度比C#慢了2.8倍左右!,只有一个结果时耗时差不多

    echo "\n用或|同时匹配多词测试: "
    正则测速(字串, r"e")      # 一个时,时间是正常的,和C#耗时差不多,基本也是把这些数据读一遍的时间
    正则测速(字串, r"e|f")    # 多一个耗时暴增啊,这个库水平不行
    正则测速(字串, r"e|f|g")  
    



    



echo "\n测试结束"
#let C1111:string = readLine(stdin) 




    