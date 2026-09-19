using System.Globalization;

namespace Asun.UI.Viewports;

public static class RoiPhysicalUnitFormatterRuntime
{
    public static string Length(double value, string unit = "px", int decimals = 3)
    {
        if (!double.IsFinite(value) || decimals < 0 || decimals > 8 || string.IsNullOrWhiteSpace(unit))
            throw new ArgumentOutOfRangeException(nameof(value));
        return $"{value.ToString("F" + decimals, CultureInfo.InvariantCulture)} {unit}";
    }

    public static string Area(double value, string unit = "px²", int decimals = 3)
    {
        if (!double.IsFinite(value) || value < 0) throw new ArgumentOutOfRangeException(nameof(value));
        return $"{value.ToString("F" + decimals, CultureInfo.InvariantCulture)} {unit}";
    }
}
