using System;
using System.Runtime.InteropServices;

namespace Arbiter
{
    [StructLayout(LayoutKind.Sequential, Size = 72)]
    public struct PROCESS_MEMORY_COUNTERS
    {
        public uint cb;
        public uint PageFaultCount;
        public uint PeakWorkingSetSize;
        public uint WorkingSetSize;
        public uint QuotaPeakPagedPoolUsage;
        public uint QuotaPagedPoolUsage;
        public uint QuotaPeakNonPagedPoolUsage;
        public uint QuotaNonPagedPoolUsage;
        public uint PagefileUsage;
        public uint PeakPagefileUsage;
    }

    public class PSAPI
    {
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool K32GetProcessMemoryInfo(IntPtr hProcess, out PROCESS_MEMORY_COUNTERS counters, uint size);
    }
}