using System;
using System.Collections.Generic;

namespace MemoryHog
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Starting");

            int i = 0;
            var startTime = DateTime.Now;

                var barrel = new List<Object>();


                for (i = 0; i < 1000000000000; i++)
                {

                    var bucket = new List<string>();

                    for (var y = 0; y < 100000; y++)
                    {
                        bucket.Add(Guid.NewGuid().ToString());
                    }
                    Console.WriteLine("bucket {0}:{1}", i, (DateTime.Now - startTime).TotalSeconds);
                    barrel.Add(bucket);
                }


            Console.WriteLine("Ending");
            Console.ReadLine();
        }
    }
}