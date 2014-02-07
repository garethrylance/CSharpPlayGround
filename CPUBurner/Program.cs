using System;

namespace CPUBurner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Press a Key to Start");
            Console.ReadLine();
            Console.WriteLine("Starting");

            int i = 0;
            var startTime = DateTime.Now;
            try
            {
                for (i = 0; i < 10000000000000000; i++)
                {
                    if (i % 100000 == 0 && i != 0)
                    {
                        Console.WriteLine("{0}:{1}", i, (DateTime.Now - startTime).TotalSeconds);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(i);
            }

            Console.WriteLine("Ending");
            Console.ReadLine();
        }
    }
}