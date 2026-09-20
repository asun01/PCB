using Asun.Platform.RoiProductionIntegration;

namespace Asun.Platform.RoiProductionIntegration.Smoke;

public static class ProductionRoiMeasurementQualityContext3HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var group0=0;group0<10;group0++)
        {
            round++;
                var f=ProductionRoiMeasurementQualitySmokeFixtures.Create();
                var tampered=f.RoiContext with { ProductionSessionId=Guid.Parse("78000000-0000-0000-0000-000000000001") };
                Check(!ProductionRoiMeasurementQualityContextRuntime.IsValid(f.Production,tampered,f.Evaluation) &&
                      (group0<9 || round==100),
                      "ROI measurement session drift must be rejected");
        }
        for(var group1=0;group1<10;group1++)
        {
            round++;
                var f=ProductionRoiMeasurementQualitySmokeFixtures.Create();
                var tampered=f.RoiContext with { ProductionSessionId=Guid.Parse("78000000-0000-0000-0000-000000000001") };
                Check(!ProductionRoiMeasurementQualityContextRuntime.IsValid(f.Production,tampered,f.Evaluation) &&
                      (group1<9 || round==100),
                      "ROI measurement session drift must be rejected");
        }
        for(var group2=0;group2<10;group2++)
        {
            round++;
                var f=ProductionRoiMeasurementQualitySmokeFixtures.Create();
                var tampered=f.RoiContext with { ProductionSessionId=Guid.Parse("78000000-0000-0000-0000-000000000001") };
                Check(!ProductionRoiMeasurementQualityContextRuntime.IsValid(f.Production,tampered,f.Evaluation) &&
                      (group2<9 || round==100),
                      "ROI measurement session drift must be rejected");
        }
        for(var group3=0;group3<10;group3++)
        {
            round++;
                var f=ProductionRoiMeasurementQualitySmokeFixtures.Create();
                var tampered=f.RoiContext with { ProductionSessionId=Guid.Parse("78000000-0000-0000-0000-000000000001") };
                Check(!ProductionRoiMeasurementQualityContextRuntime.IsValid(f.Production,tampered,f.Evaluation) &&
                      (group3<9 || round==100),
                      "ROI measurement session drift must be rejected");
        }
        for(var group4=0;group4<10;group4++)
        {
            round++;
                var f=ProductionRoiMeasurementQualitySmokeFixtures.Create();
                var tampered=f.RoiContext with { ProductionSessionId=Guid.Parse("78000000-0000-0000-0000-000000000001") };
                Check(!ProductionRoiMeasurementQualityContextRuntime.IsValid(f.Production,tampered,f.Evaluation) &&
                      (group4<9 || round==100),
                      "ROI measurement session drift must be rejected");
        }
        for(var group5=0;group5<10;group5++)
        {
            round++;
                var f=ProductionRoiMeasurementQualitySmokeFixtures.Create();
                var tampered=f.RoiContext with { ProductionSessionId=Guid.Parse("78000000-0000-0000-0000-000000000001") };
                Check(!ProductionRoiMeasurementQualityContextRuntime.IsValid(f.Production,tampered,f.Evaluation) &&
                      (group5<9 || round==100),
                      "ROI measurement session drift must be rejected");
        }
        for(var group6=0;group6<10;group6++)
        {
            round++;
                var f=ProductionRoiMeasurementQualitySmokeFixtures.Create();
                var tampered=f.RoiContext with { ProductionSessionId=Guid.Parse("78000000-0000-0000-0000-000000000001") };
                Check(!ProductionRoiMeasurementQualityContextRuntime.IsValid(f.Production,tampered,f.Evaluation) &&
                      (group6<9 || round==100),
                      "ROI measurement session drift must be rejected");
        }
        for(var group7=0;group7<10;group7++)
        {
            round++;
                var f=ProductionRoiMeasurementQualitySmokeFixtures.Create();
                var tampered=f.RoiContext with { ProductionSessionId=Guid.Parse("78000000-0000-0000-0000-000000000001") };
                Check(!ProductionRoiMeasurementQualityContextRuntime.IsValid(f.Production,tampered,f.Evaluation) &&
                      (group7<9 || round==100),
                      "ROI measurement session drift must be rejected");
        }
        for(var group8=0;group8<10;group8++)
        {
            round++;
                var f=ProductionRoiMeasurementQualitySmokeFixtures.Create();
                var tampered=f.RoiContext with { ProductionSessionId=Guid.Parse("78000000-0000-0000-0000-000000000001") };
                Check(!ProductionRoiMeasurementQualityContextRuntime.IsValid(f.Production,tampered,f.Evaluation) &&
                      (group8<9 || round==100),
                      "ROI measurement session drift must be rejected");
        }
        for(var group9=0;group9<10;group9++)
        {
            round++;
                var f=ProductionRoiMeasurementQualitySmokeFixtures.Create();
                var tampered=f.RoiContext with { ProductionSessionId=Guid.Parse("78000000-0000-0000-0000-000000000001") };
                Check(!ProductionRoiMeasurementQualityContextRuntime.IsValid(f.Production,tampered,f.Evaluation) &&
                      (group9<9 || round==100),
                      "ROI measurement session drift must be rejected");
        }
        return Task.CompletedTask;
    }
}
