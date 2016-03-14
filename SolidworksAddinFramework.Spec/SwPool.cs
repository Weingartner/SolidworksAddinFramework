using SwCSharpAddinSpecHelper;
using Xunit;

namespace SolidworksAddinFramework.Spec
{
    /// <summary>
    /// xUnit searches collection definitions only in the assembly of the test class (see https://github.com/xunit/xunit/issues/374).
    /// So this class must be compiled in all test assemblies.
    /// </summary>
    [CollectionDefinition(nameof(SwPool))]
    public class SwPool : ICollectionFixture<SwPoolFixture>
    {
    }
}