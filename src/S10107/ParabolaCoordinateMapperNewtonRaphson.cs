using System;
using System.Collections.Generic;
using System.Text;

namespace S10107;

using System;

public class ParabolaCoordinateMapperNR
{
	private readonly double _a;
	private readonly double _b;
	private readonly double _c;
	private readonly double _startX;

	public ParabolaCoordinateMapperNR(double a, double b, double c, double startX)
	{
		_a = a;
		_b = b;
		_c = c;
		_startX = startX;
	}

	// Calculates the indefinite integral of the arc length of a parabola
	private double IndefiniteArcLength(double x)
	{
		double u = 2 * _a * x + _b;
		double sqrtU2Plus1 = Math.Sqrt(u * u + 1);

		// Arc length integral: (u * sqrt(u^2 + 1) + ln(u + sqrt(u^2 + 1))) / (4 * a)
		return (u * sqrtU2Plus1 + Math.Log(u + sqrtU2Plus1)) / (4 * _a);
	}

	// Calculates the arc length from startX to a given x
	public double GetArcLength(double targetX)
	{
		return IndefiniteArcLength(targetX) - IndefiniteArcLength(_startX);
	}

	// Maps local coordinate (arc length) to global (X, Y)
	public (double GlobalX, double GlobalY) MapLocalToGlobal(double localX, double tolerance = 1e-6, int maxIterations = 20)
	{
		// Initial guess: Assume straight horizontal line
		double currentX = _startX + localX;

		// Newton-Raphson method to find X where ArcLength(X) == localX
		for (int i = 0; i < maxIterations; i++)
		{
			double currentLength = GetArcLength(currentX);
			double error = currentLength - localX;

			if (Math.Abs(error) < tolerance)
				break;

			// The derivative of arc length is simply sqrt(1 + (dy/dx)^2)
			double derivative = Math.Sqrt(1 + Math.Pow(2 * _a * currentX + _b, 2));

			currentX -= error / derivative;
		}

		double globalY = (_a * currentX * currentX) + (_b * currentX) + _c;
		return (currentX, globalY);
	}
}
