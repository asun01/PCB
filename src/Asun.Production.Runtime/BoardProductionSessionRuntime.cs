using System.Security.Cryptography;
using System.Text;
using Asun.Device.Contracts;
using Asun.Domain.Pcb;

namespace Asun.Production.Runtime;

public static class BoardProductionSessionRuntime
{
    public static async ValueTask<BoardProductionSessionReport> RunAsync(
        BoardProductionSessionDefinition definition,
        IFrameSource source,
        CancellationToken cancellationToken=default)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(source);

        if(!PcbAssemblySnapshotValidationRuntime.IsValid(definition.Assembly))
            throw new ArgumentException(
                "Production board assembly is invalid.",
                nameof(definition));

        var productionReport=await ProductionSessionRuntime.RunAsync(
            definition.Production,
            source,
            cancellationToken);

        var canonical=string.Join(
            "|",
            definition.Assembly.Fingerprint,
            productionReport.Fingerprint,
            productionReport.SessionId);

        var fingerprint=Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();

        return new BoardProductionSessionReport(
            definition.Assembly.Fingerprint,
            productionReport,
            fingerprint);
    }
}
