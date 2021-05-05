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
            Parallel.ForEach(new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10}, i =>
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

            Parallel.ForEach(new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10}, i =>
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
}