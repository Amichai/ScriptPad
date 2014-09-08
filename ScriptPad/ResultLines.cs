using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScriptPad {
    public class ResultLines : INotifyPropertyChanged {
        public ResultLines() {
            this.Lines = new ObservableCollection<ResultLine>();
        }
        private ObservableCollection<ResultLine> _Lines;
        public ObservableCollection<ResultLine> Lines {
            get { return _Lines; }
            set {
                _Lines = value;
                NotifyPropertyChanged();
            }
        }

        public void Add(string output, ResultLineType type) {
            this.Lines.Add(new ResultLine(output, type));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public enum ResultLineType { input, output };
}
