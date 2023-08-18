namespace WebApiAuthors.DTOs
{
    public class HATEOASdata
    {
        public string Link { get; private set; }
        public string Description { get; private set; }
        public string Method { get; private set; }

        public HATEOASdata(string link, string description, string method)
        {
            Link = link;
            Description = description;
            Method = method;
        }
    }
}
