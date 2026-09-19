using Asun.Domain.Pcb;

namespace Asun.Simulation.Core;

public sealed record SimulatedBoardScenario(
    PcbAssemblySnapshot Assembly,
    int Seed);
