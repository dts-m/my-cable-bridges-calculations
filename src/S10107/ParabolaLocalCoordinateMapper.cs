using System;
using static S10107.ParabolaDefinitions;

namespace MyCableBridgesCalculations
{
    /// <summary>
    /// Maps between:
    /// - Global parabola coordinates (x2, curve_y2), where curve_y2 = a*x2^2 + b*x2 + c
    /// - Local curvilinear coordinate local_x2 = arc length from x2=0 along the parabola.
    /// </summary>
    public static class ParabolaLocalCoordinateMapper
    {
        // Convenience constants
        private const double K = 2.0 * A; // derivative slope factor: y' = K*x + B

        // Constant offset so that s(0) = 0
        private static readonly double S0Term = PrimitiveTerm(B);

        /// <summary>
        /// Global parabola y value at x2.
        /// </summary>
        public static double ToCurveY2(double x2) => A * x2 * x2 + B * x2 + C;

        /// <summary>
        /// Exact closed-form arc length from x2=0 to x2 (local coordinate).
        /// local_x2 = s(x2)
        /// </summary>
        public static double ToLocalX2(double x2)
        {
            // s(x) = (1/(2K)) * ( F(Kx+B) - F(B) )
            // where F(u)=u*sqrt(1+u^2)+asinh(u), K=2a
            double u = K * x2 + B;
            return (PrimitiveTerm(u) - S0Term) / (2.0 * K);
        }

        /// <summary>
        /// Inverse map: given local arc length local_x2, solve for global x2.
        /// Uses Newton iterations with a robust initial guess.
        /// </summary>
        public static double ToGlobalX2(double localX2, double tolerance = 1e-12, int maxIterations = 50)
        {
            if (double.IsNaN(localX2) || double.IsInfinity(localX2))
                throw new ArgumentException("localX2 must be a finite number.", nameof(localX2));
            if (tolerance <= 0)
                throw new ArgumentOutOfRangeException(nameof(tolerance), "tolerance must be > 0.");
            if (maxIterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxIterations), "maxIterations must be > 0.");

            // Initial guess: linearized by initial metric ds/dx at x=0 => sqrt(1+B^2)
            double x = localX2 / Math.Sqrt(1.0 + B * B);

            for (int i = 0; i < maxIterations; i++)
            {
                double fx = ToLocalX2(x) - localX2;                  // f(x)=s(x)-target
                double dfx = Math.Sqrt(1.0 + Math.Pow(K * x + B, 2)); // ds/dx > 0 always

                double step = fx / dfx;
                x -= step;

                if (Math.Abs(step) <= tolerance)
                    return x;
            }

            // If Newton didn't meet strict tolerance, return best estimate.
            return x;
        }

        /// <summary>
        /// Convert local_x2 to global point (x2, curve_y2).
        /// </summary>
        public static (double x2, double curveY2) ToGlobalPoint(double localX2, double tolerance = 1e-12, int maxIterations = 50)
        {
            double x2 = ToGlobalX2(localX2, tolerance, maxIterations);
            return (x2, ToCurveY2(x2));
        }

        /// <summary>
        /// Helper F(u)=u*sqrt(1+u^2)+asinh(u)
        /// </summary>
        private static double PrimitiveTerm(double u)
            => u * Math.Sqrt(1.0 + u * u) + Asinh(u);

        /// <summary>
        /// asinh(u) for frameworks lacking Math.Asinh.
        /// </summary>
        private static double Asinh(double u)
            => Math.Log(u + Math.Sqrt(u * u + 1.0));
    }
}
