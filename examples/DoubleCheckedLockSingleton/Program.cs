using System;
using System.Threading;

namespace DoubleCheckedLockSingleton
{
    class Program
    {
        static void Main(string[] args)
        {
            // This example is hard to reproduce because its behavior can change based on platform, versions and if the compiler likes you.
            // Its simpler and more correct to just use Lazy<T>.
        }
    }
    
    public sealed class DoubleCheckedLockSingleton {
        static DoubleCheckedLockSingleton instance = null;
        static readonly object padlock = new object();

        DoubleCheckedLockSingleton() {
        }

        public static DoubleCheckedLockSingleton Instance {
            get {
                // First we check if the instance is null outside the lock to prevent having to lock. The idea here is to avoid the 'cost' of the lock.
                if (instance != null) {
                    return instance;
                }
                    
                lock (padlock) {
                    // Instance might have been assigned between the first check and here so lets check it again inside the lock.
                    if (instance != null) {
                        return instance;
                    }

                    // Instance was not yet created so lets create it
                    var tempInstance = new DoubleCheckedLockSingleton();

                    // For correctness we have to include a memory barrier here
                    // Not doing so might cause a not fully initialized instance of DoubleCheckedLockSingleton to be exposed to other threads.
                    // This is possible because of compiler optimizations, cpu cache etc.
                    // Depending on the cpu, .net runtime version, debug/release build etc this might or might not work correctly without a memory barrier.
                    Thread.MemoryBarrier();
                    
                    instance = tempInstance;
                }
                return instance;
            }
        }
    }
}