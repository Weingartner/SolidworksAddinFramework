using System;
using System.Reactive.Disposables;

namespace SolidworksAddinFramework.EventHandlers
{
    public abstract class DoubleBuffer<T>
    {
        public DoubleBuffer(T t)
        {
            Back = t;
            Front = t;
        }

        private T Back{ get; set; }
        public T Front { get; private set; }

        private T Current
        {
            get { return _Level == 0 ? Front : Back; }
            set
            {
                if (_Level == 0)
                {
                    Front = value;
                    return;
                }
                Back = value;
            }
        }

        public bool FrontIsActive => _Level == 0;


        private int _Level = 0;

        private void SwitchToBackBuffer()
        {
            lock (this)
            {
                _Level++;
                if (_Level == 1)
                {
                    Back = Front;
                    OnSwitchToBack();
                }
            }
        }

        private void SwitchToFrontBuffer()
        {
            lock (this)
            {
                _Level--;
                if (_Level == 0)
                {
                    Front = Back;
                    OnSwitchToFront();
                }

            }
        }

        public IDisposable SwitchToBackBufferTemporarily()
        {
            SwitchToBackBuffer();
            return Disposable.Create(SwitchToFrontBuffer);
        }


        public void Update(Func<T, T> update)
        {
            lock (this)
            {
                Current = update(Current);
            }
        }

        public abstract void OnSwitchToFront();
        public abstract void OnSwitchToBack();

    }
}