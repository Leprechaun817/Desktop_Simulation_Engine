using Xunit;

namespace SDL3_Backend_Tests;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class SdlRuntimeCollection
{
    public const string Name = "SDL runtime";
}
