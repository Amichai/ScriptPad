using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamProcessor {
    static class StateManager {
        public static ObservableCollection<DataStream> AllStreams = null;
        public static ExecutionEngine Engine = new ExecutionEngine();
    }
}
