using Common.Logging;
using Microsoft.Win32;
using ScriptCs;
using ScriptCs.Contracts;
using ScriptCs.Engine.Roslyn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
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
using System.Xml.Linq;

namespace ScriptPad {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MainWindow() {
            this.ResultLines = new ScriptPad.ResultLines();
            InitializeComponent();
            this.UsingStatements = new ObservableCollection<string>();
            this.Assemblies = new ObservableCollection<AssemblyViewModel>();

            ConsoleWriter writer = new ConsoleWriter();
            writer.WriteEvent += (s, e) => {
                this.addResultLine(e.Value, ResultLineType.output);
            };

            writer.WriteLineEvent += (s, e) => {
                this.addResultLine(e.Value, ResultLineType.output);
            };

            Console.SetOut(writer);

            this.fromXml(@"..\..\PadState.xml");
            
        }

        private void fromXml(string path) {
            XElement xml = XElement.Load(path);
            foreach (var u in xml.Element("UsingStatements").Elements()) {
                this.addUsingStatement(u.Value);
            }
            foreach (var u in xml.Element("Assemblies").Elements()) {
                this.addAssembly(u.Value);
            }
            foreach (var u in xml.Element("ToExecute").Elements()) {
                this.execute(u.Value);
            }
        }

        private List<string> commandStack = new List<string>();
        private int commandStackPointerIndex = 0;

        private bool addUsingStatement(string s) {
            this.UsingStatements.Add(s);
            this.scriptEngine.AddNamespace(s);
            return true;
        }

        private ObservableCollection<string> _UsingStatements;
        public ObservableCollection<string> UsingStatements {
            get { return _UsingStatements; }
            set {
                _UsingStatements = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<AssemblyViewModel> _Assemblies;
        public ObservableCollection<AssemblyViewModel> Assemblies {
            get { return _Assemblies; }
            set {
                _Assemblies = value;
                NotifyPropertyChanged();
            }
        }

        private ScriptEngine scriptEngine = new ScriptEngine();

        private ResultLines _ResultLines;
        public ResultLines ResultLines {
            get { return _ResultLines; }
            set {
                _ResultLines = value;
                NotifyPropertyChanged();
            }
        }


        private string input {
            get {
                return this.inputText.Text;
            }
        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.F5:
                    this.execute(this.input);
                    break;
                case Key.Down:
                    if (this.commandStackPointerIndex < 0) {
                        return;
                    }
                    this.commandStackPointerIndex--;
                    if (this.commandStackPointerIndex >= 0 && this.commandStackPointerIndex < commandStack.Count()) {
                        this.inputText.Text = commandStack[commandStackPointerIndex];
                    }
                    break;
                case Key.Up:
                    if (this.commandStackPointerIndex >= this.commandStack.Count()) {
                        return;
                    }
                    this.commandStackPointerIndex++;
                    if (this.commandStackPointerIndex >= 0 && this.commandStackPointerIndex < commandStack.Count()) {
                        this.inputText.Text = commandStack[commandStackPointerIndex];
                    }
                    break;
            }
        }

        private void execute(string input) {
            this.ResultLines.Add(input, ResultLineType.input);
            this.commandStack.Add(input);
            this.commandStackPointerIndex = this.commandStack.Count() - 1;
            //var r = this.engine.Execute(input, null, refs, namespaces, session);
            var r = this.scriptEngine.Execute(input);
            if (r.ReturnValue != null) {
                this.addResultLine(r.ReturnValue.ToString(), ResultLineType.output);
            }
            if (r.CompileExceptionInfo != null && r.CompileExceptionInfo.SourceException != null) {
                this.addResultLine(r.CompileExceptionInfo.SourceException.Message, ResultLineType.output);
            }
        }

        private void addResultLine(string val, ResultLineType type) {
            this.ResultLines.Add(val, type);
            if (replScrollViewer.VerticalOffset == replScrollViewer.ScrollableHeight) {
                App.Current.Dispatcher.Invoke((Action)(() => {
                    this.replScrollViewer.ScrollToBottom();
                }));
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

        private string _NewUsingStatement;
        public string NewUsingStatement {
            get { return _NewUsingStatement; }
            set {
                _NewUsingStatement = value;
                NotifyPropertyChanged();
            }
        }

        private void AddUsingStatement_Click(object sender, RoutedEventArgs e) {
            this.addUsingStatement(this.NewUsingStatement);
        }

        private void Browse_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            var path = ofd.FileName;
            addAssembly(path);
        }

        private void addAssembly(string path) {
            this.Assemblies.Add(new AssemblyViewModel(path));
        }
    }
}
