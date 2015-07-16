using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_CMS.Models;
using MVC_CMS.Utilities;
using MvcPaging;
using Microsoft.Reporting.WinForms;
using System.Data;

namespace MVC_CMS.Controllers
{
    [Authorize]
    public class ExamController : Controller
    {
        public const int PageSize = 10;

        //
        // GET: /Exam/


        public ActionResult Index(int? page)
        {
            int index = page.HasValue ? page.Value - 1 : 0;
            var db = DB.GetContext();
            var listExam = from exam in db.TN_Exam select exam;
            if (listExam != null && listExam.Count() > 0)
            {
                return View(listExam.OrderBy(m => m.ID).ToPagedList(index, PageSize));
            }

            return View();
        }

        //
        // GET: /Exam/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Exam/Create

        [HttpPost]
        public ActionResult Create(TN_Exam model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Content))
                {
                    ModelState.AddModelError("Content", "Bạn phải nhập vào tên cuộc thi");
                }
                else if (model.QuestionCount <= 0)
                {
                    ModelState.AddModelError("QuestionCount", "Số câu hỏi phải lớn hơn 0");
                }
                var allQuestion = DB.GetContext().TN_Question.Count();
                if (model.QuestionCount > allQuestion)
                {
                    ModelState.AddModelError("QuestionCount", "Số câu hỏi vượt quá " + allQuestion + "câu trong ngân hàng đề thi.");
                }
                else
                {
                    var listExamQuestion = CreateExamQuestion(model);
                    var db = DB.GetContext();
                    model.EndDate = model.EndDate.AddDays(1).AddSeconds(-1);
                    db.TN_Exam.AddObject(model);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        /// <summary>
        /// Tạo danh sách câu hỏi cho đợt thi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private List<TN_ExamQuestion> CreateExamQuestion(TN_Exam model)
        {
            var db = DB.GetContext();
            var random = new Random();
            var listIndex = new List<int>();
            var listQuestion = db.TN_Question.ToList();

            while (listIndex.Count < model.QuestionCount)
            {
                int index = random.Next(0, listQuestion.Count - 1);
                //Nếu index này chưa có trong danh sách listIndex
                if (!listIndex.Contains(index))
                {
                    listIndex.Add(index);
                }
            }

            var listExamQuestion = new List<TN_ExamQuestion>();
            foreach (var index in listIndex)
            {
                var examQuestion = new TN_ExamQuestion();
                examQuestion.QuestionID = listQuestion[index].ID;
                listExamQuestion.Add(examQuestion);
            }
            return listExamQuestion;
        }

        //
        // GET: /Exam/Edit/5

        public ActionResult Edit(int id)
        {
            var db = DB.GetContext();
            var exam = db.TN_Exam.SingleOrDefault(m => m.ID == id);
            if (exam != null)
            {
                return View(exam);
            }

            return RedirectToAction("Index");
        }

        //
        // POST: /Exam/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, TN_Exam model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Content))
                {
                    ModelState.AddModelError("Content", "Bạn phải nhập vào tên cuộc thi");
                }
                var allQuestion = DB.GetContext().TN_Question.Count();
                if (model.QuestionCount > allQuestion)
                {
                    ModelState.AddModelError("QuestionCount", "Hiện tại trong ngân hàng đề thi chỉ có " + allQuestion + " câu.");
                }
                else
                {
                    var db = DB.GetContext();
                    var exam = db.TN_Exam.SingleOrDefault(m => m.ID == id);
                    exam.Content = model.Content;
                    exam.StartDate = model.StartDate;
                    exam.EndDate = model.EndDate;
                    exam.QuestionCount = model.QuestionCount;
                    exam.Time = model.Time;
                    db.SaveChanges();
                    return RedirectToAction("Edit", new { id = id });
                }
            }
            return View(model);
        }

        //
        // GET: /Exam/Delete/5

        public string SetActive(int id)
        {
            var db = DB.GetContext();
            var exam = db.TN_Exam.SingleOrDefault(m => m.ID == id);
            if (exam != null)
            {
                if (exam.TN_ExamQuestion != null && exam.TN_ExamQuestion.Count >= exam.QuestionCount)
                {
                    exam.IsActive = true;
                    db.SaveChanges();
                    return "Kich hoạt cuộc thi thành công.";
                }
                return string.Format("Không thể kích hoạt cuộc thi do bạn đặt là: {0} câu. Trong khi ngân hàng chỉ có {1} câu.", exam.QuestionCount, exam.TN_ExamQuestion.Count);
            }
            return "Cuộc thi không tồn tại.";
        }

        public ActionResult Delete(int id)
        {
            var db = DB.GetContext();
            var exam = db.TN_Exam.SingleOrDefault(m => m.ID == id);
            if (exam != null)
            {
                db.TN_Exam.DeleteObject(exam);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult RemoveTest(int tnExamUserID)
        {
            var db = DB.GetContext();
            var tnExamUser = db.TN_ExamUser.FirstOrDefault(m => m.ID == tnExamUserID);
            if (tnExamUser != null)
            {
                db.TN_ExamUser.DeleteObject(tnExamUser);
                db.SaveChanges();
            }
            return RedirectToAction("ManagerTest", "Exam");
        }

        public FileContentResult ExportPDF(int tnExamUserID)
        {
            var db = DB.GetContext();
            var tnExamUser = db.TN_ExamUser.FirstOrDefault(m => m.ID == tnExamUserID);
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

        public FileContentResult ExportAllPDF()
        {
            var db = DB.GetContext();
            string fileType = "pdf";
            string fileExtendsion = "pdf";
            var listThiSinh = db.TN_ExamUser.Where(m => m.IsEnd).OrderBy(m => m.TN_User.FullName).ToList();
            if (listThiSinh.Count > 0)
            {
                int count = listThiSinh[0].TN_Exam.QuestionCount;
                string[] getAnswer = { "A", "B", "C", "D", "E", "F", "G", "H" };
                DataTable dt = new DataSet1().BCTH;
                foreach (var item in listThiSinh)
                {
                    DataRow dr = dt.NewRow();
                    dr["HoTen"] = item.TN_User.FullName;
                    dr["SBD"] = item.TN_User.SBD;
                    dr["KetQua"] = item.CorrectCount + " / " + count;
                    dt.Rows.Add(dr);
                }
                LocalReport localReport = new LocalReport();
                localReport.ReportPath = Server.MapPath("~/Views/Exam/BCTH.rdlc");
                string title = listThiSinh[0].TN_Exam != null ? listThiSinh[0].TN_Exam.Content : "";
                ReportParameter param0 = new ReportParameter("Title", title);
                ReportDataSource reportDataSource = new ReportDataSource();
                localReport.SetParameters(new ReportParameter[] { param0 });
                reportDataSource.Name = "DataSet1";
                reportDataSource.Value = dt;
                localReport.DataSources.Add(reportDataSource);
                Byte[] mybytes = localReport.Render(fileType);

                Response.AppendHeader("Content-Disposition", "inline; Tong-Hop-Ket-Qua." + fileExtendsion);
                return File(mybytes, "application/pdf");
            }
            return null;

        }

        public ActionResult ManagerTest(bool order = false)
        {
            var db = DB.GetContext();
            if (order)
            {
                var listExamUser = db.TN_ExamUser.Where(m => m.IsEnd).OrderByDescending(m => m.CorrectCount).ToList();
                return View(listExamUser);
            }
            else
            {
                var listExamUser = db.TN_ExamUser.Where(m => m.IsEnd).ToList();
                foreach (var item in listExamUser)
                {
                    item.JoinDate = item.JoinDate.AddSeconds(item.Duration);
                }
                listExamUser = listExamUser.OrderByDescending(m => m.JoinDate).ToList();
                return View(listExamUser);
            }

        }

        public ActionResult AddTimeForUser(int tnExamUserID)
        {
            var db = DB.GetContext();
            var obj = db.TN_ExamUser.FirstOrDefault(m => m.ID == tnExamUserID);
            return View(obj);
        }

        [HttpPost]
        public ActionResult AddTimeForUser(TN_ExamUser tnExamUser, string addTime)
        {
            if (string.IsNullOrEmpty(addTime)) return RedirectToAction("ManagerTest");

            double duration = 0;
            double.TryParse(addTime.Replace(".", ","), out duration);

            if (duration == 0) return RedirectToAction("ManagerTest");

            int minute = (int)duration;
            int second = (int)((duration - minute) * 60);

            var db = DB.GetContext();
            var obj = db.TN_ExamUser.FirstOrDefault(m => m.ID == tnExamUser.ID);
            if (obj != null)
            {
                var timeEnd = obj.JoinDate.AddSeconds(obj.TN_Exam.Time * 60 + obj.Duration);
                if (DateTime.Now > timeEnd)
                {
                    // xử lý
                    int timeforAdd = ((int)(DateTime.Now - timeEnd).TotalSeconds) + minute * 60 + second;
                    obj.Duration += timeforAdd;
                }
                else
                {
                    obj.Duration += minute * 60 + second;
                }
                obj.IsEnd = false;
                db.SaveChanges();
            }
            return RedirectToAction("ManagerTest");
        }
    }
}
