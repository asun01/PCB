using System.Numerics;

var failures = new List<string>();

void Assert(bool condition, string message)
{
    if (!condition)
        failures.Add(message);
}

NumericToleranceValidationHundredStageSmoke.Run(Assert);
AffineTransform2DValidationHundredStageSmoke.Run(Assert);
ImageGeometryValidationHundredStageSmoke.Run(Assert);
PointSet2DValidationHundredStageSmoke.Run(Assert);
Polygon2DValidationHundredStageSmoke.Run(Assert);

if (failures.Count > 0)
{
    foreach (var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Vision.Contracts smoke tests passed.");
return 0;
