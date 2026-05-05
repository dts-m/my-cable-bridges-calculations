# S10107 - Parabola Arc Length Calculator

Curve:

`y = 0.000557399x^2 + 0.0768503x - 3.04935`

Arc length between `x1` and `x2`:

`L = |F(x2) - F(x1)|`

where:

`F(x) = (1/(2m)) * ( t(x)*sqrt(1+t(x)^2) + asinh(t(x)) )`  
`m = 2a`, `t(x)=m*x+b`, `a=0.000557399`, `b=0.0768503`  
`asinh(z)=ln(z+sqrt(z^2+1))`

Run:

```bash
cd src/S10107
dotnet run -- <x1> <y1> <x2> <y2>
```