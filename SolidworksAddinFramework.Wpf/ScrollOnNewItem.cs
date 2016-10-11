using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using ReactiveUI;

namespace SolidworksAddinFramework.Wpf
{
    /// <summary>
    /// Copied from http://stackoverflow.com/a/11530459/158285
    /// </summary>
    public class ScrollOnNewItem : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            AssociatedObject.Loaded += OnLoaded;
            AssociatedObject.Unloaded += OnUnLoaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= OnLoaded;
            AssociatedObject.Unloaded -= OnUnLoaded;
        }

        private IDisposable d;
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var d0 = new SerialDisposable();
            var d1 = AssociatedObject
                .WhenAnyValue(p => p.ItemsSource)
                .Select
                (o =>
                 {
                     var incc = o as INotifyCollectionChanged;
                     if (incc == null)
                         return Disposable.Empty;
                     incc.CollectionChanged += OnCollectionChanged;
                     return Disposable.Create(() => incc.CollectionChanged -= OnCollectionChanged);
                 }).Subscribe(d=>d0.Disposable=d);

            d = new CompositeDisposable(d0,d1);



        }

        private void OnUnLoaded(object sender, RoutedEventArgs e)
        {
            d.Dispose();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            AssociatedObject.Items.MoveCurrentToLast();
            if(AssociatedObject.Items.Count>0)
                AssociatedObject.ScrollIntoView(AssociatedObject.Items.CurrentItem);
        }
    }
}
