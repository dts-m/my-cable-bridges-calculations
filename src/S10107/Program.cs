using System;
using S10107;

if (args.Length != 4)
{
    Console.WriteLine("Usage: dotnet run -- <x1> <y1> <x2> <y2>");
    return;
}

if (!double.TryParse(args[0], out var x1) ||
    !double.TryParse(args[1], out var y1) ||
    !double.TryParse(args[2], out var x2) ||
    !double.TryParse(args[3], out var y2))
{
    Console.WriteLine("All inputs must be valid numbers.");
    return;
}

try
{
    var length = ParabolaArcLengthCalculator.ArcLengthFromPoints((x1, y1), (x2, y2));
    Console.WriteLine($"Arc length along curve = {length}");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Input error: {ex.Message}");
}