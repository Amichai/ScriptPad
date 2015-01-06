using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ComponentBuilder {
    public class Component {
        public string Description { get; set; }

        public string Name { get; set; }

        public string Definition { get; set; }

        public Subject<ComponentSignal> Signal { get; set; }
    }

    public class ComponentSignal {
        public string Type { get; set; }
        public string Value { get; set; }
        public string Target { get; set; }
        public string Origin { get; set; }
    }

    public class SignalListener {
        public Func<ComponentSignal, bool> SignalTester { get; set; }
        public Action Action { get; set; }
    }
}
