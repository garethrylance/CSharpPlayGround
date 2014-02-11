using System;
using System.Collections.Generic;

namespace MemoryHog
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Press a Key to Start");
            Console.ReadLine();
            Console.WriteLine("Starting");

            var bucket = new List<string>();

            const string longString =
                "sdddddddddddddddddddddddddddddddddddddddddddddddddddhdddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd";
            int i = 0;
            var startTime = DateTime.Now;
            try
            {
                for (i = 0; i < 1000000000000; i++)
                {
                    bucket.Add(longString + Guid.NewGuid());

                    if (i % 10 == 0 && i != 0)
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