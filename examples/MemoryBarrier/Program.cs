using System;
using System.Threading;
using System.Threading.Tasks;

namespace MemoryBarrier
{
    class Program
    {
        // Volatile is not going to help here as a store and a load can still be reordered.
        static int x = 0;
        static int y = 0;
        static int r1 = 0;
        static int r2 = 0;
        
        static void Main(string[] args)
        {
            var counter = 0;
            do {
                counter++;
                x = y = r1 = r2 = 0;
                var t1 = new Task(() => { Thread1(); });
                var t2 = new Task(() => { Thread2(); });
                t1.Start();
                t2.Start();
                
                Task.WaitAll(t1, t2);
                
                // only exit the loop when both r1 and r2 are 0 which shouldn't be possible right?
                // In reality this is possible because without any memory barriers these 2 threads might read older values of x and y.
                // Note: we do not need a memory barrier on the main thread here as Task.WaitAll already does that for us.
            } while (!(r1 == 0 && r2 == 0));
            
            Console.WriteLine($"r1={r1}, r2={r2} after {counter} tries");
        }
        public static void Thread1()
        {
            y = 1;  // Store y
            //Thread.MemoryBarrier();
            r1 = x; // Load x            
        }
        public static void Thread2()
        {
            x = 1;  // Store x
            //Thread.MemoryBarrier();
            r2 = y; // Load y     
        }
    }
}