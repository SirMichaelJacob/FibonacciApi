namespace FibonacciApi.Model
{
    public class Fibonacci
    {
        public long StartIndex { get; set; }

        public long EndIndex { get; set; }

        public bool? UseCache { get; set; }

        public int? TimeOut { get; set; }

        public int MaxMemory { get; set; }

    }
}
