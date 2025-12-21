using System;
using System.Threading;

namespace PhishSim.Utils
{
    public static class TerminalAnimation
    {
        public static void Spinner(string message, int durationMs = 1500)
        {
            var spinner = new[] { '|', '/', '-', '\\' };
            int i = 0;
            int steps = durationMs / 100;

            for (int s = 0; s < steps; s++)
            {
                Console.Write($"\r{message} {spinner[i++ % spinner.Length]}");
                Thread.Sleep(100);
            }

            Console.Write("\r" + new string(' ', message.Length + 2) + "\r"); // clear line
        }
    }
}