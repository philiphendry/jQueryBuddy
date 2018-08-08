using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace jQueryBuddy.ErrorRendering
{
    public class BackgroundHighlightError : DocumentColorizingTransformer, IHighlightError
    {
        private readonly TextEditor _textEditor;
        private int _errorLineNumber;

        public BackgroundHighlightError(TextEditor textEditor)
        {
            _textEditor = textEditor;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            if (line.LineNumber != _errorLineNumber) return;
            ChangeLinePart(line.Offset, line.EndOffset, 
                           element => element.TextRunProperties.SetBackgroundBrush(Brushes.OrangeRed));
        }

        public void HighlightLineNumber(int lineNumber)
        {
            _errorLineNumber = lineNumber;
            _textEditor.TextArea.TextView.LineTransformers.Add(this);
        }

        public void Clear()
        {
            _textEditor.TextArea.TextView.LineTransformers.Remove(this);
        }
    }
}