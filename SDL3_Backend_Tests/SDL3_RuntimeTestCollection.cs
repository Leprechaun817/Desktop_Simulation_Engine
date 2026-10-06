using Xunit;

namespace SDL3_Backend_Tests
{
	[CollectionDefinition(Name, DisableParallelization = true)]
	public sealed class SDLRuntimeTestCollection
	{
		public const string Name = "SDL runtime";
	}
}