using System.ComponentModel;
using System.Runtime.InteropServices;

namespace 无限次元;
internal class 高精度计时 {
    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);//返回高精度计数器的值

    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceFrequency(out long lpFrequency);//返回硬件支持的高精度计数器的频率,计数器的值/计数器的频率 = 多少秒 

    private long 开始时间, 结束时间;
    private long 频率;

    public long 计数器频率 { get { return 频率; } }

    // Constructor
    public 高精度计时() {
        开始时间 = 0;
        结束时间 = 0;

        if (QueryPerformanceFrequency(out 频率) == false) {
            // high-performance counter not supported
            throw new Win32Exception();
        }
    }

    // Start the timer
    public void 开始() {
        // lets do the waiting threads there work
        Thread.Sleep(0);
        QueryPerformanceCounter(out 开始时间);
    }

    // Stop the timer
    public void 停止() {
        QueryPerformanceCounter(out 结束时间);
    }

    // Returns the duration of the timer (in milliseconds)
    public double 时长 {
        get {
            return (double)(结束时间 - 开始时间) * 1000 / (double)频率;
        }
    }

    public long Freq {
        get {
            return 频率;
        }
    }

    public void 清零() {
        开始时间 = 0;
        结束时间 = 0;
    }

    public long 获取计数() {
        QueryPerformanceCounter(out long 计数);
        return 计数;
    }

}

