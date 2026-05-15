using MyCableBridgesCalculations;
using S10107;
using System;

//if (args.Length != 4)
//{
//    Console.WriteLine("Usage: dotnet run -- <x1> <y1> <x2> <y2>");
//    return;
//}

//if (!double.TryParse(args[0], out var x1) ||
//    !double.TryParse(args[1], out var y1) ||
//    !double.TryParse(args[2], out var x2) ||
//    !double.TryParse(args[3], out var y2))
//{
//    Console.WriteLine("All inputs must be valid numbers.");
//    return;
//}


var x1 = 576.0;
//var y1 = 225.0;
var y1 = 230.194255;

var x2 = 656.0;
//var y2 = 288.0;
var y2 = 290.7359005;


//try
//{
//	var length = ParabolaArcLengthCalculator.ArcLengthFromPoints((x1, y1), (x2, y2));
//	Console.WriteLine($"Arc length along curve = {length}");
//}
//catch (ArgumentException ex)
//{
//	Console.WriteLine($"Input error: {ex.Message}");
//}


try
{
	var locX = 574.62;

	//var consts = ParabolaDefinitions.GetConstants(ParabolaDefinitions.Location.LeftSpan);

	var constsDef = ParabolaDefinitions.GetConstants(locX);
	if(constsDef.Location is null || constsDef.Definitions is null)
	{
		throw new ArgumentException("Data Point not in available ranges");
	}

	ParabolaDefinition consts = constsDef.Definitions;
	Console.WriteLine($"On Location :{constsDef.Location}");

	var mappedPt2 = ParabolaCoordinateMap.GlobalPointFromLocal(locX, consts.GloabalX0, consts.A, consts.B, consts.C);
	Console.WriteLine($"Local x:{locX} to Global x:{mappedPt2.x}, y:{mappedPt2.y}");

	var mappedPt3 = MyCableBridgesCalculations.ParabolaCoordinateMap.GlobalPointFromLocal(locX, consts.GloabalX0, consts.A, consts.B, consts.C);
	Console.WriteLine($"Local x:{locX} to Global x:{mappedPt3.x}, y:{mappedPt3.y}");

	var mpNr = new ParabolaCoordinateMapperNR(consts.A, consts.B, consts.C, consts.GloabalX0);
	var mappedPt4 = mpNr.MapLocalToGlobal(locX);
	Console.WriteLine($"Local x:{locX} to Global x:{mappedPt4.GlobalX}, y:{mappedPt4.GlobalY}");

	var dataPointsMapper = new DataDrivenMapper(consts.A, consts.B, consts.C, consts.DataPoints);
	var mappedPt5 = dataPointsMapper.MapLocalToGlobal(locX);
	Console.WriteLine($"Local x:{locX} to Global x:{mappedPt5.GlobalX}, y:{mappedPt5.GlobalY}");
}
catch (Exception ex)
{
	Console.WriteLine($"Mapping error: {ex.Message}");
}
