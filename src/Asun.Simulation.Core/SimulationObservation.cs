using Asun.Device.Contracts;
using Asun.Metrology.Core;

namespace Asun.Simulation.Core;

public sealed record SimulationObservation(
    FrameSequence Sequence,
    MetrologyPoint2D BoardOrigin,
    IReadOnlyList<SimulatedDefect> Defects,
    string Fingerprint);
