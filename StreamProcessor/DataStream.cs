using ScriptCs.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamProcessor {
    public class DataStream {
        public DataStream(string name, string func) {
            this.ID = idCounter++;
            this.Name = name;
            this.Function = func;
        }

        private static int idCounter = 0;
        public string Name { get; set; }
        public int ID { get; set; }
        public string Function { get; set; }

        internal object Hit() {
            return (StateManager.Engine.Execute(this.Function) as ScriptResult).ReturnValue;
        }

        internal string GetCode() {
            return "";
        }
    }
}
