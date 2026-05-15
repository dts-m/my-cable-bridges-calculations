using System;

namespace MyCableBridgesCalculations
{
	/// <summary>
	/// Maps between:
	///   - Local coordinate s: arc-length along the parabola measured from x0
	///   - Global coordinates (x, y) on the parabola y = a*x^2 + b*x + c
	///
	/// All parabola coefficients (a, b, c) and the origin x0 are parameters.
	/// Nothing is hardcoded.
	/// </summary>
	public static class ParabolaCoordinateMap
	{
		// ─────────────────────────────────────────────────────────────────
		// Public API
		// ─────────────────────────────────────────────────────────────────

		/// <summary>
		/// Evaluates y = a*x^2 + b*x + c at the given x.
		/// </summary>
		/// <param name="x">Global x coordinate.</param>
		/// <param name="a">Quadratic coefficient.</param>
		/// <param name="b">Linear coefficient.</param>
		/// <param name="c">Constant term.</param>
		public static double YFromX(double x, double a, double b, double c)
			=> a * x * x + b * x + c;

		/// <summary>
		/// Converts a global x coordinate to a local arc-length coordinate s,
		/// measured from x0 along y = a*x^2 + b*x + c.
		///
		/// Closed form:
		///   s(x) = [F(2ax + b) - F(2ax0 + b)] / (4a)
		///   where F(u) = u*sqrt(1+u^2) + asinh(u)
		///
		/// Degenerate case a ≈ 0 (straight line):
		///   s(x) = sqrt(1 + b^2) * (x - x0)
		/// </summary>
		/// <param name="x">Global x coordinate to convert.</param>
		/// <param name="x0">Global x of the local origin (s = 0).</param>
		/// <param name="a">Quadratic coefficient.</param>
		/// <param name="b">Linear coefficient.</param>
		public static double LocalFromGlobalX(double x, double x0, double a, double b)
		{
			if (Math.Abs(a) < Eps)
				return Math.Sqrt(1.0 + b * b) * (x - x0);

			double u = 2.0 * a * x + b;
			double u0 = 2.0 * a * x0 + b;
			return (F(u) - F(u0)) / (4.0 * a);
		}

		/// <summary>
		/// Converts a local arc-length coordinate s back to a global x coordinate.
		///
		/// Solves G(x) = LocalFromGlobalX(x, x0, a, b) - s = 0
		/// using Newton's method (G'(x) = ds/dx = sqrt(1 + (2ax+b)^2) > 0 always,
		/// so the function is strictly monotone and Newton converges reliably).
		///
		/// Initial guess: linearise ds/dx at x0.
		/// </summary>
		/// <param name="s">Local arc-length coordinate.</param>
		/// <param name="x0">Global x of the local origin (s = 0).</param>
		/// <param name="a">Quadratic coefficient.</param>
		/// <param name="b">Linear coefficient.</param>
		/// <param name="tolerance">Convergence tolerance (default 1e-12).</param>
		/// <param name="maxIterations">Newton iteration cap (default 100).</param>
		public static double GlobalXFromLocal(
			double s,
			double x0,
			double a,
			double b,
			double tolerance = 1e-12,
			int maxIterations = 100)
		{
			if (tolerance <= 0)
				throw new ArgumentOutOfRangeException(nameof(tolerance), "Must be > 0.");
			if (maxIterations <= 0)
				throw new ArgumentOutOfRangeException(nameof(maxIterations), "Must be > 0.");

			// Degenerate linear case
			if (Math.Abs(a) < Eps)
			{
				double scale = Math.Sqrt(1.0 + b * b);
				if (Math.Abs(scale) < Eps)
					throw new InvalidOperationException("Degenerate curve: slope is zero.");
				return x0 + s / scale;
			}

			// Initial guess: linearise metric at x0
			double dsdx0 = Math.Sqrt(1.0 + Math.Pow(2.0 * a * x0 + b, 2));
			double x = x0 + s / dsdx0;

			for (int i = 0; i < maxIterations; i++)
			{
				double gx = LocalFromGlobalX(x, x0, a, b) - s;     // residual
				double dgx = Math.Sqrt(1.0 + Math.Pow(2.0 * a * x + b, 2)); // ds/dx > 0

				double step = gx / dgx;
				x -= step;

				if (Math.Abs(step) <= tolerance)
					return x;
			}

			// Return best estimate even if tolerance was not met
			return x;
		}

		/// <summary>
		/// Converts a local arc-length coordinate s to a global (x, y) point.
		/// </summary>
		/// <param name="s">Local arc-length coordinate.</param>
		/// <param name="x0">Global x of the local origin (s = 0).</param>
		/// <param name="a">Quadratic coefficient.</param>
		/// <param name="b">Linear coefficient.</param>
		/// <param name="c">Constant term (needed for y only).</param>
		/// <param name="tolerance">Newton convergence tolerance.</param>
		/// <param name="maxIterations">Newton iteration cap.</param>
		/// <returns>Global (x, y) on the parabola.</returns>
		public static (double x, double y) GlobalPointFromLocal(
			double s,
			double x0,
			double a,
			double b,
			double c,
			double tolerance = 1e-12,
			int maxIterations = 100)
		{
			double x = GlobalXFromLocal(s, x0, a, b, tolerance, maxIterations);
			double y = YFromX(x, a, b, c);
			return (x, y);
		}

		// ─────────────────────────────────────────────────────────────────
		// Private helpers
		// ─────────────────────────────────────────────────────────────────

		private const double Eps = 1e-15;

		/// <summary>
		/// Anti-derivative of sqrt(1+u^2): F(u) = u*sqrt(1+u^2) + asinh(u).
		/// </summary>
		private static double F(double u)
			=> u * Math.Sqrt(1.0 + u * u) + Asinh(u);

		/// <summary>
		/// asinh(u) = ln(u + sqrt(u^2 + 1)) — compatible with .NET < 5.
		/// Use Math.Asinh(u) directly if targeting .NET 5+.
		/// </summary>
		private static double Asinh(double u)
			=> Math.Log(u + Math.Sqrt(u * u + 1.0));
	}
}
