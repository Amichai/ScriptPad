using ScriptPad;
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

namespace ComponentBuilder {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {
        public MainWindow() {
            InitializeComponent();
            this.engine.LoadDefaultAssembliesAndNamespaces();
            this.Components = new ObservableCollection<Component>();
            ConsoleWriter writer = new ConsoleWriter();
            writer.WriteEvent += (s, e) => {
                this.Output += e.Value + "\n";
            };

            writer.WriteLineEvent += (s, e) => {
                this.Output += e.Value + "\n";
            };

            Console.SetOut(writer);
        }

        private ObservableCollection<Component> _Components;
        public ObservableCollection<Component> Components {
            get { return _Components; }
            set {
                _Components = value;
                NotifyPropertyChanged();
            }
        }

        private NewComponentViewModel _NewComponent = new NewComponentViewModel();
        public NewComponentViewModel NewComponent {
            get { return _NewComponent; }
            set {
                _NewComponent = value;
                NotifyPropertyChanged();
            }
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion INotifyPropertyChanged Implementation

        private void Add_Click(object sender, RoutedEventArgs e) {
            var c = this.NewComponent.ToComponent();
            this.Components.Add(c);
        }

        private ScriptEngine engine = new ScriptEngine();

        private string _Output;
        public string Output {
            get { return _Output; }
            set {
                _Output = value;
                NotifyPropertyChanged();
            }
        }

        private void Trigger_Click(object sender, RoutedEventArgs e) {
            var c = (sender as Button).Tag as Component;
            var result = engine.Execute(c.Definition);
            Debug.Print(result.ToString());   
        }

        ///TODO: Extract the sense of a view for each component
        ///so they can be developed independently
    }
}
