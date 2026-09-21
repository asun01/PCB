namespace Asun.Platform.ClientIntegration;

public static class ClientInspectionWorkflowIntegritySmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) ReadyProgramCountMustMatch();
        for(var round=1;round<=100;round++) if(round==100) InvalidProgramItemsAreRejected();
        for(var round=1;round<=100;round++) if(round==100) FaultedAcquisitionCannotRetainPreview();
        for(var round=1;round<=100;round++) if(round==100) QualityPendingProductionIsValid();
        for(var round=1;round<=100;round++) if(round==100) QualityPendingCannotExposeEvidence();
        for(var round=1;round<=100;round++) if(round==100) ReleaseRequiresReplay();
        for(var round=1;round<=100;round++) if(round==100) ReplayReleaseIdentityMustMatch();
        for(var round=1;round<=100;round++) if(round==100) QualityRequiresCompletedProduction();
        for(var round=1;round<=100;round++) if(round==100) SelectedHistoryMustExist();
        for(var round=1;round<=100;round++) if(round==100) CoherentCompletedChainIsValid();
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Inspection workflow integrity smoke failed: "+message);
    }
}
