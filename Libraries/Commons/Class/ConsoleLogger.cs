using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Library.Logger;

public class ConsoleLogger : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    public ConsoleLogger()
    {
            
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (logLevel == LogLevel.Information)
        {
            Console.WriteLine(state.ToString());
        }
        else if (logLevel == LogLevel.Debug)
        {
            if (exception == null && state != null)
            {
                System.Diagnostics.Debugger.Log((int)logLevel, "Debug", state.ToString());
            }
            else if (exception != null)
            {
                if (state != null)
                {
                    System.Diagnostics.Debugger.Log((int)logLevel, "Debug", state.ToString());
                }
                System.Diagnostics.Debugger.Log((int)logLevel, "Debug",exception.ToString());

            }
        }
        else
        {
            if (exception == null && state != null)
            {
                Console.WriteLine(state.ToString());
            }
            else if (exception != null)
            {
                if(state != null) {
                    Console.WriteLine(state.ToString());
                }
                Console.WriteLine(exception.ToString());
            }
        }

    }


}
