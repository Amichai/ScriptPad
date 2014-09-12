using SimulationLib;
using StreamProcessor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace TemplateLoader.Elements.IRenderable {
    class PixelFunc : IRenderable, IAmNamed {
        private ExecutionEngine engine = new ExecutionEngine();
        public PixelFunc() {
            var r = engine.Execute(string.Format("var DIM = {0};", this.Width));


        }
        private void init() {
            data = new DoubleArrayColor();
            data.ArrayWidth = this.Width;
            data.ArrayHeight = this.Height;
            data.XMin = 0;
            data.YMin = 0;
            data.XMax = this.Width;
            data.YMax = this.Height;
            this.data.ClearAndInitialize();
        }

        private DoubleArrayColor data;

        public UIElement Render() {
            for (int i = 0; i < Width; i++) {
                engine.Execute(string.Format("var i = {0};", i));
                for (int j = 0; j < Height; j++) {
                    engine.Execute(string.Format("var j = {0};", j));
                    byte r, g, b;
                    r = (byte)engine.Eval<int>(R);
                    g = (byte)engine.Eval<int>(G);
                    b = (byte)engine.Eval<int>(B);
                    Color c = Color.FromRgb(r, g, b);
                    data.PixelSet(new Vector(i, j), c);
                }
            }
            var path = "testing.png";
            data.Draw();
            data.Save(path);
            //Process.Start(path);
            return null;

        }
        public string Name { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public string R { get; private set; }
        public string G { get; private set; }
        public string B { get; private set; }

        internal static PixelFunc FromXml(XElement e) {
            PixelFunc toReturn = new PixelFunc();
            toReturn.Name = e.Attribute("Name").Value;
            toReturn.Width = int.Parse(e.Attribute("Width").Value);
            toReturn.Height = int.Parse(e.Attribute("Height").Value);
            toReturn.R = e.Attribute("R").Value;
            toReturn.G = e.Attribute("G").Value;
            toReturn.B = e.Attribute("B").Value;
            toReturn.init();
            return toReturn;
        }
    }
}
