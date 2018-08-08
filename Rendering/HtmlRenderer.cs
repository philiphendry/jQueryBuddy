using System;
using System.Windows.Forms;
using jQueryBuddy.Utilities;
using Panel = System.Windows.Controls.Panel;

namespace jQueryBuddy.Rendering
{
    public class HtmlRenderer
    {
        public enum ErrorSources
        {
            CustomHtml,
            ContainerHtml,
            Javascript
        }

        public class ErrorInfo
        {
            public string Message;
            public int LineNumber;
            public ErrorSources Source;
        }

        public class RenderErrorEventArgs : EventArgs
        {
            public ErrorInfo Error { get; set; }
        }

        private readonly WebBrowser _myBrowser;

        private string _html;
        private string _jquery;

        private readonly string _basePageHtml = ResourceUtilities.GetEmbeddedResourceString("jQueryBuddy.Resources.Templates.ResultsPage.html");
        private readonly string _linqPadStyleSheet = ResourceUtilities.GetEmbeddedResourceString("jQueryBuddy.Resources.Styles.LinqPad.css");
        private readonly string _javascriptDumpScript = ResourceUtilities.GetEmbeddedResourceString("jQueryBuddy.Resources.Templates.JavascriptDump.js");

        public event EventHandler<RenderErrorEventArgs> OnError;

        private void InvokeOnError(RenderErrorEventArgs e)
        {
            var handler = OnError;
            if (handler != null) handler(this, e);
        }

        public HtmlRenderer(Panel container)
        {
            var host = new System.Windows.Forms.Integration.WindowsFormsHost();
            _myBrowser = new WebBrowser { ScriptErrorsSuppressed = true };
            _myBrowser.DocumentCompleted += MyBrowserDocumentCompleted;
            host.Child = _myBrowser;
            container.Children.Add(host);
        }

        void MyBrowserDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var errorInfo = GetErrorInfo();
            if (errorInfo != null)
            {
                InvokeOnError(new RenderErrorEventArgs { Error = errorInfo });
            }
        }

        private ErrorInfo GetErrorInfo()
        {
            var errorMessage = String.Empty;
            var lineNumber = 0;

            if (_myBrowser.Document == null) return null;

            var errorMessageElement = _myBrowser.Document.GetElementById("divErrorMessage");
            if (errorMessageElement != null)
                errorMessage = errorMessageElement.InnerText;

            var errorLineNumberElement = _myBrowser.Document.GetElementById("divErrorLineNumber");
            if (errorLineNumberElement != null)
                Int32.TryParse(errorLineNumberElement.InnerText, out lineNumber);

            if (String.IsNullOrEmpty(errorMessage) || errorMessage == "none")
                return null;

            return GetSource(new ErrorInfo { Message = errorMessage, LineNumber = lineNumber });
        }

        private ErrorInfo GetSource(ErrorInfo erroInfo)
        {
            var lineNumber = erroInfo.LineNumber;
            // TODO: Got to think of a better way to count the leading lines
            var leadingLines = _linqPadStyleSheet.CountOfLines() + _javascriptDumpScript.CountOfLines() - 2;
            var jsStartLineNumber = _basePageHtml.GetLineNumberContaining("{{Javascript}}") + leadingLines;
            var htmlStartLineNumber = _basePageHtml.GetLineNumberContaining("{{Html}}") + leadingLines;

            var jsSize = _jquery.CountOfLines();
            var htmlSize = _jquery.CountOfLines();

            if (lineNumber >= jsStartLineNumber && lineNumber <= jsStartLineNumber + jsSize)
                return new ErrorInfo {Message = erroInfo.Message, Source = ErrorSources.Javascript, LineNumber = lineNumber - jsStartLineNumber};

            if (lineNumber >= htmlStartLineNumber + jsSize && lineNumber < htmlStartLineNumber + htmlSize + jsSize)
                return new ErrorInfo {Message = erroInfo.Message, Source = ErrorSources.CustomHtml, LineNumber = lineNumber - htmlStartLineNumber - jsSize + 1};

            return new ErrorInfo {Message = erroInfo.Message, Source = ErrorSources.ContainerHtml, LineNumber = lineNumber};
        }

        public void Render(string jquery, string html, References references)
        {
            _jquery = jquery;
            _html = html;
            var documentText = _basePageHtml.AsTemplated(new {
                     References = references.GetHtml(),
                     Styles = _linqPadStyleSheet,
                     JavascriptDump = _javascriptDumpScript,
                     Javascript = _jquery, 
                     Html = _html
                 });

            //_myBrowser.DocumentText = documentText;
            App.AddPage("test.aspx", documentText);
            _myBrowser.Navigate(@"http://localhost:8085/test.aspx");
            _myBrowser.Refresh(WebBrowserRefreshOption.Completely);
        }
    }
}