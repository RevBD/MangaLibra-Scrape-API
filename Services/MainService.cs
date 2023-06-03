using MangaLibra_Scrape_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Net;
using System.Xml.Linq;
using HtmlAgilityPack;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;

namespace MangaLibra_Scrape_API.Services
{
	public class MainService
	{
		private readonly IOptions<OptionsModel> _options;

        public MainService(IOptions<OptionsModel> options)
        {
            this._options = options;
        }

        public async Task<List<MangaDataModel>> GetNewestMangaList()
        {
            List<MangaDataModel> result = new List<MangaDataModel>();

            using (var client = new WebClient())
            {
                try
                {
                    for (int i = 1; i < 2; i++)
                    {
                        Uri url = new Uri($"https://manganato.com/genre-all/{i}");
                        string html = client.DownloadString(url);

                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(html);

                        List<HtmlNode> MangaNodes = null;
                        MangaNodes = (from HtmlNode node in doc.DocumentNode.SelectNodes("//div")
                                      where node.Name == "div"
                                      && node.Attributes["class"] != null
                                      && node.Attributes["class"].Value == "content-genres-item"
                                      select node).ToList();

                        foreach (HtmlNode Manga in MangaNodes)
                        {
                            MangaDataModel model = new MangaDataModel();
                            // Name //
                            try { model.Name = Manga.SelectSingleNode(".//img[@class='img-loading']").Attributes["alt"].Value;}
                            catch (Exception ex) { model.Name = "Unknown"; }
                            // Image //
                            try { model.ImageLink = Manga.SelectSingleNode(".//img[@class='img-loading']").Attributes["src"].Value; }
                            catch (Exception ex) { model.ImageLink = "https://user-images.githubusercontent.com/24848110/33519396-7e56363c-d79d-11e7-969b-09782f5ccbab.png"; }
                            // Description //
                            try { model.Description = Manga.SelectSingleNode(".//div[@class='genres-item-description']").InnerText.ToString().Trim().Replace("&#39;", "'"); }
                            catch (Exception ex) { model.Description = "Unknown"; }
                            // Chapters //
                            try { model.Chapters = Manga.SelectSingleNode(".//a[starts-with(@class, 'genres-item-chap')]").InnerText; }
                            catch (Exception ex) { model.Chapters = "Unknown"; }
                            // Link //
                            try { model.MangaLink = Manga.SelectSingleNode(".//a[starts-with(@class, 'genres-item-img')]").Attributes["href"].Value; }
                            catch (Exception ex) { model.MangaLink = "Unknown"; }
                            // ID //
                            try { model.MangaId = model.MangaLink.Split("-")[1]; }
                            catch (Exception ex) { model.MangaLink = "Unknown"; }

                            result.Add(model);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }

            return result;
        }
        

    }
}


//////////////////////////////////////////////////////////////////////////
//////////////      Can Save Image File     //////////////////////////////
//////////////////////////////////////////////////////////////////////////
//using (WebClient webClient = new WebClient())
//{
//    string fileName = @$"{ mangaName}" + ".jpg";
//    Console.WriteLine(fileName);
//    string localPath = Path.Combine(@"C:\Users\rogalskiw\Desktop\TEST\", fileName);
//    webClient.DownloadFile(imageURL, localPath);
//}
