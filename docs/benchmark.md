# benchmark

## Encoder

|              Method |      Mean |    Error |   StdDev | Ratio |   Gen0 | Allocated | Alloc Ratio |
|-------------------- |----------:|---------:|---------:|------:|-------:|----------:|------------:|
|     EncodeAndDecode | 109.52 ns | 1.044 ns | 0.815 ns |  1.00 | 0.0261 |     328 B |        1.00 |
| FastEncodeAndDecode |  49.16 ns | 0.837 ns | 0.699 ns |  0.45 | 0.0076 |      96 B |        0.29 |