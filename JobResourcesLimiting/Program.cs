using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JobResourcesLimiting
{
    internal class Program
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


        private static void Main(string[] args)
        {

            var small = new Job("small Job",500,20);
            var large = new Job("large Job", 4000, 80);

            const string CPUBurnerCommand = @"..\..\..\CPUBurner\bin\Debug\CPUBurner.exe";
            const string MemoryHogCommand = @"..\..\..\MemoryHog\bin\Debug\MemoryHog.exe";

           

            Thread.Sleep(100);
            /*
            Console.WriteLine("Enter PID");
            var pidInput =  Console.ReadLine();
            var pid = Int32.Parse(pidInput);
             */

            small.AddProcess(Process.Start(MemoryHogCommand).Handle);
            small.AddProcess(Process.Start(CPUBurnerCommand).Handle);

            large.AddProcess(Process.Start(MemoryHogCommand).Handle);
            large.AddProcess(Process.Start(CPUBurnerCommand).Handle);

            



            Console.WriteLine("Ending");
            Console.ReadLine();
        }


    }
}

