using System;
using System.Collections.Generic;
using System.Text;

namespace S10107
{
	public static class ParabolaDefinitions
	{
		public enum Location
		{
			LeftSpan,
			MiddleSpan,
			RighSpan
		}

		public static ParabolaDefinition LeftSpanConsts = new ParabolaDefinition {
			A = 0.000302333,
			B = 0.0595647,
			C = -5.27702,
			DataPoints = [
 (0, -12.8)
, (6.5, 14.7)
, (26.9, 34.4)
, (67.97, 75.7)
, (150.23, 157.7)
, (233.15, 238)
, (316.73, 320.2)
, (401.95, 402)
, (487.61, 482.3)
, (574.62, 564.5)
, (663.24, 646.7)
]};

		public static ParabolaDefinition MiddleSpanConsts = new ParabolaDefinition() {
				A = 0.00025384,
				B = -0.748613,
				C = 562.24404,
				DataPoints = [
 (725.24  , 703.4)
,(811.82  , 785.6)
,(897.19  , 865.8)
,(981.52  , 947.7)
,(1064.98 , 1028)
,(1147.73 , 1110.4)
,(1229.9  , 1190.5)
,(1311.64 , 1272.4)
,(1393.06 , 1352.7)
,(1474.28 , 1435.1)
,(1555.4  , 1515.3)
,(1636.63 , 1597.2)
,(1718.05 , 1677.6)
,(1799.79 , 1759.5)
,(1881.96 , 1839)
,(1964.7  , 1921.8)
,(2048.17 , 2002)
,(2132.5  , 2084)
,(2217.87 , 2164.5)
,(2304.45 , 2246.5)
					]};

		public static ParabolaDefinition RightSpanConsts = new ParabolaDefinition() {
			A = 0.000294606,
			B = -1.80526,
			C = 2754.64884,
			DataPoints = [
 (2366.450833 , 2303.1)
,(2455.0675   , 2385.4)
,(2542.0775   , 2467.1)
,(2627.740833 , 2547.66)
,(2712.9575   , 2629.5)
,(2796.539167 , 2711.9)
,(2879.4575   , 2792)
,(2961.715    , 2874.17)
,(3002.781667 , 2915.14)
,(3023.185833 , 2933.7)
,(3029.685833 , 2954.4)
		]};

		public static Dictionary<Location, ParabolaDefinition> Definitions = new Dictionary<Location, ParabolaDefinition>() {
			[Location.LeftSpan] = LeftSpanConsts,
			[Location.MiddleSpan] = MiddleSpanConsts,
			[Location.RighSpan] = RightSpanConsts
		};

		/// <summary>
		/// Quadratic Coefficients & x_0 coord of 1st point on the Quadratic 
		/// </summary>
		/// <param name="loc"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public static ParabolaDefinition GetConstants(Location loc)
		{
			return Definitions[loc];
		}

		public static (Location? Location, ParabolaDefinition? Definitions) GetConstants(double LocalCoordX)
		{
			foreach (var item in Definitions)
			{
				if(item.Value.DataPoints is null || !item.Value.DataPoints.Any())
				{
					continue;
				}

				if(LocalCoordX >= item.Value.DataPoints.First().LocalX && LocalCoordX < item.Value.DataPoints.Last().LocalX)
				{
					return (item.Key, item.Value);
				}
			}

			return (null, null);
		}
	}
}
