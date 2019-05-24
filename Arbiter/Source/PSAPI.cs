using System;
using System.Runtime.InteropServices;

namespace Arbiter
{
    [StructLayout(LayoutKind.Sequential, Size = 72)]
    public struct PROCESS_MEMORY_COUNTERS
    {
        public uint cb;
        public uint PageFaultCount;
        public int PeakWorkingSetSize;
        public int WorkingSetSize;
        public int QuotaPeakPagedPoolUsage;
        public int QuotaPagedPoolUsage;
        public int QuotaPeakNonPagedPoolUsage;
        public int QuotaNonPagedPoolUsage;
        public int PagefileUsage;
        public int PeakPagefileUsage;
    }

    public class PSAPI
    {
        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool GetProcessMemoryInfo(IntPtr hProcess, out PROCESS_MEMORY_COUNTERS counters, uint size);
    }
}