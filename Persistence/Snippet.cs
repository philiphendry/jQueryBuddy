namespace jQueryBuddy.Persistence
{
    public class Snippet
    {
        public Snippet()
        {
        }
        
        public Snippet(string html, string javascript)
        {
            Html = html;
            Javascript = javascript;
        }

        public string Html { get; set; }
        public string Javascript { get; set; }
    }
}