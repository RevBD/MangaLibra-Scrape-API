using MangaLibra_Scrape_API.Models;
using MangaLibra_Scrape_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MangaLibra_Scrape_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MainController : Controller
	{
		private readonly MainService _mainService;
        public MainController(MainService mainService)
        {
			_mainService = mainService;
        }

        [HttpGet(nameof(GetNewestMangaList))]
		public async Task<List<MangaDataModel>> GetNewestMangaList(int pages)
		{
			return await _mainService.GetNewestMangaList(pages);
        }

		[HttpGet(nameof(GetMangaById))]
		public async Task<MangaDataModel> GetMangaById(string id)
		{
            return await _mainService.GetMangaByID(id);
        }
	}
}
