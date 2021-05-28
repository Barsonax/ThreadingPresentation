using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // The console program main thread does not have a SynchronizationContext. 
            
            // Executing some work asynchronously.
            var task = Task.Run(() => Console.WriteLine($"task1 task on thread {Thread.CurrentThread.ManagedThreadId}"));

            Console.WriteLine($"main on thread {Thread.CurrentThread.ManagedThreadId}");
            await task; // Basically a nicer way of writing task.ContinueWith(...) with the notable exception that await tries to schedule the continuation on the SynchronizationContext by default.
            Console.WriteLine($"main on thread {Thread.CurrentThread.ManagedThreadId}");
            
            var task2 = Task.Run(() => Console.WriteLine($"task2 task on thread {Thread.CurrentThread.ManagedThreadId}"));
            
            await task2;
            Console.WriteLine($"main on thread {Thread.CurrentThread.ManagedThreadId}");
            
            // Now lets do something both in parallel and asynchronous!
            Console.WriteLine($"Starting to do some work in parallel..");
            var tasks = new Task[10];
            for (int i = 0; i < tasks.Length; i++)
            {
                var i1 = i;
                tasks[i] = Task.Run(() => Console.WriteLine($"task{i1 + 2} on thread {Thread.CurrentThread.ManagedThreadId}"));
            }
            
            await Task.WhenAll(tasks);
            Console.WriteLine($"main on thread {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}