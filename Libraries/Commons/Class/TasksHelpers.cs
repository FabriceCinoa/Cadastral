using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Library.Class;

public static class TaskHelper
{
    public static async Task<T> PerformTaskWithTimeout<T>(string taskName,Func<T> function,  int timeoutMilliseconds)
    {
        // Create the task
        Task<T> task = Task.Run( () =>
        {
            var r =  function();
            return r;
        });

        // Create a timeout task
        Task timeoutTask = Task.Delay(timeoutMilliseconds);

        // Wait for either the task to complete or for the timeout to occur
        Task completedTask = await Task.WhenAny(task, timeoutTask);

        if (completedTask == timeoutTask)
        {
          return default(T);
        }

        // Return the result of the task
        return await task;
    }
}

