﻿using Common.Logging;
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

            this.addUsingStatement("System");
            this.addUsingStatement("System.Collections.Generic");
            this.addUsingStatement("System.Configuration");

            var r = new ScriptCs.PackageReference("Newtonsoft.Json", 
                new FrameworkName(".NET Framework, Version=4.5"),
                "6.0.5");
            
            
            var f = new ScriptHostFactory();
            
            var sessions = new List<IScriptPack>();
            this.session = new ScriptPackSession(sessions, null);
            engine = new ScriptCs.Engine.Roslyn.RoslynScriptEngine(f, 
                log);

            ConsoleWriter writer = new ConsoleWriter();
            writer.WriteEvent += (s, e) => {
                this.addResultLine(e.Value, ResultLineType.output);
            };

            writer.WriteLineEvent += (s, e) => {
                this.addResultLine(e.Value, ResultLineType.output);
            };

            Console.SetOut(writer);

            this.execute("for(int i =0; i< 5; i++) { Console.WriteLine(\"testing\"); }");
            this.addAssembly(@"C:\Users\amichai\Documents\MyProjects\ScriptPad.git\trunk\packages\Rx-Core.2.2.5\lib\net45\System.Reactive.Core.dll");
            this.addAssembly(@"C:\Users\amichai\Documents\MyProjects\ScriptPad.git\trunk\packages\Rx-Interfaces.2.2.5\lib\net45\System.Reactive.Interfaces.dll");
            this.addAssembly(@"C:\Users\amichai\Documents\MyProjects\ScriptPad.git\trunk\packages\Rx-Linq.2.2.5\lib\net45\System.Reactive.Linq.dll");
            this.addAssembly(@"C:\Users\amichai\Documents\MyProjects\ScriptPad.git\trunk\packages\Rx-PlatformServices.2.2.5\lib\net45\System.Reactive.PlatformServices.dll");
            this.execute(@"
System.Reactive.Linq.Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(i => {
    Console.WriteLine(i);
});
            ");
        }

        private List<string> commandStack = new List<string>();
        private int commandStackPointerIndex = 0;

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

        private ObservableCollection<AssemblyViewModel> _Assemblies;
        public ObservableCollection<AssemblyViewModel> Assemblies {
            get { return _Assemblies; }
            set {
                _Assemblies = value;
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
            var r = this.engine.Execute(input, null, refs, namespaces, session);
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
            refs.PathReferences.Add(path);
        }
    }
}
