//B站无限次元: https://space.bilibili.com/2139404925  https://github.com/becomequantum/Kryon

using System.Collections;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions; //还是写C#舒服,这些using现在都不用自己写,用到了VS自动加的,写C++时自动提示不如C#


Console.WriteLine("--------C#性能测试---------\n");
const int 大小 = 10000_0000;

数组读写耗时测试();

正则表达式测试();
/*C#正则表达式匹配的速度还是挺快的,但输出结果比较慢,耗时首先和匹配到的结果数目成正比.
  如果输出结果很少,表达式里没用到|,速度和读字串相同长度的数组差不多
  (用)|(或)这样同时匹配多个词时,排除掉结果数的影响,测得耗时的确是和同时要匹配的词数成正比的. 
  Email,URL,IP这三个表达式测试反应出: IP用到了|,导致耗时多些.另外两个表达式看起来长,但似乎对增加耗时影响不大 */

字典性能测试();
/*只测了整型作为健值的情况,用字串作为健值速度会更慢些 
  C#字典<整型健值>的性能会随着条目的增加而下降,测试输出的"相对基准"指的是字典读写耗时和同容量数组读取耗时的比值.
  4千万条目这个值能到80多倍,只有几千条目时会降到个位数 */

Console.WriteLine("\n测试结束!");
Console.Beep();
Console.ReadLine();

//----主程序结束----
static void 正则表达式测试()
{
    const int 容量 = 1000_0000; //一千万
    StringBuilder 建串 = new(容量 + 4);//C#里字符串编码用的都是16位的Unicode.C++里字串的默认编码是老的ANSI.
    string 文本1 = "";
    Console.WriteLine("--C#正则引擎测试:\n");
    计时("创建长度1千万的字串 ".PadRight(24), 1, () =>
    {
        for (int i = 1; i <= 4; i++)
            建串.Append((char)('a' + i - 1), 容量 / 10 * i); //1百万个a,2百万个b,3百万个c,4百万个d
        建串.Append("无限次元");
    });//时间和写1亿32位数组时间相当
    文本1 = 建串.ToString();//这个步骤耗时为上面的一半不到,看来是进行了拷贝.

    short[] 数组16 = new short[容量]; short 读;
    double 基准 = 计时(容量 + "大小数组16位 读".PadRight(19), 5, () => { for (uint i = 0; i < 数组16.Length; i++) 读 = 数组16[i]; });//用做和正则匹配时间相比较的时间基准

    #region 匹配耗时和匹配到的结果数量正相关
    Console.WriteLine("\n字串长度: " + 文本1.Length);
    Regex 正则 = new("无限次元", RegexOptions.Compiled);
    MatchCollection 匹配结果 = null;
    计时("耗时正比结果数 ", 1, () => {//匹配"无限次元"只会有1个结果,速度非常快,比读同样大小的short数组还快,看来应该是用了2个线程.
        匹配结果 = 正则.Matches(文本1);
        Console.Write("结果数目: " + 匹配结果.Count.ToString().PadRight(8));
    }, 基准);//打印结果数目要写在这里面,C#正则似乎用了多线程,输出结果不写在里面会导致测不出来耗时

    string abcd = "abcd";
    foreach (char c in abcd) {//匹配a,b,c,d结果数量分别是1,2,3,4百万,耗时很多,所以C#里的正则引擎时间都耗在了记录结果上. 结果数为1时耗时很少,
        正则 = new(c.ToString(), RegexOptions.Compiled);
        计时("耗时正比结果数 ", 1, () => {
            匹配结果 = 正则.Matches(文本1);
            Console.Write("结果数目: " + 匹配结果.Count.ToString() + " ");
        }, 基准);
    }
    Console.WriteLine("\n");
    #endregion

    #region 用"|"同时匹配多词测试
    const int 个数 = 23000; //用|同时匹配这么多个字串,大概超过这个数会爆栈
    var 数组 = 随个数组(个数, 99999, 10000);
    建串.Clear();
    for (int i = 0; i < 数组.Length; i++) 建串.Append(数组[i].ToString() + "|");
    建串.Append("无限次元");
    string 文本2 = 建串.ToString();
    Console.WriteLine(string.Concat("待匹配文本: ", 文本2.AsSpan(0, 80), " .... \n"));
    Console.Write("字串长度: " + 文本2.Length);
    Console.WriteLine("  以下测试,同时匹配的词和结果都逐次递减");
    for (int i = 1; i <= 16; i *= 2) {
        正则 = new(string.Concat(文本2.AsSpan(0, 文本2.Length / i), "a"), RegexOptions.Compiled);
        计时(" 多词|匹配|测试 ", 1, () => {
            匹配结果 = 正则.Matches(文本2);
            Console.Write("结果数目: " + 匹配结果.Count.ToString().PadRight(5) + " ");
        });
    }
    Console.WriteLine();
    #endregion

    #region 同时匹配多词会增加耗时
    Console.WriteLine("以下测试,同时匹配的词成倍递减,结果只有一个");
    建串.Clear();
    for (int i = 0; i < 数组.Length; i++) 建串.Append(string.Concat(数组[i].ToString().AsSpan(0, 4), "a|")); //这样弄一下就只会有1个匹配结果
    建串.Append("无限次元");
    string 正则串 = 建串.ToString();
    for (int i = 1; i <= 16; i *= 2) {
        正则 = new(string.Concat("a", 正则串.AsSpan(文本2.Length - (文本2.Length / i))), RegexOptions.Compiled);
        计时(" 多词|匹配|测试 ", 1, () => {
            匹配结果 = 正则.Matches(文本2);
            Console.Write("结果数目: " + 匹配结果.Count.ToString().PadRight(5) + " ");
        });
    }




    Console.WriteLine();
    #endregion

    #region 复杂度和耗时关系
    Console.WriteLine("Email,URL,IP匹配测试");
    string[] 表达式 = { @"[\w\.+-]+@[\w\.-]+\.[\w\.-]+",                                         //Email
                        @"[\w]+://[^/\s?#]+[^\s?#]+(?:\?[^\s#]*)?(?:#[^\s]*)?",                  //URL
     @"(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9])\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9])"};//IP 耗时多些,里面有|
    文本2 = string.Concat(文本2, 文本2, @" https://github.com/becomequantum/Kryon ", @" becomequantum@qq.com " + @" 202.38.11.11 ");
    for (int i = 0; i < 表达式.Length; i++) {
        正则 = new(表达式[i], RegexOptions.Compiled);
        计时(" 复杂度和耗时测试 ", 1, () => {
            匹配结果 = 正则.Matches(文本2);
            Console.Write("结果数目: " + 匹配结果.Count.ToString().PadRight(5) + " ");
        });
    }
    Console.WriteLine("\n");
    #endregion

    #region 集合判断测试
    正则 = new(@"\w+", RegexOptions.Compiled);
    计时(@"\w大集合判断耗时 ", 1, () => {
        匹配结果 = 正则.Matches(文本1);
        Console.Write("结果数目: " + 匹配结果.Count.ToString().PadRight(8));
    });//\w判断更耗时
    正则 = new(@"[abcd无限次元]+", RegexOptions.Compiled);
    计时(@"[]小集合判断耗时 ", 1, () => {
        匹配结果 = 正则.Matches(文本1);
        Console.Write("结果数目: " + 匹配结果.Count.ToString().PadRight(8));
    });
    Console.WriteLine("\n");
    #endregion

    #region Python FlashText 对比测试
    Console.WriteLine("Python FlashText 对比测试: ");
    StreamReader 读文件 = new("input-text.txt"); 
    string 待测文本 = 读文件.ReadToEnd();
    HashSet<string> 查重 = new(20000);
    计时(@"匹配单词并去重耗时 ", 1, () => {
        匹配结果 = Regex.Matches(待测文本, @"\b[a-zA-Z][a-z]{3,6}\b", RegexOptions.Compiled);
        foreach (var 结果 in 匹配结果)
            查重.Add(结果.ToString());
        Console.Write("结果数目: " + 查重.Count.ToString().PadRight(8)); 
    });//178ms,18956个词,把词控制在这个数是因为C#同时匹配的词超过两万多之后会爆栈
    StringBuilder 或正则 = new(查重.Count);
    foreach (var 词 in 查重) 
        或正则.Append(词 + '|');
    或正则.Append('蛙');
    //计时(@"同时匹配上面这些词耗时 ", 1, () => {
    //    匹配结果 = Regex.Matches(待测文本, 或正则.ToString(), RegexOptions.Compiled);
    //    Console.Write("结果数目: " + 匹配结果.Count.ToString().PadRight(8));
    //});//47秒 428005个结果, 和Python Rust的都不一样
    #endregion
}
static void 字典性能测试()
{
    int 容量 = 33554432, 读;//4000_0000,
    Console.WriteLine("--C#字典测试:\n");
    int[] 随机数组 = null;
    计时(容量 / 1000_0000 + "千万个不重复随机数产生   ", 1, () => { 随机数组 = 随个数组(容量, 容量 * 2); });
    Console.WriteLine();
    Dictionary<int, int> 字典 = new(容量);//这里把预期容量写进去,在下面创建测试中会节省大概一半时间.
    for (int N = 1; N <= Math.Pow(4, 6); N *= 4)
    {
        double 基准 = 计时("用数组读取时间作为比较基准", 10, () => { for (uint i = 0; i < 容量 / N; i++) 读 = 随机数组[i]; }, 0, false);
        计时((容量 / N).ToString().PadRight(9) + "容量字典创建(随机健值)  ", 3, () => { for (int i = 0; i < 容量 / N; i++) 字典.TryAdd(随机数组[i], i); }, 基准);
        计时((容量 / N).ToString().PadRight(9) + "容量字典读              ", 3, () => { for (int i = 0; i < 容量 / N; i++) 读 = 字典[随机数组[i]]; }, 基准);
        字典.Clear();
        Console.WriteLine();
    }
    Console.WriteLine("小容量读取耗时精测:");
    int 重复次数 = 10000;
    for (int 小容量 = 8; 小容量 <= Math.Pow(2, 13); 小容量 *= 2)
    {
        var 总基准 = 计时("", 重复次数, () => { for (uint i = 0; i < 小容量; i++) 读 = 随机数组[i]; }, 0, false, true);
        var 创建 = 计时("", 1, () => { for (int i = 0; i < 小容量; i++) 字典.TryAdd(随机数组[i], i); }, 0, false, true);
        var 读取 = 计时("", 重复次数, () => { for (int i = 0; i < 小容量; i++) 读 = 字典[随机数组[i]]; }, 0, false, true);
        字典.Clear();
        Console.WriteLine("容量: " + 小容量.ToString().PadLeft(6) + " 读取相对基准: " + (读取 / 总基准).ToString(".##").PadLeft(6) + "  " + 重复次数.ToString() + "次总耗时: " + 读取 + " ms"); ;
    }
    Console.WriteLine("\n");
}
static int[]? 随个数组(int 长度, int 值上限, int 值下限 = 0)
{
    if (值上限 - 值下限 < 长度) { throw new Exception("随机值上下限之差要比数组长度大!"); }
    Random 随机 = new();
    int[] 数组 = new int[长度];
    BitArray 查重 = new(值上限);
    int i = 0;
    while (i < 长度)
    {
        var 随机数 = 随机.Next(值下限, 值上限);
        if (!查重[随机数])
        {
            数组[i] = 随机数; i++;
            查重[随机数] = true;
        }
    }
    return 数组;
}//返回数组里的数是随机的,但没有重复的.
static void 数组读写耗时测试()
{
    ulong 读 = 0;
    Console.WriteLine(大小 / 10000_0000 + "亿大小的整型数组读写耗时测试:\n");
    数组测速<byte>();
    数组测速<ushort>();
    数组测速<uint>();
    数组测速<ulong>();
    Console.WriteLine("\n");//在fixed中用指针对数组读写性能似乎并没明显提升.
    List<int> 列表 = new(大小); //没预留空间写大概会多出一倍时间
    Console.WriteLine(大小 / 10000_0000 + "亿大小的整型列表读写耗时测试:\n");
    计时("int列表 写 ", 1, () => { for (int i = 0; i < 大小; i++) 列表.Add(i); });
    计时("int列表 读 ", 1, () => { for (int i = 0; i < 大小; i++) 读 = (ulong)列表[i]; });
    Console.WriteLine("\n");
}

static void 数组测速<T>() where T : IBinaryInteger<T> {
    T[] 数组 = new T[大小];
    T n = T.Zero;
    for (int i = 0; i < 2; i++) //写两次,两次时间不一样.再写一次耗时变小了,和读差不多,为啥呢?
        计时(typeof(T).ToString() + " 写 ", 1, () => { for (uint i = 0; i < 大小; i++) 数组[i] = n; n += T.One; });
    计时(typeof(T).ToString() + " 读 ", 1, () => { for (uint i = 0; i < 大小; i++) n = 数组[i];  });
    Console.WriteLine();
}

static double 计时(string 说明, int 重复次数, Action 待测功能, double 基准耗时 = 0, bool 打印 = true, bool 返总 = false)
{
    var 开始时刻 = DateTime.Now;
    for (int i = 0; i < 重复次数; i++) 待测功能();
    var 总耗时 = (DateTime.Now - 开始时刻).TotalMilliseconds;
    var 耗时 = 总耗时 / 重复次数;
    if (!打印) return 返总 ? 总耗时 : 耗时;
    Console.Write(说明 + 重复次数 + "次平均: " + 耗时.ToString(".#####").PadLeft(11) + " ms");
    if (基准耗时 > 0) Console.Write("  相对基准: " + (耗时 / 基准耗时).ToString(".##") + "\n");
    else Console.Write("\n");
    return 返总 ? 总耗时 : 耗时;
}


