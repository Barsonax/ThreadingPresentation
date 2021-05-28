---
theme : "white"
transition: "slide"
highlightTheme: "vs2015"
slideNumber: true

title: "Threading presentation"
enableTitleFooter: false
---

## Multithreadeding

<a>
    <img style="border: unset; box-shadow: unset" data-src="https://github.com/Barsonax/threadingpresentation/raw/master/images/bd534296c00eda2c7a74c05855d29b0c.jpg">
</a>

---

## In deze presentatie
- Wat is multithreaded?
- Waarom multithreaded?
- Hoe gebruik je multithreading?
- Code voorbeelden.
- Vragen.

---

## Wat is multithreading?
- Meerdere acties tegelijkertijd uitvoeren.
- Niet hetzelfde als asynchroon, kan wel samen.
- Complex.

---

## Waarom multithreading?
- Schalen met aantal cores.
- Meer doen in minder tijd.

---


## Hoe gebruik je multithreading?

--

## Wat is een Thread?
- `System.Threading.Thread`
- Duur om aan te maken!
```csharp
var thread = new Thread(() => Console.WriteLine("Iam executing on a thread"));            
thread.Start();
```

--

## De threadpool
- `System.Threading.ThreadPool`
- Hergebruiken van threads.
```csharp
ThreadPool.QueueUserWorkItem(c => Console.WriteLine("Well, that’s just lazy writing."));
```

--

## TPL (Task Parallel Library)
- De gangbare manier om parallele code te schrijven.
- `System.Threading.Tasks.Task`
- `System.Threading.Tasks.Parallel` (oa for and foreach)
- `System.Threading.Tasks.Dataflow`
```csharp
var taskA = Task.Run(() => Console.WriteLine("Hello from taskA."));
```

---

## Wat is een Lock?
- Mutual exclusion.
- https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/lock-statement

--

## lock keyword
- ~20ns overhead, meer voor contested locks.
- Niet alles kan parallel.
- De instance parameter geeft de scope aan.

```csharp
lock (instance)
{
    // Your code...
}
```

--

## Shorthand voor

```csharp
object __lockObj = x;
bool __lockWasTaken = false;
try
{
    System.Threading.Monitor.Enter(__lockObj, ref __lockWasTaken);
    // Your code...
}
finally
{
    if (__lockWasTaken) System.Threading.Monitor.Exit(__lockObj);
}
```

--

## Deadlock
- Lock niet op public instances.
- Neem locks altijd in dezelfde volgorde.

---

## Interlocked
- ~20ns overhead
- Heeft methods om operaties atomic te maken.
- Beperkte set mogelijkheden.

```csharp
    // Simple increment/decrement operations:
    Interlocked.Increment (ref _sum);                              // 1
    Interlocked.Decrement (ref _sum);                              // 0
 
    // Add/subtract a value:
    Interlocked.Add (ref _sum, 3);                                 // 3
 
    // Read a 64-bit field:
    Console.WriteLine (Interlocked.Read (ref _sum));               // 3
 
    // Write a 64-bit field while reading previous value:
    // (This prints "3" while updating _sum to 10)
    Console.WriteLine (Interlocked.Exchange (ref _sum, 10));       // 10
 
    // Update a field only if it matches a certain value (10):
    Console.WriteLine (Interlocked.CompareExchange (ref _sum,
                                                    123, 10);  
```

--

## Atomic?
- Eigenschap van een operatie
- Niet te verdelen in substappen vanuit het oogpunt van een andere thread.
- Andere threads zien dus of de waarde voor of de waarde na de operatie.
- Bepaalde operaties zijn al atomic.

---

## Compiler en cpu optimalisaties
- Correct in de context van een enkele thread.
- Kan gedrag veranderen als er meerdere threads zijn die met elkaar communiceren.
- Voor correct gedrag moeten we sommige optimalisaties uitzetten.

--

## Wat is een Memory barrier?
- Voorkomt het herorderen van instructies.
- Nodig voor correcte communicatie tussen threads.
- Lock en interlocked maken al een memory barrier.
- https://docs.microsoft.com/en-us/dotnet/api/system.threading.thread.memorybarrier

--

## Thread.MemoryBarrier
- ~10ns overhead

```csharp
static public void Thread1()
{
    y = 1;  // Store y
    Thread.MemoryBarrier();
    r1 = x; // Load x            
}
```

--

## Volatile
- Minder strict dan MemoryBarrier
- Acquire fence op reads, operaties na een read komen niet voor de read.
- Release fence op writes, operaties voor de write komen niet na de write.
- Een write gevolgt door een read kan nog steeds verwisseld worden!

---

## Code voorbeelden

---

## Samengevat
- Multithreading is lastig.
- Hou het simpel, een lock is vaak al genoeg.
- Gebruik .NET classes zoals Lazy, Tasks en de concurrent collections.

---

## Vragen?