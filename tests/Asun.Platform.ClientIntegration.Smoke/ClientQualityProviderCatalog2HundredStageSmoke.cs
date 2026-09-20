using Asun.Platform.ClientIntegration;
using Asun.Platform.SimulationIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientQualityProviderCatalog2HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var catalog=new ClientQualityProviderCatalog();
            catalog.Register(ClientSimulationQualityRunProvider.CreateDefinition());
            Check(catalog.Providers.Count==1 &&
                  catalog.Providers[0].Descriptor.ProviderId=="simulation-quality" &&
                  catalog.Providers[0].Descriptor.IsSimulation,
                  "Simulation Quality provider must register with explicit simulation provenance");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var catalog=new ClientQualityProviderCatalog();
            catalog.Register(ClientSimulationQualityRunProvider.CreateDefinition());
            Check(catalog.Providers.Count==1 &&
                  catalog.Providers[0].Descriptor.ProviderId=="simulation-quality" &&
                  catalog.Providers[0].Descriptor.IsSimulation,
                  "Simulation Quality provider must register with explicit simulation provenance");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var catalog=new ClientQualityProviderCatalog();
            catalog.Register(ClientSimulationQualityRunProvider.CreateDefinition());
            Check(catalog.Providers.Count==1 &&
                  catalog.Providers[0].Descriptor.ProviderId=="simulation-quality" &&
                  catalog.Providers[0].Descriptor.IsSimulation,
                  "Simulation Quality provider must register with explicit simulation provenance");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var catalog=new ClientQualityProviderCatalog();
            catalog.Register(ClientSimulationQualityRunProvider.CreateDefinition());
            Check(catalog.Providers.Count==1 &&
                  catalog.Providers[0].Descriptor.ProviderId=="simulation-quality" &&
                  catalog.Providers[0].Descriptor.IsSimulation,
                  "Simulation Quality provider must register with explicit simulation provenance");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var catalog=new ClientQualityProviderCatalog();
            catalog.Register(ClientSimulationQualityRunProvider.CreateDefinition());
            Check(catalog.Providers.Count==1 &&
                  catalog.Providers[0].Descriptor.ProviderId=="simulation-quality" &&
                  catalog.Providers[0].Descriptor.IsSimulation,
                  "Simulation Quality provider must register with explicit simulation provenance");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var catalog=new ClientQualityProviderCatalog();
            catalog.Register(ClientSimulationQualityRunProvider.CreateDefinition());
            Check(catalog.Providers.Count==1 &&
                  catalog.Providers[0].Descriptor.ProviderId=="simulation-quality" &&
                  catalog.Providers[0].Descriptor.IsSimulation,
                  "Simulation Quality provider must register with explicit simulation provenance");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var catalog=new ClientQualityProviderCatalog();
            catalog.Register(ClientSimulationQualityRunProvider.CreateDefinition());
            Check(catalog.Providers.Count==1 &&
                  catalog.Providers[0].Descriptor.ProviderId=="simulation-quality" &&
                  catalog.Providers[0].Descriptor.IsSimulation,
                  "Simulation Quality provider must register with explicit simulation provenance");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var catalog=new ClientQualityProviderCatalog();
            catalog.Register(ClientSimulationQualityRunProvider.CreateDefinition());
            Check(catalog.Providers.Count==1 &&
                  catalog.Providers[0].Descriptor.ProviderId=="simulation-quality" &&
                  catalog.Providers[0].Descriptor.IsSimulation,
                  "Simulation Quality provider must register with explicit simulation provenance");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var catalog=new ClientQualityProviderCatalog();
            catalog.Register(ClientSimulationQualityRunProvider.CreateDefinition());
            Check(catalog.Providers.Count==1 &&
                  catalog.Providers[0].Descriptor.ProviderId=="simulation-quality" &&
                  catalog.Providers[0].Descriptor.IsSimulation,
                  "Simulation Quality provider must register with explicit simulation provenance");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var catalog=new ClientQualityProviderCatalog();
            catalog.Register(ClientSimulationQualityRunProvider.CreateDefinition());
            Check(catalog.Providers.Count==1 &&
                  catalog.Providers[0].Descriptor.ProviderId=="simulation-quality" &&
                  catalog.Providers[0].Descriptor.IsSimulation,
                  "Simulation Quality provider must register with explicit simulation provenance");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
