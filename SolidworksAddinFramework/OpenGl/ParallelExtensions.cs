using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SolidworksAddinFramework.OpenGl
{
    public static class ParallelExtensions
    {
        /// <summary>
        /// Generates a partition over 'length' depending on the number of CPUs
        /// </summary>
        /// <param name="length"></param>
        /// <param name="loop"></param>
        public static void ParallelChunked(this int length, Action<int, int> loop)
        {
            if (length == 0)
                return;
            var partitioner = Partitioner.Create(0, length);
            Parallel.ForEach (partitioner, (range, loopState) =>
            {
                loop(range.Item1, range.Item2);
            });
            
        }
    }
}