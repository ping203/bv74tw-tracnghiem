using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_CMS.Utilities;
using System.Web.UI.WebControls;
using MVC_CMS.Models;
using MvcPaging;
using System.Collections;
using System.Data;
using Microsoft.Reporting.WinForms;

namespace MVC_CMS.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public const int PageSize = 20;
        TRACNGHIEMEntities db = DB.GetContext();

        /*
         * Session["Exam"]//Thông tin của cuộc thi
         * Session["ExamStartTime"]//Thời gian bắt đầu thi
         * Session["ExamEndTime"]//Thời gian kết thúc thi
         * Session["ExamTime"]//Tổng thời gian cho phép của bài thi
         * Session["ExamDuringTime"]//Thời gian còn lại         
         * Session["QuestionsAndAnswers"]//Danh sách câu hỏi và câu trả lời         
         * Session["TestQuestionIndex"]//Thứ tự câu hỏi đang hiển thị
         * Session["TestQuestionCount"]//Tổng số câu hỏi
         * Session["TestEnd"]//Báo kết thúc thi         
         */

        public ActionResult Index()
        {
            var exam = DB.GetContext().TN_Exam.FirstOrDefault(m => m.IsActive);
            //var listQuestion = db.TN_Question.ToList();
            //foreach (var item in listQuestion)
            //{
            //    var tmp = new TN_ExamQuestion()
            //    {
            //        QuestionID = item.ID,
            //        ExamID = 1
            //    };
            //    db.TN_ExamQuestion.AddObject(tmp);
            //}
            //db.SaveChanges();
            ViewBag.ExamTitle = exam.Content;
            Session[SessionKey.ExamObject] = exam;
            return View(new UserLogin());
        }

        [HttpPost]
        public ActionResult Index(UserLogin model)
        {
            if (ModelState.IsValid)
            {
                var getUser = db.TN_User.FirstOrDefault(m => m.SBD.Equals(model.SBD.Trim().Replace(" ", ""), StringComparison.OrdinalIgnoreCase));
                if (getUser == null)
                {
                    getUser = new TN_User()
                    {
                        SBD = model.SBD.Trim().Replace(" ", ""),
                        FullName = model.FullName.Trim()
                    };
                    db.TN_User.AddObject(getUser);
                    db.SaveChanges();
                }
                var exam = (TN_Exam)Session[SessionKey.ExamObject];
                Session[SessionKey.UserObject] = getUser;
                var getExamUser = db.TN_ExamUser.FirstOrDefault(m => m.UserID == getUser.ID && m.ExamID == exam.ID);
                if (getExamUser != null)
                {
                    Session[SessionKey.ExamUserObject] = getExamUser;
                    return RedirectToAction("Test", "Home");
                }

                return RedirectToAction("ConfirmTest", "Home");
            }
            return View(model);
        }


        public ActionResult ConfirmTest()
        {
            if (Session[SessionKey.UserObject] != null)
            {
                var exam = DB.GetContext().TN_Exam.FirstOrDefault(m => m.IsActive);
                if (exam != null)
                {
                    Session[SessionKey.ExamObject] = exam;
                    return View(exam);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult ConfirmTest(string ok = "")
        {
            if (Session[SessionKey.UserObject] == null || Session[SessionKey.ExamObject] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var exam = (TN_Exam)Session[SessionKey.ExamObject];
            var user = (TN_User)Session[SessionKey.UserObject];
            var tnExamUser = new TN_ExamUser()
            {
                JoinDate = DateTime.Now,
                ExamID = exam.ID,
                UserID = user.ID,
                Duration = 0,
                CorrectCount = 0
            };
            db.TN_ExamUser.AddObject(tnExamUser);
            db.SaveChanges();

            Session[SessionKey.ExamUserObject] = tnExamUser;

            // Tạo danh sách câu hỏi cho User
            var listQuestionExam = db.TN_ExamQuestion.Where(m => m.ExamID == exam.ID).ToList();
            if (listQuestionExam.Count > 0)
            {

                if (listQuestionExam.Count == tnExamUser.TN_Exam.QuestionCount)
                {
                    foreach (var item in listQuestionExam)
                    {
                        TN_ExamUserAnswer tnExamUserAnswer = new TN_ExamUserAnswer();
                        tnExamUserAnswer.ExamUserID = tnExamUser.ID;
                        tnExamUserAnswer.QuestionID = item.QuestionID;
                        db.TN_ExamUserAnswer.AddObject(tnExamUserAnswer);
                    }
                    db.SaveChanges();
                }
                else
                {
                    var allQuestion = listQuestionExam.Select(m => m.TN_Question).GroupBy(m => m.Type).ToList();
                    var eachTypeQuestions = Math.Floor((double)exam.QuestionCount / (double)allQuestion.Count);
                    var listQuestion = new List<TN_Question>();
                    foreach (var item in allQuestion)
                    {
                        listQuestion.AddRange(GetRandomListQuestion(item.ToList(), (int)eachTypeQuestions));
                    }
                    foreach (var item in listQuestion)
                    {
                        TN_ExamUserAnswer tnExamUserAnswer = new TN_ExamUserAnswer();
                        tnExamUserAnswer.ExamUserID = tnExamUser.ID;
                        tnExamUserAnswer.QuestionID = item.ID;
                        db.TN_ExamUserAnswer.AddObject(tnExamUserAnswer);
                    }
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Test", "Home");
        }

        [HttpGet, OutputCache(NoStore = true, Duration = 1)]
        public ActionResult Test()
        {
            if (Session[SessionKey.UserObject] == null || Session[SessionKey.ExamObject] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var exam = (TN_Exam)Session[SessionKey.ExamObject];
            var user = (TN_User)Session[SessionKey.UserObject];
            var tnExamUser = db.TN_ExamUser.FirstOrDefault(m => m.ExamID == exam.ID && m.UserID == user.ID);
            if (tnExamUser == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (DateTime.Now > tnExamUser.JoinDate.AddSeconds(exam.Time * 60 + tnExamUser.Duration) || tnExamUser.IsEnd)
            {
                NopBai();
                Session[SessionKey.ExamUserObject] = tnExamUser;
                return RedirectToAction("Complete", "Home");
            }
            ViewBag.ExamTitle = exam.Content;
            ViewBag.Time = exam.Time * 60 - (DateTime.Now - tnExamUser.JoinDate).TotalSeconds + tnExamUser.Duration;
            var listTN_ExamUserAnswer = db.TN_ExamUserAnswer.Where(m => m.ExamUserID == tnExamUser.ID).ToList();

            return View(listTN_ExamUserAnswer);
        }

        public bool AnswerQuestion(int answerID, int tnExamUserAnswerID, bool value)
        {
            var tn_ExamUser = (TN_ExamUser)Session[SessionKey.ExamUserObject];
            var exam = (TN_Exam)Session[SessionKey.ExamObject];
            var obj = db.TN_ExamUserAnswer.FirstOrDefault(m => m.ID == tnExamUserAnswerID);
            if (obj != null && exam != null)
            {
                if (DateTime.Now > tn_ExamUser.JoinDate.AddSeconds(exam.Time * 60 + tn_ExamUser.Duration) || tn_ExamUser.IsEnd)
                {
                    NopBai();
                }
                if (value)
                {
                    obj.AnswerID = answerID;
                }
                else
                {
                    obj.AnswerID = null;
                }
                db.SaveChanges();
                return true;
            }
            return false;
        }

        public ActionResult Complete()
        {
            var tn_ExamUser = (TN_ExamUser)Session[SessionKey.ExamUserObject];
            if (tn_ExamUser == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var getTNExamUser = db.TN_ExamUser.FirstOrDefault(m => m.ID == tn_ExamUser.ID);

            if (getTNExamUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var listTN_ExamUserAnswer = getTNExamUser != null ? getTNExamUser.TN_ExamUserAnswer : null;

            int count = 0;
            if (listTN_ExamUserAnswer != null && listTN_ExamUserAnswer.Count > 0)
            {
                foreach (var item in listTN_ExamUserAnswer)
                {
                    bool pass = item.TN_Question != null && item.TN_Question.TN_Answer != null && item.TN_Question.TN_Answer.Count > 0;
                    var answerQ = pass ? item.TN_Question.TN_Answer.FirstOrDefault(m => m.IsCorrect) : null;
                    if (answerQ != null && answerQ.ID == item.AnswerID)
                    {
                        count++;
                    }
                }
            }
            getTNExamUser.CorrectCount = count;
            db.SaveChanges();

            return RedirectToAction("ExportPDF", "Home");
            //ViewBag.KetQua = count + " / " + listTN_ExamUserAnswer.Count;
            //return View(getTNExamUser);
        }

        public FileContentResult ExportPDF()
        {
            var tn_ExamUser = (TN_ExamUser)Session[SessionKey.ExamUserObject];
            if (tn_ExamUser == null) return null;

            var tnExamUser = db.TN_ExamUser.FirstOrDefault(m => m.ID == tn_ExamUser.ID);
            if (tnExamUser != null)
            {
                var user = tnExamUser.TN_User;
                string fileType = "pdf";
                string fileExtendsion = "pdf";

                var listTN_ExamUserAnswer = tnExamUser != null ? tnExamUser.TN_ExamUserAnswer : null;
                int count = 0;
                string[] getAnswer = { "A", "B", "C", "D", "E", "F", "G", "H" };
                if (listTN_ExamUserAnswer != null && listTN_ExamUserAnswer.Count > 0)
                {
                    DataTable dt = new DataSet1().KetQua;
                    for (int i = 0; i < listTN_ExamUserAnswer.Count; i++)
                    {
                        var quest = listTN_ExamUserAnswer.ElementAt(i).TN_Question;
                        var answerTrue = quest != null && quest.TN_Answer != null && quest.TN_Answer.Count > 0 ? quest.TN_Answer : null;
                        if (answerTrue != null && answerTrue.Count > 0)
                        {
                            string traloi = "", dapan = "";
                            for (int j = 0; j < answerTrue.Count; j++)
                            {
                                if (answerTrue.ElementAt(j).ID == listTN_ExamUserAnswer.ElementAt(i).AnswerID)
                                {
                                    traloi = getAnswer[j];
                                }
                                if (answerTrue.ElementAt(j).IsCorrect)
                                {
                                    dapan = getAnswer[j];
                                    if (answerTrue.ElementAt(j).ID == listTN_ExamUserAnswer.ElementAt(i).AnswerID)
                                    {
                                        count++;
                                    }
                                }
                            }
                            DataRow dr = dt.NewRow();
                            dr["CauHoi"] = "Câu " + (i + 1);
                            dr["TraLoi"] = traloi;
                            dr["DapAn"] = dapan;
                            dt.Rows.Add(dr);
                        }

                    }
                    LocalReport localReport = new LocalReport();
                    localReport.ReportPath = Server.MapPath("~/Views/Home/KetQua.rdlc");
                    string title = tnExamUser.TN_Exam != null ? tnExamUser.TN_Exam.Content : "";
                    ReportParameter param0 = new ReportParameter("Title", title);
                    ReportParameter param1 = new ReportParameter("Name", user.FullName);
                    ReportParameter param2 = new ReportParameter("SBD", user.SBD);
                    ReportParameter param3 = new ReportParameter("Mark", count + " / " + listTN_ExamUserAnswer.Count);

                    ReportDataSource reportDataSource = new ReportDataSource();
                    localReport.SetParameters(new ReportParameter[] { param0, param1, param2, param3 });
                    reportDataSource.Name = "DataSet1";
                    reportDataSource.Value = dt;
                    localReport.DataSources.Add(reportDataSource);
                    Byte[] mybytes = localReport.Render(fileType);

                    Response.AppendHeader("Content-Disposition", "inline; Ket-qua_" + user.SBD + "." + fileExtendsion);
                    return File(mybytes, "application/pdf");
                }
            }
            return null;

        }

        public bool NopBai()
        {
            var tn_ExamUser = (TN_ExamUser)Session[SessionKey.ExamUserObject];
            if (tn_ExamUser == null)
            {
                return false;
            }
            var tnExamUser = db.TN_ExamUser.FirstOrDefault(m => m.ID == tn_ExamUser.ID);
            if (tnExamUser != null)
            {
                var timeRest = (tnExamUser.TN_Exam.Time * 60 - (DateTime.Now - tnExamUser.JoinDate).TotalSeconds + tnExamUser.Duration);
                var time = (tnExamUser.TN_Exam.Time * 60 - timeRest) / (tnExamUser.TN_Exam.Time * 60);
                if (time >= (2.0 / 3.0))
                {
                    tnExamUser.IsEnd = true;
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        private List<TN_Question> GetRandomListQuestion(List<TN_Question> list, int count)
        {
            Random random = new Random();
            List<TN_Question> getList = new List<TN_Question>();
            List<int> listRDExists = new List<int>();
            for (int i = 0; i < count; i++)
            {
                int rnd = random.Next(0, list.Count);
                while (listRDExists.Contains(rnd))
                {
                    rnd = random.Next(0, list.Count);
                }
                listRDExists.Add(rnd);
                getList.Add(list.ElementAt(rnd));
            }
            return getList;
        }
    }

    public class SessionKey
    {
        public const string UserObject = "UserObject";
        public const string ExamObject = "ExamObject";
        public const string ExamUserObject = "ExamUserObject";
    }
}
