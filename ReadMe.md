

# Fibonacci API

## Description

This is an API for generating Fibonacci sequences with configurable parameters. It has a single endpoint that computes a Fibonacci sequence based on input settings.

Key features:

- Generate sequence between specified start and end indexes
- Set maximum memory usage allowed 
- Specify timeout limit
- Option to use cached sequence for better performance
- Outputs sequence as JSON with metadata  

The API handles all computation and caching server-side for generating sequences. It allows tweaking parameters to balance performance vs. memory usage.
## Input and Output

**Input:**

- `start` - Start index of sequence 
- `end` - End index of sequence
- `timeOutMs` - Timeout in ms 
- `maxMemoryMb` - Max memory allowed in MB
- `useCache` - Whether to use cached sequence

**Output:**

JSON object with:

- `Sequence` - Array with Fibonacci sequence 
- `ExceededMemory` - Boolean if memory exceeded
- `TimeOutOccurred` - Boolean if timeout occurred

## Development and Contributing 

(Comming Soon) Unit tests for contributions. Clone and submit pull requests to contribute.

Follow .NET Core coding conventions and commenting standards.

## License

None

## Contact

Michael Jacob (sirmichaeljacob@gmail.com)

## Acknowledgements

- Uses [LazyCache](https://github.com/alastairtree/LazyCache) for caching
- Uses [Microsoft.Extensions.Caching.Memory](https://dot.net) for caching