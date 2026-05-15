using System;

public static class ParabolaCoordinateMap
{
	/// <summary>
	/// y = a*x^2 + b*x + c
	/// </summary>
	public static double YFromX(double x, double a, double b, double c)
		=> a * x * x + b * x + c;

	/// <summary>
	/// Local arc-length coordinate s from x0 to x along y = a*x^2 + b*x + c.
	/// s(x) = [F(2ax+b) - F(2ax0+b)] / (4a), where F(u)=u*sqrt(1+u^2)+asinh(u)
	/// Special case a≈0 (line): s = sqrt(1+b^2) * (x - x0)
	/// </summary>
	public static double LocalFromGlobalX(double x, double x0, double a, double b)
	{
		const double eps = 1e-15;

		if (Math.Abs(a) < eps)
		{
			// y' = b (constant), so ds/dx = sqrt(1+b^2)
			return Math.Sqrt(1.0 + b * b) * (x - x0);
		}

		double u = 2.0 * a * x + b;
		double u0 = 2.0 * a * x0 + b;

		return (PrimitiveF(u) - PrimitiveF(u0)) / (4.0 * a);
	}

	/// <summary>
	/// Inverse map: find global x from local arc-length s (measured from x0).
	/// Uses Newton's method on:
	/// G(x) = LocalFromGlobalX(x, x0, a, b) - s = 0
	/// </summary>
	public static double GlobalXFromLocal(
		double s,
		double x0,
		double a,
		double b,
		double tolerance = 1e-12,
		int maxIterations = 100)
	{
		if (tolerance <= 0) throw new ArgumentOutOfRangeException(nameof(tolerance));
		if (maxIterations <= 0) throw new ArgumentOutOfRangeException(nameof(maxIterations));

		const double eps = 1e-15;

		// Linear case
		if (Math.Abs(a) < eps)
		{
			double scale = Math.Sqrt(1.0 + b * b);
			return x0 + s / scale;
		}

		// Initial guess from local linearization at x0
		double dsdx0 = Math.Sqrt(1.0 + Math.Pow(2.0 * a * x0 + b, 2));
		double x = x0 + s / dsdx0;

		for (int i = 0; i < maxIterations; i++)
		{
			double gx = LocalFromGlobalX(x, x0, a, b) - s;
			double dgx = Math.Sqrt(1.0 + Math.Pow(2.0 * a * x + b, 2)); // ds/dx > 0 always

			double step = gx / dgx;
			x -= step;

			if (Math.Abs(step) <= tolerance)
				return x;
		}

		return x; // best estimate if not fully converged
	}

	/// <summary>
	/// Map local coordinate s to global point (x,y).
	/// </summary>
	public static (double x, double y) GlobalPointFromLocal(
		double s,
		double x0,
		double a,
		double b,
		double c,
		double tolerance = 1e-12,
		int maxIterations = 500)
	{
		double x = GlobalXFromLocal(s, x0, a, b, tolerance, maxIterations);
		double y = YFromX(x, a, b, c);
		return (x, y);
	}

	/// <summary>
	/// F(u) = u*sqrt(1+u^2) + asinh(u)
	/// </summary>
	private static double PrimitiveF(double u)
		=> u * Math.Sqrt(1.0 + u * u) + Asinh(u);

	/// <summary>
	/// asinh(u) implementation compatible with older frameworks.
	/// </summary>
	private static double Asinh(double u)
		=> Math.Log(u + Math.Sqrt(u * u + 1.0));
}
