using System;
using System.DoubleNumerics;
using System.Reactive.Subjects;
using LanguageExt;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SolidworksAddinFramework.OpenGl
{
    /// <summary>
    /// A renderable which has a replaceable inner renderable
    /// </summary>
    public class SerialRenderer :ReactiveObject, IRenderer
    {
        private readonly Transformable _Transformable = new Transformable();
        private bool _Accumulate = false;

        IRenderer _Renderer = new EmptyRenderer();
        private readonly ISubject<Unit> _NeedsRedraw = new Subject<Unit>();

        public IRenderer Renderer 
        {
            get { return _Renderer; }
            set
            {
                this.RaiseAndSetIfChanged(ref _Renderer, value);
                // ReSharper disable once ExplicitCallerInfoArgument
                this.RaisePropertyChanged(nameof(BoundingSphere));
                _NeedsRedraw.OnNext(Unit.Default);
            }
        }

        public IObservable<Unit> NeedsRedraw => _NeedsRedraw;

        public void Render(DateTime time, IDrawContext drawContext, double parentOpacity = 1.0, Matrix4x4? renderTransform = null)
        {
            var transform = _Transformable.Transform*(renderTransform ?? Matrix4x4.Identity);

            Renderer.Render(time, drawContext, parentOpacity*Opacity, transform);
        }

        public void ApplyTransform(Matrix4x4 transform, bool accumulate = false)
        {
            _Transformable.ApplyTransform(transform, accumulate);
        }


        public Tuple<Vector3, double> BoundingSphere => Renderer.BoundingSphere;
        public double Opacity { get; set; } = 1.0;
        public bool Visibility { get; set; } = true;
    }
}