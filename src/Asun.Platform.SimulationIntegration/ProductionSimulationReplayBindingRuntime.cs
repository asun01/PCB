using System.Security.Cryptography;
using System.Text;
using Asun.Production.Runtime;
using Asun.Simulation.Core;

namespace Asun.Platform.SimulationIntegration;

public static class ProductionSimulationReplayBindingRuntime
{
    public static ProductionSimulationReplayBinding Create(
        ProductionSessionReport productionReport,
        IReadOnlyList<SimulationObservation> observations)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(observations);

        if(productionReport.FrameCount!=observations.Count)
            throw new ArgumentException(
                "Simulation observation count must match the production frame count.",
                nameof(observations));

        var productionFrames=productionReport.Frames
            .OrderBy(frame=>frame.Sequence.Value)
            .ToArray();
        var simulationFrames=observations
            .OrderBy(observation=>observation.Sequence.Value)
            .ToArray();
        var links=new List<ProductionSimulationFrameLink>(simulationFrames.Length);

        for(var index=0;index<simulationFrames.Length;index++)
        {
            var productionFrame=productionFrames[index];
            var simulationFrame=simulationFrames[index];

            if(productionFrame.Sequence.Value!=simulationFrame.Sequence.Value)
                throw new ArgumentException(
                    $"Simulation sequence {simulationFrame.Sequence.Value} does not align with production sequence {productionFrame.Sequence.Value}.");

            if(simulationFrame.Fingerprint.Length!=64 ||
               !simulationFrame.Fingerprint.All(character=>
                   Uri.IsHexDigit(character) &&
                   char.ToLowerInvariant(character)==character))
            {
                throw new ArgumentException(
                    $"Simulation fingerprint for sequence {simulationFrame.Sequence.Value} is invalid.");
            }

            links.Add(
                new ProductionSimulationFrameLink(
                    productionFrame.Sequence.Value,
                    productionFrame.InputFingerprint,
                    simulationFrame.Sequence.Value,
                    simulationFrame.Fingerprint));
        }

        var fingerprint=CreateFingerprint(
            productionReport.SessionId,
            productionReport.Fingerprint,
            links);

        return new ProductionSimulationReplayBinding(
            productionReport.SessionId,
            productionReport.Fingerprint,
            links,
            fingerprint);
    }

    internal static string CreateFingerprint(
        Guid productionSessionId,
        string productionFingerprint,
        IReadOnlyList<ProductionSimulationFrameLink> frames)
    {
        var builder=new StringBuilder();
        builder.Append(productionSessionId).Append('|')
            .Append(productionFingerprint).Append('|');

        foreach(var frame in frames.OrderBy(item=>item.ProductionSequence))
        {
            builder.Append(frame.ProductionSequence).Append('|')
                .Append(frame.ProductionInputFingerprint).Append('|')
                .Append(frame.SimulationSequence).Append('|')
                .Append(frame.SimulationObservationFingerprint).Append('|');
        }

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
