using System.Security.Cryptography;
using System.Text;

namespace Asun.Metrology.Core;

public static class DistanceMeasurementRuntime
{
    public static DistanceMeasurementResult Measure(
        MetrologyPoint2D start,
        MetrologyPoint2D end,
        string unit)
    {
        if(!MetrologyPoint2DValidationRuntime.IsValid(start))
            throw new ArgumentException("Start point is invalid.",nameof(start));

        if(!MetrologyPoint2DValidationRuntime.IsValid(end))
            throw new ArgumentException("End point is invalid.",nameof(end));

        if(string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Measurement unit cannot be blank.",nameof(unit));

        var normalizedUnit=unit.Trim();
        var distance=start.DistanceTo(end);

        var canonical=$"{start.X:R}|{start.Y:R}|{end.X:R}|{end.Y:R}|{normalizedUnit}|{distance:R}";
        var fingerprint=Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();

        return new DistanceMeasurementResult(
            start,
            end,
            distance,
            normalizedUnit,
            fingerprint);
    }
}
