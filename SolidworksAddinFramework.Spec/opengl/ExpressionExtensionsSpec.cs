using FluentAssertions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Weingartner.WeinCad.Interfaces.Reflection;
using Xunit;

namespace SolidworksAddinFramework.Spec.Reflection
{
    public class ExpressionExtensionsSpec
    {
        class B : ReactiveObject
        {
            [Reactive]
            public int X { get; set; }
        }

        class A : ReactiveObject
        {
            [Reactive]
            public B B { get; set; } = new B();
        }

        [Fact]
        public void ShouldWorkNested()
        {

            var a = new A();

            var proxy = a.GetProxy(p => p.B.X);

            proxy.Value = 10;

            a.B.X.Should().Be(10);


        }
        
    }
}