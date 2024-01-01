namespace MangaLibra_Scrape_API.Models
{
    public class MangaDataModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Chapters { get; set; }
        public string? ImageLink { get; set; }
        public string? MangaLink { get; set; }
        public string? MangaId { get; set;}
        public List<ChapterDataModel>? ChapterList { get; set; }
    }
}
