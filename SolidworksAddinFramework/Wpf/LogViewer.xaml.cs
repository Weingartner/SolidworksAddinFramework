using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace SolidworksAddinFramework.Wpf
{
    /// <summary>
    /// A simple class for creating a log viewer. Call the static Log message.
    /// </summary>
    public partial class LogViewer
    {
        public ObservableCollection<LogEntry> LogEntries { get; set; }

        public LogViewer()
        {
            InitializeComponent();
            DataContext = LogEntries = new ObservableCollection<LogEntry>();
        }

        private static Lazy<LogViewer> Window = new Lazy<LogViewer>(() =>
        {
            var viewer = new LogViewer();
            viewer.Show();
            return viewer;
        }); 

        public static void Log(LogEntry entry)
        {
            var window = Window.Value;
            window.Dispatcher.Invoke(()=> window.LogEntries.Add(entry));
        }

        private static int CurrentIndex = 0;

        public static void LogWithIndent(int i, string message)
        {
            Log(new string(' ',i*4) + message);

            
        }
        public static void Log(string message)
        {
            try
            {
                Log(new LogEntry() {Message = message,DateTime = DateTime.Now,Index = CurrentIndex++});
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

    }
}