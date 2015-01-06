using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ComponentBuilder {
    public class NewComponentViewModel: INotifyPropertyChanged {

        private string _Name;
        public string Name {
            get { return _Name; }
            set {
                _Name = value;
                NotifyPropertyChanged();
            }
        }

        private string _Description;
        public string Description {
            get { return _Description; }
            set {
                _Description = value;
                NotifyPropertyChanged();
            }
        }

        private string _Definition;
        public string Definition {
            get { return _Definition; }
            set {
                _Definition = value;
                NotifyPropertyChanged();
            }
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion INotifyPropertyChanged Implementation

        internal Component ToComponent() {
            return new Component() {
                Name = this.Name,
                Description = this.Description,
                Definition = this.Definition,
            };
        }
    }
}
