using System;
using System.Threading.Tasks;

namespace ParallelIncrementWrong
{
    class Program
    {
        private static int sharedCounter;
        
        static void Main(string[] args)
        {
            for (int i = 0; i < 100; i++)
            {
                Parallel.For(0, 100, y =>
                {
                    // This is not threadsafe because the variable counter can change after reading it.
                    // In steps this normally happens:
                    // 1. Read counter, we get a value of 3                
                    // 2. Add 1 to counter to get 4
                    // 3. Store 4 in counter
                
                    // However with multiple threads this can happen:
                    // 1. Read counter, we get a value of 3                
                    // 2. Add 1 to counter to get 4
                    // 3. Other thread updates counter to 4
                    // 4. Store 4 in counter
                    // Now we ended up with the value of 4 while we should have 5!
                    for (int j = 0; j < 100; j++)
                    {
                        sharedCounter++;
                        //Interlocked.Increment(ref sharedCounter);
                    }
                });
                Console.WriteLine(sharedCounter);
                
                sharedCounter = 0;
            }
            Console.ReadKey();
        }
    }
}