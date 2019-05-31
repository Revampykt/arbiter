using System;
using System.Runtime.InteropServices;

namespace Arbiter
{
    public class Invoker2
    {
        public static void MesaureProcess(string executable, string input, string output, out uint memory, out uint time)
        {
            SECURITY_ATTRIBUTES securityAttributes = new SECURITY_ATTRIBUTES
            {
                nLength = (Int32)Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES)),
                lpSecurityDescriptor = IntPtr.Zero,
                bInheritHandle = true
            };

            IntPtr hStdInput = Kernel32.CreateFile(input, Kernel32.GENERIC_READ, Kernel32.FILE_SHARE_READ, ref securityAttributes, Kernel32.OPEN_ALWAYS, Kernel32.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);
            IntPtr hStdOutput = Kernel32.CreateFile(output, Kernel32.GENERIC_WRITE, Kernel32.FILE_SHARE_WRITE, ref securityAttributes, Kernel32.CREATE_ALWAYS, Kernel32.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);

            STARTUPINFO startupInfo = new STARTUPINFO()
            {
                cb = (Int32)Marshal.SizeOf(typeof(STARTUPINFO)),
                dwFlags = (Int32)Kernel32.STARTF_USESTDHANDLES,
                hStdInput = hStdInput,
                hStdOutput = hStdOutput,
                hStdError = IntPtr.Zero
            };

            PROCESS_INFORMATION processInforamtion = new PROCESS_INFORMATION();

            SYSTEMTIME startTime = new SYSTEMTIME();
            Kernel32.GetSystemTime(ref startTime);

            SECURITY_ATTRIBUTES zero = new SECURITY_ATTRIBUTES();

            bool result = Kernel32.CreateProcess(executable, null, ref zero, ref zero, true, 0, IntPtr.Zero, null, ref startupInfo, out processInforamtion);

            int exitCode = Kernel32.WaitForSingleObject(processInforamtion.hProcess, 10000);

            SYSTEMTIME stopTime = new SYSTEMTIME();
            Kernel32.GetSystemTime(ref stopTime);

            time = (uint)(Math.Abs(stopTime.wHour - startTime.wHour)
                          * 60
                          * 60
                          * 1000
                          + Math.Abs(stopTime.wHour - startTime.wHour)
                          * 60
                          * 60
                          * 1000
                          + Math.Abs(stopTime.wMinute - startTime.wMinute)
                          * 60
                          * 1000
                          + Math.Abs(stopTime.wSecond - startTime.wSecond)
                          * 1000
                          + Math.Abs(stopTime.wMilliseconds - startTime.wMilliseconds));

            PROCESS_MEMORY_COUNTERS pmc = new PROCESS_MEMORY_COUNTERS
            {
                cb = (uint)Marshal.SizeOf(typeof(PROCESS_MEMORY_COUNTERS))
            };

            bool getProcessMemoryInfoSuccess = PSAPI.K32GetProcessMemoryInfo(processInforamtion.hProcess, out pmc, pmc.cb);

            if (getProcessMemoryInfoSuccess)
            {
                Logger.Log($"ok {pmc.cb} {pmc.PageFaultCount} {pmc.PagefileUsage} {pmc.PeakPagefileUsage} {pmc.PeakWorkingSetSize} {pmc.QuotaNonPagedPoolUsage} {pmc.QuotaPagedPoolUsage} " +
                    $"{pmc.QuotaPeakNonPagedPoolUsage} {pmc.QuotaPeakPagedPoolUsage} {pmc.WorkingSetSize}");
            }
            else
            {
                Logger.Log("fail");
            }

            memory = pmc.PagefileUsage;

            Kernel32.CloseHandle(processInforamtion.hProcess);
            Kernel32.CloseHandle(hStdInput);
            Kernel32.CloseHandle(hStdOutput);
        }
    }
}