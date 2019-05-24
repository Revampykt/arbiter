//using System;
//using System.Runtime.InteropServices;

//namespace Arbiter.InvokerOld
//{
//    // Информация о процессе
//    public struct PROCESS_INFORMATION
//    {
//        public IntPtr hProcess;     // HANDLE
//        public IntPtr hThread;      // HANDLE
//        public uint dwProcessId;    // DWORD
//        public uint dwThreadId;     // DWORD
//    }

//    // Информация о запуске процесса
//    public struct STARTUPINFO
//    {
//        public uint cb;                 // DWORD
//        public string lpReserved;       // LPTSTR
//        public string lpDesktop;        // LPTSTR
//        public string lpTitle;          // LPTSTR
//        public uint dwX;                // DWORD
//        public uint dwY;                // DWORD
//        public uint dwXSize;            // DWORD
//        public uint dwYSize;            // DWORD
//        public uint dwXCountChars;      // DWORD
//        public uint dwYCountChars;      // DWORD
//        public uint dwFillAttribute;    // DWORD
//        public uint dwFlags;            // DWORD
//        public ushort wShowWindow;      // WORD
//        public ushort cbReserved2;      // WORD
//        public IntPtr lpReserved2;      // LPBYTE
//        public IntPtr hStdInput;        // HANDLE
//        public IntPtr hStdOutput;       // HANDLE
//        public IntPtr hStdError;        // HANDLE
//    }

//    // Атрибуты безопасности
//    public struct SECURITY_ATTRIBUTES
//    {
//        public uint nLength;                // DWORD
//        public IntPtr lpSecurityDescriptor; // LPVOID
//        public bool bInheritHandle;         // BOOL
//    }

//    // Метрики памяти процесса
//    public struct PROCESS_MEMORY_COUNTERS
//    {
//        public uint cb;                             // DWORD
//        public uint PageFaultCount;                 // DWORD
//        public uint PeakWorkingSetSize;             // SIZE_T
//        public uint WorkingSetSize;                 // SIZE_T
//        public uint QuotaPeakPagedPoolUsage;        // SIZE_T
//        public uint QuotaPagedPoolUsage;            // SIZE_T
//        public uint QuotaPeakNonPagedPoolUsage;     // SIZE_T
//        public uint QuotaNonPagedPoolUsage;         // SIZE_T
//        public uint PagefileUsage;                  // SIZE_T
//        public uint PeakPagefileUsage;              // SIZE_T
//    }

//    // Момент времени
//    public struct SYSTEMTIME
//    {
//        public ushort wYear;            // WORD
//        public ushort wMonth;           // WORD
//        public ushort wDayOfWeek;       // WORD
//        public ushort wDay;             // WORD
//        public ushort wHour;            // WORD
//        public ushort wMinute;          // WORD
//        public ushort wSecond;          // WORD
//        public ushort wMilliseconds;    // WORD
//    }

//    // Режимы защиты отображения файла
//    enum FileMapProtection : uint
//    {
//        PageReadonly = 0x02,
//        PageReadWrite = 0x04,
//        PageWriteCopy = 0x08,
//        PageExecuteRead = 0x20,
//        PageExecuteReadWrite = 0x40,
//        SectionCommit = 0x8000000,
//        SectionImage = 0x1000000,
//        SectionNoCache = 0x10000000,
//        SectionReserve = 0x4000000,
//    }

//    [ComVisible(true)]
//    public class Invoker
//    {
//        // Константы
//        public const uint GENERIC_READ = 0x80000000;
//        public const uint FILE_SHARE_READ = 0x00000001;
//        public const uint GENERIC_WRITE = 0x40000000;
//        public const uint FILE_SHARE_WRITE = 0x00000002;
//        public const uint CREATE_ALWAYS = 1;
//        public const uint OPEN_ALWAYS = 4;
//        public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
//        public const uint STARTF_USESTDHANDLES = 0x00000100;
//        public const ulong WAIT_OK = 0L;
//        public const ulong WAIT_TIMEOUT = 258L;

//        /// <summary>
//        /// Запуск процесса
//        /// </summary>
//        [DllImport("kernel32.dll", SetLastError = true)]
//        static extern bool CreateProcess(
//            string lpApplicationName,                     // LPCSTR
//            string lpCommandLine,                         // LPSTR
//            IntPtr lpProcessAttributes,                   // LPSECURITY_ATTRIBUTES
//            IntPtr lpThreadAttributes,                    // LPSECURITY_ATTRIBUTES
//            bool bInheritHandles,                         // BOOL
//            uint dwCreationFlags,                         // DWORD
//            IntPtr lpEnvironment,                         // LPVOID
//            string lpCurrentDirectory,                    // LPCSTR
//            ref STARTUPINFO lpStartupInfo,                // LPSTARTUPINFO
//            ref PROCESS_INFORMATION lpProcessInformation  // LPPROCESS_INFORMATION
//        );

//        // Открытие файла
//        [DllImport("kernel32.dll", SetLastError = true)]
//        static extern IntPtr CreateFile(
//            string lpFileName,                              // LPCSTR lpFileName
//            uint dwDesiredAccess,                           // DWORD dwDesiredAccess
//            uint dwSharedMode,                              // DWORD dwSharedMode
//            ref SECURITY_ATTRIBUTES lpSecurityAttributes,   // LPSECURITY_ATTRIBUTES lpSecurityAttributes
//            uint dwCreationDisposition,                     // DWORD dwCreationDisposition
//            uint dwFladsAndAttributes,                      // DWORD dwFladsAndAttributes
//            IntPtr hTemplateFile                            // HANDLE hTemplateFile
//        );

//        // Создание анонимного канала
//        [DllImport("kernel32.dll", SetLastError = true)]
//        static extern bool CreatePipe(
//            ref IntPtr hReadPipe,                           // PHANDLE hReadPipe
//            ref IntPtr hWritePipe,                          // PHANDLE hWritePipe
//            ref SECURITY_ATTRIBUTES lpSecurityAttributes,   // LPSECURITY_ATTRIBUTES lpSecurityAttributes
//            uint nSize                                      // DWORD nSize
//        );

//        // Открытие отображения файла в память
//        [DllImport("kernel32.dll", SetLastError = true)]
//        static extern IntPtr CreateFileMapping(
//            IntPtr hFile,                                   // LPCSTR lpFileName
//            IntPtr lpFileMappingAttributes,                 // DWORD dwDesiredAccess
//            FileMapProtection flProtect,                    // DWORD dwSharedMode
//            uint dwMaximumSizeHigh,                         // LPSECURITY_ATTRIBUTES lpSecurityAttributes
//            uint dwMaximumSizeLow,                          // DWORD dwCreationDisposition
//            string lpName,                                  // DWORD dwFladsAndAttributes
//            IntPtr hTemplateFile                            // HANDLE hTemplateFile
//        );

//        // Завершение процесса
//        [DllImport("kernel32.dll")]
//        static extern bool TerminateProcess(
//            IntPtr hProcess,                                // HANDLE
//            uint uExitCode                                  // UINT
//        );

//        // Ожидание завершения процесса
//        [DllImport("kernel32.dll")]
//        static extern uint WaitForSingleObject(
//            IntPtr hHandle,                                 // HANDLE
//            uint dwMilliseconds                             // DWORD
//        );

//        // Завершить работу с дескриптором
//        [DllImport("kernel32.dll")]
//        static extern void CloseHandle(
//            IntPtr hHandle                                  // HANDLE
//        );

//        // Получить момент времени
//        [DllImport("kernel32.dll")]
//        static extern void GetSystemTime(
//            ref SYSTEMTIME lpSystemTime                     // LPSYSTEMTIME
//        );

//        // Получить информацию о памяти процесса
//        [DllImport("psapi.dll", SetLastError = true)]
//        static extern bool GetProcessMemoryInfo(
//            IntPtr hProcess,                                // HANDLE
//            ref PROCESS_MEMORY_COUNTERS counters,           // PPROCESS_MEMORY_COUNTERS
//            uint size                                       // DWORD
//        );

//        // Абсолютное значение числа
//        public static uint Abs(int number)
//        {
//            return (uint)((number >= 0) ? number : -number);
//        }

//        // Стартовая информация
//        public static STARTUPINFO si;

//        // Информация о процессе
//        public static PROCESS_INFORMATION pi;

//        // Режим наследования файловых дескрипторов
//        public static bool inheritance;

//        // Запуск Windows приложения
//        public static bool Common(string executable, ref uint memory, ref uint time)
//        {
//            // Информация о процессе
//            pi = new PROCESS_INFORMATION();

//            // Момент запуска дочернего процесса
//            SYSTEMTIME startTime = new SYSTEMTIME();
//            GetSystemTime(ref startTime);

//            // Запускаем дочерний процесс
//            bool result = CreateProcess(executable, null, IntPtr.Zero, IntPtr.Zero, inheritance, 0, IntPtr.Zero, null, ref si, ref pi);

//            if (!result)
//                return false;

//            // Ожидание завершения процесса
//            uint res_code = WaitForSingleObject(pi.hProcess, (uint)time);
//            if (res_code == WAIT_TIMEOUT)
//            {
//                TerminateProcess(pi.hProcess, (uint)0);
//            }
//            else if (res_code != WAIT_OK)
//            {
//                return false;
//            }

//            // Момент завершения дочернего процесса
//            SYSTEMTIME stopTime = new SYSTEMTIME();
//            GetSystemTime(ref stopTime);

//            // Время работы в миллисекундах
//            time =
//                Abs(stopTime.wHour - startTime.wHour) * 60 * 60 * 1000 +
//                Abs(stopTime.wMinute - startTime.wMinute) * 60 * 1000 +
//                Abs(stopTime.wSecond - startTime.wSecond) * 1000 +
//                Abs(stopTime.wMilliseconds - startTime.wMilliseconds);

//            // Получим данные об использовании памяти
//            PROCESS_MEMORY_COUNTERS pmc = new PROCESS_MEMORY_COUNTERS
//            {
//                cb = (uint)Marshal.SizeOf(typeof(PROCESS_MEMORY_COUNTERS))
//            };
//            result = GetProcessMemoryInfo(pi.hProcess, ref pmc, pmc.cb);

//            // Память использованная процессом
//            memory = pmc.PeakPagefileUsage;

//            // Завершаем работу с дескрипторами
//            CloseHandle(pi.hProcess);

//            return true;
//        }

//        // Запуск Windows приложения с файловым вводом-выводом
//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool FileInputOutput(
//            string executable,
//            ref uint memory,
//            ref uint time
//        )
//        {
//            // Режим наследования файловых дескрипторов
//            inheritance = false;

//            // Стартовая информация
//            si = new STARTUPINFO
//            {
//                cb = (uint)Marshal.SizeOf(typeof(STARTUPINFO))
//            };

//            return Common(executable, ref memory, ref time);
//        }

//        // Запуск Windows приложения с консольным вводом-выводом
//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool ConsoleInputOutput(string executable, string input, string output, ref uint memory, ref uint time)
//        {
//            // Режим наследования файловых дескрипторов
//            inheritance = true;

//            // Разрешаем дочернему процессу использовать входной и выходной файлы
//            SECURITY_ATTRIBUTES securityAttributes = new SECURITY_ATTRIBUTES
//            {
//                nLength = (uint)Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES)),
//                lpSecurityDescriptor = IntPtr.Zero,
//                bInheritHandle = inheritance
//            };

//            // Входной файл
//            IntPtr hStdInput = CreateFile(
//                input,                  // LPCSTR lpFileName
//                GENERIC_READ,           // DWORD dwDesiredAccess
//                FILE_SHARE_READ,        // DWORD dwSharedMode
//                ref securityAttributes, // LPSECURITY_ATTRIBUTES lpSecurityAttributes
//                OPEN_ALWAYS,            // DWORD dwCreationDisposition
//                FILE_ATTRIBUTE_NORMAL,  // DWORD dwFladsAndAttributes
//                IntPtr.Zero             // HANDLE hTemplateFile
//            );

//            // Выходной файл
//            IntPtr hStdOutput = CreateFile(
//                output,                 // LPCSTR lpFileName
//                GENERIC_WRITE,          // DWORD dwDesiredAccess
//                FILE_SHARE_WRITE,       // DWORD dwSharedMode
//                ref securityAttributes, // LPSECURITY_ATTRIBUTES lpSecurityAttributes
//                CREATE_ALWAYS,          // DWORD dwCreationDisposition
//                FILE_ATTRIBUTE_NORMAL,  // DWORD dwFladsAndAttributes
//                IntPtr.Zero             // HANDLE hTemplateFile
//            );

//            // Стартовая информация
//            si = new STARTUPINFO
//            {
//                cb = (uint)Marshal.SizeOf(typeof(STARTUPINFO)),
//                dwFlags = STARTF_USESTDHANDLES,
//                hStdInput = hStdInput,
//                hStdOutput = hStdOutput,
//                hStdError = IntPtr.Zero
//            };

//            bool result = Common(executable, ref memory, ref time);

//            CloseHandle(hStdInput);
//            CloseHandle(hStdOutput);

//            return result;
//        }

//        // Запуск Windows приложения с интерактивным вводом-выводом
//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool InteractiveInputOutput(string executable, string input, string output, ref uint verdict, ref uint memory, ref uint time)
//        {
//            // Режим наследования файловых дескрипторов
//            inheritance = true;

//            // Разрешаем дочернему процессу использовать прямой и обратный каналы
//            SECURITY_ATTRIBUTES securityAttributes = new SECURITY_ATTRIBUTES
//            {
//                nLength = (uint)Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES)),
//                lpSecurityDescriptor = IntPtr.Zero,
//                bInheritHandle = inheritance
//            };

//            IntPtr forward_read = IntPtr.Zero;
//            IntPtr forward_write = IntPtr.Zero;
//            IntPtr backward_read = IntPtr.Zero;
//            IntPtr backward_write = IntPtr.Zero;

//            // Прямой анонимный канал
//            bool forward_created = CreatePipe(ref forward_read, ref forward_write, ref securityAttributes, 0);

//            // Обратный анонимный канал
//            bool backward_created = CreatePipe(ref backward_read, ref backward_write, ref securityAttributes, 0);

//            // Не удалось создать каналы
//            if (!forward_created || !backward_created)
//                return false;

//            // Информационные структуры для решения
//            STARTUPINFO solution_si = new STARTUPINFO
//            {
//                cb = (uint)Marshal.SizeOf(typeof(STARTUPINFO)),
//                dwFlags = STARTF_USESTDHANDLES,
//                hStdInput = forward_read,
//                hStdOutput = backward_write,
//                hStdError = IntPtr.Zero
//            };
//            PROCESS_INFORMATION solution_pi = new PROCESS_INFORMATION();

//            // Информационные структуры для интерактора
//            STARTUPINFO interactor_si = new STARTUPINFO
//            {
//                cb = (uint)Marshal.SizeOf(typeof(STARTUPINFO)),
//                dwFlags = STARTF_USESTDHANDLES,
//                hStdInput = backward_read,
//                hStdOutput = forward_write,
//                hStdError = IntPtr.Zero
//            };
//            PROCESS_INFORMATION interactor_pi = new PROCESS_INFORMATION();

//            // Момент запуска
//            SYSTEMTIME startTime = new SYSTEMTIME();
//            GetSystemTime(ref startTime);

//            // Запуск решения
//            bool solution_started = CreateProcess(executable, null, IntPtr.Zero, IntPtr.Zero, inheritance, 0, IntPtr.Zero, null, ref solution_si, ref solution_pi);

//            // Запуск интерактора
//            bool interactor_started = true;//CreateProcess("..\\interactor.exe", "input.txt output.txt", IntPtr.Zero, IntPtr.Zero, inheritance, 0, IntPtr.Zero, null, ref interactor_si, ref interactor_pi);

//            // Не удалось запустить решение или интерактор
//            if (!solution_started || !interactor_started)
//                return false;

//            // Ожидание завершения решения
//            uint res_code = WaitForSingleObject(solution_pi.hProcess, (uint)time);
//            if (res_code == WAIT_TIMEOUT)
//            {
//                TerminateProcess(solution_pi.hProcess, (uint)0);
//            }
//            else if (res_code != WAIT_OK)
//            {
//                return false;
//            }

//            // Момент завершения
//            SYSTEMTIME stopTime = new SYSTEMTIME();
//            GetSystemTime(ref stopTime);

//            // Ждем вердикт интерактора, таймаут 10 секунд
//            verdict = WaitForSingleObject(interactor_pi.hProcess, 10000);

//            // Время работы в миллисекундах
//            time =
//                Abs(stopTime.wHour - startTime.wHour) * 60 * 60 * 1000 +
//                Abs(stopTime.wMinute - startTime.wMinute) * 60 * 1000 +
//                Abs(stopTime.wSecond - startTime.wSecond) * 1000 +
//                Abs(stopTime.wMilliseconds - startTime.wMilliseconds);

//            // Получим данные об использовании памяти
//            PROCESS_MEMORY_COUNTERS pmc = new PROCESS_MEMORY_COUNTERS();
//            pmc.cb = (uint)Marshal.SizeOf(typeof(PROCESS_MEMORY_COUNTERS));
//            bool memory_query_succeed = GetProcessMemoryInfo(solution_pi.hProcess, ref pmc, pmc.cb);

//            if (!memory_query_succeed)
//                return false;

//            Logger.Log($"ok {pmc.cb} {pmc.PageFaultCount} {pmc.PagefileUsage} {pmc.PeakPagefileUsage} {pmc.PeakWorkingSetSize} {pmc.QuotaNonPagedPoolUsage} {pmc.QuotaPagedPoolUsage} " +
//    $"{pmc.QuotaPeakNonPagedPoolUsage} {pmc.QuotaPeakPagedPoolUsage} {pmc.WorkingSetSize}");


//            // Память использованная процессом
//            memory = pmc.PeakPagefileUsage;

//            // Завершаем работу с дескрипторами
//            CloseHandle(solution_pi.hProcess);
//            CloseHandle(interactor_pi.hProcess);
//            CloseHandle(forward_read);
//            CloseHandle(forward_write);
//            CloseHandle(backward_read);
//            CloseHandle(backward_write);

//            return true;
//        }
//    }
//}