using System.IO;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;
using HTMLConverter;

namespace jQueryBuddy.Utilities
{
    public static class HtmlUtilities
    {
        public static FlowDocument CreateXamlFromHtml(string html)
        {
            var flowDocument = new FlowDocument();
            var xaml = HtmlToXamlConverter.ConvertHtmlToXaml(html, true);
            return XamlReader.Load(new XmlTextReader(new StringReader(xaml))) as FlowDocument;
            //using (var msDocument = new MemoryStream((new ASCIIEncoding()).GetBytes(xaml)))
            //{
            //    var textRange = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
            //    textRange.Load(msDocument, DataFormats.Xaml);
            //}
            //return flowDocument;
        }
       
    }
}