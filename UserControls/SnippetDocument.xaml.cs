using System;
using System.Diagnostics;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit.Highlighting;
using jQueryBuddy.CodeCompletion;
using jQueryBuddy.ErrorRendering;
using jQueryBuddy.Rendering;

namespace jQueryBuddy.UserControls
{
    /// <summary>
    /// Interaction logic for ActiveTabDocument.xaml
    /// </summary>
    public partial class SnippetDocument : UserControl
    {

        // ReSharper disable UnaccessedField.Local
        private readonly CodeCompletionBase _htmlCompletion;
        private readonly CodeCompletionBase _javascriptCompletion;
        private readonly CodeCompletionBase _css3SelectorCompletion;
        // ReSharper restore UnaccessedField.Local

        private readonly IHighlightError _highlightHtmlError;
        private readonly IHighlightError _highlightJavascriptError;

        public event EventHandler<InstantHelpEventArgs> JavascriptCompletion;
        internal void InvokeJavascriptCompletion(InstantHelpEventArgs e)
        {
            var handler = JavascriptCompletion;
            if (handler != null) handler(this, e);
        }

        public SnippetDocument()
        {
            InitializeComponent();

            try
            {
                _htmlCompletion = new HtmlCodeCompletion(txtHtml);
                _javascriptCompletion = new JQueryCodeCompletion(txtJavascript);
                _javascriptCompletion.PreviewInstantHelp += PreviewJavascriptCompletion;
            }
            catch (Exception e)
            {
                //todo: Need to improve the error handling!!
                Debug.WriteLine(e);
            }

            _highlightJavascriptError = new RedSquigglyError(txtJavascript);
            _highlightHtmlError = new RedSquigglyError(txtHtml);

            txtHtml.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("HTML");
            txtJavascript.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("JavaScript");

        }

        private void PreviewJavascriptCompletion(object sender, InstantHelpEventArgs e)
        {
            InvokeJavascriptCompletion(e);
        }

        public Boolean IsModified
        {
            get { return txtHtml.IsModified || txtJavascript.IsModified; }
            set { txtHtml.IsModified = value; txtJavascript.IsModified = value; }
        }

        public string Html
        {
            get { return txtHtml.Text; }
            set { txtHtml.Text = value; }
        }

        public string Javascript
        {
            get { return txtJavascript.Text; }
            set { txtJavascript.Text = value; }
        }

        public void RenderError(HtmlRenderer.ErrorInfo errorInfo)
        {
            switch (errorInfo.Source)
            {
                case HtmlRenderer.ErrorSources.CustomHtml:
                    _highlightHtmlError.HighlightLineNumber(errorInfo.LineNumber);
                    break;
                case HtmlRenderer.ErrorSources.Javascript:
                    _highlightJavascriptError.HighlightLineNumber(errorInfo.LineNumber);
                    break;
            }
        }

        public void ClearErrors()
        {
            _highlightJavascriptError.Clear();
            _highlightHtmlError.Clear();            
        }

    }
}
