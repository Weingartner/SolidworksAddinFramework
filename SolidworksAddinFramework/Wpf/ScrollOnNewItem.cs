using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var incc = AssociatedObject.ItemsSource as INotifyCollectionChanged;
            if (incc == null) return;

            incc.CollectionChanged += OnCollectionChanged;
        }

        private void OnUnLoaded(object sender, RoutedEventArgs e)
        {
            var incc = AssociatedObject.ItemsSource as INotifyCollectionChanged;
            if (incc == null) return;

            incc.CollectionChanged -= OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                int count = AssociatedObject.Items.Count;
                if (count == 0)
                    return;

                var item = AssociatedObject.Items[count - 1];

                var frameworkElement =
                    AssociatedObject.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                if (frameworkElement == null) return;

                frameworkElement.BringIntoView();
            }
        }
    }
}
