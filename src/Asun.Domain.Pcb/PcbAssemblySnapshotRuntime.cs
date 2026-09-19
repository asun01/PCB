using System.Security.Cryptography;
using System.Text;

namespace Asun.Domain.Pcb;

public static class PcbAssemblySnapshotRuntime
{
    public static PcbAssemblySnapshot Create(
        PcbBoardDefinition board,
        IEnumerable<PcbComponentReference> components)
    {
        var canonical=PcbComponentCollectionRuntime.Create(board,components);
        var statistics=PcbComponentStatisticsRuntime.Create(canonical);

        var builder=new StringBuilder();
        builder.Append(board.BoardId).Append('|')
            .Append(board.Name.Length).Append(':').Append(board.Name).Append('|')
            .Append(board.WidthMm).Append('|')
            .Append(board.HeightMm).Append('|')
            .Append(board.LayerCount).Append('|');

        foreach(var component in canonical)
        {
            builder.Append(component.Id.Value.Length).Append(':').Append(component.Id.Value).Append('|')
                .Append(component.Designator.Length).Append(':').Append(component.Designator).Append('|')
                .Append(component.Value.Length).Append(':').Append(component.Value).Append('|')
                .Append(component.PackageName.Length).Append(':').Append(component.PackageName).Append('|')
                .Append(component.LayerIndex).Append('|')
                .Append((int)component.Side).Append('|')
                .Append(component.Position.Xmm).Append('|')
                .Append(component.Position.Ymm).Append('|')
                .Append(component.RotationRadians).Append('|');
        }

        var fingerprint=Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();

        return new PcbAssemblySnapshot(
            board,
            canonical,
            statistics,
            fingerprint);
    }
}
