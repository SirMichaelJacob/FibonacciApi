using FibonacciApi.Caching;
using FibonacciApi.Helpers;
using FibonacciApi.Model;
using LazyCache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Text.Json;

namespace FibonacciApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FibonacciController : ControllerBase
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IMemoryCache _cache;
        //private readonly MemoryCacheOptions _cacheOptions;


        public FibonacciController(ICacheProvider cacheProvider, IMemoryCache cache)
        {
            _cacheProvider = cacheProvider;
            _cache = cache;

        }

        //Api end point controller
        /// <summary>
        /// This End point Accepts 5 parameters
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="timeOutMs"></param>
        /// <param name="maxMemoryMb"></param>
        /// <param name="useCache"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<IActionResult> Sequence(int start, int end, int timeOutMs, long maxMemoryMb, bool useCache = false)
        {
            var result = "";

            //CacheData
            List<long> cacheData = new List<long>();

            //Computes a Parent Sequence and saves in the Cache with a default limit from CacheKeys.defaultParentLimit

            if (!_cacheProvider.TryGetValue(CacheKeys.SequenceKey, out List<long> cachedSequence))
            {
                List<long> parentSequence = GenerateParentSequence(CacheKeys.defaultParentLimit);
                cachedSequence = parentSequence;
                cacheData = cachedSequence;


                var cacheEntryOption = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(30),
                    SlidingExpiration = TimeSpan.FromMinutes(10),
                    Size = 1024,
                };

                _cacheProvider.Set(CacheKeys.SequenceKey, cachedSequence, cacheEntryOption);
            }

            //When User is using stored fibonacci cache
            if (useCache)
            {
                //Use CacheData as Parent Sequence to generate Subsequence
                var res = await RunTaskWithTimeout(Task.Run(() => { result = GenerateSubsequence(start, end, maxMemoryMb, cacheData); }), timeOutMs);


                return Ok(result);
            }
            else
            {
                //Generates a new Parent Fibonacci sequence and Subsequence
                var res = await RunTaskWithTimeout(Task.Run(() => { result = GenerateSubsequence(start, end, maxMemoryMb); }), timeOutMs);

                return Ok(result);
            }

        }


        /// <summary>
        /// This method generates the Fibonnaci subsequence 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="maxMem"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static string GenerateSubsequence(int start, int end, long maxMem = 10, List<long> parent = null)
        {
            List<long> subsequence = new List<long>();
            var memoryChecker = new MemoryUsageChecker(maxMem);
            var exceeded = false;
            if (parent != null)
            {
                if (parent.Count >= end)
                {
                    //When fetching from Cache
                    for (int i = start; i <= end; i++)
                    {
                        //Generate subsequence

                        subsequence.Add(parent.ElementAt(i));
                    }
                }
                else
                {
                    //Generate parent sequence
                    CacheKeys.defaultParentLimit = end;
                    parent = GenerateParentSequence(CacheKeys.defaultParentLimit);

                    //When fetching from Cache
                    for (int i = start; i <= end; i++)
                    {
                        //Generate subsequence

                        subsequence.Add(parent.ElementAt(i));
                    }

                }
            }
            else
            {
                CacheKeys.defaultParentLimit = end;
                //Generate parent sequence
                parent = GenerateParentSequence(CacheKeys.defaultParentLimit);
                //Generate subsequence
                exceeded = CreateSequence(start, end, parent, subsequence, memoryChecker, exceeded);
            }

            var resp = new FibonacciResponse()
            {
                Sequence = subsequence,
                ExceededMemory = exceeded,
                TimeOutOcurred = false,
            };
            var jsonString = JsonSerializer.Serialize(resp);
            return jsonString;
        }

        /// <summary>
        /// This method Creates the sequence while Checking the Memory Usage and returns a boolean 'exceeded'
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="parent"></param>
        /// <param name="subsequence"></param>
        /// <param name="memoryChecker"></param>
        /// <param name="exceeded"></param>
        /// <returns></returns>
        private static bool CreateSequence(int start, int end, List<long> parent, List<long> subsequence, MemoryUsageChecker memoryChecker, bool exceeded)
        {
            for (int i = start; i <= end; i++)
            {
                //To simulate a CPU intensive task there is an artificial requirement to do 500ms delay after a new Fibonacci number is computed
                SimulateDelay();
                subsequence.Add(parent.ElementAt(i));

                //Generate subsequence
                if (memoryChecker.HasExceededMemoryLimit())
                {
                    exceeded = true;
                    break;
                }
                subsequence.Add(parent.ElementAt(i));
            }

            return exceeded;
        }

        /// <summary>
        /// Method to Generate Parent Fibonnaci Sequence
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static List<long> GenerateParentSequence(int limit)
        {
            List<long> fibNumbers = new List<long>();
            for (int i = 0; i <= limit; i++)
            {
                var fibNum = GenerateFibonacci.FibonaccNumber(i);
                fibNumbers.Add(fibNum);
            }
            return fibNumbers;
        }


        /// <summary>
        /// This Runs an asynchronous Task using a timeout
        /// </summary>
        /// <param name="task"></param>
        /// <param name="timeoutMs"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<long> RunTaskWithTimeout(Task task, int timeoutMs)
        {
            var stWatch = new Stopwatch();
            using (var timeoutCancellation = new CancellationTokenSource())
            {
                var completedTask = await Task.WhenAny(task, Task.Delay(timeoutMs, timeoutCancellation.Token)
                );

                if (completedTask == task)
                {

                    // Task completed within timeout
                    timeoutCancellation.Cancel();
                    return timeoutMs;
                }
                else
                {
                    // Timeout exceeded
                    timeoutCancellation.Cancel();
                    throw new Exception(new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError).ToString())
                    {

                    };
                }
            }
        }

        /// <summary>
        /// Simulate a CPU intensive task there is an artificial requirement to do 500ms delay
        /// </summary>
        /// <returns></returns>
        public static Task SimulateDelay()
        {
            return Task.Delay(500);
        }

    }
}
