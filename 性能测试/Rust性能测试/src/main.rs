//#![allow(warnings, unused)]
use rand::Rng;
use nohash_hasher::BuildNoHashHasher;
use num::Integer;
use std::{thread, collections::{HashMap, HashSet}, time::Instant, path::Path, iter::repeat_with};
use regex::{Regex/* ,RegexBuilder*/};
use bit_set::BitSet;

//B站无限次元: https://space.bilibili.com/2139404925  https://github.com/becomequantum/Kryon
const 长度:usize = 10000_0000;
fn main() {
    let 子线程 = thread::Builder::new()
        .stack_size(长度 * 8)
        .spawn(大栈测试)
        .unwrap();  //测试很大的栈数组, 要开个线程, 把栈弄大点才能测
    // Wait for thread to join
    子线程.join().unwrap();
}
fn 大栈测试(){
    println!("\n用cargo build --release编译后测试, 不然一些测试会很慢!");
    println!("-----Rust性能测试:\n");

    println!("一亿长度数组读写测试:\n");
    数组测试::<u8>();  //栈数组写似乎比C++略快,读大概差不多
    数组测试::<u16>(); //用cargo build --release 测试, debug模式下会报 attempt to add with overflow
    数组测试::<u32>();
    数组测试::<u64>();

    println!("\n正则引擎测试:\n");
    正则引擎测试();
    
    println!("\n哈希表测试:\n");
    哈希表测试();

    println!("\n\n了解下字串:\n");
    字符串测试();
    println!("\n-----测试结束:");
}


fn 哈希表测试(){
    const 容量:usize = 33554432;
    let mut 数组:[i32; 容量] = [0; 容量];
    let mut 表: HashMap<i32, i32, BuildNoHashHasher<i32>> 
              = HashMap::with_capacity_and_hasher(容量, BuildNoHashHasher::default());
    //let mut 表: HashMap<i32, i32> = HashMap::new(); //不取消默认哈希, 读慢了一倍, 写慢了50%

    let 计时 = Instant::now();
    随个数组(&mut 数组, 容量 * 2);
    let 耗时 = 计时.elapsed().as_micros() as f64 / 1000.0;
    println!("三千万不重复随机数产生: {耗时} ms");
    
    for i in 0..7{ //千万以上比C#写要慢些, 读要快些
        let n = 容量 / 4_usize.pow(i);
        let 计时 = Instant::now();
        for i in 0..n{
            表.insert(数组[i], i as i32);
        }
        let 耗时 = 计时.elapsed().as_micros() as f64 / 1000.0;
        println!("{:>8} 容量随机健值哈希表写耗时: {耗时} ms", n);  //写哈希表总算没被编译器优化掉, 写顺序的健值要快很多

        let mut 读 = 0;
        let 计时 = Instant::now();
        for i in 0..n{
            读 = *表.get(&数组[i]).unwrap();
        }
        let 耗时 = 计时.elapsed().as_micros() as f64 / 1000.0;
        println!("{:>8} 容量随机健值哈希表读耗时: {耗时} ms   {}\n", n, 读); 
    }

}

fn 正则引擎测试(){
    let 大小 = 100_0000;
    let mut 字串 = String::with_capacity(大小 * 10 * 3 + 4); //中文字符3个字节, 预留空间后12ms, 比C#一倍, 和C++差不多
    
    println!("--耗时正比结果数测试:");
    let 计时 = Instant::now();
    let mut i = 1;
    for 字 in "无限次元".chars(){
        字串.push_str(&字.to_string().repeat(i * 大小));//用 字串.repeat(n) 生成重复字串最快
        i += 1;  //字串长这样: "无..限限....次次次......元........" + "👽"  长度: 一千万零一
    } 
    let 耗时 = 计时.elapsed().as_micros() as f64 / 1000.0;
    println!("--生成千万长字串耗时: {耗时} ms"); //8.7ms
    字串.push('👽');
    for 字 in "👽无限次元".chars(){
        正则测速(&字串, &字.to_string());
    }

    println!("\n--同时匹配多词测试:");
    let 量级 = 10000;
    let mut 或正则 = String::with_capacity(6 * 量级);
    for n in 1..6{
        for i in 0..量级 * n{
            let 字 = char::from_u32(i as u32 % 10000 + ('一' as u32)).unwrap();  
            或正则.push(字);
            或正则.push_str("a|");//随便填充一个表达式: "一a|丁a|......|👽" 前面的都匹配不上, 除了最后一个
        }
        或正则.push('👽');
        print!("同时匹配词数: {} ", 量级 * n);
        正则测速(&字串, &或正则);
    }

    println!("\n--类型匹配测速:");
    字串.clear();
    字串.push_str(&"$".repeat(字串.capacity() - 2));
    字串.push_str("11");
    print!(r"\w: ");
    正则测速(&字串, r"\w");
    print!(r"\d: ");
    正则测速(&字串, r"[0123456789]"); //\w \d没区别 [0123456789]快了很多
    
    println!("\n--Git上的测试:");
    let 路径 = Path::new("input-text.txt"); //input-text.txt 在: https://github.com/mariomka/regex-benchmark
    let 文本 = std::fs::read_to_string(路径).unwrap();
    // Email
    正则测速(&文本, r"[\w\.+-]+@[\w\.-]+\.[\w\.-]+"); //这里的测速用的是captures_iter,并记录了结果,所以会比上面Git库里的代码测出来的略慢一点
    // URI
    正则测速(&文本, r"[\w]+://[^/\s?#]+[^\s?#]+(?:\?[^\s#]*)?(?:#[^\s]*)?");
    // IP
    正则测速(&文本, r"(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9])\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9])");

    println!("\n--Python FlashText对比测试:");
    let mut 去重 = HashSet::with_capacity(40000);
    let 计时 = Instant::now();
    let 正则 = Regex::new(r"\b[a-zA-Z][a-z]{3,6}\b").unwrap();
    正则.find_iter(&文本).map(|结果| 去重.insert(结果.as_str())).count();//匹配全文,并把结果塞到哈希集里去掉重复单词
    let 耗时 = 计时.elapsed().as_micros() as f64 / 1000.0;
    println!("匹配并去重后结果数  : {:>7}  耗时: {:>7} ms", 去重.len(), 耗时); //匹配加去重18956个词 130ms, C# 耗时178ms
    
    let mut 或正则 = String::with_capacity(去重.len());
    或正则.push_str(r"\b(");
    去重.iter().map(|词| {或正则.push_str(词); 或正则.push('|');}).count();
    或正则.push_str(r"蛙)\b");
    let 计时 = Instant::now();
    let 或正则 = Regex::new(&或正则).unwrap();
    let 结果 :Vec<_>= 或正则.captures_iter(&文本).collect(); 
    let 耗时 = 计时.elapsed().as_micros() as f64 / 1000.0; 
    println!("同时匹配这些词结果数: {:>7}  耗时: {:>7} ms", 结果.len(), 耗时); 
     //18956个词耗时690ms, 3万多词耗时差不多1秒, Flash_Text几万个词耗时稳定在1秒. 但两者匹配到的结果数有出入
     
}

fn 正则测速(文本: &str, 正则字串: &str) {
    let 计时 = Instant::now();
    // let 正则 = RegexBuilder::new(正则字串).unicode(false).build().unwrap(); //适用于正则字串中没有Unicode, 不匹配Unicode会快大概一倍, 纯英文可用这个
    // let 结果数 = 正则.find_iter(文本.as_bytes()).count();
    let 正则 = Regex::new(正则字串).unwrap();
    //let 正则 = RegexBuilder::new(正则字串).size_limit(2000_0000).build().unwrap();
    //let 结果数 = 正则.find_iter(文本).count();                //2 15 28 42 54ms 
    //let 结果 :Vec<_>= 正则.find_iter(文本).collect();         //2 31 62 95 120ms 记录结果耗时翻倍, Vec预留空间没效果 find_iter返回了匹配到的字串和它的range
    let 结果 :Vec<_>= 正则.captures_iter(文本).collect();       //2 80 162 254 333 比C#快5倍, captures_iter返回的东西更多, 如果要用到分组功能, 就得用captures
    let 耗时 = 计时.elapsed().as_micros() as f64 / 1000.0;
    println!("结果数: {:>7}  耗时: {:>7} ms", 结果.len(), 耗时);
}

fn 数组测试<T>() where T: Integer + Copy + std::fmt::Display, {
    let mut 数组:[T; 长度] = [T::zero(); 长度]; //[初值,长度],数组在栈上. 这句初始化也需要一点时间
    let mut n = T::zero();
    let 计时 = Instant::now();
    for i in 0..数组.len(){
        数组[i] = n; 
        n = n + T::one(); //直接 数组[i] = i 想实现比较麻烦,只能这样整一下, 并未增加耗时
    }
    let 耗时 = 计时.elapsed().as_micros() as f64 / 1000.0;
    println!("{}", std::any::type_name::<T>()); //打印T的类型
    println!("栈数组写耗时: {耗时} ms    {}", 数组[长度 - 1]); //不打印一个数编译器会把代码优化掉,和C++一样
    
    let 计时 = Instant::now();
    for i in 1..数组.len(){
        数组[i] = 数组[i - 1];//这样弄一下才能测出读取时间, 不然循环过程会被优化掉, Rust编译器👍, 比C++强
    }
    let 耗时 = 计时.elapsed().as_micros() as f64 / 1000.0;
    println!("栈数组读耗时: {耗时} ms    {}", 数组[长度 - 1]);

    let mut 列表: Vec<T> = Vec::with_capacity(长度); //不预留空间会慢1.5-2倍. vector写比C++也快一点
    n = T::zero();
    let 计时 = Instant::now();
    repeat_with(|| {列表.push(n); n = n + T::one();}).take(长度).count();
    let 耗时 = 计时.elapsed().as_micros() as f64 / 1000.0;
    println!("Vector写耗时: {耗时} ms    {} {}", 列表[长度 - 1], 列表[0]);


}

fn 随个数组(数组: &mut [i32], 值上限:usize){//用不重复的随机数填充一个数组
    if 值上限 <= 数组.len() {
        panic!("随机数值上限要大于数组个数!");
    }
    let mut 查重 = BitSet::with_capacity(值上限);
    let mut n = 0;
    while n < 数组.len() {
        let 随机数 = rand::thread_rng().gen_range(0..值上限);
        if 查重.insert(随机数){
            数组[n] = 随机数 as i32;
            n += 1;
        }
    }
}

fn 字符串测试(){
    let 字 ='一';
    let 字= 字 as i32;
    let 表情 = '👻';
    
    let mut 字串 = String::with_capacity(10); //mut:可变的, 默认不可变 
    字串.push(表情);
    字串.push('一');
    字串.push_str("👼😄👽👏");
    for c in 字串.escape_unicode() {
        print!("{c} "); //打印出来的是Unicode值的字串
    }
    println!("\n{:#x} {}  {字串}\n",字,表情); //char是Unicode,字串是utf-8,字串不能[i]这样读取
}

