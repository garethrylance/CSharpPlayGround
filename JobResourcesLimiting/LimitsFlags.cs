using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobResourcesLimiting
{
    internal class LimitsFlags
    {
       public const uint JOB_OBJECT_LIMIT_ACTIVE_PROCESS = 0x00000008;
       public const uint JOB_OBJECT_LIMIT_JOB_MEMORY = 0x00000200;
       public const uint JOB_OBJECT_LIMIT_JOB_TIME = 0x00000040;
       public const uint LimitTimeCpu = 8706;
 
 
    }
}
