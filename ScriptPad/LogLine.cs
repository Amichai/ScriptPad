using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ScriptPad {
    public class LogLine : INotifyPropertyChanged {
        public LogLine(string val, int lineNumber) {
            this.Value = val;
            this.HighlightedText = "";
            this.LineNumber = lineNumber;
            this.Highlights = new ObservableCollection<Highlight>();
        }
        public string Value { get; set; }
        public string HighlightedText { get; private set; }

        private int _LineNumber;
        public int LineNumber {
            get { return _LineNumber; }
            set {
                _LineNumber = value;
                NotifyPropertyChanged("LineNumber");
            }
        }

        public override string ToString() {
            return Value;
        }

        private Size MeasureString(string candidate) {
            int trailingWhiteSpaces = countTrailingWhitespaces(candidate);

            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Courier"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                12,
                Brushes.Black);

            return new Size(formattedText.Width + trailingWhiteSpaces * spaceWidth, formattedText.Height);
        }


        private ObservableCollection<Highlight> _Highlights;
        public ObservableCollection<Highlight> Highlights {
            get { return _Highlights; }
            set {
                _Highlights = value;
                NotifyPropertyChanged();
            }
        }
        private double spaceWidth = 3.286666666666667;

        private int countTrailingWhitespaces(string val) {
            int count = 0;
            for (int i = val.Length - 1; i >= 0; i--) {
                if (char.IsWhiteSpace(val[i])) {
                    count++;
                } else {
                    return count;
                }
            }
            return count;
        }


        private Highlight getHighlight(string prefix, string val, ref double offset) {
            var p = this.MeasureString(prefix);
            var s = this.MeasureString(val);
            var width = s.Width;
            var height = s.Height;
            var padding = p.Width;
            
            var h = new Highlight() {
                Left = padding - offset,
                Height = height, 
                Width = width
            };
            offset = padding + width;
            return h;
        }

        public void ClearAllHighlights() {
            Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() => {
                this.Highlights.Clear();
            }));
        }

        public bool Highlight(string toHighlight) {
            try {
                double offset = 0;
                int startIdx = this.Value.IndexOf(toHighlight);
                var prefix = string.Concat(this.Value.Take(startIdx));
                var h = getHighlight(prefix, toHighlight, ref offset);
                Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() => {
                    this.Highlights.Add(h);
                }));
                return true;
            } catch (Exception ex) {
                return false;
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
