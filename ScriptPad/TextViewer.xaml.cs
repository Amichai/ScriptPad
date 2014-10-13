using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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

namespace ScriptPad {
    /// <summary>
    /// Interaction logic for TextViewer.xaml
    /// </summary>
    public partial class TextViewer : UserControl, INotifyPropertyChanged {
        public TextViewer() {
            InitializeComponent();
        }

        public List<LogLine> LogLines {
            get { return (List<LogLine>)GetValue(LogLinesProperty); }
            set { SetValue(LogLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LogLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LogLinesProperty =
            DependencyProperty.Register("LogLines", typeof(List<LogLine>), typeof(TextViewer), 
            new PropertyMetadata(null, 
                new PropertyChangedCallback(linesPropertyChanged)));

        private static void linesPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs prop){
            (obj as TextViewer).logLinesControl.ItemsSource = prop.NewValue as List<LogLine>;
        }

        public List<string> Filepaths {
            get { return (List<string>)GetValue(FilepathsProperty); }
            set { SetValue(FilepathsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Filepaths.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilepathsProperty =
            DependencyProperty.Register("Filepaths", typeof(List<string>), typeof(TextViewer), 
            new PropertyMetadata(null,
                new PropertyChangedCallback(filepathsPropertyChanged)));

        private static void filepathsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs prop) {
            (obj as TextViewer).loadFilepaths(prop.NewValue as List<string>);
        }

        public int TotalLines { get; set; }

        private List<string> allLines = new List<string>();

        private void loadFilepaths(List<string> filepaths) {
            Task.Run(() => {
                this.allLines = filepaths.SelectMany(f => System.IO.File.ReadAllLines(f)).ToList();
                this.TotalLines = this.allLines.Count();
                Dispatcher.Invoke((Action)(() => {
                    this.LogLines  = this.getLogLines(0, this.allLines.Count());
                }));
            });
        }



        public ScrollBarVisibility VerticalScrollBarVisible {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibleProperty); }
            set { SetValue(VerticalScrollBarVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalScrollBarVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalScrollBarVisibleProperty =
            DependencyProperty.Register("VerticalScrollBarVisible", typeof(ScrollBarVisibility), typeof(TextViewer), 
            new PropertyMetadata(ScrollBarVisibility.Auto, 
                new PropertyChangedCallback((s, e) => { (s as TextViewer).scrollViewer.VerticalScrollBarVisibility = (ScrollBarVisibility)e.NewValue; })));




        public ScrollBarVisibility HorizontalScrollBarVisible {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibleProperty); }
            set { SetValue(HorizontalScrollBarVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalScrollBarVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalScrollBarVisibleProperty =
            DependencyProperty.Register("HorizontalScrollBarVisible", typeof(ScrollBarVisibility), typeof(TextViewer),
            new PropertyMetadata(ScrollBarVisibility.Auto,
                new PropertyChangedCallback((s, e) => { (s as TextViewer).scrollViewer.HorizontalScrollBarVisibility = (ScrollBarVisibility)e.NewValue; })));

        


        private List<LogLine> getLogLines(int start, int range) {
            return allLines.Skip(start).Take(range)
                .Select((i, j) => new LogLine(i, j))
                .ToList();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
