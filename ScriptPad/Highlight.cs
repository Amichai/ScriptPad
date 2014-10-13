using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ScriptPad {
    public class Highlight : INotifyPropertyChanged {
        public Highlight() {
            this.Width = 10;
            this.Height = 10;
            this.Left = 10;
        }


        public double Width { get; set; }
        public double Height { get; set; }
        public double Left { get; set; }

        public Rectangle GetRectangle {
            get {
                return new Rectangle() {
                    Width = Width,
                    Height = Height,
                    Fill = Brushes.Yellow,
                    Opacity = .85,
                    Margin = new System.Windows.Thickness(this.Left, 0, 0, 0),
                };
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
    }
}
