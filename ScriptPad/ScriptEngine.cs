using Common.Logging;
using ScriptCs;
using ScriptCs.Contracts;
using ScriptCs.Engine.Roslyn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ScriptPad {
    public class ScriptEngine {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<string> namespaces = new List<string>();
        private RoslynScriptEngine engine;
        private ScriptPackSession session;
        public ScriptEngine() {
            var sessions = new List<IScriptPack>();
            var f = new ScriptHostFactory();
            this.session = new ScriptPackSession(sessions, null);
            engine = new ScriptCs.Engine.Roslyn.RoslynScriptEngine(f,
                log);
        }

        private AssemblyReferences refs = new AssemblyReferences();

        public ScriptResult Execute(string input) {
            var r = this.engine.Execute(input, null, refs, namespaces, session);
            return r;
        }

        public void AddNamespace(string s) {
            this.namespaces.Add(s);
        }

        public void AddAssembly(string path) {
            refs.PathReferences.Add(path);
        }

        public void LoadDefaultAssembliesAndNamespaces() {
            XElement xml = XElement.Parse(defaultState);
            foreach (var u in xml.Element("UsingStatements").Elements()) {
                this.AddNamespace(u.Value);
            }
            foreach (var u in xml.Element("Assemblies").Elements()) {
                this.AddAssembly(u.Value);
            }
        }


        string defaultState = @"
<State>
  <UsingStatements>
    <Using>System</Using>
    <Using>System.Collections.Generic</Using>
    <Using>System.Configuration</Using>
  </UsingStatements>
  <Assemblies>
    <Assembly>C:\Users\amichai\Documents\MyProjects\ScriptPad.git\trunk\packages\Rx-Core.2.2.5\lib\net45\System.Reactive.Core.dll</Assembly>
    <Assembly>C:\Users\amichai\Documents\MyProjects\ScriptPad.git\trunk\packages\Rx-Interfaces.2.2.5\lib\net45\System.Reactive.Interfaces.dll</Assembly>
    <Assembly>C:\Users\amichai\Documents\MyProjects\ScriptPad.git\trunk\packages\Rx-Linq.2.2.5\lib\net45\System.Reactive.Linq.dll</Assembly>
    <Assembly>C:\Users\amichai\Documents\MyProjects\ScriptPad.git\trunk\packages\Rx-PlatformServices.2.2.5\lib\net45\System.Reactive.PlatformServices.dll</Assembly>
  </Assemblies>
</State>
";
    }
}
