using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using jQueryBuddy.Utilities;

namespace jQueryBuddy.ErrorRendering
{
    public class RedSquigglyError : IBackgroundRenderer, IHighlightError
    {
        private readonly TextEditor _textEditor;
        private int _errorLineNumber;

        public RedSquigglyError(TextEditor textEditor)
        {
            _textEditor = textEditor;
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (textView == null)
                throw new ArgumentNullException("textView");
            if (drawingContext == null)
                throw new ArgumentNullException("drawingContext");

            var visualLines = textView.VisualLines;
            if (visualLines.Count == 0)
                return;
            if (_errorLineNumber < 0 || _errorLineNumber > textView.Document.LineCount) 
                return;

            var errorLine = textView.Document.GetLineByNumber(_errorLineNumber);
            var lineText = textView.Document.GetText(errorLine.Offset, errorLine.Length);
            var segment = new TextSegment {StartOffset = errorLine.Offset + lineText.IndexOfFirstCharAfter(" \t"), EndOffset = errorLine.EndOffset};

            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, segment))
            {
                var startPoint = rect.BottomLeft;
                var endPoint = rect.BottomRight;
                var usedPen = new Pen(Brushes.Red, 1);
                usedPen.Freeze();

                const double offset = 2.5;
                var count = Math.Max((int)((endPoint.X - startPoint.X) / offset) + 1, 4);
                var geometry = new StreamGeometry();
                using (StreamGeometryContext ctx = geometry.Open())
                {
                    ctx.BeginFigure(startPoint, false, false);
                    ctx.PolyLineTo(CreatePoints(startPoint, offset, count).ToArray(), true, false);
                }
                geometry.Freeze();
                drawingContext.DrawGeometry(Brushes.Transparent, usedPen, geometry);
            }
        }

        static IEnumerable<Point> CreatePoints(Point start, double offset, int count)
        {
            for (var i = 0; i < count; i++)
                yield return new Point(start.X + i * offset, start.Y - ((i + 1) % 2 == 0 ? offset : 0));
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Selection; }
        }

        public void HighlightLineNumber(int lineNumber)
        {
            _errorLineNumber = lineNumber;
            _textEditor.TextArea.TextView.BackgroundRenderers.Add(this);
        }

        public void Clear()
        {
            _textEditor.TextArea.TextView.BackgroundRenderers.Remove(this);
        }
    }
}