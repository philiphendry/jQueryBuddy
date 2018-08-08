using System.Globalization;
using System.Linq;
using System.Text;

namespace jQueryBuddy.Utilities
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns the zero-based line number where <paramref name="source"/>
        /// appears in <paramref name="target"/>. Lines are counted wherever \n appears.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int GetLineNumberContaining(this string target, string source)
        {
            if (string.IsNullOrEmpty(target)) return -1;
            if (string.IsNullOrEmpty(source)) return -1;

            var position = target.IndexOf(source);
            if (position == -1) return -1;

            var lineCount = 0;
            for (var i = 0; i < position; i++)
            {
                if (target[i] == '\n') lineCount++;
            }
            return lineCount;
        }

        /// <summary>
        /// Returns the number of lines appearing in <paramref name="target"/>
        /// where a line is counted as a '\n'
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int CountOfLines(this string target)
        {
            if (string.IsNullOrEmpty(target)) return 0;
            return target.Count(t => t == '\n') + 1;            
        }

        public static int IndexOfFirstCharAfter(this string target, string source)
        {
            if (string.IsNullOrEmpty(target)) return 0;
            if (string.IsNullOrEmpty(source)) return 0;

            for (var i = 0; i < target.Length; i++)
            {
                if (!source.Contains(target[i])) 
                    return i;
            }
            return target.Length;
        }

        public static string DefaultIfNullOrEmpty(this string target, string defaultValue)
        {
            return string.IsNullOrEmpty(target) ? defaultValue : target;
        }

        public static bool ContainedWithin(this string target, string source)
        {
            return source.Contains(target);
        }

        public static bool NotContainedWithin(this string target, string source)
        {
            return !ContainedWithin(target, source);
        }

        /// <summary>
        /// HTML-encodes a string and returns the encoded string.
        /// </summary>
        /// <param name="html">The html string to encode. </param>
        /// <returns>The HTML-encoded text.</returns>
        public static string EncodeHtml(this string html)
        {
            if (html == null)
                return null;

            var sb = new StringBuilder(html.Length);

            var len = html.Length;
            for (var i = 0; i < len; i++)
            {
                switch (html[i])
                {

                    case '<':
                        sb.Append("&lt;");
                        break;
                    case '>':
                        sb.Append("&gt;");
                        break;
                    case '"':
                        sb.Append("&quot;");
                        break;
                    case '&':
                        sb.Append("&amp;");
                        break;
                    default:
                        if (html[i] > 159)
                        {
                            // decimal numeric entity
                            sb.Append("&#");
                            sb.Append(((int)html[i]).ToString(CultureInfo.InvariantCulture));
                            sb.Append(";");
                        }
                        else
                            sb.Append(html[i]);
                        break;
                }
            }
            return sb.ToString();
        }

        public static string AsTemplated(this string target, object replacements)
        {
            var properties = ReflectionUtilities.GetProperties(replacements);
            foreach (var property in properties)
            {
                target = target.Replace("{{" + property.Name + "}}", property.Value.ToString());
            }
            return target;
        }
    }
}