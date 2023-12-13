using System.Diagnostics;

namespace FibonacciApi.Helpers
{
    public class MemoryUsageChecker
    {
        private readonly long memoryLimitBytes;

        public MemoryUsageChecker(long memoryLimitBytes)
        {
            this.memoryLimitBytes = memoryLimitBytes;
        }

        public bool HasExceededMemoryLimit()
        {
            var currentProcess = Process.GetCurrentProcess();
            long workingSetBytes = currentProcess.WorkingSet64;

            //1MB = 1*1024*1024

            return workingSetBytes > memoryLimitBytes * 1024 * 1024;
        }
    }
}
