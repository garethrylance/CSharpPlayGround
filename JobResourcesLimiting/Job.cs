using System;
using System.Runtime.InteropServices;

namespace JobResourcesLimiting
{

    [Flags]
    public enum JobObjectCpuRateFlags : uint
    {
        Enable = 0x00000001,
        WeightBased = 0x00000002,
        HardCap = 0x00000004,
        Notify = 0x00000008
    }



    [StructLayout(LayoutKind.Explicit)]
    struct JOBOBJECT_CPU_RATE_CONTROL_INFORMATION
    {
        [FieldOffset(0)]
        public UInt32 ControlFlags;
        [FieldOffset(4)]
        public UInt32 CpuRate;
        [FieldOffset(4)]
        public UInt32 Weight;
    }

    public enum CpuFlags
    {
        JOB_OBJECT_CPU_RATE_CONTROL_ENABLE = 0x00000001,
        JOB_OBJECT_CPU_RATE_CONTROL_WEIGHT_BASED = 0x00000002,
        JOB_OBJECT_CPU_RATE_CONTROL_HARD_CAP = 0x00000004
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


        private uint maxMemory =  UInt32.MaxValue;


        public Job(String name, uint maxMemoryMB, uint maxCPUPercent)
        {
            maxMemory = maxMemoryMB*1000*1000;

            var securityAttributes = new SECURITY_ATTRIBUTES();

            m_handle = CreateJobObject(ref securityAttributes, name);

            JOBOBJECT_BASIC_LIMIT_INFORMATION basicInformation = new JOBOBJECT_BASIC_LIMIT_INFORMATION();
            basicInformation.LimitFlags = JobObjectLimitType.ProcessMemory | JobObjectLimitType.JobMemory |
                                            JobObjectLimitType.DieOnUnhandledException | JobObjectLimitType.KillOnJobClose;

            //JOB_OBJECT_CPU_RATE_CONTROL_ENABLE

            JOBOBJECT_EXTENDED_LIMIT_INFORMATION extendedInfo = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION();
            extendedInfo.BasicLimitInformation = basicInformation;
            extendedInfo.ProcessMemoryLimit = maxMemory;
            extendedInfo.JobMemoryLimit = maxMemory;
            extendedInfo.IoInfo = new IO_COUNTERS();
            extendedInfo.PeakJobMemoryUsed = maxMemory;
            extendedInfo.PeakProcessMemoryUsed = maxMemory;
            extendedInfo.ProcessMemoryLimit = maxMemory;




            var cpuLimits = new JOBOBJECT_CPU_RATE_CONTROL_INFORMATION();
            cpuLimits.ControlFlags = (UInt32)(CpuFlags.JOB_OBJECT_CPU_RATE_CONTROL_ENABLE | CpuFlags.JOB_OBJECT_CPU_RATE_CONTROL_HARD_CAP);
            cpuLimits.CpuRate = maxCPUPercent * 100; // Limit CPu usage to 45%

            var pointerToJobCpuLimits = Marshal.AllocHGlobal(Marshal.SizeOf(cpuLimits));
            Marshal.StructureToPtr(cpuLimits, pointerToJobCpuLimits, false);
            if (!SetInformationJobObject(m_handle, JobObjectInfoType.JobObjectCpuRateControlInformation, pointerToJobCpuLimits, (uint)Marshal.SizeOf(cpuLimits)))
            {
                throw new Exception(string.Format("Unable to set information.  Error: {0}", Marshal.GetLastWin32Error()));
            }


            int length = Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
            IntPtr extendedInfoPtr = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);

            if (
                !SetInformationJobObject(m_handle, JobObjectInfoType.ExtendedLimitInformation, extendedInfoPtr,
                    (uint) length))
            {
                throw new Exception(string.Format("Unable to set information.  Error: {0}", Marshal.GetLastWin32Error()));
            }
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