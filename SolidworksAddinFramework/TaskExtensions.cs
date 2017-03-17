using System;
using System.Threading.Tasks;

namespace SolidworksAddinFramework
{
    public static class TaskExtensions
    {
        public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(timeout)))
                return task.Result;
            throw new TimeoutException();
        }
    }
}
