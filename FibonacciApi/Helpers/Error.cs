using System.Text.Json;

namespace FibonacciApi.Helpers
{
    public class Error
    {
        public string Message { get; set; }
        public string StatusCode { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
