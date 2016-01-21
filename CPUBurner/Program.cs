using System;
using System.Threading;

namespace CPUBurner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
           
            Console.WriteLine("Starting");

  
            var threadProc = new ThreadStart(Burn);

            for (var n = 0; n < 4; n++)
            {
                var thread  = new Thread(threadProc);
                thread.Priority = ThreadPriority.Highest;
                thread.Name = string.Format("Burn-{0}", n);
                thread.IsBackground = true; // Not important enough to hold process running.
                thread.Start();
            }
          
            Console.WriteLine("Ending");
            Console.ReadLine();
        }



        private static void Burn()
        {
            var i = 1L;
            var startTime = DateTime.Now;
            try
            {
                for (; i < 10000000000000000; i++)
                {
                    for (var y = 1.0; y < 10000; )
                    {
                        y = y + 0.1;
                    }

                    Console.WriteLine("{0}:{1}", i, (DateTime.Now - startTime).TotalSeconds);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(i);
            }




        }





    }
}