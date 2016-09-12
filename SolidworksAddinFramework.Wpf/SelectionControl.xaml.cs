using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;
using SolidWorks.Interop.sldworks;

namespace SolidworksAddinFramework.Wpf
{
public static class ObservableEx
{
    public static IObservable<Unit> FromEvent(
        Action<Func<int>> addHandler, 
        Action<Func<int>> removeHandler)
    {
        return Observable.Create<Unit>(observer =>
            {
                Func<int> eventHandler = () =>
                {
                    observer.OnNext(Unit.Default);
                    return 0;
                };

                addHandler(eventHandler);

                return Disposable.Create(() => removeHandler(eventHandler));
            });
    }
}
    /// <summary>
    /// Interaction logic for SelectionControl.xaml
    /// </summary>
    public partial class SelectionControl : UserControl
    {
        public static readonly DependencyProperty SldWorksProperty = DependencyProperty.Register(
            "SldWorks", typeof (ISldWorks), typeof (SelectionControl), new PropertyMetadata(default(ISldWorks)));

        public ISldWorks SldWorks
        {
            get { return (ISldWorks) GetValue(SldWorksProperty); }
            set { SetValue(SldWorksProperty, value); }
        }

        public static readonly DependencyProperty ModelDoc2Property = DependencyProperty.Register(
            "ModelDoc2", typeof (IModelDoc2), typeof (SelectionControl), new PropertyMetadata(default(IModelDoc2)));

        public IModelDoc2 ModelDoc2
        {
            get { return (IModelDoc2) GetValue(ModelDoc2Property); }
            set { SetValue(ModelDoc2Property, value); }
        }

        public SelectionControl()
        {
            InitializeComponent();
            /*
            this.WhenAnyValue(p=>p.ModelDoc2)
                .OfType<PartDoc>()
                .Select(doc => doc.SelectionObservable().Select(v=>doc))
                .Switch()
                .Subscribe(doc =>
                {
                    if (partDoc != null)
                    {
                        partDoc.UserSelectionPostNotify += () =>
                        {

                        };
                    }
                })
                */
        }
    }
}
