using System;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelForeach
{
    class Program
    {
        static void Main(string[] args)
        {
            // This will run in parallel on the threadpool.
            Parallel.ForEach(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, i =>
            {
                for (int j = 0; j < 10; j++)
                {
                    Thread.Sleep(100);
                    Console.WriteLine($"My thread is {Thread.CurrentThread.ManagedThreadId}");
                }
            });
        }
    }
}