using Common.Logging;
using ScriptCs;
using ScriptCs.Contracts;
using ScriptCs.Engine.Roslyn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamProcessor {
    public class ExecutionEngine {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ExecutionEngine() {
            var f = new ScriptHostFactory();
            this.setupReferencesAndNamespaces();
            var sessions = new List<IScriptPack>();
            this.session = new ScriptPackSession(sessions, null);

            this.engine = new RoslynScriptEngine(f, log);
        }

        private void setupReferencesAndNamespaces() {
            this.namespaces.Add("System");
            this.namespaces.Add("System.Collections.Generic");
            this.namespaces.Add("System.Configuration");
        }

        private RoslynScriptEngine engine;
        private ScriptPackSession session;
        private AssemblyReferences refs = new AssemblyReferences();
        private List<string> namespaces = new List<string>();


        public T Eval<T>(string input) {
            return (T)this.Execute(input).ReturnValue;
        }

        public ScriptResult Execute(string input) {
            var r = this.engine.Execute(input, null, refs, namespaces, session);
            return r;
        }
    }
}
