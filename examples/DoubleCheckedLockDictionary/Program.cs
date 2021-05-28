using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DoubleCheckedLockDictionary
{
    class Program
    {
        private static Dictionary<int, int> _languageLists = new Dictionary<int, int>();
        private static object locker = new object();
        
        static void Main(string[] args)
        {
            // Inspired by Uptrends.DashboardEngine.Business.LanguageProvider.InitializeLanguageList
            // Dictionary is not threadsafe and accessing it from multiple threads at the same time will lead to undetermined behavior.

            var counter = 0;
            try
            {
                while (true)
                {
                    counter++;
                    _languageLists = new Dictionary<int, int>();
                
                    var t1 = new Task(() =>
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            InitializeLanguageList(i);
                        }
                    });
                    var t2 =  new Task(() =>
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            InitializeLanguageList(i);
                        }
                    });
                
                    t1.Start();
                    t2.Start();

                    Task.WaitAll(t1, t2);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error after {counter} tries");
                throw;
            }

            
            // May result in 
            // Unhandled exception. System.NullReferenceException: Object reference not set to an instance of an object.
            // at System.Collections.Generic.Dictionary`2.FindEntry(TKey key)
            // at System.Collections.Generic.Dictionary`2.ContainsKey(TKey key)
            // at DoubleCheckedLockDictionary.Program.InitializeLanguageList(Int32 languageCode) in C:\Users\damric\OneDrive - Uptrends\Presentaties\Threading\examples\DoubleCheckedLockDictionary\Program.cs:line 47
            // at DoubleCheckedLockDictionary.Program.<>c.<Main>b__2_1() in C:\Users\damric\OneDrive - Uptrends\Presentaties\Threading\examples\DoubleCheckedLockDictionary\Program.cs:line 32
            // at System.Threading.ThreadHelper.ThreadStart_Context(Object state)
            // at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
            //   --- End of stack trace from previous location where exception was thrown ---
            //    at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
            // at System.Threading.ThreadHelper.ThreadStart()

        }
        
        private static void InitializeLanguageList(int languageCode)
        {
            // See if already present
            if (_languageLists.ContainsKey(languageCode))
            {
                return;
            }

            // Lock this object and try again
            lock (locker)
            {
                if (_languageLists.ContainsKey(languageCode))
                {
                    return;
                }

                _languageLists.Add(languageCode, languageCode);
            }
        }
    }
}