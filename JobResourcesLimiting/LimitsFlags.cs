using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobResourcesLimiting
{
    internal class LimitsFlags
    {
       public const uint LimitActiveProcess = 8;
       public const uint LimitJobTime = 64;
       public const uint LimitJobMemory = 512;
       public const uint LimitKillOnJobClose =8192;
       public const uint LimitJobTimeJobMemory = 8706;


    }
}
