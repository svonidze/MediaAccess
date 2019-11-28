namespace MediaServer.Workflow
{
    using System;
    using System.Linq;

    using Common.Collections;

    using MediaServer.Contracts;

    public class ServerLogger : IServerLogger
    {
        public void Log(params object[] texts)
        {
            LogAll(DateTime.UtcNow.ToString("s"), texts);
        }

        private static void LogAll(params object[] texts)
        {
            object[] Extract(object obj)
            {
                return obj is object[] array
                    ? array.SelectMany(Extract).ToArray()
                    : new[] { obj };
            }

            Console.WriteLine(texts.SelectMany(Extract).JoinToString('\t'));
        }
    }
}