using System.Reflection;
using System.IO;

namespace jQueryBuddy.Utilities
{
    public class ResourceUtilities
    {
        public static string GetEmbeddedResourceString(string resourceName)
        {
            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                 using (var reader = new StreamReader(s))
                 {
                     return reader.ReadToEnd();
                 }
            }
        }
    }
}