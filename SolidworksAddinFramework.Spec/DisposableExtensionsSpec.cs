using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Xunit;

namespace SolidworksAddinFramework.Spec
{
    public class DisposableExtensionsSpec
    {
        [Fact]
        public void SelectDisposableShouldWork()
        {
            var scheduler = new TestScheduler();
            var disposables = new List<BooleanDisposable>();
            var list = new CompositeDisposable();
            var sub = scheduler.CreateColdObservable(
                new Recorded<Notification<long>>(100, Notification.CreateOnNext(0L)),
                new Recorded<Notification<long>>(200, Notification.CreateOnNext(1L)),
                new Recorded<Notification<long>>(300, Notification.CreateOnNext(2L)),
                new Recorded<Notification<long>>(400, Notification.CreateOnNext(3L)),
                new Recorded<Notification<long>>(400, Notification.CreateOnCompleted<long>())
            )
            .SelectDisposable(list, i => {
                var d = new BooleanDisposable();
                disposables.Add(d);
                return d;
            }, (i, _) => i)
            .Subscribe()
            .DisposeWith(list);

            scheduler.AdvanceTo(300);

            disposables.Count.Should().Be(3);

            disposables.Select(d => d.IsDisposed).Should().NotContain(true);

            list.Dispose();

            disposables.Select(d => d.IsDisposed).Should().NotContain(false);

        }
    }
}
