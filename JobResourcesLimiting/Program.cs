using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JobResourcesLimiting
{
    class Program
    {

        /*Example of resource limiting using http://msdn.microsoft.com/en-us/library/ms684161.aspx
         * 
         * Taken from
         * 
         * http://www.xtremevbtalk.com/showpost.php?p=1335552&postcount=22
         * http://stackoverflow.com/questions/3342941/kill-child-process-when-parent-process-is-killed
         * * 
         * 
         * */


        static void Main(string[] args)
        {

            var job = new Job();
            var cpuBurnerProcess = Process.Start(@"..\..\..\CPUBurner\bin\Debug\CPUBurner.exe");
            var memoryHogProcess = Process.Start(@"..\..\..\MemoryHog\bin\Debug\MemoryHog.exe");
           
            Thread.Sleep(100);
            /*
            Console.WriteLine("Enter PID");
            var pidInput =  Console.ReadLine();
            var pid = Int32.Parse(pidInput);
             */

            job.AddProcess(cpuBurnerProcess.Handle);
            job.AddProcess(memoryHogProcess.Handle);


            Console.WriteLine("Ending");
            Console.ReadLine();
        }

        public enum JobObjectInfoType
        {
            AssociateCompletionPortInformation = 7,
            BasicLimitInformation = 2,
            BasicUIRestrictions = 4,
            EndOfJobTimeInformation = 6,
            ExtendedLimitInformation = 9,
            SecurityLimitInformation = 5,
            GroupInformation = 11
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct JOBOBJECT_BASIC_LIMIT_INFORMATION
        {
            public Int64 PerProcessUserTimeLimit;
            public Int64 PerJobUserTimeLimit;
            public UInt32 LimitFlags;
            public UInt32 MinimumWorkingSetSize;
            public UInt32 MaximumWorkingSetSize;
            public Int16 ActiveProcessLimit;
            public Int64 Affinity;
            public Int16 PriorityClass;
            public Int16 SchedulingClass;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct IO_COUNTERS
        {
            public UInt64 ReadOperationCount;
            public UInt64 WriteOperationCount;
            public UInt64 OtherOperationCount;
            public UInt64 ReadTransferCount;
            public UInt64 WriteTransferCount;
            public UInt64 OtherTransferCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
        {
            public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
            public IO_COUNTERS IoInfo;
            public UInt32 ProcessMemoryLimit;
            public UInt32 JobMemoryLimit;
            public UInt32 PeakProcessMemoryUsed;
            public UInt32 PeakJobMemoryUsed;
        }

        public class Job : IDisposable
        {

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            static extern IntPtr CreateJobObject([In] ref SECURITY_ATTRIBUTES
               lpJobAttributes, string lpName);

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern bool SetInformationJobObject(IntPtr hJob, JobObjectInfoType infoType, IntPtr lpJobObjectInfo, uint cbJobObjectInfoLength);

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);


            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool CloseHandle(IntPtr hObject);

            private IntPtr m_handle;
            private bool m_disposed = false;

            public Job()
            {
                var xx = new SECURITY_ATTRIBUTES();

                m_handle = CreateJobObject(ref xx, "testJob");

                JOBOBJECT_BASIC_LIMIT_INFORMATION info = new JOBOBJECT_BASIC_LIMIT_INFORMATION();
                info.LimitFlags = LimitsFlags.LimitJobTimeJobMemory;
                info.PerProcessUserTimeLimit = (long)5e7;

                JOBOBJECT_EXTENDED_LIMIT_INFORMATION extendedInfo = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION();
                extendedInfo.BasicLimitInformation = info;
                extendedInfo.ProcessMemoryLimit = 10000000;
                extendedInfo.JobMemoryLimit = 10000000;
                extendedInfo.IoInfo = new IO_COUNTERS();
                extendedInfo.PeakJobMemoryUsed = 10000000;
                extendedInfo.PeakProcessMemoryUsed = 10000000;
                extendedInfo.ProcessMemoryLimit = 10000000;


                int length = Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
                IntPtr extendedInfoPtr = Marshal.AllocHGlobal(length);
                Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);


                if (!SetInformationJobObject(m_handle, JobObjectInfoType.ExtendedLimitInformation, extendedInfoPtr, (uint)length))
                    throw new Exception(string.Format("Unable to set information.  Error: {0}", Marshal.GetLastWin32Error()));
            }

            #region IDisposable Members

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion

            private void Dispose(bool disposing)
            {
                if (m_disposed)
                    return;

                if (disposing) { }

                Close();
                m_disposed = true;
            }

            public void Close()
            {
                CloseHandle(m_handle);
                m_handle = IntPtr.Zero;
            }

            public bool AddProcess(IntPtr handle)
            {
                return AssignProcessToJobObject(m_handle, handle);
            }

        }



    }
}
