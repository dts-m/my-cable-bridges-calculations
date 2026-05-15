using System;
using System.Collections.Generic;
using System.Text;

namespace S10107
{
	public class ParabolaDefinition
	{
		public double A { get; set; }
		public double B { get; set; }
		public double C { get; set; }
		public IEnumerable<(double LocalX, double GlobalX)> DataPoints { get; set; }
		public double[] LocalXDomainLimits { get; set; } = new double[] { 0, 0 };
		public double GloabalX0 => DataPoints.Any() ? DataPoints.First().GlobalX : 0.0;
	}
}
