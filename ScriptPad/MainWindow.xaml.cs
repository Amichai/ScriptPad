using Common.Logging;
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

            this.addUsingStatement("System");
            this.addUsingStatement("System.Collections.Generic");
            this.addUsingStatement("System.Configuration");


            var f = new ScriptHostFactory();
            var sessions = new List<IScriptPack>();
            this.session = new ScriptPackSession(sessions, null);
            engine = new ScriptCs.Engine.Roslyn.RoslynScriptEngine(f, 
                log);

            ConsoleWriter writer = new ConsoleWriter();
            writer.WriteEvent += (s, e) => {
                this.ResultLines.Add(e.Value, ResultLineType.output);
            };

            writer.WriteLineEvent += (s, e) => {
                this.ResultLines.Add(e.Value, ResultLineType.output);
            };

            Console.SetOut(writer);

            this.execute("for(int i =0; i< 5; i++) { Console.WriteLine(\"testing\"); }");
        }

        private bool addUsingStatement(string s) {
            this.UsingStatements.Add(s);
            this.namespaces.Add(s);
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

        private RoslynScriptEngine engine;
        private ScriptPackSession session;
        private List<string> namespaces = new List<string>();

        private ResultLines _ResultLines;
        public ResultLines ResultLines {
            get { return _ResultLines; }
            set {
                _ResultLines = value;
                NotifyPropertyChanged();
            }
        }

        private AssemblyReferences refs = new AssemblyReferences();

        private string input {
            get {
                return this.inputText.Text;
            }
        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.F5) {
                execute(this.input);
            }
        }

        private void execute(string input) {
            this.ResultLines.Add(input, ResultLineType.input);
            var r = this.engine.Execute(input, null, refs, namespaces, session);
            if (r.ReturnValue != null) {
                this.ResultLines.Add(r.ReturnValue.ToString(), ResultLineType.output);
            }
            if (r.CompileExceptionInfo != null && r.CompileExceptionInfo.SourceException != null) {
                this.ResultLines.Add(r.CompileExceptionInfo.SourceException.Message, ResultLineType.output);
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
    }
}
