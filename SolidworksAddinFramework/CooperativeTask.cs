using System.Threading;
using System.Threading.Tasks;

namespace SolidworksAddinFramework
{
    /// <summary>
    /// Helper methods to implement cooperative tasks using coroutines (https://en.wikipedia.org/wiki/Coroutine)
    /// </summary>
    public static class CooperativeTask
    {
        /// <summary>
        /// Allows an algorithm to specify a location where it may be paused or cancelled
        /// to allow other tasks to run or continue.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task Yield(CancellationToken ct)
        {
            await TaskEx.Delay(1, ct);
        }
    }
}