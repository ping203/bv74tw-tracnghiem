
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace MVC_CMS.Controllers
{
    public class FileController : Controller
    {
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload(int id, HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string url = ""; // url to return 
            string message = "Upload failure"; // message to display (optional) 

            // here logic to upload image 
            // and get file path of the image 
            if (upload.ContentLength > 0)
            {
                var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(upload.FileName);
                var path = Path.Combine(Server.MapPath("~/Upload/Images"), fileName);
                upload.SaveAs(path);

                // path of the image 
                path = "Upload/Images/" + fileName;

                // will create http://localhost/Upload/Images/my_uploaded_image.jpg 
                url = Request.Url.GetLeftPart(UriPartial.Authority) + "/" + path;

                // passing message success/failure 
                message = "Upload success";
            }

            // since it is an ajax request it requires this string 
            string output = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + url + "\", \"" + message + "\");</script></body></html>";
            return Content(output);
        }
    }
}
