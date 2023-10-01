//B站无限次元: https://space.bilibili.com/2139404925  https://github.com/becomequantum/Kryon  
#pragma once 
#include <iostream>
#include <concepts> //VS语言标准要设为: C++20
#include <chrono>
#include <string>
#include <unordered_map>
#include <random>
#include <bitset>
#include <ska/flat_hash_map.hpp> https://github.com/skarupke/flat_hash_map 直接下载包含进来就能用
#include <boost/regex.hpp>      //vcpkg 可以自动安装这个库
#include <boost/regex/icu.hpp>  //还需要icu这个Unicode官方库,vcpkg也可以装
#include <vector>

constexpr size_t 大小 = 100000000; //用了很大的数组,会报栈溢出,VS里修改:链接器->系统->堆栈保留大小 为1600000000(8个0) 就可以了
using 串 = std::string;

#pragma region 测试函数
class 计时器 {
	using 高精时钟 = std::chrono::high_resolution_clock;
private:
	高精时钟::time_point 起点;
	串 名字;
public:
	计时器(串 name = "") { //创建时如果不想加名字,后面就不要跟();
		名字 = name;
		起点 = 高精时钟::now();
	}
	~计时器() {
		auto 终点 = 高精时钟::now();
		std::chrono::duration<double, std::ratio<1, 1000>> 耗时毫秒(终点 - 起点);
		std::cout << std::setw(10) << std::left << 名字 << "耗时: ";
		std::cout << std::setw(10) << std::left << 耗时毫秒.count() << " ms\n";
	}//这个计时的方法是从Cherno的"BENCHMARKING in C++ (how to measure performance)"这个视频里学的
};

串 右填串(串 const& 字串, size_t s, char 填充字符 = ' ') {
	if (字串.size() < s) 
		return 字串 + 串(s - 字串.size(), 填充字符);
	else 
		return 字串;
}

template<std::integral T, size_t 值上限>
void 随个数组(T* 数组, size_t 长度, size_t 值下限 = 0) {
	if (值上限 - 值下限 < 长度) { throw "随机值上下限之差要比数组长度大!"; }
	std::random_device 种子机;
	std::default_random_engine 引擎(种子机());
	std::uniform_int_distribution<T> 分布(值下限, 值上限); //随机数 = 分布(引擎) 这里的随机数能等于上限,C#里面是不会等于上限的
	std::bitset<值上限 + 1> 查重; //如果没有参数,后面别跟()
	size_t i = 0;
	while (i < 长度) {
		auto 随机数 = 分布(引擎);
		if (!查重.test(随机数)) {
			数组[i] = 随机数; i++;
			查重.set(随机数);
		}
	}
}//往数组里填充不会重复的随机数

template<std::integral T, size_t 数组长>
T 数组测速(串 类型名) {
	T 栈数组[数组长], 读栈 = 0, 读堆 = 0;
	T* 堆数组 = new T[数组长];
	{ 计时器 计时(类型名 + " 栈数组写"); for (size_t i = 0; i < 数组长; i++) 栈数组[i] = i;  }
	{ 计时器 计时(类型名 + " 堆数组写"); for (size_t i = 0; i < 数组长; i++) 堆数组[i] = i;  }
	std::cout << std::endl;
	{ 计时器 计时(类型名 + " 栈数组读"); for (size_t i = 0; i < 数组长; i++) 读栈 = 栈数组[i]; }
	{ 计时器 计时(类型名 + " 堆数组读"); for (size_t i = 0; i < 数组长; i++) 读堆 = 堆数组[i]; }
	std::vector<T> 列表;
	列表.reserve(数组长);
	{ 计时器 计时(类型名 + " Vector写"); for (size_t i = 0; i < 数组长; i++) 列表.push_back(i);  }
	std::cout << "\n\n";
	读栈 += 栈数组[数组长 - 1] + 堆数组[数组长 - 1] + 读堆 + 列表[数组长 - 1];
	delete[]堆数组;
	return  读栈; //不这样返回一下上面代码会被优化掉.
}

void 栈和堆数组测试() {
	std::cout << "---C++数组测速:\n";
	数组测速<int8_t, 大小>("int8 ");
	数组测速<int16_t, 大小>("int16");
	数组测速<int32_t, 大小>("int32");
	数组测速<int64_t, 大小>("int64");
}

struct custom_hash { size_t operator()(int x) const { return x; } };//用来替换哈希表里的默认hash函数,用整型做key不需要hash
using ska哈希表 = ska::flat_hash_map<int, int, custom_hash>;
using std哈希表 = std::unordered_map<int, int, custom_hash>;

template<typename T表>
void 表测速(T表& 表, size_t* 数组, size_t 容量) requires std::is_same_v<T表, ska哈希表> || std::is_same_v<T表, std哈希表> 
{                                               //不加概念约束用两个不同类型的表套模板也能跑通,加了能防止用别的类来套
	表.reserve(容量); //ska表留了容量*2个buckets.
	size_t 串长 = std::to_string(容量).size() + 1;
	int 读;
	for (size_t N = 1; N <= pow(4, 6); N *= 4) {
		{
			计时器 计时(右填串(std::to_string(容量 / N), 串长) + "容量哈希表创建(随机健值)");
			for (size_t i = 0; i < 容量 / N; i++)
				表.insert(std::pair<int, int>(数组[i], i));
		}
		{
			计时器 计时(右填串(std::to_string(容量 / N), 串长) + "容量哈希表读取          ");
			for (size_t i = 0; i < 容量 / N; i++)
				读 = 表[数组[i]];
		}
		表.clear();
		std::cout << std::endl;
	}
	size_t 重复次数 = 10000, 容量上限 = pow(2, 13);
	串长 = std::to_string(容量上限).size() + 1;
	std::cout << "C++重复" << 重复次数 << "次 小容量读取耗时精测:\n";
	for (size_t i = 0; i < 容量上限; i++)
		表.insert(std::pair<int, int>(数组[i], i));
	for (size_t 小容量 = 8; 小容量 <= 容量上限; 小容量 *= 2) {
		{
			计时器 计时(右填串(std::to_string(小容量), 串长) + "容量读取总");
			for (size_t i = 0; i < 重复次数; i++) 
				for (size_t i = 0; i < 小容量; i++)
					读 = 表[数组[i]];
		}
	}
	std::cout << std::endl;
}

size_t 哈希表测试() {
	constexpr size_t 容量 = 33554432;
	std::cout << "---C++哈希表测速:\n";
	size_t 随机数组[容量], 读;
	{ 计时器 计时("随机产生3千万个数");  随个数组<size_t, 容量 * 2>(随机数组, 容量); }
	{ 计时器 计时("3千万个数随机读取");  for (size_t i = 0; i < 容量; i++) 读 = 随机数组[随机数组[i] % 容量]; }

	ska哈希表 表ska; //ska::flat_hash_map
	std哈希表 表std; //std::unordered_map
	std::cout << "\nska哈希表:\n";//创建速度C#字典最快,读取ska在2千万以上和2千以下似乎有些优势,其它似乎还是C#略快一点.看来C#里的字典的确还可以
	表测速<ska哈希表>(表ska, 随机数组, 容量);

	std::cout << "\nstd哈希表:\n";//std创建速度明显慢一些,读取并没有慢多少,而且和C#有点像,大概两千万条目往上读取会明显慢于ska
	表测速<std哈希表>(表std, 随机数组, 容量);//std耗内存比ska大
	
	std::cout << std::endl;
	return 读;
}

void 正则测速(const char16_t* 文本, const char16_t* 正则字串) {
	const boost::u32regex Unicode正则 = boost::make_u32regex(正则字串);
	boost::u16match u16结果;
	boost::u32regex_iterator<const UChar*> i(boost::make_u32regex_iterator(文本, Unicode正则)), j;
	size_t 结果计数 = 0;
	{
		计时器 计时("Boost U16匹配");
		while (i != j) {
			结果计数++;
			++i;
		}
	}
	std::cout << "结果数: " << 结果计数 << "\n";
}

void 正则引擎测试() {
	std::cout << "---C++ Boost正则引擎测速:\n";
	constexpr size_t 串长 = 500000; //40万,大概超过这个数boost U32迭代器就会出bug
	char16_t U16长串[串长], U16正则[80 + 1], 读;//前面加u字串的编码就是Unicode 16,似乎只能放在"const char16_t*"里
	{
		计时器 计时(std::to_string(串长) + "长char16填充");
		for (size_t i = 0; i < 串长 - 1; i++)
			U16长串[i] = i / 10000 + u'一';
	}
	size_t 正则长 = 80;
	for (char16_t i = 0; i < 正则长; i+= 2) {
		U16正则[i] = i / 2 + u'一';
		U16正则[i + 1] = u'|';
	}//构造 一|丁|丂|七...这样的正则表达式,最后一个字符不能是'|'
	U16正则[正则长] = u'蛙';
	正则测速(U16长串, u"\\w"); //用Unicode模式,正则字符串也得是Unicode的,匹配结果有点问题,最后四个数据好像够不着
	//40万,都匹配上,耗时66ms,C#要200ms,也存在匹配结果越多耗时越多的现象.u"\\w" 60ms u"\\w+"极速
	//40个字符用|连起来同时匹配上耗时180ms,看来也存在同时匹配的词越多,耗时也增多的现象.
}

#pragma endregion

int main()
{
	std::cout << "C++性能测试:\n\n";
	栈和堆数组测试(); //读比写快,C#和C++差不多
	
	哈希表测试();

	正则引擎测试();
	
}
//C++在智能提示和报错方面远不如C#,看来C四个+的确要好用一些. 有些地方少打个;会报"内部编译错误".         





