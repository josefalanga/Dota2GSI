using Dota2GSI;
using System;
using System.IO;
using System.Threading;

namespace Dota2GSI.Capture
{
    /// <summary>
    /// Captures the raw GSI JSON payloads Dota 2 posts on each tick and writes each
    /// one to a sequence-numbered file under <c>captured/&lt;run-timestamp&gt;/</c>.
    /// These captures are the ground truth used to build typed nodes.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            int port = 3000;
            if (args.Length > 0 && int.TryParse(args[0], out int parsed))
            {
                port = parsed;
            }
            else
            {
                string env = Environment.GetEnvironmentVariable("GSI_PORT");
                if (!string.IsNullOrEmpty(env) && int.TryParse(env, out int envPort))
                {
                    port = envPort;
                }
            }

            string outputRoot = args.Length > 1
                ? args[1]
                : Path.Combine(AppContext.BaseDirectory, "captured");
            string runDir = Path.Combine(outputRoot, DateTime.Now.ToString("yyyyMMdd-HHmmss"));
            Directory.CreateDirectory(runDir);

            long sequence = 0;
            var gsl = new GameStateListener(port);
            gsl.NewRawGameState += json =>
            {
                long tick = Interlocked.Increment(ref sequence);
                string file = Path.Combine(runDir, $"{tick:D8}.json");
                File.WriteAllText(file, json);
                Console.WriteLine($"Captured {file} ({json.Length} bytes)");
            };

            if (!gsl.Start())
            {
                Console.WriteLine("GameStateListener could not start. Try running as Administrator. Exiting.");
                Environment.Exit(1);
            }

            Console.WriteLine($"Listening on http://localhost:{port}/ — writing payloads to {runDir}");
            Console.WriteLine("Press ESC to quit.");

            if (!Console.IsInputRedirected)
            {
                do
                {
                    while (!Console.KeyAvailable)
                    {
                        Thread.Sleep(1000);
                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            }
            else
            {
                // No interactive console (e.g. run in background): capture until killed.
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}