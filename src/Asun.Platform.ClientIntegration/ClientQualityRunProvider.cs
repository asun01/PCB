using Asun.Domain.Quality;
using Asun.Production.Runtime;

namespace Asun.Platform.ClientIntegration;

public sealed record ClientQualityProviderDescriptor(
    string ProviderId,
    string DisplayName,
    bool IsSimulation);

public interface IClientQualityRunProvider
{
    ClientQualityProviderDescriptor Descriptor { get; }

    QualityInspectionRun Create(
        ProductionSessionReport productionReport);
}
