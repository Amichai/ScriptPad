using Common.Logging;
using ScriptCs;
using ScriptCs.Contracts;
using System;
using System.Collections.Generic;
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
            InitializeComponent();
            var f = new ScriptHostFactory();
            var sessions = new List<IScriptPack>();
            this.session = new ScriptPackSession(sessions, null);
            engine = new ScriptCs.Engine.Roslyn.RoslynScriptEngine(f, 
                log);

            ConsoleWriter writer = new ConsoleWriter();
            writer.WriteEvent += (s, e) => {

                this.OutputText += e.Value;

            };

            writer.WriteLineEvent += (s, e) => {

                this.OutputText += e.Value + "\n";
            };

            Console.SetOut(writer);
        }

        ScriptCs.Engine.Roslyn.RoslynScriptEngine engine;
        ScriptPackSession session;
        private List<string> namespaces = new List<string>() {
            "System",
            "System.Collections.Generic",
            "System.Configuration",
        };
            //"System.Linq",
            //"System.Reflection",
            //"System.Threading.Tasks",
            //"System.Windows",

        private string _OutputText;
        public string OutputText {
            get { return _OutputText; }
            set {
                _OutputText = value;
                NotifyPropertyChanged();
            }
        }

        AssemblyReferences refs = new AssemblyReferences();

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.F5) {
                var r = this.engine.Execute(this.inputText.Text, null, refs, namespaces, session);
                if (r.ReturnValue != null) {
                    OutputText += r.ReturnValue.ToString() + "\n";
                }
                if (r.CompileExceptionInfo != null  && r.CompileExceptionInfo.SourceException != null) {
                    OutputText += r.CompileExceptionInfo.SourceException.Message + "\n";
                }
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
    }
}
