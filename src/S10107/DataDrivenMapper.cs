using System;
using System.Collections.Generic;
using System.Text;

namespace S10107;

public class DataDrivenMapper
{
	private readonly double _a, _b, _c;
	private readonly List<(double LocalX, double GlobalX)> _mappingPoints;

	public DataDrivenMapper(double a, double b, double c, IEnumerable<(double, double)> sensorData)
	{
		_a = a;
		_b = b;
		_c = c;
		// Ensure points are sorted by LocalX for binary search/interpolation
		_mappingPoints = sensorData.OrderBy(p => p.Item1).ToList();
	}

	public (double GlobalX, double GlobalY) MapLocalToGlobal(double localX)
	{
		double globalX = InterpolateX(localX);
		double globalY = (_a * globalX * globalX) + (_b * globalX) + _c;

		return (globalX, globalY);
	}

	private (int LeftIndex, int RightIndex) FindSegmentIndices(double targetLocalX)
	{
		int count = _mappingPoints.Count;

		// Edge cases: out of bounds (for extrapolation)
		if (targetLocalX <= _mappingPoints[0].LocalX)
			return (0, 1);

		if (targetLocalX >= _mappingPoints[count - 1].LocalX)
			return (count - 2, count - 1);

		int low = 0;
		int high = count - 1;

		// Binary search
		while (low <= high)
		{
			int mid = low + (high - low) / 2;
			double midValue = _mappingPoints[mid].LocalX;

			if (midValue == targetLocalX)
			{
				// Exact match found. Return the segment starting at mid 
				// (safeguard against out-of-bounds if mid is the very last element)
				return mid == count - 1 ? (mid - 1, mid) : (mid, mid + 1);
			}

			if (targetLocalX < midValue)
			{
				high = mid - 1;
			}
			else
			{
				low = mid + 1;
			}
		}

		// When the loop breaks without an exact match, 'low' represents the insertion index.
		// Therefore, the target falls exactly between 'low - 1' and 'low'.
		return (low - 1, low);
	}

	private double InterpolateX_Loop(double localX)
	{
		// Edge cases: Extrapolate if outside the bounds
		if (localX <= _mappingPoints.First().LocalX) return Extrapolate(_mappingPoints[0], _mappingPoints[1], localX);
		if (localX >= _mappingPoints.Last().LocalX) return Extrapolate(_mappingPoints[^2], _mappingPoints[^1], localX);

		// Find the segment containing the localX
		for (int i = 0; i < _mappingPoints.Count - 1; i++)
		{
			var p1 = _mappingPoints[i];
			var p2 = _mappingPoints[i + 1];

			if (localX >= p1.LocalX && localX <= p2.LocalX)
			{
				// Linear interpolation (Lerp)
				double t = (localX - p1.LocalX) / (p2.LocalX - p1.LocalX);
				return p1.GlobalX + t * (p2.GlobalX - p1.GlobalX);
			}
		}
		return 0; // Fallback
	}

	private double InterpolateX(double localX)
	{
		// Find the indices using binary search
		var (left, right) = FindSegmentIndices(localX);

		var p1 = _mappingPoints[left];
		var p2 = _mappingPoints[right];

		// If outside the bounds, extrapolate (the math is identical to linear interpolation)
		// If inside bounds, interpolate
		double t = (localX - p1.LocalX) / (p2.LocalX - p1.LocalX);
		return p1.GlobalX + t * (p2.GlobalX - p1.GlobalX);
	}

	private double Extrapolate((double LocalX, double GlobalX) p1, (double LocalX, double GlobalX) p2, double targetLocal)
	{
		double slope = (p2.GlobalX - p1.GlobalX) / (p2.LocalX - p1.LocalX);
		return p1.GlobalX + slope * (targetLocal - p1.LocalX);
	}
}
