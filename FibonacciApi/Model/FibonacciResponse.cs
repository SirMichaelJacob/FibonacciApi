namespace FibonacciApi.Model
{
    public class FibonacciResponse
    {
        public List<long>? Sequence { get; set; }
        public bool ExceededMemory { get; set; } = false;
        public bool TimeOutOcurred { get; set; } = false;
    }
}
