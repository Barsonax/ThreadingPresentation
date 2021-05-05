---
theme : "white"
transition: "slide"
highlightTheme: "vs2015"
slideNumber: true
logoImg: "https://github.com/Barsonax/threadingpresentation/raw/master/images/threading.jpeg"
title: "Threading presentation"
enableTitleFooter: false
---

## Multi threaded programming

<a>
    <img style="border: unset; box-shadow: unset" data-src="https://github.com/Barsonax/threadingpresentation/raw/master/images/threading.jpeg">
</a>

---

## In deze presentatie
- Wat is Multi threaded programming?
- Waarom Multi threaded programming?
- De techniek in
- Vragen

---

## Wat is multi threaded programming?
- Meerdere acties tegelijkertijd uitvoeren
- Niet hetzelfde als asynchroon, kan wel samen
- Complex

---

## Waarom multi threaded programming?
- Meer doen in minder tijd
- Schalen met aantal cores

---

## Hoe gebruik je multi threaded programming?
- Threads
- Locking
- Memory barriers

---

## Wat is een Thread?
- System.Threading.Thread

---

## Wat is een Lock?
- Mutual exclusion
- https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/lock-statement

--

## lock keyword
- ~20ns overhead, meer voor contested locks

```csharp
lock (x)
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

---
## Interlocked
- ~20ns overhead

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

---

## Wat is een Memory barrier?
- Voorkomt het herorderen van geheugen toegang
- https://docs.microsoft.com/en-us/dotnet/api/system.threading.thread.memorybarrier

--

## Thread.MemoryBarrier
- ~10ns overhead

```csharp
public void SetFlag(int flag)
{
    var flags = m_flags;
    Thread.MemoryBarrier();
    m_flags = flags | flag;
}
```

--

## Volatile



---

## Anti patterns and how to fix them

--

## Locking on public fields

```csharp
lock (this){ }
```
```csharp
lock (somePublicField){ }
```
```csharp
lock (someInternalField){ }
```

--

## Double checked locking
-
```csharp
// Business.Api\AppService\Operator\SettingsAppService.cs : 179
private void Initialize()
{
    if (!_isInitialized)
    {
        lock (this)
        {
            if (!_isInitialized)
            {
                _settings = _context.tSettings.ToDictionary(s => s.sSettingID, s => s.sValue);
                _isInitialized = true;
            }
        }
    }
}
```

--

## The Fix

```csharp
private Lazy<Dictionary<string, string>> _settings;
```