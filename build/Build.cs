using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nuke.Common;
using Nuke.Common.Execution;

[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.NewThread);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")] readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target NewThread => _ => _
        .Executes(() =>
        {
            var thread = new Thread(() =>
            {
                Console.WriteLine($"My thread is {Thread.CurrentThread.ManagedThreadId}");
            });

            thread.Start();
            thread.Join();
        });

    Target NewTask => _ => _
        .Executes(() =>
        {
            var task = Task.Run(() =>
            {
                Console.WriteLine($"My thread is {Thread.CurrentThread.ManagedThreadId}");
            });

            task.Wait();
        });

    Target ParallelForeach => _ => _
        .Executes(() =>
        {
            Parallel.ForEach(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, i =>
              {
                  for (int j = 0; j < 10; j++)
                  {
                      Thread.Sleep(100);
                      Console.WriteLine($"My thread is {Thread.CurrentThread.ManagedThreadId}");
                  }
              });
        });

    Target Locking => _ => _
        .Executes(() =>
        {
            var locker = new object();

            Parallel.ForEach(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, i =>
              {
                  lock (locker)
                  {
                      for (int j = 0; j < 10; j++)
                      {
                          Thread.Sleep(100);
                          Console.WriteLine($"My thread is {Thread.CurrentThread.ManagedThreadId}");
                      }
                  }
              });
        });

    Target DeadLock => _ => _
    .Executes(() =>
    {
        var thread1 = new Thread(() =>
        {
            lock (typeof(int))
            {
                Console.WriteLine("Thread 1 got the int lock");
                Thread.Sleep(1000);
                lock (typeof(float))
                {
                    Console.WriteLine("Thread 1 got both locks");
                }

            }
        });

        var thread2 = new Thread(() =>
        {
            lock (typeof(float))
            {
                Console.WriteLine("Thread 2 got the float lock");
                Thread.Sleep(1000);
                lock (typeof(int))
                {                    
                    Console.WriteLine("Thread 2 got both locks");
                }
            }
        });

        thread1.Start();
        thread2.Start();

        thread1.Join();
        thread2.Join();
    });

    Target ParallelIncrementWrong => _ => _
        .Executes(() =>
        {
            var counter = 0;
            Parallel.For(0, 100, i =>
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
                counter = counter + 1;
            });
            Console.WriteLine(counter);
        });

    Target ParallelIncrement => _ => _
        .Executes(() =>
        {
            var counter = 0;
            Parallel.For(0, 100, i =>
            {
                Interlocked.Increment(ref counter);
            });
            Console.WriteLine(counter);
        });

    public class DataClass {
        public DataClass(int someNumber, int someNegativeNumber)
        {
            SomeNumber = someNumber;
            SomeNegativeNumber = someNegativeNumber;
        }

    public int SomeNumber { get; }
    public int SomeNegativeNumber { get; }
}

    Target CompareAndSwapExample => _ => _
        .Executes(() =>
        {      
            var totalValue = new DataClass(0, 0);
            Parallel.For(0, 100, i =>
            {
                DataClass initialValue, newValue;
                do
                {   
                    // First we are going to take a snapshot of our current state.
                    initialValue = totalValue;

                    // Lets do some work on our snapshot, without mutating anything shared to other threads.          
                    newValue = new DataClass(initialValue.SomeNumber + 1, initialValue.SomeNegativeNumber - 1);
                
                // Check if the state was not modified in the meantime. 
                // If not then we can safely update the state by swapping the reference else we have to repeat.
                // Note: this is a simple example in which we just repeat all of the work if another thread modified the state
                // However we can also do optimizations such as reusing the DataClass instance we already made.
                } while (initialValue != Interlocked.CompareExchange(ref totalValue, newValue, initialValue));

            });
            Console.WriteLine(totalValue.SomeNumber);
            Console.WriteLine(totalValue.SomeNegativeNumber);
        });
}