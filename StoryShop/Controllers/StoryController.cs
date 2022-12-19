﻿using Infrastructuur.extensions;
using Infrastructuur.Models;
using Infrastructuur.Services.Interfaces;
using Infrastructuur.singleton;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Diagnostics;
using System.Security.Claims;
using System.Speech.Synthesis;
using System.Text;

namespace StoryShop.Controllers
{
    public class StoryController : Controller
    {
        private readonly IStoryZonService _storyZonService;
        private readonly IFileService _fileService;
        private readonly IUserService _userService;
        private readonly IReviewService _reviewService;
        private readonly  SpeechSynthesizer _synthesizer;
        private readonly UserSingleton _userSingleton;
        private static string? detailId;
        public static bool isPlay = false;

        public StoryController(IStoryZonService storyZonService, IFileService fileService, IUserService userService, UserSingleton userSingleton, IReviewService reviewService, SpeechSynthesizer synthesizer)
        {
            _storyZonService = storyZonService;
            _fileService = fileService;
            _userService = userService;
            _userSingleton = userSingleton;
            _reviewService = reviewService;
            _synthesizer = synthesizer;
             _synthesizer.SpeakAsyncCancelAll();

        }

        // GET: StoryController
        public async Task<ActionResult> Index()
        {
            
             var stories = await _storyZonService.GetStoryzonsAsync();
            _userSingleton.User = (await _userService.GetUsersAsync()).FirstOrDefault(x => x.Email == HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value);
            ViewData["reviews"] = (await _reviewService.GetReviews());
            ViewData["topStories"] = stories;
            return View(stories);
        }
        //admin list
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> AdminList(string filtering,string searchInput)
        {
            var stories = (await _storyZonService.GetStoryzonsAsync()).ToList();
            ViewData["reviews"] = (await _reviewService.GetReviews());
            if (!string.IsNullOrEmpty(searchInput))
            {
                stories = stories.Where(x => x.Title.ToLower().Contains(searchInput.ToLower())
                || x.Genre.ToLower().Contains(searchInput.ToLower())
                || x.Rating.ToString().Contains(searchInput.ToLower())
                || x.AddedDate.ToLower().Contains(searchInput.ToLower())).ToList();
            }
            switch (filtering)
            {
                case "Title":
                    stories = stories.OrderBy(x => x.Title).ToList();
                    break;
                case "Genre":
                    stories = stories.OrderBy(x => x.Genre).ToList();
                    break;
                case "Rating":
                    stories = stories.OrderBy(x => x.Rating).ToList();
                    break;
                case "AddedDate ascending":
                    stories = stories.OrderBy(x => x.AddedDate).ToList();
                    break;
                case "AddedDate descending":
                    stories = stories.OrderByDescending(x => x.AddedDate).ToList();
                    break;
            }
            return View(stories);
        }
        // GET: getallstories
        public async Task<ActionResult> Stories(int amountToShow)
        {
            var stories = await _storyZonService.GetStoryzonsAsync();
            ViewData["stoyList"] = stories;
            ViewData["reviews"] = (await _reviewService.GetReviews());
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

            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login" , "User");
            detailId = id;
            var reviews = await _reviewService.GetReviewsByStoryId(id);
            var usersRev = await _userService.GetUsersAsync(); 
            if (reviews is not null)
            {
                ViewData["reviews"] = reviews;
                ViewData["usersRevs"] = usersRev;
             
            }
            //var userSs = new UserEntity();

            //foreach (var item in reviews)
            //{
            //    if (item.StoryId == id)
            //    {
            //        userSs = usersRev.FirstOrDefault(x => x.Id == item.UserId);
            //    }
            //}

            return View(await _storyZonService.GetStoryzonByIdAsync(id));
        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        // GET: StoryController/Create
        public async Task<ActionResult> Create()
        {
            return View();
        }

        // POST: StoryController/Create
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StoryzonEntity storyzon, IFormFile fileImage)
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
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult> Edit(string id)
        {
            return View(await _storyZonService.GetStoryzonByIdAsync(id));
        }

        // POST: StoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
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
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult> Delete(string id)
        {
            return View(await _storyZonService.GetStoryzonByIdAsync(id));
        }

        // POST: StoryController/Delete/5
        [Authorize(Roles = "Admin,SuperAdmin")]
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

        //write to excell
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> WriteToExcel()
        {

            if ((await _storyZonService.GetStoryzonsAsync()).ToList().WriteDataToExcel<StoryzonEntity>("StoryData.xls", new Dictionary<string, string>
            {
                {"Title","Title" },
                {"Genre","Genre" },
                {"Rating","Rating" },
                {"AddedDate","AddedDate" }
            }))
            {
                return RedirectToAction(nameof(AdminList));
            }
            return RedirectToAction(nameof(AdminList));
        }

        //add review
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(string reviewTitle, string message, string rating)
        {
            var review = new ReviewEntity();
            review.ReviewTitle = reviewTitle;
            review.ReviewBody = message;
            review.Rating = rating;
            review.StoryId = detailId;
            review.UserId = _userSingleton.User.Id;
            var reviewToAdd = await _reviewService.AddReview(review);
            return RedirectToAction(nameof(Details), new { id = detailId });
        }
        public IActionResult TextToSpeach(string textToRead)
        {
                SynthesizeState(textToRead);

                if (_synthesizer.State == SynthesizerState.Paused)
                {
                    _synthesizer.Resume();
                }
            return RedirectToAction(nameof(Details), new { id = detailId });
        }

        private void SynthesizeState(string? textToRead = null)
        {
            // Cancel the speech synthesis
            if (_synthesizer.State != SynthesizerState.Speaking && _synthesizer.State != SynthesizerState.Paused)
            {
                try
                {
                    isPlay = !isPlay;
                    if((bool)isPlay)
                    {
                        _synthesizer.SelectVoiceByHints(VoiceGender.Female);
                        _synthesizer.Rate = 1;
                        _synthesizer.Speak(textToRead);
                    } else
                    {
                        _synthesizer.Pause();
                    }
                   
                } catch
                {

                }
            }
            else
            {
                _synthesizer.Pause();
            }
        }

        public IActionResult CreatePdf(StoryzonEntity storyzonEntity)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            PdfDocument pdf = new PdfDocument();
            pdf.Info.Title = storyzonEntity.Title;

            PdfPage page = pdf.AddPage();
            XGraphics graph = XGraphics.FromPdfPage(page);

            XImage image = XImage.FromFile(@"C:\Users\louag\Desktop\storyzon\Story\StoryShop\wwwroot\" + storyzonEntity.Image);

            // Draw the image at the top of the page
            graph.DrawImage(image, 0, 0, page.Width, page.Height / 2);

            XFont font = new XFont("Verdana", 8, XFontStyle.Regular);
            XFont titleFont = new XFont("Verdana", 25, XFontStyle.Bold);

            var text =  storyzonEntity.BodyEn.Split('.');
            graph.DrawString(storyzonEntity.Title, font, XBrushes.Black, new XRect(0, 480, page.Width.Point, titleFont.Size), XStringFormat.Center);

            // Draw each line of text
            string text2 = "";
            int counter = 0;
            for (int i = 0; i < text.Length; i++)
            {
                graph.DrawString(text[i], font, XBrushes.Black, new XRect(0, 500+counter, page.Width.Point, font.Size), XStringFormat.Center);
                counter += 10;

            }
            // Calculate the vertical position for the line of text
            //double y = page.Height - (text.Length - i) * font.Size;
            // Draw the text
            //graph.DrawString(string.Concat(text), font, XBrushes.Black, new XRect(0, 500, page.Width.Point, font.Size), XStringFormat.Center);
            string fileName = $"{storyzonEntity.Title}.pdf";
            pdf.Save(fileName);
            //Process.Start(fileName);
            FileStream stream = new FileStream(fileName, FileMode.Open);
            return File(stream, "application/octet-stream", fileName);
        }
    }
}
