using Asun.Platform.MetrologyProductionIntegration;

namespace Asun.Platform.ReplayIntegration.Smoke;

public static class PcbMeasurementReplayComponentBinding2HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var group1=0;group1<10;group1++)
        {
            for(var i1=0;i1<10;i1++)
            {
                round++;
                var fixture=PcbMeasurementReplayFixtureRuntime.Create();
                var descriptors=fixture.ReplayDescriptors.ToArray();
                descriptors[i1%2]=descriptors[i1%2] with
                {
                    ComponentId=i1%2==0 ? "UNKNOWN-ID" : "R1-ID"
                };
                Check(
                    !PcbMeasurementReplayComponentBindingRuntime.IsValid(
                        fixture.ExecutionBinding,fixture.MeasurementBindings,descriptors) &&
                    (group1<9 || round==100),
                    "replay component identity drift must be rejected");
            }
        }
        for(var group2=0;group2<10;group2++)
        {
            for(var i2=0;i2<10;i2++)
            {
                round++;
                var fixture=PcbMeasurementReplayFixtureRuntime.Create();
                var descriptors=fixture.ReplayDescriptors.ToArray();
                descriptors[i2%2]=descriptors[i2%2] with
                {
                    ComponentId=i2%2==0 ? "UNKNOWN-ID" : "R1-ID"
                };
                Check(
                    !PcbMeasurementReplayComponentBindingRuntime.IsValid(
                        fixture.ExecutionBinding,fixture.MeasurementBindings,descriptors) &&
                    (group2<9 || round==100),
                    "replay component identity drift must be rejected");
            }
        }
        for(var group3=0;group3<10;group3++)
        {
            for(var i3=0;i3<10;i3++)
            {
                round++;
                var fixture=PcbMeasurementReplayFixtureRuntime.Create();
                var descriptors=fixture.ReplayDescriptors.ToArray();
                descriptors[i3%2]=descriptors[i3%2] with
                {
                    ComponentId=i3%2==0 ? "UNKNOWN-ID" : "R1-ID"
                };
                Check(
                    !PcbMeasurementReplayComponentBindingRuntime.IsValid(
                        fixture.ExecutionBinding,fixture.MeasurementBindings,descriptors) &&
                    (group3<9 || round==100),
                    "replay component identity drift must be rejected");
            }
        }
        for(var group4=0;group4<10;group4++)
        {
            for(var i4=0;i4<10;i4++)
            {
                round++;
                var fixture=PcbMeasurementReplayFixtureRuntime.Create();
                var descriptors=fixture.ReplayDescriptors.ToArray();
                descriptors[i4%2]=descriptors[i4%2] with
                {
                    ComponentId=i4%2==0 ? "UNKNOWN-ID" : "R1-ID"
                };
                Check(
                    !PcbMeasurementReplayComponentBindingRuntime.IsValid(
                        fixture.ExecutionBinding,fixture.MeasurementBindings,descriptors) &&
                    (group4<9 || round==100),
                    "replay component identity drift must be rejected");
            }
        }
        for(var group5=0;group5<10;group5++)
        {
            for(var i5=0;i5<10;i5++)
            {
                round++;
                var fixture=PcbMeasurementReplayFixtureRuntime.Create();
                var descriptors=fixture.ReplayDescriptors.ToArray();
                descriptors[i5%2]=descriptors[i5%2] with
                {
                    ComponentId=i5%2==0 ? "UNKNOWN-ID" : "R1-ID"
                };
                Check(
                    !PcbMeasurementReplayComponentBindingRuntime.IsValid(
                        fixture.ExecutionBinding,fixture.MeasurementBindings,descriptors) &&
                    (group5<9 || round==100),
                    "replay component identity drift must be rejected");
            }
        }
        for(var group6=0;group6<10;group6++)
        {
            for(var i6=0;i6<10;i6++)
            {
                round++;
                var fixture=PcbMeasurementReplayFixtureRuntime.Create();
                var descriptors=fixture.ReplayDescriptors.ToArray();
                descriptors[i6%2]=descriptors[i6%2] with
                {
                    ComponentId=i6%2==0 ? "UNKNOWN-ID" : "R1-ID"
                };
                Check(
                    !PcbMeasurementReplayComponentBindingRuntime.IsValid(
                        fixture.ExecutionBinding,fixture.MeasurementBindings,descriptors) &&
                    (group6<9 || round==100),
                    "replay component identity drift must be rejected");
            }
        }
        for(var group7=0;group7<10;group7++)
        {
            for(var i7=0;i7<10;i7++)
            {
                round++;
                var fixture=PcbMeasurementReplayFixtureRuntime.Create();
                var descriptors=fixture.ReplayDescriptors.ToArray();
                descriptors[i7%2]=descriptors[i7%2] with
                {
                    ComponentId=i7%2==0 ? "UNKNOWN-ID" : "R1-ID"
                };
                Check(
                    !PcbMeasurementReplayComponentBindingRuntime.IsValid(
                        fixture.ExecutionBinding,fixture.MeasurementBindings,descriptors) &&
                    (group7<9 || round==100),
                    "replay component identity drift must be rejected");
            }
        }
        for(var group8=0;group8<10;group8++)
        {
            for(var i8=0;i8<10;i8++)
            {
                round++;
                var fixture=PcbMeasurementReplayFixtureRuntime.Create();
                var descriptors=fixture.ReplayDescriptors.ToArray();
                descriptors[i8%2]=descriptors[i8%2] with
                {
                    ComponentId=i8%2==0 ? "UNKNOWN-ID" : "R1-ID"
                };
                Check(
                    !PcbMeasurementReplayComponentBindingRuntime.IsValid(
                        fixture.ExecutionBinding,fixture.MeasurementBindings,descriptors) &&
                    (group8<9 || round==100),
                    "replay component identity drift must be rejected");
            }
        }
        for(var group9=0;group9<10;group9++)
        {
            for(var i9=0;i9<10;i9++)
            {
                round++;
                var fixture=PcbMeasurementReplayFixtureRuntime.Create();
                var descriptors=fixture.ReplayDescriptors.ToArray();
                descriptors[i9%2]=descriptors[i9%2] with
                {
                    ComponentId=i9%2==0 ? "UNKNOWN-ID" : "R1-ID"
                };
                Check(
                    !PcbMeasurementReplayComponentBindingRuntime.IsValid(
                        fixture.ExecutionBinding,fixture.MeasurementBindings,descriptors) &&
                    (group9<9 || round==100),
                    "replay component identity drift must be rejected");
            }
        }
        for(var group10=0;group10<10;group10++)
        {
            for(var i10=0;i10<10;i10++)
            {
                round++;
                var fixture=PcbMeasurementReplayFixtureRuntime.Create();
                var descriptors=fixture.ReplayDescriptors.ToArray();
                descriptors[i10%2]=descriptors[i10%2] with
                {
                    ComponentId=i10%2==0 ? "UNKNOWN-ID" : "R1-ID"
                };
                Check(
                    !PcbMeasurementReplayComponentBindingRuntime.IsValid(
                        fixture.ExecutionBinding,fixture.MeasurementBindings,descriptors) &&
                    (group10<9 || round==100),
                    "replay component identity drift must be rejected");
            }
        }
        return Task.CompletedTask;
    }
}
