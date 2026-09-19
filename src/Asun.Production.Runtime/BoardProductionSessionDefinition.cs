using Asun.Domain.Pcb;

namespace Asun.Production.Runtime;

public sealed record BoardProductionSessionDefinition(
    PcbAssemblySnapshot Assembly,
    ProductionSessionDefinition Production);
