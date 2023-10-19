#pragma once
#include <iostream>
#include <array>
#include <algorithm>
#include <chrono>
#include <fstream>
#include <sstream>
#include <string>
#include <vector>
#include <queue>
#include <map>
#include <iomanip>  //std::setw
#include <Windows.h> 

//B站無限次元: https://space.bilibili.com/2139404925  https://github.com/becomequantum/Kryon  
//用了很大的数组,会报栈溢出,VS里修改:项目->属性->链接器->系统->堆栈保留大小 为1600000000(8个0) 就可以了

#define 宽词打印码 CP_ACP  //MSC编译转为ANSI
#if defined (__clang__)
#define 宽词打印码  CP_UTF8  //clang编译转为utf-8 https://zhuanlan.zhihu.com/p/660330347 <彻底搞清C++中各种字符编码：Unicode，utf-8，ANSI之间的相互转换和正确打印！>
#endif
#define 打印(宽词) Unicode转其它(宽词, 宽词打印码) //Unicode wstring 打印时需先转码
#ifdef _DEBUG
#define 调试打印(...) printf(__VA_ARGS__)
#else 
#define 调试打印(...)
#endif

#pragma region 词库处理
/// <summary>
/// Unicode编码wstring转为其它编码
/// </summary>
/// <param name="宽字串">Unicode编码的wstring</param>
/// <param name="输出编码"> 设为CP_UTF8 就转为utf-8, CP_ACP就转为ANSI</param>
/// <returns>返回其它编码的string</returns>
std::string Unicode转其它(const std::wstring& 宽字串, size_t 输出编码) {
    if (宽字串.empty()) return std::string();
    int 所需长度 = WideCharToMultiByte(输出编码, 0, &宽字串[0], (int)宽字串.size(), NULL, 0, NULL, NULL);
    std::string 输出字串(所需长度, 0);
    WideCharToMultiByte(输出编码, 0, &宽字串[0], (int)宽字串.size(), &输出字串[0], 所需长度, NULL, NULL);
    return 输出字串;
}//输入Unicode wstring, 输出编码为 CP_UTF8 就转为utf-8, CP_ACP就转为ANSI. 

/// <summary>
/// 把UTF-8编码的字串转为Unicode 16
/// </summary>
/// <param name="输入">为utf-8编码字串</param>
/// <returns>返回Unicode wstring</returns>
std::wstring UTF8转U16(const std::string& 输入) {//输入的string是utf-8, 输出wstring是Unicode
    if (输入.empty()) return std::wstring();
    int 宽串长度 = MultiByteToWideChar(CP_UTF8, 0, &输入[0], (int)输入.size(), NULL, 0);
    std::wstring 宽串(宽串长度, 0);
    MultiByteToWideChar(CP_UTF8, 0, &输入[0], (int)输入.size(), &宽串[0], 宽串长度);
    return 宽串;
}//https://stackoverflow.com/questions/215963/how-do-you-properly-use-widechartomultibyte

/// <summary>
/// 把带Bom的UTF-8文本文件前三个字节的Bom去掉, 不带的不影响
/// </summary>
/// <param name="文件名">输入文本文件需是UTF-8编码的</param>
/// <returns>返回已略过Bom的ifstream</returns>
std::ifstream 去UTF8的BOM(const char* 文件名) {
    std::ifstream 文件流(文件名);
    if (!文件流.good()) { std::cout << 文件名 << " 该文件不存在!"; return 文件流; }
    char 缓存[3] = { 0 };
    文件流.read(缓存, 3);
    if (缓存[0] != (char)0xEF || 缓存[1] != (char)0xBB || 缓存[2] != (char)0xBF) //(char)必需要加, 类型不同会比出问题
        文件流.seekg(0); //没有Bom就把文件流读起始位置再重设为0
    return 文件流;
}

/// <summary>
/// 根据输入的分隔符分割待分字串, 并把结果存在字串vector结果表中
/// </summary>
/// <param name="待分字串"></param>
/// <param name="结果表"></param>
/// <param name="分隔符"></param>
template<typename Tstr, typename Tchar>
void 字串分割(Tstr 待分字串, std::vector<Tstr>& 结果表, Tchar 分隔符) {
    size_t 起始位置 = 0;                        //这里的&必需加
    do {
        size_t 分隔位置 = 待分字串.find_first_of(分隔符, 起始位置);
        if (分隔位置 == std::string::npos)
            分隔位置 = 待分字串.length();//已经没换行了
        size_t 词长 = 分隔位置 - 起始位置;
        if (词长 > 0)  结果表.push_back(Tstr(待分字串, 起始位置, 词长));
        起始位置 = 分隔位置 + 1;
    } while (起始位置 < 待分字串.length());
}

/// <summary>
/// 读入词库文件, 转码为utf-16, 再用换行分割单词并存入词表
/// </summary>
/// <param name="词库文件名">要求文件内一行一个词, 不要有空格等其它非单词字符</param>
/// <returns>单词表</returns>
std::vector<std::wstring> 读入词库(const char* 词库文件名) {
    std::ifstream 文件流 = 去UTF8的BOM(词库文件名);
    std::vector<std::wstring> U16词表;
    if (!文件流.good()) return U16词表;
    U16词表.reserve(200000);
    std::ostringstream 输出串流;
    输出串流 << 文件流.rdbuf();
    const std::string 词库文本(输出串流.str());        //把词库全部内容读到这个字串里
    std::wstring U16词库 = UTF8转U16(词库文本); //然后把utf8编码转为Unicode 16
    字串分割(U16词库, U16词表, L'\n');
    文件流.close();
    return U16词表;
}

enum 语言 { 英文, 中文, 中英 };

struct 字符区间 {
    wchar_t 起, 终;
    size_t 长;
} constexpr 字母区间[] = { {L'a', L'z',L'z' - L'a' + 1}, {L'一', L'鿿', L'鿿' - L'一' + 1}, {L'a', L'鿿', L'鿿' - L'a' + 1} };//'一'是4e00, '鿿'是9FFF, Unicode汉字的起终字符

/// <summary>
/// 把词表里含有非某语言字符的词都去掉, 包括空格
/// </summary>
/// <param name="词表"></param>
/// <param name="词库语言"></param>
void 检查词库(std::vector<std::wstring>& 词表, 语言 语, bool 打印被删 = false) {
    if (词表.size() == 0) return;
    size_t 计数 = 0;
    for (auto 迭 = 词表.begin(); 迭 != 词表.end(); ) {
        bool 删 = false;
        for (auto 字 : *迭)
            if (字 < 字母区间[语].起 || 字 > 字母区间[语].终) {
                删 = true;
                计数++;
                if (打印被删) std::cout << 计数 << ": " << 打印(*迭) << std::endl;
                break;
            }
        if (删)
            迭 = 词表.erase(迭); //erase后iter会自动加一
        else
            迭++;
    }
    if (计数 > 0) std::cout << 计数 << "个含有非单词字符(包括空格)的词被删掉了!" << std::endl;
}

/// <summary>
/// 对单词进行排序, 并把相同的词去掉
/// </summary>
/// <param name="词表"></param>
void 排序去重(std::vector<std::wstring>& 词表) {
    if (词表.size() <= 1) return;
    std::sort(词表.begin(), 词表.end());
    for (auto 迭 = 词表.begin(); 迭 != 词表.end() - 1; ) {
        size_t 重复个数 = 0;
        for (auto 代 = 迭 + 1; 代 != 词表.end(); 代++)
            if (*迭 == *代)
                重复个数++;
            else
                break;
        if (重复个数 > 0)
            迭 = 词表.erase(迭, 迭 + 重复个数); //[first, last)
        else
            迭++;
    }
}

void 词表写回文本(std::vector<std::wstring>& 词表, const char* 文件名) {
    std::ofstream 文本输出(文件名);
    for (auto 词 : 词表)
        文本输出 << Unicode转其它(词, CP_UTF8) << std::endl;
}

size_t 词表乱序指数(std::vector<std::wstring>& 词表) {
    size_t 不连续计数 = 0; //字串vector中, 不论咋整, 大概都有50%字符存储地址不连续
    for (size_t i = 0; i < 词表.size() - 1; i++) {
        auto d = 词表[i + 1].c_str() - 词表[i].c_str();
        if (d < 0 || d > 64) 不连续计数++;
    }
    return 不连续计数;
}//查看词表中字符存储地址的混乱情况

size_t 相同前缀长(std::wstring 串1, std::wstring 串2) {
    auto 短 = min(串1.length(), 串2.length());
    if (短 == 0) return 0;
    for (size_t i = 0; i < 短; i++)
        if (串1[i] != 串2[i]) return i;
    return 短;
}
#pragma endregion

#pragma region 字典树
/// <summary>
/// 把要计时的代码放入大括号中, 第一行例化一个计时器即可计时
/// </summary>
class 计时器 {
private:
    std::chrono::high_resolution_clock::time_point 起点;
    std::string info;
public:
    计时器(std::string 说明 = "") { //创建时如果不想加名字,后面就不要跟();
        info = 说明;
        起点 = std::chrono::high_resolution_clock::now();

    }
    ~计时器() {
        auto 终点 = std::chrono::high_resolution_clock::now();
        std::chrono::duration<double, std::ratio<1, 1000>> 耗时毫秒(终点 - 起点);
        std::cout << info << "耗时:" << 耗时毫秒.count() << "毫秒" << std::endl;
    }
};

using 查找表 = std::map<int, int>;

struct 横表哈希节点 {
    int 词序号 = -1;
    int 失败指针 = -1; //用于AC(Aho-Corasick)自动机
    查找表 下个节点;

    void 获取健值表(std::vector<std::pair<int, int>>& 健值表) {
        for (auto 健值 : 下个节点)
            健值表.push_back(健值);
    }
};

template<size_t 字母个数>
struct 横表数组节点 {
    int 词序号 = -1;  //非终结状态等于-1, 终结状态等于该词在词表中的序号
    int 失败指针 = -1;
    int 下个节点[字母个数] = { 0 };
    
    void 获取健值表(std::vector<std::pair<int, int>>& 健值表) {
        for (size_t i = 0; i < 字母个数; i++) 
            if (下个节点[i] != 0) //剪枝字典树中下个状态号有可能会是负值
                健值表.emplace_back(i, 下个节点[i]);
    }

    void 拷贝(横表哈希节点 源) {
        词序号 = 源.词序号;
        for (size_t i = 0; i < 字母个数; i++) 
            下个节点[i] = 源.下个节点[i];
    }
};

struct 双数组 {
    int next = INT32_MIN, check = -1, 词序号 = -1;
};//用于双数组法压缩字典树状态转换表

template<typename T节点>
void 打印横向状态表(size_t 节点表长, 语言 语种, T节点* 总节点表, std::vector<std::wstring> 词表, size_t N = 0) {
    constexpr size_t 对齐宽度 = 5;
    if (N == 0) N = 节点表长; else N = min(N, 节点表长);
    std::cout << "\n输入:";
    for (auto ch = 字母区间[语种].起; ch <= 字母区间[语种].终; ch++) //在一行打印字母, 也就是状态装换表中的输入维度
        std::wcout << std::setw(对齐宽度) << ch;
    std::cout << std::setw(对齐宽度) << "Fail";  //失败指针
    std::cout << "\n状态\n"; //以上在打印表头
    std::vector<std::pair<int, int>> 健值表;
    健值表.reserve(字母区间[语种].长); //用于临时存放一行表的健值
    for (size_t 节点号 = 0; 节点号 <= N; 节点号++) {
        std::cout << std::setw(5) << 节点号; //状态节点编号
        总节点表[节点号].获取健值表(健值表);
        size_t 健值索引 = 0;
        for (size_t ch = 0; ch < 字母区间[语种].长; ch++) {
            auto 下个状态号 = 0;
            if (健值索引 < 健值表.size() && ch == 健值表[健值索引].first) {
                下个状态号 = 健值表[健值索引].second; //first:输入, second:下个状态号
                健值索引++;
            }
            if (下个状态号 != 0)
                std::cout << std::setw(对齐宽度) << 下个状态号;
            else
                std::cout << std::setw(对齐宽度) << " ";
        }
        std::cout << std::setw(5) << 总节点表[节点号].失败指针;
        if (总节点表[节点号].词序号 >= 0) //如果这个节点是终结状态, 就把它对应的词也打出来
            std::cout << " " << 打印(词表[总节点表[节点号].词序号]);
        std::cout << "\n";
        健值表.clear();
    }
    std::cout << "  总表长:" << 节点表长 + 1<< "\n";
}

template<typename T节点>
void 双数组压缩(size_t 节点表长, 语言 语种, T节点* 总节点表, std::vector<int>& base, std::vector<双数组>& next_check) {
    base.reserve(节点表长 + 1); //算法介绍: https://www.linux.thai.net/~thep/datrie/
    next_check.reserve(max(节点表长 + 1, 字母区间[语种].长));
    std::vector<std::pair<int, int>> 健值表;
    健值表.reserve(字母区间[语种].长); //用于临时存放一行表的健值
    for (size_t i = 0; i <= 节点表长; i++) //先把base塞满, 后面好直接base[]这样索引
        base.push_back(INT32_MIN);
    for (size_t 当前状态节点 = 0; 当前状态节点 <= 节点表长; 当前状态节点++) {
        总节点表[当前状态节点].获取健值表(健值表);
        if (健值表.size() == 0) continue;
        int 偏置 = -字母区间[语种].终; //偏置初值设为负的是为了让base数组最开始的空间能被用上
        while (true) {//一个个的试"当前状态节点"可以用的偏置值
            for (size_t i = 0; i < 健值表.size(); i++) {
                int 双数组索引 = 偏置 + 健值表[i].first + 'a'; //base[] + c(输入)
                if(双数组索引 >= (int)next_check.size())        //next_check数组不够大就先扩容
                    for (size_t j = next_check.size(); j <= 双数组索引 + 2; j++) 
                        next_check.emplace_back(INT32_MIN, -1, -1);
                if (双数组索引 < 0 || next_check[双数组索引].check != -1) goto 下个偏置; //check != -1意味着该位置已经被占用
            }//这个循环结束了,没跳到"下个偏置",意味着当前"偏置"值可用, 用它加上输入得到的索引位置都还没被占用
            base[当前状态节点] = 偏置;
            for (size_t i = 0; i < 健值表.size(); i++) {
                int 双数组索引 = 偏置 + 健值表[i].first + 'a';
                next_check[双数组索引].next = 健值表[i].second; //first是输入, second是下个状态节点号
                next_check[双数组索引].check = 当前状态节点;   //check用于确认当前索引上的下个状态号的确是当前状态的某个下个状态
                next_check[双数组索引].词序号 = 总节点表[健值表[i].second].词序号; //放下个状态的词序号, 用于查词
            }
            break; //加塞成功了就跳出while循环
        下个偏置:
            偏置++;
        }
        健值表.clear();
    }
   
    //size_t 未用空间 = 0; 
    //   printf("\n");//打印双数组内容
    //for (size_t i = 0; i < next_check.size(); i++) {
    //    if (next_check[i].check < 0) {
    //        未用空间++; printf("\n"); continue;
    //    }
    //    printf("%5d, %5d, %5d\n", next_check[i].next, next_check[i].check, next_check[i].词序号);
    //}
    //printf("未用空间: %d 浪费率: %f", 未用空间, (float)未用空间/ next_check.size()); //浪费率惊人的小
}

struct 查词返回 {
    int 节点号 = 0;
    int 词序号 = -1;
    size_t 字母号 = 0; //查到哪个字母了
};

static constexpr size_t 节点表容量 = 500000;

template<typename T节点>
class 横表字典树 {
private:
    T节点 总节点表[节点表容量];
    //横表数组节点<26> 总节点表[节点表容量]; //写代码时把上一行和上面的 template<>注释掉, 用这一行的具体类型, 代码好写些
    size_t 节点表长 = 0;  //实际被使用的数量
    语言 语种;
    std::vector<std::wstring> 词表;
    bool 是哈希节点;
    std::vector<int> base;  //用于双数组压缩, 这里其实用了三个数组, "双数组"指的是把base和next合并进了一个数组, 道理都一样
    std::vector<双数组> next_check; 
    friend class 剪枝压缩字典树;

    void 构造() {
        if (语种 != 中英) {
            检查词库(词表, 语种); //如果语种是英文, 它会确保词库中不会出现除了26个小写字母之外的字符
            排序去重(词表);
        }
        for (size_t 词序号 = 0; 词序号 < 词表.size(); 词序号++) {
            auto 结果 = 查词(词表[词序号]); //加一个词之前需先查, 返回结果会告诉该词在表中能查到第几个字母和查到哪个节点
            if (结果.词序号 >= 0) continue;  //已经查到这个词就不用重复添加了
            if (结果.字母号 == 词表[词序号].length()) {
                总节点表[结果.节点号].词序号 = 词序号; //用于abc, ab 这种添加词序, 先加了abc, 再加ab时就会遇到这种情况
                continue; //如果词库排序了, 这个判断可以不需要.
            }
            for (size_t n = 结果.字母号; n < 词表[词序号].size(); n++) { //结果.字母号 指向还没查到的字母, 从它开始添加
                节点表长++;
                if (节点表长 >= 节点表容量) { std::cout << "节点表容量不够了" << std::endl; return; }
                总节点表[结果.节点号].下个节点[词表[词序号][n] - 字母区间[语种].起] = 节点表长; //结果.节点号 在一开始是上个能查到的节点号
                总节点表[节点表长].失败指针 = 结果.节点号; //"失败指针"先指向父节点, 方便后续AC自动机的构建
                结果.节点号 = 节点表长;
            }
            总节点表[结果.节点号].词序号 = 词序号; //节点都加完之后要把词序号赋值给最后一个节点, 示意它是终结节点.
        }

    }

    void 广优构造AC() {
        std::queue<std::pair<int, int>> 节点队列;  
        节点队列.emplace(0, 0);
        while (节点队列.size() > 0) {
            auto 输入和节点 = 节点队列.front(); //Get一个节点.  
            auto 输入 = 输入和节点.first, 节点 = 输入和节点.second; //first是输入, second是节点号
            auto 父节点 = 总节点表[节点].失败指针;  //构造字典树时, "失败指针"被赋值为它的父节点号.
            //调试打印( "(%c, %d, 父: %d) ", 输入 + 'a', 节点, 父节点);
            if (!(节点 == 0 || 父节点 == 0)) {//AC构造开始. 0节点和一阶节点不用处理. 如果父节点是0,它就是一阶节点, 它的失败指针就应该指向0, 不用处理
                auto 父败针节点 = 总节点表[父节点].失败指针;
                总节点表[节点].失败指针 = 0; //如果下面的过程, 找到0节点都没找到就指向0. 找到了会覆盖这个赋值
                while (父败针节点 >=0) { //0节点的失败指针是-1
                    if (总节点表[父败针节点].下个节点[输入] > 0) { //父节点的失败指针指向的节点有和当前"输入"相同的子节点
                        总节点表[节点].失败指针 = 总节点表[父败针节点].下个节点[输入]; //此时当前节点的失败指针就指向该节点
                        break;
                    }
                    父败针节点 = 总节点表[父败针节点].失败指针; //如果上一步没找到, 就继续找"父败针节点"的失败指针指向的节点, 看它有没有
                }
            }//AC构造结束
            for (size_t 输入 = 0; 输入 < 字母区间[语种].长; 输入++) {
                auto 节点 = 总节点表[输入和节点.second].下个节点[输入]; //这样读Map, 会导致它创建条目,导致bug
                if (节点 > 0) 节点队列.emplace(输入, 节点);
            }
            //调试打印("%d \n", 总节点表[节点].下个节点.size());
            节点队列.pop(); 
        }
    }//Aho-Corasick

public:
    横表字典树(std::vector<std::wstring>& 表, 语言 语) {
        语种 = 语;
        词表 = 表;
        是哈希节点 = typeid(T节点) == typeid(横表哈希节点);
        构造();
        if(!是哈希节点)
            广优构造AC(); //AC构造代码目前还不适用于哈希表作为下个节点表
    };

    查词返回 查词(std::wstring 词) {
        查词返回 结果; //"结果.节点号"初值为0, 所以每个词首字母查的都是'总节点表'中第0个节点
        for (size_t i = 0; i < 词.length(); i++) {
            auto 下个节点 = 总节点表[结果.节点号].下个节点[词[i] - 字母区间[语种].起]; //英文 '- 字母区间[语种].起' 就是 - L'a'
            //_mm_prefetch((char*)&总节点表[下个节点] , _MM_HINT_T0); 然并卵
            结果.字母号 = i;
            if (下个节点 == 0)
                return 结果; //查不到就返回, 此时"结果.字母号"记录的是首个没查到的字母序号, "节点号"记录的是上个查到的节点, 这些信息会用于字典树的构建
            else
                结果.节点号 = 下个节点; //能查到才给它赋值, 用于记录上个能查到的节点
            if (下个节点 != 0 && i == 词.length() - 1 ) {
                结果.字母号 = 词.length(); 
                结果.节点号 = 下个节点; //用于abc, ab 这种添加词序
            }
        }
        
        结果.词序号 = 总节点表[结果.节点号].词序号; //每个字母都查到了就要返回词序号, 此时该节点如果是终结节点, 词序号就会是该词在词库中的序号
        return 结果;
    }

    void 压缩() {
        双数组压缩<T节点>(节点表长, 语种, 总节点表, base, next_check);
    }

    void AC查词() {
        throw "先懒得写了";
    }

    int 双数组查词(std::wstring 词) {
        int 下个节点 = 0, 索引 = 0;
        for (size_t i = 0; i < 词.length(); i++) {
            索引 = base[下个节点] + 词[i];
            if (索引< 0 || next_check[索引].check != 下个节点)
                return -1;
            下个节点 = next_check[索引].next;
        }
        return next_check[索引].词序号; //如果一直查到了最后一个字, 词序号就存在最后那个next_check[索引]里
    }

    void 打印状态表(size_t N = 0) {
        打印横向状态表<T节点>(节点表长, 语种, 总节点表, 词表, N);
    }

    void 打印树信息() {
        auto 表长 = 节点表长 + 1;
        size_t 总字母数 = 0;
        for (auto 词 : 词表) 总字母数 += 词.size();
        auto 词均字数 = (float)总字母数 / 词表.size();
        std::cout << "词数:" << 词表.size() << "  总字母数:" << 总字母数 << "  词均字数:" << 词均字数;
        std::cout << "  总状态节点数:" << 表长;
        if (是哈希节点) { std::cout << std::endl; return; }
        size_t 分支节点数 = 0;
        for (size_t i = 0; i < 表长; i++) {
            size_t 一行占用 = 0;
            for (size_t ch = 0; ch < 字母区间[语种].长; ch++)
                if (总节点表[i].下个节点[ch] > 0) 一行占用++;
            if (一行占用 > 1) 分支节点数++;
        }
        std::cout << " 分支节点数: " << 分支节点数 << " 占比:" << 分支节点数 / (float)表长 * 100 << "%  ";
        std::cout << "数组利用率: 这个就恒等于 1/26" << std::endl;
    }
};

class 纵表哈希字典树 {
private:
    查找表 状态转换表[UINT16_MAX]; //这个<int,int>哈希表,查一个不存在的key时, 会返回0, 正好满足需求. 所以0不能作为一个有别的用途的值, 只能用于指示没查到
    //int 状态转换表[123][300000];
    int 节点查词[节点表容量] = { -1 };
    size_t 节点表长 = 0;
public:
    纵表哈希字典树(std::vector<std::wstring>& 词表) {
        for (size_t 词序号 = 0; 词序号 < 词表.size(); 词序号++) {
            auto 结果 = 查词(词表[词序号]);
            for (size_t n = 结果.字母号; n < 词表[词序号].size(); n++) {
                节点表长++;
                if (节点表长 >= 节点表容量) { std::cout << "节点表容量不够了" << std::endl; return; }
                状态转换表[词表[词序号][n]][结果.节点号] = 节点表长;
                结果.节点号 = 节点表长;
            }
            节点查词[结果.节点号] = 词序号;
        }
    }

    查词返回 查词(std::wstring 词) {
        查词返回 结果;
        for (size_t i = 0; i < 词.length(); i++) {
            结果.字母号 = i;
            auto 下个节点 = 状态转换表[词[i]][结果.节点号];//纵表字典树查询时, [][]这两个里面的索引和横表是反的, 这里是输入的字符作为第一个索引
            if (下个节点 == 0)
                return 结果;
            else
                结果.节点号 = 下个节点;
        }
        结果.词序号 = 节点查词[结果.节点号];
        return 结果;
    }

    void 打印表信息() {
        for (wchar_t i = 0; i < UINT16_MAX; i++)
            if (状态转换表[i].size() > 0)//在这项统计中, 英文j下面的状态最少, j在英文单词中的出现非常上下对齐
                std::cout << std::setw(5) << (uint16_t)i << ": " << 状态转换表[i].size() << std::endl;
    }

};

constexpr size_t 分表容量 = 100000;
constexpr size_t 尾表容量 = 200000;
constexpr size_t 节点组容量 = 32;

struct 尾枝节点 {
    uint16_t 起点 = 0; //线性分支的起点
    int 终结词序号 = -1; //尾枝终结节点上的词序号
    int 词序号[节点组容量] = { -1 }; //尾枝上可能终结了不止一个词
};

class 剪枝压缩字典树 {
    struct 遍历信息 {
        int 节点 = 0;
        int N = 0;
        int 尾枝起始 = -1;
        std::pair<int, int> 输入节点组[节点组容量] = { {0,0} };//用于记录遍历过的输入和节点号
        查找表 旧查新;
    };

private:
    横表数组节点<字母区间[英文].长> 分支表[分表容量]; //有分支的节点
    尾枝节点 尾枝表[尾表容量];  //无分支, 直到终结的节点. 为了省事, 这里依然弄了个固定大小
    语言 语种;
    std::vector<std::wstring> 词表;
    size_t 分支表长 = 0;
    int 尾枝表长 = 1; //尾枝从1计起
    std::vector<int> base;  //用于双数组压缩, 这里其实用了三个数组, "双数组"指的是把base和next合并进了一个数组, 道理都一样
    std::vector<双数组> next_check;

    void 深优剪枝压缩(横表哈希节点* 旧节点表, 遍历信息& 信息) {//&引用不能忘, 深度优先遍历一个哈希表字典树, 并进行剪枝压缩
        auto 当前表大小 = 旧节点表[信息.节点].下个节点.size(); //"信息.节点"初值为0, 最初的"当前表大小"就是0节点的大小
        if(当前表大小 > 0)
            for (auto 下个 : 旧节点表[信息.节点].下个节点) {
                if ((当前表大小 > 1) && 旧节点表[下个.second].下个节点.size() <= 1)   //当前表大小大于1就是分支节点, 下一个若不是, 那就有可能是尾支的起点
                    信息.尾枝起始 = 信息.N;
                信息.输入节点组[信息.N] = 下个; //深度遍历的过程中把遍历到的节点信息记录下来, 遍历到终点时再进行剪枝, 拷贝操作
                信息.N++; if (信息.N >= 节点组容量) { std::cout << "遍历信息中的节点组容量不够了!" << std::endl; return; }
                信息.节点 = 下个.second;
                深优剪枝压缩(旧节点表, 信息);
            }
        else {
            for (size_t i = 信息.尾枝起始; i < 信息.N; i++) 
                尾枝表[尾枝表长].词序号[i] = 旧节点表[信息.输入节点组[i].second].词序号;
            尾枝表[尾枝表长].终结词序号 = 旧节点表[信息.输入节点组[信息.N - 1].second].词序号;
            尾枝表[尾枝表长].起点 = 信息.尾枝起始 + 1;
            for (int i = 0; i < 信息.尾枝起始; i++) {
                auto 旧节点号 = 信息.输入节点组[i].second; //first是输入, second是节点号
                if (信息.旧查新[旧节点号] == 0) { //没查到意味着该分支节点还没被复制, 复制之前必须查一下
                    分支表[分支表长].拷贝(旧节点表[旧节点号]);
                    //复制之后要先找到它在旧表中的父节点, 再找该父节点在新"分支表"中的拷贝, 然后连接上
                    if (i == 0) //最前的这个, 父节点号就是0, 在新表中也是0
                        分支表[0].下个节点[信息.输入节点组[i].first] = 分支表长; //"分支表长"就是新节点号
                    else //树里面的节点只有一个父节点, 深度遍历中记录下的前一个节点就是它的父节点
                        分支表[信息.旧查新[信息.输入节点组[i - 1].second]].下个节点[信息.输入节点组[i].first] = 分支表长;//注意"输入节点组[]"里的索引一个是i - 1,一个是i

                    信息.旧查新[旧节点号] = 分支表长;
                    分支表长++;
                    if (分支表长 >= 分表容量) { std::cout << "分支表容量不够用了" << std::endl; return; }
                }
            }
            //尾枝之前的分支节点拷贝完之后, 再把新的尾枝和它前一个有分支的父节点连上
            if (信息.尾枝起始 == 0)  //0节点之后就没有分支的情况. 还有字典树里就一个词的情况
                分支表[0].下个节点[信息.输入节点组[0].first] = -尾枝表长;
            else if(信息.尾枝起始 > 0)
                分支表[信息.旧查新[信息.输入节点组[信息.尾枝起始 - 1].second]].下个节点[信息.输入节点组[信息.尾枝起始].first] = -尾枝表长; //新"分支表"中, 尾枝节点的编号用负数, 以示区别
            尾枝表长++;
            if (尾枝表长 >= 尾表容量) { std::cout << "尾支表容量不够用了" << std::endl; return; }
        }
        信息.N--;
    }

public:
    剪枝压缩字典树(横表字典树<横表哈希节点>& 哈希树) {
        语种 = 哈希树.语种;
        词表 = 哈希树.词表;
        遍历信息 信息;
        //信息.旧查新.reserve(分表容量);
        分支表[0].拷贝(哈希树.总节点表[0]); 分支表长++;
        深优剪枝压缩(哈希树.总节点表, 信息);
    };
    void 打印状态表(size_t N = 0) {
        打印横向状态表<横表数组节点<字母区间[英文].长>>(分支表长, 语种, 分支表, 词表, N);
        for (int i = 1; i < 尾枝表长; i++) {
            //std::cout << -i << ": " << 打印(词表[尾枝表[i].词序号]) << ", ";
        }
        std::cout << "\n";
    }

    void 压缩() {
        双数组压缩<横表数组节点<字母区间[英文].长>>(分支表长, 语种, 分支表, base, next_check);
    }

    int 双数组查词(std::wstring 词) {
        int 下个节点 = 0, 索引 = 0;
        for (size_t i = 0; i < 词.length(); i++) {
            索引 = base[下个节点] + 词[i];
            if (索引 < 0 || next_check[索引].check != 下个节点)
                return -1;
            下个节点 = next_check[索引].next;
            if (下个节点 < 0) { //下个节点是尾枝节点, 代码就会在下面某处返回, 不会再继续上面的for循环
                if (i == 词.length() - 1) //最后一个字查到分支表
                    return 尾枝表[-下个节点].词序号[i];
                auto& 待比词 = 词表[尾枝表[-下个节点].终结词序号];
                if (词.length() <= 待比词.length()) {
                    size_t j = 0;
                    for (j = 尾枝表[-下个节点].起点; j < 词.length(); j++)
                        if (词[j] != 待比词[j])
                            return 尾枝表[-下个节点].词序号[j - 1]; //有字母不同
                    return  尾枝表[-下个节点].终结词序号; //剩下的都一样了就返回词序号
                }
                else
                    return -1; //长度比 待比词 大
            }
        }
        return next_check[索引].词序号;
    }

    int 查词(std::wstring 词) {
        int 下个节点 = 0;
        for (size_t i = 0; i < 词.length(); i++) {
            下个节点 = 分支表[下个节点].下个节点[词[i] - 字母区间[语种].起];
            if (下个节点 < 0) { //下个节点是尾枝节点, 代码就会在下面某处返回, 不会再继续上面的for循环
                if (i == 词.length() - 1) //最后一个字查到分支表
                    return 尾枝表[-下个节点].词序号[i];
                auto& 待比词 = 词表[尾枝表[-下个节点].终结词序号];
                if (词.length() <= 待比词.length()) {
                    size_t j = 0;
                    for ( j = 尾枝表[-下个节点].起点; j < 词.length(); j++)
                        if (词[j] != 待比词[j])
                            return 尾枝表[-下个节点].词序号[j - 1]; //有字母不同
                    return  尾枝表[-下个节点].终结词序号; //剩下的都一样了就返回词序号
                }
                else
                    return -1; //长度比 待比词 大
            }
        }
        
        return 分支表[下个节点].词序号; //一直没进入尾枝, 就返回最后一个分支节点的词序号
    }
};

void 字典树性能测试() {
    auto 词表 = 读入词库("dictionary.txt");//https://github.com/hankcs/AhoCorasickDoubleArrayTrie/tree/master/src/test/resources/en
    查词返回 结果;
    横表字典树<横表数组节点<字母区间[英文].长>>  数组树(词表, 英文);
    数组树.打印树信息();
    std::cout << "\n双数组压缩大概需要30秒......\n";
    数组树.压缩();
    std::sort(词表.begin(), 词表.end(), [](std::wstring a, std::wstring b) {return a.size() > b.size(); }); //打乱词序, 对耗时有影响
    size_t 重复次数 = 10;
    {
        计时器 计时("英词 数组树查询10次: ");
        for (size_t j = 0; j < 重复次数; j++)
            for (size_t i = 0; i < 词表.size(); i++)
                结果 = 数组树.查词(词表[i]);
    }
    {
        计时器 计时("英词 双数组查询10次: ");
        for (size_t j = 0; j < 重复次数; j++)
            for (size_t i = 0; i < 词表.size(); i++)
                auto 结果 = 数组树.双数组查词(词表[i]);
    }
    std::cout << std::endl;
    横表字典树<横表哈希节点> 横表哈希树(词表, 英文); //英文哈希表树创建慢了一倍, 查也慢了一倍
    {
        计时器 计时("英词 哈希树查询10次: ");
        for (size_t j = 0; j < 重复次数; j++)
            for (size_t i = 0; i < 词表.size(); i++)
                结果 = 横表哈希树.查词(词表[i]);
    }
    std::cout << std::endl;
    剪枝压缩字典树 剪枝树(横表哈希树);
    剪枝树.压缩();
    {
        计时器 计时("英词 乱序剪枝树查询10次: ");
        for (size_t j = 0; j < 重复次数; j++)
            for (size_t i = 0; i < 词表.size(); i++)
                auto 结果 = 剪枝树.查词(词表[i]);
    }
    {
        计时器 计时("英词 乱序剪枝双数组查询10次: ");
        for (size_t j = 0; j < 重复次数; j++)
            for (size_t i = 0; i < 词表.size(); i++)
                auto 结果 = 剪枝树.双数组查词(词表[i]);
    }
    std::cout << std::endl;
}
#pragma endregion

int main() {
#if defined (__clang__)
    SetConsoleOutputCP(65001);
    std::cout << "clang编译\n";
#endif
    std::vector<std::wstring> 字串组 = {L"pool", L"prepare",L"preview", L"prize",  L"produce",L"progress"};
    std::vector<std::wstring> AC测试1 = { L"he", L"hers",L"his",L"she"};
    横表字典树<横表数组节点<字母区间[英文].长>> 横英树(字串组, 英文);
    横表字典树<横表数组节点<字母区间[英文].长>> AC树(AC测试1, 英文);
    横英树.打印状态表();
    AC树.打印状态表();
    std::cout << std::endl << "14万英文词库字典树信息: \n";
    字典树性能测试();
}
//英文词库127142个单词, 1049729个字母(最多这么多个状态,实际状态数:264177), 平均一个单词8.256个字母, 二维表大概要 1049729 * 26 * 4 = 109171816字节(109MB)
//中文词库大概399615个字, 149682个词, 一个词平均2.61个字. 中英总共276824个词
//Unicode汉字范围4E00-9FFF  20992个字 
