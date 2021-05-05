namespace DefaultNamespace
{
    /// <summary>
    /// Class containing nasty antipatterns which may or may not work
    /// </summary>
    public class AntiPatterns
    {
        public sealed class DoubleCheckedLockSingleton {
            static DoubleCheckedLockSingleton instance = null;
            static readonly object padlock = new object();

            DoubleCheckedLockSingleton() {
            }

            public static DoubleCheckedLockSingleton Instance {
                get {
                    // First we check if the instance is null outside the lock to prevent having to lock.
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

                        // initialize the object with data

                        instance = tempInstance;
                    }
                    return instance;
                }
            }
        }
    }
}