namespace MediaServer.Workflow;

using System;
using System.Linq;

using Common.System.Collections;

using MediaServer.Contracts;

public class ServerConsoleLogger : IServerLogger
{
    public void Log(params object[] texts)
    {
        LogAll(DateTime.UtcNow.ToString("s"), texts);
    }

    protected static void LogAll(params object[] texts)
    {
        Console.WriteLine(texts.SelectMany(Extract).JoinToString('\t'));
        return;

        object[] Extract(object obj) =>
            obj is object[] array
                ? array.SelectMany(Extract).ToArray()
                : [obj];
    }
}