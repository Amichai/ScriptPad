using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptPad {
    public class AssemblyViewModel {
        public AssemblyViewModel(string path) {
            this.Path = path;
        }
        public string Path { get; private set; }
        public string Name {
            get {
                return new System.IO.FileInfo(this.Path).Name;
            }
        }
    }
}
