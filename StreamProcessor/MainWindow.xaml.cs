using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace StreamProcessor {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MainWindow() {
            InitializeComponent();
            this.DataStreams = new ObservableCollection<DataStream>();
            this.CustomButtons = new ObservableCollection<CustomButton>();
            StateManager.AllStreams = this.DataStreams;
            this.DataStreams.CollectionChanged += (s, e) => {
                foreach (var i in e.NewItems) {
                    var ds = i as DataStream;
                    string toExecute = ds.GetCode();
                    //var toExecute = "object " + ds.Name + "() { return " + ds.Function + "; }";
                    //StateManager.Engine.Execute(toExecute);
                }
            };
            this.DataStreams.Add(new DataStream("CurrentTime", "DateTime.Now"));
            this.DataStreams.Add(new DataStream("StartTime", ""));

            this.CustomButtons.Add(new CustomButton("Reset", "StartTime = CurrentTime"));
        }

        private ObservableCollection<DataStream> _DataStreams;
        public ObservableCollection<DataStream> DataStreams {
            get { return _DataStreams; }
            set {
                _DataStreams = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<CustomButton> _CustomButtons;
        public ObservableCollection<CustomButton> CustomButtons {
            get { return _CustomButtons; }
            set {
                _CustomButtons = value;
                NotifyPropertyChanged("CustomButtons");
            }
        }

        private string _NewStreamFunction;
        public string NewStreamFunction {
            get { return _NewStreamFunction; }
            set {
                _NewStreamFunction = value;
                NotifyPropertyChanged("NewStreamFunction");
            }
        }

        private string _NewStreamName;
        public string NewStreamName {
            get { return _NewStreamName; }
            set {
                _NewStreamName = value;
                NotifyPropertyChanged("NewStreamName");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void AddNewDataStream_Click(object sender, RoutedEventArgs e) {
            var ds = new DataStream(this.NewStreamName, this.NewStreamFunction);
            this.DataStreams.Add(ds);
        }

        private void TestDataStream_Click(object sender, RoutedEventArgs e) {
            var ds = (sender as Button).Tag as DataStream;
            var h = ds.Hit();
            log.DebugFormat(h.ToString());
        }

        

        private void AddButton_Click(object sender, RoutedEventArgs e) {
            this.CustomButtons.Add(new CustomButton(this.NewStreamName, this.NewStreamFunction));
        }

        private void CustomButton_Click(object sender, RoutedEventArgs e) {
            var btn = (sender as Button).Tag as CustomButton;
            btn.Hit();
        }
    }
}
