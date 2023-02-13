``` ini

BenchmarkDotNet=v0.13.4, OS=Windows 11 (10.0.22000.613/21H2)
Intel Core i7-7700 CPU 3.60GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.301
  [Host]     : .NET 5.0.17 (5.0.1722.21314), X64 RyuJIT AVX2 [AttachedDebugger]
  DefaultJob : .NET 5.0.17 (5.0.1722.21314), X64 RyuJIT AVX2


```
|              Method |      Mean |     Error |    StdDev |
|-------------------- |----------:|----------:|----------:|
|     SquareEachValue | 25.809 μs | 0.3631 μs | 0.3396 μs |
| SquareEachValuePart |  7.216 μs | 0.0994 μs | 0.0930 μs |
