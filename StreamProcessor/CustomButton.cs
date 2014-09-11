using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamProcessor {
    public class CustomButton {
        public CustomButton(string name, string effect) {
            this.Name = name;
            this.Effect = effect;
        }
        public string Name { get; set; }
        public string Effect { get; set; }

        internal void Hit() {
            StateManager.Engine.Execute(Effect);
        }
    }
}
