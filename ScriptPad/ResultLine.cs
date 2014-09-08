using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptPad {
    public class ResultLine {
        public ResultLine(string value, ResultLineType type) {
            this.Value = value;
            this.Type = type;
        }

        public string Value { get; private set; }

        public ResultLineType Type { get; private set; }

        public string Prefix {
            get {
                switch (this.Type) {
                    case ResultLineType.input:
                        return ">>> ";
                    case ResultLineType.output:
                        return "";
                    default:
                        throw new Exception();
                }
            }
        }
    }
}
