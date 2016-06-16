using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SolidworksAddinFramework.Wpf
{
    public class CollapsibleLogEntry : LogEntry
    {
        public List<LogEntry> Contents { get; set; }
    }

    public class LogEntry : ReactiveObject
    {
        [Reactive,DataMember] public DateTime DateTime {get; set;}
        [Reactive,DataMember] public int Index {get; set;}
        [Reactive,DataMember] public string Message {get; set;}
    }
}