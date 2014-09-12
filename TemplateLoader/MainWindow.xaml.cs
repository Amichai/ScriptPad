using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using TemplateLoader.Elements.IRenderable;

namespace TemplateLoader {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            XElement xml = XElement.Load(@"Template.xml");
            this.loadElements(xml);
        }

        private void loadElements(XElement xml) {
            foreach (var e in xml.Elements()) {
                switch (e.Name.ToString()) {
                    case "PixelFunc":
                        var p = PixelFunc.FromXml(e);
                        p.Render();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
