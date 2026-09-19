using Asun.Metrology.Core;

namespace Asun.Simulation.Core;

public sealed record SimulatedDefect(
    SimulationDefectKind Kind,
    string TargetDesignator,
    MetrologyPoint2D Position,
    double Magnitude,
    string Code)
{
    public bool IsValid=>
        Enum.IsDefined(Kind) &&
        !string.IsNullOrWhiteSpace(TargetDesignator) &&
        Position.IsFinite &&
        double.IsFinite(Magnitude) &&
        Magnitude>=0 &&
        !string.IsNullOrWhiteSpace(Code);
}
