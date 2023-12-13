namespace FibonacciApi.Model
{
    public static class GenerateFibonacci
    {
        public static int FibonaccNumber(int n)
        {
            if (n < 0)
            {
                throw new ArgumentException("Start value cannot be negative");
            }
            if (n <= 1)
            {
                return n;
            }
            int prevFib = 0;
            int curFib = 1;

            for (int i = 2; i <= n; i++)
            {
                int nextFib = prevFib + curFib;
                prevFib = curFib;
                curFib = nextFib;
            }

            return curFib;
        }
    }
}
