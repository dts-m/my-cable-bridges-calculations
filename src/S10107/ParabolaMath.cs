using System;
using System.Collections.Generic;
using System.Text;

namespace S10107;

public class ParabolaMath
{
	/// <summary>
	/// Calculates the arc length (distance along the curve) of a parabola between two points.
	/// </summary>
	/// <param name="a">Coefficient a of ax^2 + bx + c</param>
	/// <param name="b">Coefficient b of ax^2 + bx + c</param>
	/// <param name="c">Coefficient c of ax^2 + bx + c</param>
	/// <param name="p1">Starting point (x1, y1)</param>
	/// <param name="p2">Ending point (x2, y2)</param>
	/// <returns>The distance along the parabola between p1 and p2</returns>
	public static double CalculateArcLength(double a, double b, double c, (double X, double Y) p1, (double X, double Y) p2)
	{
		// Edge case: If 'a' is extremely close to 0, it's a straight line (y = bx + c).
		// To avoid division by zero in the integral, use the standard Euclidean distance formula.
		double epsilon = 1e-10;
		if (Math.Abs(a) < epsilon)
		{
			double dx = p2.X - p1.X;
			double dy = p2.Y - p1.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}

		// Calculate the definite integral from X1 to X2
		double length1 = GetIndefiniteIntegral(a, b, p1.X);
		double length2 = GetIndefiniteIntegral(a, b, p2.X);

		// The arc length is the absolute difference between the two evaluated limits
		return Math.Abs(length2 - length1);
	}

	/// <summary>
	/// Evaluates the indefinite integral of the arc length of a parabola at a specific x.
	/// Integral of sqrt(1 + (2ax + b)^2) dx
	/// </summary>
	private static double GetIndefiniteIntegral(double a, double b, double x)
	{
		// Substitute u = y' = 2ax + b
		double u = 2 * a * x + b;
		double sqrtU2Plus1 = Math.Sqrt(u * u + 1);

		// The closed-form antiderivative is: [u * sqrt(u^2 + 1) + ln(u + sqrt(u^2 + 1))] / (4a)
		// Note: Math.Log(u + sqrtU2Plus1) is mathematically equivalent to Math.Asinh(u)
		return (u * sqrtU2Plus1 + Math.Log(u + sqrtU2Plus1)) / (4 * a);
	}
}
