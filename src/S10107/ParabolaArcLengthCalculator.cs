using System;

namespace S10107;

public static class ParabolaArcLengthCalculator
{
    private const double A = 0.000557399;
    private const double B = 0.0768503;
    private const double C = -3.04935;

    public static double ArcLengthFromPoints((double x, double y) p1, (double x, double y) p2, double tolerance = 1e-6)
    {
        ValidatePointOnCurve(p1, tolerance, nameof(p1));
        ValidatePointOnCurve(p2, tolerance, nameof(p2));
        return ArcLengthFromX(p1.x, p2.x);
    }

    public static double ArcLengthFromX(double x1, double x2)
    {
        double m = 2.0 * A;
        double n = B;

        double F(double x)
        {
            double t = m * x + n;
            return (1.0 / (2.0 * m)) * (t * Math.Sqrt(1.0 + t * t) + Asinh(t));
        }

        return Math.Abs(F(x2) - F(x1));
    }

    public static double EvaluateY(double x) => A * x * x + B * x + C;

    private static void ValidatePointOnCurve((double x, double y) p, double tolerance, string paramName)
    {
        var expected = EvaluateY(p.x);
        if (Math.Abs(expected - p.y) > tolerance)
        {
            throw new ArgumentException(
                $"{paramName} is not on the curve within tolerance {tolerance}. Given y={p.y}, expected y={expected} at x={p.x}.");
        }
    }

    private static double Asinh(double z) => Math.Log(z + Math.Sqrt(z * z + 1.0));
}