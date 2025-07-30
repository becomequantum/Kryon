from os import system
import random as 随机
from typing import Any
from graphviz import Digraph

class 数学运算符:
    @staticmethod
    def 加(左, 右):
        return 左 + 右
    
    @staticmethod
    def 剪(左, 右):
        return 左 - 右
    
    @staticmethod
    def 乘(左, 右):
        return 左 * 右
    
    @staticmethod
    def 除(左, 右):
        return 左 / 右
    
    运算符表 = ['+','-','*','/']
    优先级表 = dict(zip(运算符表, [0, 0, 1, 1]))
    索引表 = dict(zip(运算符表, [0, 1, 2, 3]))
    函数表 = dict(zip(运算符表, [加, 剪, 乘, 除]))
    def __init__(self) -> None:
        self.符: str = 随机.choice(数学运算符.运算符表)
    
    def __gt__(self, other) -> bool:
        return 数学运算符.优先级表[self.符] > 数学运算符.优先级表[other.符]
    
    def __call__(self, 左, 右) -> Any:
        return 数学运算符.函数表[self.符](左, 右)

class 表达式:
    当前的 = None #用于记录当前正在生成的表达式实例
    def __init__(self) -> None:
        表达式.当前的 = self
        self.终结计数: int = 1
        self.非终结计数: int = 0
        self.生成概率: float = 0.8
        self.头节点 = None #记录表达式树最顶层的节点
        self.树状图 = Digraph(comment='算术表达式树状图')
        非终结()
        self.深优遍历(self.头节点)
        self.算式: str = self.头节点.算式
        self.树状图.node('算式',self.算式, shape = 'none')
        self.树状图.edge('算式', self.头节点.节点名, style = 'dotted')
        
    
    def 深优遍历(self, 节点):
        if isinstance(节点.左, 非终结): #左右如果有非叶子节点就继续递归遍历
            self.深优遍历(节点.左)
        if isinstance(节点.右, 非终结):    
            self.深优遍历(节点.右)
       
        节点.数值 = 节点.运算符.运算(节点.左.数值, 节点.右.数值)      #深优遍历求值的结果最终存在"self.头节点.数值"里
        #节点.算式 = '(' + 节点.左.算式 + 节点.运算符.运算.符 + 节点.右.算式 + ')' #这个结果在"self.头节点.算式"里

        if isinstance(节点.左, 终结_数值) and isinstance(节点.右, 终结_数值):
            节点.算式 = 节点.左.算式 + 节点.运算符.运算.符 + 节点.右.算式
            if 节点.运算符.优先级 < 节点.最低优先级:
                节点.最低优先级 = 节点.运算符.优先级
        else:
            self.__省括号(节点)

    @staticmethod
    def __省括号(节点):
        节点.最低优先级 = 节点.运算符.优先级
        if isinstance(节点.左, 非终结):  #左边是算式, 当前算符优先级要是比左边算式中任意一个算符的优先级高, 左边算式就要打括号
            if 节点.运算符.优先级 > 节点.左.最低优先级:
                节点.算式 = '(' + 节点.左.算式 + ')' 
            else:
                节点.算式 = 节点.左.算式
                if 节点.左.最低优先级 < 节点.运算符.优先级:
                    节点.最低优先级 = 节点.左.最低优先级
        else:  #左边是数字
            节点.算式 = 节点.左.算式

        节点.算式 += 节点.运算符.运算.符 

        if isinstance(节点.右, 非终结):  #右边是算式, 
            if  节点.运算符.运算.符 == '+' or \
                节点.运算符.运算.符 == '*' and 节点.运算符.优先级 <= 节点.右.最低优先级 or \
                节点.运算符.运算.符 == '-' and 节点.运算符.优先级 < 节点.右.最低优先级:  
                    
                节点.算式 +=  节点.右.算式
            else:
                节点.算式 +=  '(' + 节点.右.算式 + ')' 
        else: #右边是数字
            节点.算式 +=  节点.右.算式

    def __repr__(self) -> str:
        return   str(self.头节点.数值) + " = "+ self.头节点.算式
            

class 非终结(): #非叶子节点
    def __init__(self, 原头节点 = None) -> None:
        self.节点名 = 'E'+ str(表达式.当前的.非终结计数)  #用于画树状图
        self.数值 = 0.0                     #用于求值
        self.算式: str = ""                 #用于把树结构表达式投影为一维表达式字符串
        self.最低优先级: int = 11     #记录算式中包含的最低优先级算符的优先级, 用于去掉多余括号
        self.运算符 = 终结_算符() #随一个运算符
        表达式.当前的.树状图.node(self.节点名, self.运算符.运算.符, shape = 'circle', fontname="Microsoft Yahei",fixedsize = 'true') #第一个参数是节点名, 用于区别节点. 第二个参数如果有, 就是节点中显示的内容
        表达式.当前的.非终结计数 += 1

        if 原头节点 is None:
            self.左 = 终结_数值('L')  #只从右边开始生成
            self.__添加左右节点(self.左)
        else:
            self.左 = 原头节点                    #这个新节点要后算, 跑到上面去了, 所以它的左节点要指向原来的头节点
            表达式.当前的.头节点 = self   #然后它自己成为头节点
            表达式.当前的.树状图.edge(self.节点名, 原头节点.节点名) #这个新节点指向原来的头节点

        右边不终结: bool = 随机.random() <= 表达式.当前的.生成概率
        if 表达式.当前的.头节点 is None:
                表达式.当前的.头节点 = self  #头节点为空, 也就是当前是首个节点的情况下, 就先把自己设为头结点再说
        if not 右边不终结:
            self.右 = 终结_数值('R') #终结生成, 右边就给个数值节点, 然后返回
            self.__添加左右节点(self.右)
            return
        
        #如果继续生成, 先随一下, 看看新的表达式是要先算还是后算
        右边先算: bool = 随机.random() < 0.5  #新生成的右表达式先算, 意味着右表达式会成为当前表达式的右下子表达式
        if(右边先算):
            self.右 = 非终结() #这就形成了递归调用
            表达式.当前的.树状图.edge(self.节点名, self.右.节点名) 
        else: #当前表达式先算
            self.右 = 终结_数值('R')
            self.__添加左右节点(self.右)
            非终结(表达式.当前的.头节点) #当前先算时, 它的层级会下降, 会变成新表达式的左下子表达式, 所以当前的头节点会作为参数, 让新节点的左节点指向它


    def __添加左右节点(self, 左右) -> None:
            表达式.当前的.树状图.node(左右.节点名, shape = 'circle', fixedsize = 'true' ) #先加左节点, 图上就会画在左边
            表达式.当前的.树状图.edge(self.节点名, 左右.节点名) #这个边是有指向性的

class 终结_数值(): #叶子节点
    def __init__(self, 左右: str) -> None:
        self.节点名 = str(表达式.当前的.终结计数)
        self.数值: float = 表达式.当前的.终结计数 #终结_数值.随个数() #
        self.算式: str = str(self.数值)
        表达式.当前的.终结计数 += 1
    
    @staticmethod
    def 随个数() -> float:
        数 = int(随机.random() * 1e4) / 10 ** 随机.randint(1,4)
        if 随机.random() < 0.5:
            数 = int(数)
        if 随机.random() < 0.5:
            数 = -数
        if abs(数) <= 1e-5:
            数 = 终结_数值.随个数()
        return 数

class 终结_算符():
    def __init__(self) -> None:
        self.运算 = 数学运算符()  #会随一个加减乘除运算
        self.优先级: int = 数学运算符.优先级表[self.运算.符]

if __name__ == "__main__":
    system("cls")
    表1 = 表达式()
    print(表1)
    #表1.树状图.render("表达式1.gv", view= True, format='png')
    值 = eval(表1.算式)
    print(str(值))
    正确 = False
    不正确计数 = 0
    # for i in range(40):
    #     表 = 表达式()
    #     表达式值 = eval(表.算式)
    #     误差 = abs(表.头节点.数值 - 表达式值)
    #     正确 = 误差 <= 5e-6
    #     if not 正确:
    #         不正确计数 += 1
    #         print("err " + str(误差))
    #         print(表达式值)
    #         print(表)
    #         #表.树状图.render("表达式.gv", view= True, format='png')
    
    if 不正确计数 == 0:
        print('全对')

    表1.__dict__['a'] = 'aa'
    print(表1.a)
    表1.__repr__ = lambda x : '表1'
    表达式.__repr__ = lambda x: '草房'
    表达式.__call__ = lambda x: print('执行')
    for kv in 表达式.__dict__:
        print(kv)
    print()
    for kv in 表1.__dict__:
        print(kv)
    
 