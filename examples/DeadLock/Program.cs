using System;
using System.Threading;

namespace DeadLock
{
    class Program
    {
        static void Main(string[] args)
        {
            // Rules to avoid deadlocks:
            // Never lock on a instance that might be locked upon beyond your control, this includes types, strings, public properties, this etc.
            // Acquire locks in the same order
            
            var thread1 = new Thread(() =>
            {
                lock (typeof(int))
                {
                    Console.WriteLine("Thread 1 got the int lock");
                    // For demonstration purposes we are going to wait here so the other thread has time to acquire the float lock.
                    // In reality this might or might not happen in time which can make reproducing deadlocks very hard.
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
            
            // This will never be executed.
            Console.WriteLine("Complete");
        }
    }
}