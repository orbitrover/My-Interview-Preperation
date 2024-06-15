using Core.InterviewPrep.PostgreSQL.Data;
using Core.InterviewPrep.PostgreSQL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Core.InterviewPrep.PostgreSQL.Controllers
{
    [Authorize]
    public class PrepsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly string _userId;
        private readonly SignInManager<IdentityUser> _signInManager;
        public PrepsController(ApplicationDbContext context, IMemoryCache cache, SignInManager<IdentityUser> SignInManager)
        {
            _context = context;
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _signInManager = SignInManager;
            _userId = Getuserid();
        }
        public string Getuserid()
        {
            var userId = "";
            if (User != null && _signInManager.IsSignedIn(User))
            {
                var item = _signInManager.UserManager.FindByNameAsync(User.Identity.Name).Result;
                userId = item.Id;
            }
            return userId;
        }
        // GET: Master
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> _HighlightViewer()
        {
            return PartialView("_HighlightViewer");
        }
        public async Task<IActionResult> _HighlightFilter()
        {
            return PartialView("_HighlightFilter");
        }
        #region Topics
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, NoStore = false, VaryByQueryKeys = new string[] { "id", "eventtype" }, VaryByHeader = "User-Agent")]
        public async Task<IActionResult> TopicList(int? id, string eventtype = null)
        {
            List<ValueTypeMaster> topics;
            if (id > 0)
                topics = await _context.ValueTypeMaster.Where(x => x.ValueTypeGroupId == 1 && x.Id == id && (x.CreatedBy == "1" || x.CreatedBy == Getuserid())).Include(h => h.Headings).ThenInclude(q => q.Questions).ToListAsync();
            else
                topics = await _context.ValueTypeMaster.Where(x => x.ValueTypeGroupId == 1 && (x.CreatedBy == "1" || x.CreatedBy == Getuserid())).Include(h => h.Headings).ThenInclude(q => q.Questions).ToListAsync();
            ViewData["TopicId"] = id;
            return View(topics);
        }
        // GET: Master/AddTopics
        public async Task<IActionResult> AddTopics()
        {
            var topicList = await _context.ValueTypeGroupMaster.Where(x => x.Id == 1).ToListAsync();
            ViewData["ValueTypeGroupId"] = new SelectList(topicList, "Id", "Name");
            return View();
        }

        // POST: Master/AddTopics
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTopics([Bind("Id,ValueTypeGroupId,Name,Description")] ValueTypeMaster topic)
        {
            try
            {
                topic.CreatedBy = Getuserid();
                _context.Add(topic);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/TopicList?eventtype=Add" });
            }
            return Json(new { msg = "Topic Created Successfully", msgType = "success", Url = "/Preps/TopicList?eventtype=Add" });
        }

        // GET: Master/EditTopics
        public async Task<IActionResult> EditTopics(int? id)
        {
            var topic = await _context.ValueTypeMaster.Where(x => x.Id == id).SingleOrDefaultAsync();
            var valueTypeGroupList = await _context.ValueTypeGroupMaster.Where(x => x.Id == 1).ToListAsync();
            ViewData["ValueTypeGroupId"] = new SelectList(valueTypeGroupList, "Id", "Name");
            return View(topic);
        }

        // POST: Master/EditTopics
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTopics([Bind("Id,ValueTypeGroupId,Name,Description, CreatedBy, CreatedDate")] ValueTypeMaster topic)
        {
            try
            {
                topic.ModifiedBy = Getuserid();
                topic.ModifiedDate = DateTime.Now;
                _context.Update(topic);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/TopicList?eventtype=Edit" });
            }
            return Json(new { msg = "Topic Updated Successfully", msgType = "success", Url = "/Preps/TopicList?eventtype=Edit" });
        }

        // POST: Master/DeleteTopics
        [HttpPost]
        public async Task<IActionResult> DeleteTopics(int id)
        {
            try
            {
                var topic = await _context.ValueTypeMaster.Where(x => x.Id == id).SingleOrDefaultAsync();
                _context.Remove(topic);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/TopicList?eventtype=Delete" });
            }
            return Json(new { msg = "Topic Deleted Successfully", msgType = "success", Url = "/Preps/TopicList?eventtype=Delete" });
        }
        #endregion;
        #region Headings
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, NoStore = false, VaryByQueryKeys = new[] { "id" })]
        public async Task<IActionResult> HeadingList(int? id)
        {
            List<Headings> headings;
            if (id > 0)
                headings = await _context.Headings.Where(x => x.TopicId == id && x.CreatedBy == Getuserid()).ToListAsync();
            else
                headings = await _context.Headings.Where(x => x.CreatedBy == Getuserid()).ToListAsync();
            ViewData["TopicId"] = id;
            return View(headings);
        }
        // GET: Master/AddHeadings
        public IActionResult AddHeadings(int? id)
        {
            ViewData["TopicId"] = id;
            return View();
        }

        // POST: Master/AddHeadings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddHeadings([Bind("HeadingId,TopicId,HeadingName")] Headings heading)
        {
            try
            {
                heading.CreatedBy = Getuserid();
                _context.Add(heading);
                await _context.SaveChangesAsync();
                heading = await _context.Headings.Where(x => x.HeadingId == heading.HeadingId).Include(t => t.Topic).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/HeadingList/" + heading.TopicId });
            }
            return Json(new { msg = "Heading Created Successfully", msgType = "success", Url = "/Preps/HeadingList/" + heading.TopicId });
        }

        // GET: Master/EditQuestions
        public async Task<IActionResult> EditHeadings(int? id)
        {
            var heading = await _context.Headings.Where(x => x.HeadingId == id).SingleOrDefaultAsync();
            var topicList = await _context.ValueTypeMaster.Where(x => x.ValueTypeGroupId == 1).Include(t => t.ValueTypeGroupMaster).ToListAsync();
            ViewData["TopicId"] = new SelectList(topicList, "Id", "Name");
            return View(heading);
        }

        // POST: Master/EditHeadings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHeadings([Bind("HeadingId,TopicId,HeadingName, CreatedBy, CreatedDate")] Headings heading)
        {
            try
            {
                heading.ModifiedBy = Getuserid();
                heading.ModifiedDate = DateTime.Now;
                _context.Update(heading);
                await _context.SaveChangesAsync();
                heading = await _context.Headings.Where(x => x.HeadingId == heading.HeadingId).Include(t => t.Topic).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/HeadingList/" + heading.TopicId });
            }
            return Json(new { msg = "Heading Updated Successfully", msgType = "success", Url = "/Preps/HeadingList/" + heading.TopicId });
        }

        // POST: Master/DeleteHeadings
        [HttpPost]
        public async Task<IActionResult> DeleteHeadings(int id)
        {
            Headings heading = new Headings();
            try
            {
                heading = await _context.Headings.Where(x => x.HeadingId == id).SingleOrDefaultAsync();
                _context.Remove(heading);
                await _context.SaveChangesAsync();
                heading = await _context.Headings.Where(x => x.HeadingId == id).Include(t => t.Topic).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/HeadingList/" + heading.TopicId });
            }
            return Json(new { msg = "Heading Deleted Successfully", msgType = "success", Url = "/Preps/HeadingList/" + heading.TopicId });
        }

        #endregion;
        #region Questions
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, NoStore = false, VaryByQueryKeys = new[] { "id", "topicId" })]
        public async Task<IActionResult> QuestionList(int? id, int? topicId)
        {
            List<Questions> questions;
            if (id > 0)
                questions = await _context.Questions.Where(x => x.HeadingId == id && x.CreatedBy == Getuserid()).ToListAsync();
            else
                questions = await _context.Questions.Where(x => x.CreatedBy == Getuserid()).ToListAsync();
            ViewData["TopicId"] = topicId;
            ViewData["HeadingId"] = id;
            return View(questions);
        }
        // GET: Master/AddQuestions
        public IActionResult AddQuestions(int? id)
        {
            ViewData["HeadingId"] = id;
            return View();
        }

        // POST: Master/AddQuestions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuestions([Bind("QuestionId,HeadingId,QuestionName")] Questions question)
        {
            try
            {
                question.CreatedBy = Getuserid();
                _context.Add(question);
                await _context.SaveChangesAsync();
                question = await _context.Questions.Where(x => x.QuestionId == question.QuestionId).Include(h => h.Heading).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/QuestionList?id=" + question.HeadingId + "&topicId=" + question.Heading.TopicId });
            }
            return Json(new { msg = "Question Created Successfully", msgType = "success", Url = "/Preps/QuestionList?id=" + question.HeadingId + "&topicId=" + question.Heading.TopicId });
        }

        // GET: Master/EditQuestions
        public async Task<IActionResult> EditQuestions(int? id)
        {
            var question = await _context.Questions.Where(x => x.QuestionId == id).SingleOrDefaultAsync();
            var topicId = _context.Headings.Where(x => x.HeadingId == question.HeadingId).Include(t => t.Topic).SingleOrDefault().TopicId;
            var headingList = await _context.Headings.Where(x => x.TopicId == topicId).Include(t => t.Topic).ToListAsync();
            ViewData["HeadingId"] = new SelectList(headingList, "HeadingId", "HeadingName");
            return View(question);
        }

        // POST: Master/EditQuestions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuestions([Bind("QuestionId,HeadingId,QuestionName, CreatedBy, CreatedDate")] Questions question)
        {
            try
            {
                question.ModifiedBy = Getuserid();
                question.ModifiedDate = DateTime.Now;
                _context.Update(question);
                await _context.SaveChangesAsync();
                question = await _context.Questions.Where(x => x.QuestionId == question.QuestionId).Include(h => h.Heading).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/QuestionList?id=" + question.HeadingId + "&topicId=" + question.Heading.TopicId });
            }
            return Json(new { msg = "Question Updated Successfully", msgType = "success", Url = "/Preps/QuestionList?id=" + question.HeadingId + "&topicId=" + question.Heading.TopicId });
        }

        // POST: Master/DeleteQuestions
        [HttpPost]
        public async Task<IActionResult> DeleteQuestions(int id)
        {
            Questions question = new Questions();
            try
            {
                question = await _context.Questions.Where(x => x.QuestionId == id).SingleOrDefaultAsync();
                _context.Remove(question);
                await _context.SaveChangesAsync();
                question = await _context.Questions.Where(x => x.QuestionId == id).Include(h => h.Heading).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/QuestionList?id=" + question.HeadingId + "&topicId=" + question.Heading.TopicId });
            }
            return Json(new { msg = "Answer Deleted Successfully", msgType = "success", Url = "/Preps/QuestionList?id=" + question.HeadingId + "&topicId=" + question.Heading.TopicId });
        }
        #endregion;
        #region Answers
        //[OutputCache(Duration = 180, NoStore = false, PolicyName = "ansExipire180", VaryByRouteValueNames = new[] { "id,headingId,topicId" })]
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, NoStore = false, VaryByQueryKeys = new[] { "id", "headingId", "topicId" })]
        public async Task<IActionResult> AnswerList(int? id, int? headingId, int? topicId)
        {
            List<Answers> Answers;
            if (id > 0)
                Answers = await _cache.GetOrCreateAsync("AnswerList_" + id, async entry => await (from ans in _context.Answers.Where(x => x.QuestionId == id && x.CreatedBy == Getuserid()) select ans).ToListAsync());
            else
                Answers = await _cache.GetOrCreateAsync("AnswerList_" + id, async entry => await (from ans in _context.Answers.Where(x => x.CreatedBy == Getuserid()) select ans).ToListAsync());
            ViewData["TopicId"] = topicId;
            ViewData["HeadingId"] = headingId;
            ViewData["QuestionId"] = id;
            return View(Answers);
        }
        // GET: Master/AddAnswers
        public IActionResult AddAnswers(int? id)
        {
            ViewData["QuestionId"] = id;
            return View();
        }

        // POST: Master/AddAnswers
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAnswers([Bind("AnswerId,QuestionId,AnswerName")] Answers answer)
        {
            try
            {
                answer.CreatedBy = Getuserid();
                _context.Add(answer);
                await _context.SaveChangesAsync();
                answer = await _context.Answers.Where(x => x.AnswerId == answer.AnswerId).Include(q => q.Question).ThenInclude(h => h.Heading).SingleOrDefaultAsync();
                _cache.Remove("AnswerList_" + answer.QuestionId);
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/AnswerList?id=" + answer.QuestionId + "&headingId=" + answer.Question.HeadingId + "&topicId=" + answer.Question.Heading.TopicId });
            }
            return Json(new { msg = "Answer Created Successfully", msgType = "success", Url = "/Preps/AnswerList?id=" + answer.QuestionId + "&headingId=" + answer.Question.HeadingId + "&topicId=" + answer.Question.Heading.TopicId });
        }

        // GET: Master/EditAnswers
        public async Task<IActionResult> EditAnswers(int? id)
        {
            var answer = await _context.Answers.Where(x => x.AnswerId == id).SingleOrDefaultAsync();
            var headingid = _context.Questions.Where(x => x.QuestionId == answer.QuestionId).Include(h => h.Heading).SingleOrDefault().HeadingId;
            var questionList = await _context.Questions.Where(x => x.HeadingId == headingid).Include(h => h.Heading).ToListAsync();
            ViewData["QuestionId"] = new SelectList(questionList, "QuestionId", "QuestionName");
            return View(answer);
        }

        // POST: Master/EditAnswers
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAnswers([Bind("AnswerId,QuestionId,AnswerName, CreatedBy, CreatedDate")] Answers answer)
        {
            try
            {
                answer.ModifiedBy = Getuserid();
                answer.ModifiedDate = DateTime.Now;
                _context.Update(answer);
                await _context.SaveChangesAsync();
                answer = await _context.Answers.Where(x => x.AnswerId == answer.AnswerId).Include(q => q.Question).ThenInclude(h => h.Heading).SingleOrDefaultAsync();
                _cache.Remove("AnswerList_" + answer.QuestionId);
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/AnswerList?id=" + answer.QuestionId + "&headingId=" + answer.Question.HeadingId + "&topicId=" + answer.Question.Heading.TopicId });
            }
            return Json(new { msg = "Answer Updated Successfully", msgType = "success", Url = "/Preps/AnswerList?id=" + answer.QuestionId + "&headingId=" + answer.Question.HeadingId + "&topicId=" + answer.Question.Heading.TopicId });
        }

        // POST: Master/DeleteAnswers
        [HttpPost]
        public async Task<IActionResult> DeleteAnswers(int id)
        {
            Answers answer = new Answers();
            try
            {
                answer = await _context.Answers.Where(x => x.AnswerId == id).SingleOrDefaultAsync();
                _context.Remove(answer);
                await _context.SaveChangesAsync();
                answer = await _context.Answers.Where(x => x.AnswerId == id).Include(q => q.Question).ThenInclude(h => h.Heading).SingleOrDefaultAsync();
                _context.Remove("AnswerList_" + answer.QuestionId);
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/AnswerList?id=" + answer.QuestionId + "&headingId=" + answer.Question.HeadingId + "&topicId=" + answer.Question.Heading.TopicId });
            }
            return Json(new { msg = "Answer Deleted Successfully", msgType = "success", Url = "/Preps/AnswerList?id=" + answer.QuestionId + "&headingId=" + answer.Question.HeadingId + "&topicId=" + answer.Question.Heading.TopicId });
        }
        #endregion;
    }
}
