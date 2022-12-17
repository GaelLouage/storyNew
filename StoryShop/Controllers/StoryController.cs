using Infrastructuur.extensions;
using Infrastructuur.Models;
using Infrastructuur.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StoryShop.Controllers
{
    public class StoryController : Controller
    {
        private readonly IStoryZonService _storyZonService;
        private readonly IFileService _fileService;

        public StoryController(IStoryZonService storyZonService, IFileService fileService)
        {
            _storyZonService = storyZonService;
            _fileService = fileService;
        }

        // GET: StoryController
        public async Task<ActionResult> Index()
        {
            var stories = await _storyZonService.GetStoryzonsAsync();

            ViewData["topStories"] = stories;
            return View(stories);
        }
        //admin list
        public async Task<IActionResult> AdminList()
        {
            var stories = (await _storyZonService.GetStoryzonsAsync()).ToList();
            return View(stories);
        }
        // GET: getallstories
        public async Task<ActionResult> Stories(int amountToShow)
        {
            var stories = await _storyZonService.GetStoryzonsAsync();
            ViewData["stoyList"] = stories;
            switch (amountToShow)
            {
                case 6:
                    stories = (await _storyZonService.GetStoryzonsAsync()).Take(6).ToList();
                    break;
                case 12:
                    stories = (await _storyZonService.GetStoryzonsAsync()).Skip(6).Take(6).ToList();
                    break;
                case 18:
                    stories = (await _storyZonService.GetStoryzonsAsync()).Skip(12).Take(6).ToList();
                    break;
                case 24:
                    stories = (await _storyZonService.GetStoryzonsAsync()).Skip(18).Take(6).ToList();
                    break;
                case 32:
                    stories = (await _storyZonService.GetStoryzonsAsync()).Skip(24).Take(6).ToList();
                    break;
                default:
                    stories = (await _storyZonService.GetStoryzonsAsync()).Take(6).ToList();
                    break;
            }
           
            return View(stories);
        }

        // GET: StoryController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            return View(await _storyZonService.GetStoryzonByIdAsync(id));
        }

        // GET: StoryController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StoryzonEntity storyzon,IFormFile fileImage)
        {
            try
            {
                // to use this make sure u add
                // enctype="multipart/form-data"
                // to the form in the htmlcs file
                storyzon.Image = fileImage.UploadImage();
                await _storyZonService.AddStoryZonAsync(storyzon);
                return RedirectToAction(nameof(AdminList));
            }
            catch
            {
                return View();
            }
        }

        // GET: StoryController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            return View(await _storyZonService.GetStoryzonByIdAsync(id));
        }

        // POST: StoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, StoryzonEntity storyzon, IFormFile image)
        {
            try
            {
                storyzon.Image = image.UploadImage();
                //var storyToUpdate = (await _storyZonService.GetStoryzonByIdAsync(id));
                await _storyZonService.UpdateStoryZonByIdAsync(id, storyzon);
                return RedirectToAction(nameof(AdminList));
            }
            catch
            {
                return View(await _storyZonService.GetStoryzonByIdAsync(id));
            }
        }

        // GET: StoryController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            return View(await _storyZonService.GetStoryzonByIdAsync(id));
        }

        // POST: StoryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, StoryzonEntity storyzon)
        {
            try
            {
                await _storyZonService.DeleteStoryzonByIdAsync(id);
                return RedirectToAction(nameof(AdminList));
            }
            catch
            {
                return View();
            }
        }
    }
}
