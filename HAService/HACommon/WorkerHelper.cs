//using System;
//using System.Web;
//using System.Threading;
////using System.Diagnostics;
//using System.Reflection;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using System.IO;

//namespace Imps.Services.HA
//{
//    public static class WorkerHelper
//    {
//        private static volatile bool _isSetUnhandledExceptionHandler = false;
//        public static class MINIDUMP_TYPE
//        {
//            public const int MiniDumpNormal = 0x00000000;
//            public const int MiniDumpWithDataSegs = 0x00000001;
//            public const int MiniDumpWithFullMemory = 0x00000002;
//            public const int MiniDumpWithHandleData = 0x00000004;
//            public const int MiniDumpFilterMemory = 0x00000008;
//            public const int MiniDumpScanMemory = 0x00000010;
//            public const int MiniDumpWithUnloadedModules = 0x00000020;
//            public const int MiniDumpWithIndirectlyReferencedMemory = 0x00000040;
//            public const int MiniDumpFilterModulePaths = 0x00000080;
//            public const int MiniDumpWithProcessThreadData = 0x00000100;
//            public const int MiniDumpWithPrivateReadWriteMemory = 0x00000200;
//            public const int MiniDumpWithoutOptionalData = 0x00000400;
//            public const int MiniDumpWithFullMemoryInfo = 0x00000800;
//            public const int MiniDumpWithThreadInfo = 0x00001000;
//            public const int MiniDumpWithCodeSegs = 0x00002000;
//        }

//        [DllImport("dbghelp.dll")]
//        public static extern bool MiniDumpWriteDump(IntPtr hProcess,
//                                                    Int32 ProcessId,
//                                                    IntPtr hFile,
//                                                    int DumpType,
//                                                    IntPtr ExceptionParam,
//                                                    IntPtr UserStreamParam,
//                                                    IntPtr CallackParam);

//        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
//        {
//            try
//            {
//                string msg = "Cactched Unhandled Exception!" + e.ExceptionObject.ToString();
//                Trace.Write(msg);
//                if (!EventLog.SourceExists(EventSource))
//                    EventLog.CreateEventSource(EventSource, "Application");

//                EventLog.WriteEntry(EventSource, msg, EventLogEntryType.Error, 4444);
//            }
//            catch (Exception ex)
//            {
//                Trace.Write(ex.ToString());
//            }

//            if (e.IsTerminating == true)
//            {
//                Process process = Process.GetCurrentProcess();
//                try
//                {
//                    CreateMiniDump(process);
//                }
//                catch (Exception ex)
//                {
//                    EventLog.WriteEntry(EventSource, string.Format("zhua dump chu cuo:{0}", ex), EventLogEntryType.Error, 4445);
//                }
//            }
//        }

//        public static void CreateMiniDump(Process process)
//        {
//            string path = AppDomain.CurrentDomain.BaseDirectory;
//            if (!path.EndsWith("\\"))
//                path = path + "\\";

//            string fileName = string.Format("{0}CrashDump_{1}.dmp", path, DateTime.Now.ToString("yyyy_MM_ddThh_mm_ss.fff"));

//            using (FileStream fs = new FileStream(fileName, FileMode.Create))
//            {
//                MiniDumpWriteDump(process.Handle,
//                     process.Id,
//                     fs.SafeFileHandle.DangerousGetHandle(),
//                     MINIDUMP_TYPE.MiniDumpNormal,
//                     IntPtr.Zero,
//                     IntPtr.Zero,
//                     IntPtr.Zero);
//            };
//        }


//        public static void SetUnhandledExceptionHandler()
//        {
//            if (!_isSetUnhandledExceptionHandler)
//            {
//                _isSetUnhandledExceptionHandler = true;
//                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
//            }
//        }

//        public static string GetCurrentProcessName()
//        {
//            return Process.GetCurrentProcess().ProcessName;
//            /// todo
//            //if (HttpRuntime.AppDomainId == null)
//            //{
//            //    return Process.GetCurrentProcess().ProcessName;
//            //}
//            //else
//            //{
//            //    return string.Format("IIS_{0}", HttpRuntime.AppDomainAppVirtualPath);
//            //}
//        }

//        /// <summary>
//        /// ≤È’“IHAComponent
//        /// </summary>
//        /// <returns></returns>
//        public static Type FindComponent()
//        {
//            string basePath = typeof(WorkerHelper).Assembly.CodeBase;
//            basePath = basePath.Substring(8, basePath.LastIndexOf('/') - 8);

//            string[] dllFiles = Directory.GetFiles(basePath, "*.dll");
//            string[] exeFiles = Directory.GetFiles(basePath, "*.exe");

//            string[] files = new string[dllFiles.Length + exeFiles.Length];
//            dllFiles.CopyTo(files, 0);
//            exeFiles.CopyTo(files, dllFiles.Length);

//            foreach (string fileName in files)
//            {
//                Assembly asm = null;
//                try
//                {
//                    asm = Assembly.LoadFrom(fileName);
//                    foreach (Type type in asm.GetTypes())
//                    {
//                        if (type.BaseType == typeof(IHAComponent))
//                        {
//                            return type;
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    throw ex;
//                }
//            }
//            return null;
//        }
//        public static readonly string EventSource = "HaWorker[" + GetCurrentProcessName() + "]";
//    }
//}