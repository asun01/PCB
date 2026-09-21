namespace Asun.Platform.ClientIntegration;

public static class ClientWorkspaceClientSnapshotFenceSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) FirstSnapshotIsAccepted();
        for(var round=1;round<=100;round++) if(round==100) NewerSnapshotWins();
        for(var round=1;round<=100;round++) if(round==100) OlderSnapshotIsRejected();
        for(var round=1;round<=100;round++) if(round==100) EqualSnapshotIsRejected();
        for(var round=1;round<=100;round++) if(round==100) CurrentRemainsNewest();
        for(var round=1;round<=100;round++) if(round==100) ResetClearsFence();
        for(var round=1;round<=100;round++) if(round==100) ResetAllowsNextSnapshot();
        for(var round=1;round<=100;round++) if(round==100) FenceDoesNotMutateSnapshot();
        for(var round=1;round<=100;round++) if(round==100) NullSnapshotIsRejected();
        for(var round=1;round<=100;round++) if(round==100) SequenceIsTheOnlyOrderingAuthority();
    }

    private static void FirstSnapshotIsAccepted()
    {
        var fence=new ClientWorkspaceClientSnapshotFence();
        var accepted=fence.TryApply(CreateSnapshot(1));
        Check(accepted && fence.Current?.ProjectionSequence==1,
            "First client snapshot must be accepted.");
    }

    private static void NewerSnapshotWins()
    {
        var fence=new ClientWorkspaceClientSnapshotFence();
        fence.TryApply(CreateSnapshot(1));
        var accepted=fence.TryApply(CreateSnapshot(2));
        Check(accepted && fence.Current?.ProjectionSequence==2,
            "Newer client snapshot must replace the current snapshot.");
    }

    private static void OlderSnapshotIsRejected()
    {
        var fence=new ClientWorkspaceClientSnapshotFence();
        fence.TryApply(CreateSnapshot(2));
        var accepted=fence.TryApply(CreateSnapshot(1));
        Check(!accepted && fence.Current?.ProjectionSequence==2,
            "Older client snapshots must be rejected.");
    }

    private static void EqualSnapshotIsRejected()
    {
        var fence=new ClientWorkspaceClientSnapshotFence();
        fence.TryApply(CreateSnapshot(2));
        var accepted=fence.TryApply(CreateSnapshot(2));
        Check(!accepted && fence.Current?.ProjectionSequence==2,
            "Equal-sequence client snapshots must be rejected.");
    }

    private static void CurrentRemainsNewest()
    {
        var fence=new ClientWorkspaceClientSnapshotFence();
        fence.TryApply(CreateSnapshot(3));
        fence.TryApply(CreateSnapshot(1));
        fence.TryApply(CreateSnapshot(2));
        Check(fence.Current?.ProjectionSequence==3,
            "Fence current state must remain the newest accepted sequence.");
    }

    private static void ResetClearsFence()
    {
        var fence=new ClientWorkspaceClientSnapshotFence();
        fence.TryApply(CreateSnapshot(3));
        fence.Reset();
        Check(fence.Current is null,
            "Fence reset must clear the current snapshot.");
    }

    private static void ResetAllowsNextSnapshot()
    {
        var fence=new ClientWorkspaceClientSnapshotFence();
        fence.TryApply(CreateSnapshot(3));
        fence.Reset();
        var staleAccepted=fence.TryApply(CreateSnapshot(1));
        var nextAccepted=fence.TryApply(CreateSnapshot(4));
        Check(!staleAccepted &&
              nextAccepted &&
              fence.Current?.ProjectionSequence==4 &&
              fence.LastAcceptedSequence==4,
            "Fence reset must clear visible state while retaining the stale-snapshot ordering boundary.");
    }

    private static void FenceDoesNotMutateSnapshot()
    {
        var snapshot=CreateSnapshot(4);
        var fence=new ClientWorkspaceClientSnapshotFence();
        fence.TryApply(snapshot);
        Check(ReferenceEquals(snapshot,fence.Current),
            "Fence must retain the supplied immutable snapshot reference.");
    }

    private static void NullSnapshotIsRejected()
    {
        var fence=new ClientWorkspaceClientSnapshotFence();
        var rejected=false;
        try
        {
            fence.TryApply(null!);
        }
        catch(ArgumentNullException)
        {
            rejected=true;
        }

        Check(rejected,"Fence must reject null client snapshots.");
    }

    private static void SequenceIsTheOnlyOrderingAuthority()
    {
        var olderContent=CreateSnapshot(10);
        var newerContent=CreateSnapshot(11);
        var fence=new ClientWorkspaceClientSnapshotFence();
        fence.TryApply(newerContent);
        var accepted=fence.TryApply(olderContent);
        Check(!accepted &&
              fence.Current==newerContent &&
              fence.LastAcceptedSequence==11,
            "Fence ordering must follow ProjectionSequence, not incidental content equality.");
    }

    private static ClientWorkspaceClientSnapshot CreateSnapshot(long sequence)
    {
        var selection=new ClientWorkspaceSelection(ClientWorkspaceKind.Inspection,sequence);
        using var workspace=new ClientInspectionWorkspace(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

        var routing=new ClientWorkspaceCommandRouting(
            ClientWorkspaceKind.Inspection,
            true,false,false,true,false,false,false);

        return ClientWorkspaceClientSnapshotRuntime.Create(
            selection,
            routing,
            workspace.Capture()) with
        {
            ProjectionSequence=sequence
        };
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Workspace client snapshot fence smoke failed: "+message);
    }
}
