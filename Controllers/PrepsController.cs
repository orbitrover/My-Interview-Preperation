using Core.InterviewPrep.PostgreSQL.Data;
using Core.InterviewPrep.PostgreSQL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Core.InterviewPrep.PostgreSQL.Controllers
{
    [Authorize]
    public class PrepsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrepsController(ApplicationDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> TopicList()
        {

            List<Headings> headings = new List<Headings>();

            var topics = _context.ValueTypeMaster.Where(x => x.ValueTypeGroupId == 1).Include(h=> h.Headings).ThenInclude(q => q.Questions).ThenInclude(a => a.Answers).ToList();

            //foreach (var topic in topics)
            //{
            //    var heading = await _context.Headings.Where(x => x.TopicId == topic.Id).Include(q => q.Questions).ThenInclude(a => a.Answers).ToListAsync();
            //    headings.AddRange(heading);
            //}

            //ViewData["Headings"] = headings;

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
                //topic.ValueTypeGroupId = 1001;
                _context.Add(topic);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/TopicList" });
            }
            return Json(new { msg = "Topic Created Successfully", msgType = "success", Url = "/Preps/TopicList" });
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
        public async Task<IActionResult> EditTopics([Bind("Id,ValueTypeGroupId,Name,Description")] ValueTypeMaster topic)
        {
            try
            {
                topic.ModifiedBy = "2";
                topic.ModifiedDate = DateTime.Now;
                _context.Update(topic);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/TopicList" });
            }
            return Json(new { msg = "Topic Updated Successfully", msgType = "success", Url = "/Preps/TopicList" });
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
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/TopicList" });
            }
            return Json(new { msg = "Topic Deleted Successfully", msgType = "success", Url = "/Preps/TopicList" });
        }
        #endregion;
        #region Headings
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
                _context.Add(heading);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/TopicList" });
            }
            return Json(new { msg = "Heading Created Successfully", msgType = "success", Url = "/Preps/TopicList" });
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
        public async Task<IActionResult> EditHeadings([Bind("HeadingId,TopicId,HeadingName")] Headings heading)
        {
            try
            {
                heading.ModifiedBy = "2";
                heading.ModifiedDate = DateTime.Now;
                _context.Update(heading);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/TopicList" });
            }
            return Json(new { msg = "Heading Updated Successfully", msgType = "success", Url = "/Preps/TopicList" });
        }

        // POST: Master/DeleteHeadings
        [HttpPost]
        public async Task<IActionResult> DeleteHeadings(int id)
        {
            try
            {
                var heading = await _context.Headings.Where(x => x.HeadingId == id).SingleOrDefaultAsync();
                _context.Remove(heading);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/TopicList" });
            }
            return Json(new { msg = "Heading Deleted Successfully", msgType = "success", Url = "/Preps/TopicList" });
        }

        #endregion;
        #region Questions
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
                _context.Add(question);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/TopicList" });
            }
            return Json(new { msg = "Question Created Successfully", msgType = "success", Url = "/Preps/TopicList" });
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
        public async Task<IActionResult> EditQuestions([Bind("QuestionId,HeadingId,QuestionName")] Questions question)
        {
            try
            {
                question.ModifiedBy = "2";
                question.ModifiedDate = DateTime.Now;
                _context.Update(question);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/TopicList" });
            }
            return Json(new { msg = "Question Updated Successfully", msgType = "success", Url = "/Preps/TopicList" });
        }

        // POST: Master/DeleteQuestions
        [HttpPost]
        public async Task<IActionResult> DeleteQuestions(int id)
        {
            try
            {
                var question = await _context.Questions.Where(x => x.QuestionId == id).SingleOrDefaultAsync();
                _context.Remove(question);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/TopicList" });
            }
            return Json(new { msg = "Answer Deleted Successfully", msgType = "success", Url = "/Preps/TopicList" });
        }
        #endregion;
        #region Answers
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
                _context.Add(answer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/TopicList" });
            }
            return Json(new { msg = "Answer Created Successfully", msgType = "success", Url = "/Preps/TopicList" });
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
        public async Task<IActionResult> EditAnswers([Bind("AnswerId,QuestionId,AnswerName")] Answers answer)
        {
            try
            {
                answer.ModifiedBy = "2";
                answer.ModifiedDate = DateTime.Now;
                _context.Update(answer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/TopicList" });
            }
            return Json(new { msg = "Answer Updated Successfully", msgType = "success", Url = "/Preps/TopicList" });
        }

        // POST: Master/DeleteAnswers
        [HttpPost]
        public async Task<IActionResult> DeleteAnswers(int id)
        {
            try
            {
                var answer = await _context.Answers.Where(x => x.AnswerId == id).SingleOrDefaultAsync();
                _context.Remove(answer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Exception: " + ex.Message, msgType = "error", Url = "/Preps/TopicList" });
            }
            return Json(new { msg = "Answer Deleted Successfully", msgType = "success", Url = "/Preps/TopicList" });
        }
        #endregion;
    }
}
