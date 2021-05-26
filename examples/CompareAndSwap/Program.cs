using System;
using System.Threading;
using System.Threading.Tasks;

namespace CompareAndSwap
{
    class Program
    {
        static void Main(string[] args)
        {
            var totalValue = new DataClass(0, 0);
            Parallel.For(0, 100000, i =>
            {
                // This pattern demonstrates how we can do more complex work without resorting to locks. This is a more advanced pattern which may lead to better performance but quite often you won't need this.
                DataClass initialValue, newValue;
                do
                {   
                    // First we are going to take a snapshot of our current state.
                    initialValue = totalValue;

                    // Lets do some work on our snapshot, without mutating anything shared to other threads.          
                    newValue = new DataClass(initialValue.SomeNumber + 1, initialValue.SomeNegativeNumber - 1);
                
                    // Check if the state was not modified in the meantime. 
                    // If not then we can safely update the state by swapping the reference else we have to repeat.
                    // Note: this is a simple example in which we just repeat all of the work if another thread modified the state, the performance gain versus locks is small.
                    // However we can also do optimizations such as reusing the DataClass instance we already made, in that case you can get bigger performance gains.
                } while (initialValue != Interlocked.CompareExchange(ref totalValue, newValue, initialValue));

            });
            Console.WriteLine(totalValue.SomeNumber);
            Console.WriteLine(totalValue.SomeNegativeNumber);
        }
    }
    
    public class DataClass {
        public DataClass(int someNumber, int someNegativeNumber)
        {
            SomeNumber = someNumber;
            SomeNegativeNumber = someNegativeNumber;
        }

        public int SomeNumber { get; }
        public int SomeNegativeNumber { get; }
    }
}