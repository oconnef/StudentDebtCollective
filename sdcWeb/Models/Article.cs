namespace sdcWeb.Models
{
    public class Article
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublicationDate { get; set; }
        public string Author { get; set; }
        public List<string> MetaTags { get; set; }  
    }
}
