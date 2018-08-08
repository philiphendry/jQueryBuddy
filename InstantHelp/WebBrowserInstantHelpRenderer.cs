using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using jQueryBuddy.CodeCompletion;
using jQueryBuddy.Properties;
using jQueryBuddy.Utilities;
using WebBrowser = System.Windows.Forms.WebBrowser;

namespace jQueryBuddy.InstantHelp
{
    public class WebBrowserInstantHelpRenderer : IInstantHelpRenderer
    {
        private readonly string _stylesForTemplates = ResourceUtilities.GetEmbeddedResourceString("jQueryBuddy.Resources.Templates.InstantHelpStyles.css");
        private readonly string _exampleHtmlTemplate = ResourceUtilities.GetEmbeddedResourceString("jQueryBuddy.Resources.Templates.InstantHelpjQueryExample.html");
        private readonly string _instantHelpHtmlTemplate = ResourceUtilities.GetEmbeddedResourceString("jQueryBuddy.Resources.Templates.InstantHelpPage.html");

        private const string ScriptReferenceHtmlTemplate = @"<script type='text/javascript' src='{{Filename}}'></script>";

        public UIElement RenderHelp(string htmlHelp, IList<InstantHelpEventArgs.HtmlExample> htmlExamples)
        {
            var tabs = new TabControl();

            var tab = new TabItem { Header = "Help" };
            var host = new System.Windows.Forms.Integration.WindowsFormsHost();
            var myBrowser = new WebBrowser { 
                ScriptErrorsSuppressed = true, 
                DocumentText = _instantHelpHtmlTemplate.AsTemplated(new { Styles = _stylesForTemplates, Body = htmlHelp }) 
            };
            host.Child = myBrowser;
            tab.Content = host;
            tabs.Items.Add(tab);

            foreach (var example in htmlExamples)
            {
                tab = new TabItem {Header = "Example"};
                tabs.Items.Add(tab);
                host = new System.Windows.Forms.Integration.WindowsFormsHost();
                myBrowser = new WebBrowser { ScriptErrorsSuppressed = true, DocumentText = GetExampleHtml(example) };
                host.Child = myBrowser;
                tab.Content = host;
            }

            return tabs;
        }

        private string GetExampleHtml(InstantHelpEventArgs.HtmlExample example)
        {
            return _exampleHtmlTemplate.AsTemplated(new{ 
                                                           DefaultStyles = _stylesForTemplates,
                                                           References = String.Join("\n", example.JavascriptReferences.Select(r => ScriptReferenceHtmlTemplate.AsTemplated(new { Filename = r})).ToArray()),
                                                           ExampleStyles = example.Css,
                                                           ExampleCode = example.Code,
                                                           ExampleDescription = example.Description,
                                                           ExampleCodeText = example.Code.EncodeHtml(),
                                                           ExampleHtmlText = example.Html.EncodeHtml(),
                                                           ExampleHtml = example.Html,
                                                       });
        }

        public void LoadPreferences(Settings settings)
        {
            //throw new NotImplementedException();
        }

        public void SavePreferences(Settings settings)
        {
            //throw new NotImplementedException();
        }
    }
}