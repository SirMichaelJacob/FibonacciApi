namespace FibonacciApi.Caching
{
    public static class CacheKeys
    {
        public static string SequenceKey = BuildCacheKey();
        public static int defaultParentLimit = 1000;
        private static string BuildCacheKey()
        {
            return $"fibonacci_{DateTime.Now}";
        }
    }


}
