using System;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace SpaceHeater
{
    internal class Program
    {
        private static void Main()
        {
            const int interval = 5 * 1000 * 60;
            var timer = new Timer(interval);
            var cts = new CancellationTokenSource();
            timer.Elapsed += (sender, args) =>
                                 {
                                     Console.WriteLine("Time is up");
                                     cts.Cancel();
                                     Environment.Exit(0);
                                 };

            var options = new ParallelOptions
                              {
                                  MaxDegreeOfParallelism = (int)Math.Ceiling(0.75 * Environment.ProcessorCount),
                                  CancellationToken = cts.Token
                              };

            Task.Factory.StartNew(() =>
                                              {
                                                  try
                                                  {
                                                      // Useless work
                                                      Parallel.For(long.MinValue + 1, long.MaxValue, options,
                                                                   i => { i--; });
                                                  }
                                                  catch (OperationCanceledException)
                                                  {
                                                  }
                                              },
                                          cts.Token);

            timer.Start();

            Console.WriteLine("Press any key to cancel...");
            Console.ReadKey();
            return;
        }
    }
}