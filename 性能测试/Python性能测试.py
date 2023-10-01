#B站无限次元: https://space.bilibili.com/2139404925  https://github.com/becomequantum/Kryon
import re
from timeit import timeit
from timeit import default_timer as now
import numpy as np
import random
from flashtext import KeywordProcessor

def Unicode测试():
    print("\n---Unicode测试\n'一'的Unicode值: ", hex(ord("一")))  #字符转Unicode值
    for 匹配 in re.finditer("找出所有匹配|用|finditer", "找出所有匹配的字串要用finditer,结果包含起终点位置"):
        print(str(匹配.span()[0]) + ", " + str(匹配.span()[1]) + " " + 匹配.group())
    无 = open("Python性能测试.py", encoding = 'utf-8').read()[3] #把本代码文本读入字符串,看看第三个字符是啥
    print(无) #本代码第三个字符就是"无",看来内部字串编码用的就是Unicode,和C#一样

def 正则测速(正则字串, 文本):
    结果表 = []  #[None]*预留空间大小 用这个方法反而慢了一点
    开始 = now()
    for 匹配 in re.finditer(正则字串, 文本): #finditer第一个参数就是正则字串, 不是re("")
        结果表.append(匹配)      #记录结果后速度慢了6倍, 耗时前三个比C#略快10%
    耗时 = (now() - 开始) * 1e3  #re.findall速度更快些,但没有记录位置信息,不需要位置信息的可以用它
    print("结果数: " + str(len(结果表)).rjust(7,' ') + " 耗时: " + '%.3f' % 耗时 + " ms")
    return 结果表

def 生成长字串():
    大小 = 100_0000
    串 = ""
    for n in range(1,5):
        串 = 串.ljust(n * 大小 + len(串), "无限次元"[n - 1]) 
    return 串

def 正则引擎测试():
    print("\n---Python正则引擎性能测试:\n")
    字串 = 生成长字串()  # 无...... 限限...... 次次次... 元元元元... 蛙 
    字串 = 字串 + "蛙同时增加C"   #  一百万个    两百万

    耗时 = timeit(生成长字串, number = 10) * 100  #计时10次乘100,得到耗时单位为毫秒
    print("生成" + str(int(len(字串)/10000)) + "万长字串耗时: " + '%.3f' % 耗时 + " ms\n") #耗时和C++差不多,比C#快

    开始 = now()
    for i in range(0,len(字串)):
        读 = 字串[i]
    print(str(int(len(字串)/10000)) + "万长字串读: " + '%.3f' % 耗时 + " ms  " + 读 + " 的速度\n")  
    #读字串耗时也和C++差不多, 为啥读数组那么慢呢? 可能因为字串是纯数值数组
   
    print("耗时正比于结果数测试:")
    for 字正则 in "蛙无限次元":
        正则测速(字正则, 字串)

    print("\n同时匹配多词耗时测试:")
    正则测速("蛙", 字串)
    正则测速("蛙|同", 字串)  #多一个耗时增加了10倍,再多耗时似乎也没再增加. 词加的还是少了,下面测FlashText测试, 词加多了时间就爆了
    正则测速(r"蛙|同|时|匹|配|多|词|再多一点|词长一点|好像耗时|还|是|没|增加", 字串)

    print("\nGithub上的测试(匹配Email URI IP):")
    测试文本 = open("input-text.txt", encoding = 'utf-8').read() # input-text.txt在: https://github.com/mariomka/regex-benchmark/
    print("文本长度: " + str(len(测试文本)))
    # Email 耗时294ms C#: 1.7 
    正则测速('[\w\.+-]+@[\w\.-]+\.[\w\.-]+', 测试文本) 
    # URI 耗时218ms C#: 3.2
    正则测速('[\w]+://[^/\s?#]+[^\s?#]+(?:\?[^\s#]*)?(?:#[^\s]*)?', 测试文本) 
    # IP 耗时279 C#: 20.3 这项测试匹配到的结果数不算多, Python慢了不少
    正则测速('(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9])\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9])', 测试文本)

    print("\n表达式复杂度和耗时关系测试:")
    正则测速('@', 测试文本) #结果更多,耗时很小
    正则测速('[\w\.+-]+@[\w\.-]+\.[\w\.-]+', 测试文本) #耗时是上面100倍,看来表达式复杂点Python耗时会暴增100倍,比C#就差在这了
    print("\n\w []大小集合判断耗时对比:")
    正则测速('\w+', 字串) #这两个结果一样,但下面的耗时更少
    正则测速('[无限次元蛙]+', 字串) #原因大概是: '\w'这个集合比'[无限次元蛙]'大不少,python在判断一个字符是否属于\w时花了更多时间,C#也存在这个问题

    #正则测速('[\w\.+-]+@[\w\.-]+\.[\w\.-]+', 字串[0:100000])  #这么大就会导致卡死, 不知为啥
    print("\n")

def FlashText测试():
    print("---FlashText测试:\n")
    测试文本 = open("input-text.txt", encoding = 'utf-8').read() #input-text.txt 在: https://github.com/mariomka/regex-benchmark
    匹配单词 = re.compile(r'\b[a-zA-Z][a-z]{3,6}\b') #匹配英文开头的单词,长度4-6, 这里的表达式字串前面得加 r''

    开始 = now()
    结果表 = re.findall(匹配单词, 测试文本) 
    耗时 = (now() - 开始) * 1e3
    print("re.findall匹配所有单词耗时: " + '%.3f' % 耗时 + " ms")

    单词集 = set(结果表)
    单词表的表 = [[词] for 词 in 单词集] #FlashText要求字典的Value是单词表
    词典 = dict(zip(np.arange(0, len(单词集), dtype=np.int32), 单词表的表))
    print("单词数: " + str(len(词典))) #18956
    
    Flash提词 = KeywordProcessor()
    开始 = now()
    Flash提词.add_keywords_from_dict(词典)
    耗时 = (now() - 开始) * 1e3
    print("add_keywords_from_list耗时: " + '%.3f' % 耗时 + " ms")

    开始 = now()
    提词结果 = Flash提词.extract_keywords(测试文本)
    耗时 = (now() - 开始) * 1e3
    print("Flash提取所有词耗时: " + '%.3f' % 耗时 + " ms") #1.2秒, 好几万个词都是这个耗时. C#同时提两万三千词耗时1.6秒, 词再多就爆栈了
    print("提词结果数: " + str(len(提词结果)))  #288915
    for i in range(5):
        N = 提词结果[i]
        print(N," " ,词典[N])
    
    正则字串 = ""
    for 词 in 单词集:
        正则字串 += 词 + "|"
    正则字串 += "蛙"

    开始 = now()
    正则 = re.compile(正则字串)
    耗时 = (now() - 开始) 
    print("生成所有单词正则耗时: " + '%.3f' % 耗时 + " 秒")

    开始 = now()
    #结果表 = re.findall(正则, 测试文本) 
    耗时 = (now() - 开始) 
    print("正则匹配所有单词耗时: " + '%.3f' % 耗时 + " 秒 要花126秒") #126秒
    print("\n")

def 列表字典测速(): 
    print("---列表字典测试:\n")
    一亿 = 10000_0000 
    开始 = now()
    列表 = [i for i in range(一亿)]
    耗时 = (now() - 开始) * 1e3
    print("Python一亿列表初始化: " + '%.3f' % 耗时 + " ms") #这个速度有点慢
    
    开始 = now()
    for i in range(一亿):
        读 = 列表[i]
    耗时 = (now() - 开始) * 1e3
    print("Python一亿列表读取  : " + '%.3f' % 耗时 + " ms\n") #读是写的2/3

    开始 = now()
    np表 = np.arange(0, 一亿, dtype=np.int32)
    耗时 = (now() - 开始) * 1e3
    print("numpy一亿列表初始化: " + '%.3f' % 耗时 + " ms") #numpy数组初始化速度和C++#差不多

    开始 = now()
    for i in np表:
        读 = i      #这样遍历比写慢很多,难道读的姿势不对?
    耗时 = (now() - 开始) * 1e3
    print("numpy一亿列表读取  : " + '%.3f' % 耗时 + " ms\n") 

    字典 = {}
    大小 = 33554432

    开始 = now()
    #随机 = random.sample(range(0, 大小 * 2), 大小)
    耗时 = (now() - 开始) * 1e3
    print("随三千万不重复随机数耗时: " + '%.3f' % 耗时 + " ms 要17秒") #要17秒 C#需要半秒

    开始 = now()
    for i in range(大小):
        字典[i] = i        #不随机速度还行
        #字典[随机[i]] = i #随机健值创建耗时12秒
    耗时 = (now() - 开始) * 1e3
    print("三千万字典创建耗时: " + '%.3f' % 耗时 + " ms")

    开始 = now()
    for i in range(大小):
        读 = 字典[i]
        #读 = 字典[随机[i]] #随机健值读取耗时9秒
    耗时 = (now() - 开始) * 1e3
    print("三千万字典读取耗时: " + '%.3f' % 耗时 + " ms")
    print("\n")


if __name__ == '__main__':
    Unicode测试()
    正则引擎测试()
    FlashText测试()
    列表字典测速()
    print("\n测试结束!")
