using System;
using System.Runtime.InteropServices;

namespace Arbiter
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_ATTRIBUTES
    {
        public int nLength;
        public IntPtr lpSecurityDescriptor;
        public bool bInheritHandle;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct STARTUPINFO
    {
        public Int32 cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public Int32 dwX;
        public Int32 dwY;
        public Int32 dwXSize;
        public Int32 dwYSize;
        public Int32 dwXCountChars;
        public Int32 dwYCountChars;
        public Int32 dwFillAttribute;
        public Int32 dwFlags;
        public Int16 wShowWindow;
        public Int16 cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput;
        public IntPtr hStdOutput;
        public IntPtr hStdError;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_INFORMATION
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }

    public struct SYSTEMTIME
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;
    }

    public class Kernel32
    {
        public const uint GENERIC_READ = 0x80000000;
        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint FILE_SHARE_WRITE = 0x00000002;
        public const uint CREATE_ALWAYS = 1;
        public const uint OPEN_ALWAYS = 4;
        public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
        public const uint STARTF_USESTDHANDLES = 0x00000100;
        public const ulong WAIT_OK = 0L;
        public const ulong WAIT_TIMEOUT = 258L;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool CreateProcess(string lpApplicationName,
                                         string lpCommandLine,
                                         ref SECURITY_ATTRIBUTES lpProcessAttributes,
                                         ref SECURITY_ATTRIBUTES lpThreadAttributes,
                                         bool bInheritHandles,
                                         uint dwCreationFlags,
                                         IntPtr lpEnvironment,
                                         string lpCurrentDirectory,
                                         [In] ref STARTUPINFO lpStartupInfo,
                                         out PROCESS_INFORMATION lpProcessInformation);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(string lpFileName,
                                        uint dwDesiredAccess,
                                        uint dwSharedMode,
                                        ref SECURITY_ATTRIBUTES lpSecurityAttributes,
                                        uint dwCreationDisposition,
                                        uint dwFladsAndAttributes,
                                        IntPtr hTemplateFile);

        [DllImport("kernel32.dll")]
        public static extern void GetSystemTime(ref SYSTEMTIME lpSystemTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Int32 WaitForSingleObject(IntPtr Handle, Int32 Wait);

        [DllImport("kernel32.dll")]
        public static extern void CloseHandle(IntPtr hHandle);
    }
}