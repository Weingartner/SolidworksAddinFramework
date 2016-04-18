using System;
using System.Collections.Generic;
using ReactiveUI;

namespace SolidworksAddinFramework.Wpf
{
    public class CollapsibleLogEntry : LogEntry
    {
        public List<LogEntry> Contents { get; set; }
    }

    public class LogEntry : ReactiveObject
    {
        DateTime _DateTime;
        public DateTime DateTime 
        {
            get { return _DateTime; }
            set { this.RaiseAndSetIfChanged(ref _DateTime, value); }
        }

        int _Index;
        public int Index 
        {
            get { return _Index; }
            set { this.RaiseAndSetIfChanged(ref _Index, value); }
        }


        string _Message;
        public string Message 
        {
            get { return _Message; }
            set { this.RaiseAndSetIfChanged(ref _Message, value); }
        }
    }
}