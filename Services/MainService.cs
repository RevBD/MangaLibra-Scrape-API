using HtmlAgilityPack;
using MangaLibra_Scrape_API.Models;
using System.Net;

namespace MangaLibra_Scrape_API.Services
{
    public class MainService
	{
        public async Task<List<MangaDataModel>> GetNewestMangaList(int pages)
        {
            List<MangaDataModel> result = new List<MangaDataModel>();
            using (var client = new WebClient())
            {
                try
                {
                    for (int i = 0; i < pages; i++)
                    {
                        Uri url = new Uri($"https://manganato.com/genre-all/{i+1}");
                        string html = await client.DownloadStringTaskAsync(url);

                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(html);

                        List<HtmlNode> MangaNodes = doc.DocumentNode.SelectNodes("//div[@class='content-genres-item']").ToList();

                        foreach (HtmlNode Manga in MangaNodes)
                        {
                            MangaDataModel model = new MangaDataModel();
                            // Name //
                            try { model.Name = Manga.SelectSingleNode(".//img[@class='img-loading']").Attributes["alt"].Value;}
                            catch { model.Name = "Unknown"; }
                            // Image //
                            try { model.ImageLink = Manga.SelectSingleNode(".//img[@class='img-loading']").Attributes["src"].Value; }
                            catch { model.ImageLink = "https://user-images.githubusercontent.com/24848110/33519396-7e56363c-d79d-11e7-969b-09782f5ccbab.png"; }
                            // Description //
                            try { model.Description = Manga.SelectSingleNode(".//div[@class='genres-item-description']").InnerText.ToString().Trim().Replace("&#39;", "'"); }
                            catch { model.Description = "Unknown"; }
                            // Chapters //
                            try { model.Chapters = Manga.SelectSingleNode(".//a[starts-with(@class, 'genres-item-chap')]").InnerText; }
                            catch { model.Chapters = "Unknown"; }
                            // Link //
                            try { model.MangaLink = Manga.SelectSingleNode(".//a[starts-with(@class, 'genres-item-img')]").Attributes["href"].Value; }
                            catch { model.MangaLink = "Unknown"; }
                            // ID //
                            try { model.MangaId = model.MangaLink.Split("-")[1]; }
                            catch { model.MangaLink = "Unknown"; }
                            model.ChapterList = new List<ChapterDataModel>();

                            result.Add(model);
                        }
                    }
                }
                catch
                {
                    return new List<MangaDataModel>();
                }
            }
            return result;
        }

        public async Task<MangaDataModel> GetMangaByID(string id)
        {
            using (var client = new WebClient())
            {
                try
                {
                    Uri url = new Uri($"https://chapmanganato.com/manga-{id}");
                    string html = await client.DownloadStringTaskAsync(url);

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    MangaDataModel model = new MangaDataModel();
                    // Name //
                    try { model.Name = doc.DocumentNode.SelectSingleNode(".//h1").InnerText; }
                    catch { model.Name = "Unknown"; }
                    // Image //
                    try { model.ImageLink = doc.DocumentNode.SelectNodes(".//div[@class='panel-story-info']")[0].SelectSingleNode(".//img[@class='img-loading']").Attributes["src"].Value; }
                    catch { model.ImageLink = "https://user-images.githubusercontent.com/24848110/33519396-7e56363c-d79d-11e7-969b-09782f5ccbab.png"; }
                    // Description //
                    try { 
                        model.Description = doc.DocumentNode.SelectNodes(
                            ".//div[@class='panel-story-info']")[0]
                            .SelectSingleNode(".//div[@class='panel-story-info-description']")
                            .InnerText
                            .ToString()
                            .Trim().Replace("&#39;", "'")
                            .Replace("Description :", "").Trim();
                    }
                    catch { model.Description = "Unknown"; }
                    // Chapter List & Chapters//
                    try
                    {
                        List<ChapterDataModel> chapters = new List<ChapterDataModel>();

                        List<HtmlNode> ChapterList = doc.DocumentNode.SelectNodes(".//div[@class='panel-story-chapter-list']")[0].SelectNodes(".//li").ToList();

                        foreach (HtmlNode Chapter in ChapterList)
                        {
                            ChapterDataModel chapterModel = new ChapterDataModel();

                            chapterModel.ChapterName = Chapter.SelectSingleNode(".//a").InnerText;
                            chapterModel.ChapterLink = Chapter.SelectSingleNode(".//a").Attributes["href"].Value;
                            chapterModel.ChapterDate = Chapter.SelectSingleNode(".//span[starts-with(@class, 'chapter-time')]").Attributes["title"].Value;

                            chapters.Add(chapterModel);
                        }
                        model.ChapterList = chapters;
                        model.Chapters = chapters.Count.ToString();
                        model.MangaId = id;
                    }
                    catch
                    {
                        model.ChapterList = new List<ChapterDataModel>();
                    }
                    // Link //
                    try 
                    {
                        model.MangaLink = doc.DocumentNode.SelectNodes(
                            ".//div[@class='panel-breadcrumb']")[0]
                            .SelectSingleNode($".//a[@title='{model.Name}']")
                            .Attributes["href"].Value;
                        
                    }
                    catch { model.MangaLink = "Unknown"; }

                    return model;
                }
                catch
                {
                    return new MangaDataModel();
                }
            }
        }
    }
}