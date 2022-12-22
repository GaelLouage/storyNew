using Infrastructuur.EnumsAndStaticProps;
using Infrastructuur.extensions;
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
        private readonly IUserSelectedStoryService _userSelectedStory;
        private readonly IUserSelectedStoryService _userSelectedStoryService;
        private readonly SpeechSynthesizer _synthesizer;
        private readonly UserSingleton _userSingleton;
        private static string? detailId;
        public static bool isPlay = false;

        public StoryController(IStoryZonService storyZonService, IFileService fileService, IUserService userService, UserSingleton userSingleton, IReviewService reviewService, SpeechSynthesizer synthesizer, IUserSelectedStoryService userSelectedStory, IUserSelectedStoryService userSelectedStoryService)
        {
            _storyZonService = storyZonService;
            _fileService = fileService;
            _userService = userService;
            _userSingleton = userSingleton;
            _reviewService = reviewService;
            _synthesizer = synthesizer;
            _userSelectedStory = userSelectedStory;
            _userSelectedStoryService = userSelectedStoryService;
            _synthesizer.SpeakAsyncCancelAll();
        }

        // GET: StoryController
        public async Task<ActionResult> Index()
        {
            var storiesForRecommended = new List<StoryzonEntity>();
            List<StoryzonEntity> userSelectedThose = null;
            var stories = await _storyZonService.GetStoryzonsAsync();
            _userSingleton.User = (await _userService.GetUsersAsync()).FirstOrDefault(x => x.Email == HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value);
            ViewData["reviews"] = (await _reviewService.GetReviews());
            ViewData["topStories"] = stories;
            if(_userSingleton.User is not null)
            {
                userSelectedThose = (await _userSelectedStoryService?.GetStoryzonsByUserSelectedIdAsync(_userSingleton.User.Id)).ToList();
                storiesForRecommended = GetByGenreAndNotSelected(storiesForRecommended, userSelectedThose, stories);
            }
            return View(stories);
        }

    

        //admin list
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> AdminList(string filtering, string searchInput)
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

            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "User");
            detailId = id;
            var reviews = await _reviewService.GetReviewsByStoryId(id);
            var usersRev = await _userService.GetUsersAsync();
            if (reviews is not null)
            {
                ViewData["reviews"] = reviews;
                ViewData["usersRevs"] = usersRev;

            }
            if(!await _userSelectedStory.AddSelectedStoryToUserByIdAsync(new UserStorySelectEntity
            {
                StoryId = id,
                UserId = _userSingleton.User.Id
            }))
            {

                //get error message for admin
            } 

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
            var file = (await _storyZonService.GetStoryzonsAsync()).ToList().WriteDataToExcel<StoryzonEntity>("StoryData.xls", new Dictionary<string, string>
            {
                {"Title","Title" },
                {"Genre","Genre" },
                {"Rating","Rating" },
                {"AddedDate","AddedDate" }
            });
            Response.Headers.Add("Content-Disposition", "attachment; filename=data.xls");

            return File(file.ToArray(), "application/octet-stream");
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
                    if ((bool)isPlay)
                    {
                        _synthesizer.SelectVoiceByHints(VoiceGender.Female);
                        _synthesizer.Rate = 1;
                        _synthesizer.Speak(textToRead);
                    }
                    else
                    {
                        _synthesizer.Pause();
                    }

                }
                catch
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


            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\" + storyzonEntity.Image);
            XImage image = XImage.FromFile(imagePath);

            // Draw the image at the top of the page
            graph.DrawImage(image, 0, 0, page.Width, page.Height / 2);

            XFont font = new XFont("Verdana", 10, XFontStyle.Regular);
            XFont titleFont = new XFont("Verdana", 30, XFontStyle.Bold);


            XStringFormat format = new XStringFormat();
            format.Alignment = XStringAlignment.Center;
            format.LineAlignment = XLineAlignment.Center;


            graph.DrawString(storyzonEntity.Title, titleFont, XBrushes.White, new XRect(0, 120, page.Width.Point, titleFont.Size), format);

            // Split the text into words
            string[] words = storyzonEntity.BodyEn.Split(' ');

            // Set the maximum width for each line of text
            double maxWidth = page.Width.Point / 1.5;

            // Set the starting position for the text
            double x = 0;
            double y = 500;

            // Keep track of the current line of text
            string line = "";
            foreach (string word in words)
            {
                // Measure the width of the current line of text
                double width = graph.MeasureString(line, font).Width;
                // If the width of the line exceeds the maximum width, draw the line and start a new one
                if (width > maxWidth)
                {
                    graph.DrawString(line, font, XBrushes.Black, new XRect(x, y, page.Width.Point, font.Size), format);
                    line = "";
                    y += font.Size;
                }
                line += word + " ";
            }
          

            graph.DrawString(line, font, XBrushes.Black, new XRect(x, y, page.Width.Point, font.Size), format);

            string fileName = $"{storyzonEntity.Title}.pdf";

            // Save the PDF to a memory stream
            MemoryStream stream = new MemoryStream();
            pdf.Save(stream, false);

            Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName);

            return File(stream.ToArray(), "application/octet-stream");
        }


        /*get stories by max selected genre and not selected*/
        private List<StoryzonEntity> GetByGenreAndNotSelected(List<StoryzonEntity> storiesForRecommended, List<StoryzonEntity> userSelectedThose, List<StoryzonEntity> stories)
        {
            var mostSelectedGenre = userSelectedThose?
              .GroupBy(x => x)
              ?.OrderByDescending(x => x.Count()).Select(x => x.Key)
              ?.FirstOrDefault()?.Genre;
            if (mostSelectedGenre is not null)
            {
                storiesForRecommended = stories.Where(story => story.Genre == mostSelectedGenre && !userSelectedThose.Any(x => x.Id == story.Id)).ToList();
                ViewData["mostSelectedGenre"] = mostSelectedGenre;
                ViewData["storiesNotSelectedByUser"] = storiesForRecommended;

            }
            return storiesForRecommended;
        }
    }
}
