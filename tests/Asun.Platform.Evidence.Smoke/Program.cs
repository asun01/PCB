using Asun.Platform.Evidence;

var failures=new List<string>();
var round=0;

void Check(bool condition,string message)
{
    round++;

    if(!condition)
        failures.Add($"Round {round}: {message}");
}

var handle=EvidenceHandle.Create("frame://001");
var descriptor=new EvidenceDescriptor(
    handle,
    EvidenceKind.Image,
    "image/raw",
    1024,
    "frame-001");

var invalidHandle=new EvidenceDescriptor(
    new EvidenceHandle(""),
    EvidenceKind.Image,
    "image/raw",
    1024,
    "invalid");

var invalidLength=descriptor with {ByteLength=-1};
var invalidKind=descriptor with {Kind=(EvidenceKind)99};

for(var i=0;i<10;i++) Check(handle.IsValid,"Evidence handle should remain valid.");
for(var i=0;i<10;i++) Check(handle.Value=="frame://001","Evidence handle value should remain stable.");
for(var i=0;i<10;i++) Check(EvidenceDescriptorValidationRuntime.IsValid(descriptor),"Descriptor should remain valid.");
for(var i=0;i<10;i++) Check(descriptor.Handle==handle,"Descriptor handle should remain stable.");
for(var i=0;i<10;i++) Check(descriptor.Kind==EvidenceKind.Image,"Descriptor kind should remain stable.");
for(var i=0;i<10;i++) Check(descriptor.MediaType=="image/raw","Descriptor media type should remain stable.");
for(var i=0;i<10;i++) Check(!EvidenceDescriptorValidationRuntime.IsValid(invalidHandle),"Invalid handle should remain rejected.");
for(var i=0;i<10;i++) Check(!EvidenceDescriptorValidationRuntime.IsValid(invalidLength),"Negative length should remain rejected.");
for(var i=0;i<10;i++) Check(!EvidenceDescriptorValidationRuntime.IsValid(invalidKind),"Invalid kind should remain rejected.");
for(var i=0;i<10;i++) Check(EvidenceHandle.Create(" frame://001 ").Value=="frame://001","Handle normalization should remain deterministic.");

if(round!=100)
    failures.Add($"Evidence smoke should execute exactly 100 numbered rounds; actual {round}.");

if(failures.Count>0)
{
    foreach(var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Platform.Evidence smoke tests passed.");
return 0;
