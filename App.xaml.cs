using System;
using System.IO;
using System.Reflection;
using System.Windows;
using jQueryBuddy.Rendering;
using jQueryBuddy.Utilities;

namespace jQueryBuddy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static TestWebServer _webServer;
        private const int WebServerPort = 8085;
        private const string WebServerVDir = "/";

        public App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Loader.FindAssemblies;

            _webServer = new TestWebServer(WebServerPort, WebServerVDir);
            _webServer.Start();
            _webServer.ExtractResource("jQueryBuddy.Resources.WebServer.Web.Config", "web.config");
            _webServer.ExtractResource("jQueryBuddy.Resources.Templates.InstantHelpStyles.css", "InstantHelpStyles.css");
            _webServer.ExtractResource("jQueryBuddy.Resources.Templates.JavascriptDump.js", "JavascriptDump.js");
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            jQueryBuddy.Properties.Settings.Default.Save();
            _webServer.Stop();
        }

        public static void AddPage(string name, string content)
        {
            _webServer.WritePage(name, content);
        }
    }
}
