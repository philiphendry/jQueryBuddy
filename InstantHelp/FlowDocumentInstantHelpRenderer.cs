using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using jQueryBuddy.CodeCompletion;
using jQueryBuddy.Properties;
using jQueryBuddy.Utilities;

namespace jQueryBuddy.InstantHelp
{
    public class FlowDocumentInstantHelpRenderer : IInstantHelpRenderer
    {
        private readonly FlowDocumentReader _flowDocumentReader;

        public FlowDocumentInstantHelpRenderer()
        {
            _flowDocumentReader = new FlowDocumentReader();
        }

        public UIElement RenderHelp(string htmlHelp, IList<InstantHelpEventArgs.HtmlExample> htmlExamples)
        {
            _flowDocumentReader.Document = HtmlUtilities.CreateXamlFromHtml(htmlHelp);
            return _flowDocumentReader;
        }

        public void LoadPreferences(Settings settings)
        {
            _flowDocumentReader.ViewingMode = settings.InstantHelpViewingMode;
            _flowDocumentReader.Zoom = settings.InstantHelpZoom;
        }

        public void SavePreferences(Settings settings)
        {
            settings.InstantHelpViewingMode = _flowDocumentReader.ViewingMode;
            settings.InstantHelpZoom = _flowDocumentReader.Zoom;
        }
    }
}