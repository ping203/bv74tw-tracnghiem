using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_CMS.Models;
using MVC_CMS.Utilities;
using MvcPaging;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Reflection;

namespace MVC_CMS.Controllers
{
    [Authorize]
    public class QuestionController : Controller
    {
        public const int PageSize = 10;
        TRACNGHIEMEntities db = DB.GetContext();
        //
        // GET: /Question/


        public ActionResult Index(int? page)
        {
            int index = page.HasValue ? page.Value - 1 : 0;
            var db = DB.GetContext();
            var listQuestion = from question in db.TN_Question select question;
            if (listQuestion != null && listQuestion.Count() > 0)
            {
                return View(listQuestion.OrderBy(m => m.ID).ToPagedList(index, PageSize));
            }

            return View();
        }

        //
        // GET: /Question/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Question/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(TN_Question model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Content))
                {
                    ModelState.AddModelError("", "Bạn phải nhập vào nội dung câu hỏi");
                }
                else
                {
                    var db = DB.GetContext();
                    db.TN_Question.AddObject(model);
                    db.SaveChanges();
                    return RedirectToAction("Edit", new { id = model.ID });
                }
            }
            return View(model);
        }

        //
        // GET: /Question/Edit/5

        public ActionResult Edit(int id, bool? isAddAnswer)
        {
            ViewData["AddAnswer"] = isAddAnswer.HasValue && isAddAnswer.Value;
            var db = DB.GetContext();
            var question = db.TN_Question.SingleOrDefault(m => m.ID == id);
            if (question != null)
            {
                return View(question);
            }

            return RedirectToAction("Index");
        }

        //
        // POST: /Question/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, TN_Question model)
        {
            var db = DB.GetContext();
            var question = db.TN_Question.SingleOrDefault(m => m.ID == id);
            question.Content = model.Content;
            question.IsMultiChoose = model.IsMultiChoose;
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = id });
        }

        //
        // GET: /Question/Delete/5

        public ActionResult Delete(int id)
        {
            var db = DB.GetContext();
            var question = db.TN_Question.SingleOrDefault(m => m.ID == id);
            if (question != null)
            {
                db.TN_Question.DeleteObject(question);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        //
        // GET: /Question/DeleteAnswer/5

        public ActionResult DeleteAnswer(int id, int answerID)
        {
            var db = DB.GetContext();
            var answer = db.TN_Answer.SingleOrDefault(m => m.ID == answerID);
            if (answer != null)
            {
                db.TN_Answer.DeleteObject(answer);
                db.SaveChanges();
            }
            return RedirectToAction("Edit", new { id = id });
        }

        //
        // GET: /Question/AddAnswer/5

        public ActionResult AddAnswer(int id)
        {
            return RedirectToAction("Edit", new { id = id, isAddAnswer = true });
        }

        //
        // POST: /Question/SaveAnswer/5

        [HttpPost]
        public ActionResult SaveAnswer(int id, FormCollection collection)
        {
            var db = DB.GetContext();
            var question = db.TN_Question.SingleOrDefault(m => m.ID == id);
            if (question != null)
            {
                var rbAnswer = collection["rb" + id.ToString()];
                //Sửa câu trả lời cũ
                foreach (var answer in question.TN_Answer)
                {
                    var content = collection["txt" + answer.ID.ToString()];
                    if (string.IsNullOrEmpty(content))
                    {
                        ModelState.AddModelError("", "Bạn phải nhập vào nội dung câu trả lời");
                        return View(question);
                    }
                    else
                    {
                        var cbAnswer = collection["cb" + answer.ID.ToString()];
                        answer.IsCorrect = (!string.IsNullOrEmpty(cbAnswer) && cbAnswer.Contains("true")) || (!string.IsNullOrEmpty(rbAnswer) && rbAnswer.Contains(answer.Content));
                        answer.Content = content;
                    }
                }

                //Thêm câu trả lời mới
                if (!string.IsNullOrEmpty(collection["txtAddAnswer"]))
                {
                    var answer = new TN_Answer();
                    var cbAnswer = collection["cbAddAnswer"];
                    var txtAnswer = collection["txtAddAnswer"];
                    if (string.IsNullOrEmpty(txtAnswer))
                    {
                        ModelState.AddModelError("", "Bạn phải nhập vào nội dung câu trả lời");
                        return View(question);
                    }
                    else
                    {
                        answer.IsCorrect = (!string.IsNullOrEmpty(cbAnswer) && cbAnswer.Contains("true")) || (!string.IsNullOrEmpty(rbAnswer) && rbAnswer.Contains("AddAnswer"));
                        answer.QuestionID = id;
                        answer.Content = txtAnswer.Trim();

                        question.TN_Answer.Add(answer);
                    }
                }

                db.SaveChanges();
            }

            return RedirectToAction("Edit", new { id = id });
        }

        public ActionResult CreateQuestionFromExcelFile()
        {
            var listExam = db.TN_Exam.ToList();
            return View(listExam);
        }

        [HttpPost]
        public ActionResult CreateQuestionFromExcelFile(int examID, HttpPostedFileBase file)
        {
            try
            {
                if (file != null)
                {
                    var fileUtil = new FileUtil();
                    string folderUpload = @"D:\FileUploads\";
                    if (!fileUtil.CheckDirectoryExists(folderUpload))
                    {
                        fileUtil.CreateDirectory(folderUpload);
                    }
                    string fileName = "nganhangcauhoi" + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    file.SaveAs(folderUpload + fileName);

                    // đọc file excel và lưu vào csdl
                    //Stream streamDictionary = GetResourceFileStream(folderUpload + fileName);
                    using (var document = SpreadsheetDocument.Open(folderUpload + fileName, false))
                    {
                        var workbookPart = document.WorkbookPart;
                        var workbook = workbookPart.Workbook;

                        var sheets = workbook.Descendants<Sheet>();
                        foreach (var sheet in sheets)
                        {
                            if (sheet.SheetId.HasValue && sheet.SheetId.ToString().Equals("1"))
                            {
                                #region Read excel

                                var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                                var sharedStringPart = workbookPart.SharedStringTablePart;
                                var values = sharedStringPart.SharedStringTable.Elements<SharedStringItem>().ToArray();

                                var cells = worksheetPart.Worksheet.Descendants<Cell>();
                                var rows = worksheetPart.Worksheet.Descendants<Row>();
                                TN_Question questionTemp = new TN_Question();
                                int countDataNull = 0;
                                bool flag = false;
                                foreach (Row row in rows)
                                {
                                    var question = new TN_Question();
                                    var answer = new TN_Answer();
                                    foreach (Cell c in row.Elements<Cell>())
                                    {
                                        string value = string.Empty;
                                        if ((c.DataType != null) && (c.DataType == CellValues.SharedString))
                                        {
                                            int ssid = int.Parse(c.CellValue.Text);
                                            value = sharedStringPart.SharedStringTable.ChildElements[ssid].InnerText;
                                        }
                                        else if (c.CellValue != null)
                                        {
                                            value = c.CellValue.Text;
                                        }

                                        // xử lý ban đầu
                                        value = value.TrimEnd('%');

                                        if (c.CellReference == "A" + row.RowIndex)
                                        {
                                            if (!string.IsNullOrEmpty(value))
                                            {
                                                question.Content = value;
                                                answer.Content = value;
                                                countDataNull = 0;
                                            }
                                            else
                                            {
                                                countDataNull++;
                                                break;
                                            }
                                        }
                                        if (c.CellReference == "B" + row.RowIndex)
                                        {
                                            if (!string.IsNullOrEmpty(value))
                                            {
                                                answer.IsCorrect = true;
                                            }
                                            else
                                            {
                                                answer.IsCorrect = false;
                                            }
                                        }
                                        if (c.CellReference == "C" + row.RowIndex && !string.IsNullOrEmpty(value))
                                        {
                                            if (!string.IsNullOrEmpty(value))
                                            {
                                                question.Type = value;
                                                flag = true;
                                            }
                                        }
                                    }

                                    if (countDataNull == 0)
                                    {
                                        if (flag)
                                        {
                                            question.IsMultiChoose = false;
                                            db.TN_Question.AddObject(question);

                                            questionTemp = question;

                                            var questionExam = new TN_ExamQuestion()
                                            {
                                                ExamID = examID,
                                                QuestionID = question.ID

                                            };
                                            db.TN_ExamQuestion.AddObject(questionExam);
                                            flag = false;
                                        }
                                        else
                                        {
                                            answer.QuestionID = questionTemp.ID;
                                            db.TN_Answer.AddObject(answer);
                                        }
                                        db.SaveChanges();
                                    }
                                }
                                #endregion
                                break;
                            }
                        }
                    }
                    ViewBag.Success = true;
                }
            }
            catch (Exception ex)
            {

            }
            return View(new List<TN_Exam>());
        }

        private Stream GetResourceFileStream(string fileName)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            // Get all embedded resources
            string[] arrResources = currentAssembly.GetManifestResourceNames();

            foreach (string resourceName in arrResources)
            {
                if (resourceName.Contains(fileName))
                {
                    return currentAssembly.GetManifestResourceStream(fileName);
                }
            }
            return currentAssembly.GetManifestResourceStream(fileName);

            return null;
        }
    }
}
