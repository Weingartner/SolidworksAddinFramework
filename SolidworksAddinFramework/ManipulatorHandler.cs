using System;
using System.Reactive;
using System.Reactive.Subjects;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;

namespace SolidworksAddinFramework
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ManipulatorHandler : SwManipulatorHandler2
    {
        public virtual void OnDirectionFlipped(object pManipulator)
        {
        }

        public virtual void OnHandleSelected(object pManipulator, int handleIndex)
        {
        }

        public virtual void OnUpdateDrag(object pManipulator, int handleIndex, object newPosMathPt)
        {
        }

        public virtual void OnEndDrag(object pManipulator, int handleIndex)
        {
        }

        public virtual void OnEndNoDrag(object pManipulator, int handleIndex)
        {
        }

        public virtual void OnHandleRmbSelected(object pManipulator, int handleIndex)
        {
        }
        public virtual void OnItemSetFocus(object pManipulator, int handleIndex)
        {
        }

        #region cancelable
        public virtual bool OnHandleLmbSelected(object pManipulator)
        {
            return true;
        }

        public virtual bool OnDelete(object pManipulator)
        {
            return true;
        }

        public virtual bool OnDoubleValueChanged(object pManipulator, int handleIndex, ref double value)
        {
            return true;
        }

        public virtual bool OnStringValueChanged(object pManipulator, int handleIndex, ref string Value)
        {
            return true;
        }
        #endregion

    }

    public class TriadManipulatorTs : ManipulatorHandler, ITriadManipulator, IManipulator
    {
        ITriadManipulator _Impl;
        public void UpdateScale(double Scale)
        {
            _Impl.UpdateScale(Scale);
        }

        public bool UpdatePosition()
        {
            return _Impl.UpdatePosition();
        }

        public void SetColorRefAtIndex(int index, int colorRef)
        {
            _Impl.SetColorRefAtIndex(index, colorRef);
        }

        public int DoNotShow
        {
            get { return _Impl.DoNotShow; }
            set { _Impl.DoNotShow = value; }
        }

        public MathPoint Origin
        {
            get { return _Impl.Origin; }
            set { _Impl.Origin = value; }
        }

        public MathVector XAxis
        {
            get { return _Impl.XAxis; }
            set { _Impl.XAxis = value; }
        }

        public MathVector YAxis
        {
            get { return _Impl.YAxis; }
            set { _Impl.YAxis = value; }
        }

        public MathVector ZAxis
        {
            get { return _Impl.ZAxis; }
            set { _Impl.ZAxis = value; }
        }

        public int Cursor
        {
            set { _Impl.Cursor = value; }
        }

        public MathPoint PreviousDragPoint => _Impl.PreviousDragPoint;

        Subject<Tuple<swTriadManipulatorControlPoints_e, double>> _DoubleChangedSubject = new Subject<Tuple<swTriadManipulatorControlPoints_e, double>>();

        public IObservable<Tuple<swTriadManipulatorControlPoints_e, double>> DoubleChangedObservable
            => _DoubleChangedSubject; 

        public TriadManipulatorTs(IModelDoc2 doc)
        {
            _Manipulator = doc.ModelViewManager.CreateManipulator((int) swManipulatorType_e.swTriadManipulator, this);
            _Impl = (ITriadManipulator) _Manipulator.GetSpecificManipulator();
            _Impl.Cursor = (int)swManipulatorCursor_e.swManipulatorMoveCursor;
        }

        public override bool OnDoubleValueChanged(object pManipulator, int handleIndex, ref double value)
        {
            _DoubleChangedSubject.OnNext(Tuple.Create((swTriadManipulatorControlPoints_e)handleIndex, value));
            return true;
        }

        private Subject<swTriadManipulatorControlPoints_e> _EndDragSubject = new Subject<swTriadManipulatorControlPoints_e>();
        private readonly Manipulator _Manipulator;
        public IObservable<swTriadManipulatorControlPoints_e> EndDragObservable => _EndDragSubject; 
        public override void OnEndDrag(object pManipulator, int handleIndex)
        {
            _EndDragSubject.OnNext((swTriadManipulatorControlPoints_e)handleIndex);
        }

        public void Show(object PModelDoc)
        {
            _Manipulator.Show(PModelDoc);
        }

        public void Remove()
        {
            _Manipulator.Remove();
        }

        public object GetSpecificManipulator()
        {
            return _Manipulator.GetSpecificManipulator();
        }

        public bool Select(bool Append, SelectData Data)
        {
            return _Manipulator.Select(Append, Data);
        }

        public bool Visible
        {
            get { return _Manipulator.Visible; }
            set { _Manipulator.Visible = value; }
        }

        public string Name
        {
            get { return _Manipulator.Name; }
            set { _Manipulator.Name = value; }
        }

        public bool Selectable
        {
            get { return _Manipulator.Selectable; }
            set { _Manipulator.Selectable = value; }
        }

        public int Options
        {
            get { return _Manipulator.Options; }
            set { _Manipulator.Options = value; }
        }
    }
}