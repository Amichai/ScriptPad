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
            this.Lines.CollectionChanged += Lines_CollectionChanged;
        }

        void Lines_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add || e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove) {
                NotifyPropertyChanged("LineStrings");
            }
        }


        public List<LogLine> LineStrings {
            get {
                return this.Lines.Select((i, j) => new LogLine(i.ToString(), j)).ToList();
            }
        }

        private ObservableCollection<ResultLine> _Lines;
        public ObservableCollection<ResultLine> Lines {
            get { return _Lines; }
            set {
                _Lines = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("LineStrings");
            }
        }

        public void Add(string output, ResultLineType type) {
            App.Current.Dispatcher.Invoke((Action)(() => {
                this.Lines.Add(new ResultLine(output, type));
            }));
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
