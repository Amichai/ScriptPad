using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace StreamProcessor {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));

        public App() {
            XmlConfigurator.Configure();

            log.Info("Application initialized");
        }
    }
}
