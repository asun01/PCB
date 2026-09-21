namespace Asun.Platform.ClientIntegration;

public sealed record ClientProductionRunHistoryEntry(
    long Ordinal,
    Guid ProgramId,
    Guid ProductionSessionId,
    int FrameCount,
    string ReplayFingerprint,
    bool ReleaseReady,
    string ArtifactPath)
{
    public string QualityFingerprint { get; init; }="";
}

public sealed record ClientProductionRunHistorySnapshot(
    int Capacity,
    long NextOrdinal,
    long DroppedCount,
    IReadOnlyList<ClientProductionRunHistoryEntry> Entries);

public sealed class ClientProductionRunHistory
{
    private readonly object _sync=new();
    private readonly int _capacity;
    private readonly List<ClientProductionRunHistoryEntry> _entries;
    private long _nextOrdinal=1;
    private long _droppedCount;

    public ClientProductionRunHistory(int capacity=20)
    {
        if(capacity<=0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        _capacity=capacity;
        _entries=new List<ClientProductionRunHistoryEntry>(capacity);
    }

    public ClientProductionRunHistorySnapshot Capture()
    {
        lock(_sync)
        {
            return new ClientProductionRunHistorySnapshot(
                _capacity,
                _nextOrdinal,
                _droppedCount,
                _entries.ToArray());
        }
    }

    public ClientProductionRunHistoryEntry Append(
        ClientProductionReplaySnapshot replaySnapshot,
        ClientReleaseProjection releaseProjection)
    {
        ArgumentNullException.ThrowIfNull(replaySnapshot);
        ArgumentNullException.ThrowIfNull(releaseProjection);

        if(replaySnapshot.Status!=ClientExecutionStatus.Completed)
            throw new ArgumentException("Only completed client runs can enter run history.",nameof(replaySnapshot));

        if(replaySnapshot.ProgramId!=releaseProjection.ProgramId ||
           replaySnapshot.ProductionSessionId!=releaseProjection.ProductionSessionId)
            throw new ArgumentException("Run history identities must agree across replay and Release projections.");

        if(!IsLowerHex(replaySnapshot.ReplayFingerprint))
            throw new ArgumentException("Run history requires a valid Replay fingerprint.",nameof(replaySnapshot));

        if(string.IsNullOrWhiteSpace(replaySnapshot.QualityFingerprint) ||
           !IsLowerHex(replaySnapshot.QualityFingerprint))
            throw new ArgumentException("Run history requires an authoritative Quality fingerprint.",nameof(replaySnapshot));

        if(replaySnapshot.QualityFingerprint!=releaseProjection.QualityFingerprint)
            throw new ArgumentException("Run history Quality fingerprints must agree across replay and Release projections.");

        if(replaySnapshot.ReplayFingerprint!=releaseProjection.ReplayFingerprint)
            throw new ArgumentException("Run history Replay fingerprints must agree across replay and Release projections.");

        lock(_sync)
        {
            var entry=new ClientProductionRunHistoryEntry(
                _nextOrdinal++,
                replaySnapshot.ProgramId,
                replaySnapshot.ProductionSessionId,
                replaySnapshot.FrameCount,
                replaySnapshot.ReplayFingerprint,
                releaseProjection.ReleaseReady,
                releaseProjection.ArtifactPath)
            {
                QualityFingerprint=replaySnapshot.QualityFingerprint
            };

            if(_entries.Count==_capacity)
            {
                _entries.RemoveAt(0);
                _droppedCount++;
            }

            _entries.Add(entry);
            return entry;
        }
    }

    private static bool IsLowerHex(string value)=>
        value.Length==64 &&
        value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);

    public void Reset()
    {
        lock(_sync)
        {
            _entries.Clear();
            _nextOrdinal=1;
            _droppedCount=0;
        }
    }
}
