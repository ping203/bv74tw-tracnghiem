using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using MVC_CMS.Models;
using MVC_CMS.Utilities;
using MvcPaging;

namespace MVC_CMS.Controllers
{
    [HandleError]
    public class AccountController : Controller
    {
        public const int PageSize = 10;

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            return View();
        }


        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Exam");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Tên tài khoản hoặc mật khẩu vừa cung cấp không đúng.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            FormsService.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/Register
        // **************************************

        public ActionResult Register()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterUserModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user

                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, "123456", "74tw@gmail.com");

                if (createStatus == MembershipCreateStatus.Success)
                {
                    MembershipUser membershipUser = Membership.GetUser(model.UserName);
                    if (membershipUser != null)
                    {
                        //Insert in to UsersInRoles 
                        Roles.AddUserToRole(membershipUser.UserName, RoleList.RegistedUser);

                        //Insert in to Reader User
                        var db = DB.GetContext();
                        var user = new User();
                        user.AspnetUserId = (Guid)membershipUser.ProviderUserKey;
                        user.FullName = model.FullName;
                        //user.Address = model.Address;
                        //user.PhoneNumber = model.PhoneNumber;
                        //user.Company = model.Company;
                        db.User.AddObject(user);
                        db.SaveChanges();

                        //Get user was inserted
                        var addedUser = db.User.SingleOrDefault(m => m.AspnetUserId == (Guid)membershipUser.ProviderUserKey);
                    }
                    FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "Mật khẩu hiện tại không đúng hoặc mật khẩu mới không hợp lệ.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public static UserModel CurrentUser
        {
            get
            {
                var aspnetUser = Membership.GetUser();
                if (aspnetUser == null)
                {
                    return null;
                }

                return GetUser(aspnetUser);
            }
        }

        public static UserModel GetUser(int userId)
        {
            var db = DB.GetContext();
            var user = db.User.SingleOrDefault(m => m.ID == userId);
            if (user == null)
            {
                return null;
            }

            return GetUser(user.AspnetUserId);
        }

        public static UserModel GetUser(Guid aspnetUserId)
        {
            var aspnetUser = Membership.GetUser(aspnetUserId);
            if (aspnetUser == null)
            {
                return null;
            }

            return GetUser(aspnetUser);
        }

        public static UserModel GetUser(string userName)
        {
            var aspnetUser = Membership.GetUser(userName);
            if (aspnetUser == null)
            {
                return null;
            }

            return GetUser((Guid)aspnetUser.ProviderUserKey);
        }

        public static UserModel GetUser(MembershipUser aspnetUser)
        {
            var db = DB.GetContext();
            var user = db.User.SingleOrDefault(m => m.AspnetUserId == (Guid)aspnetUser.ProviderUserKey);
            if (user == null)
            {
                return null;
            }

            return GetUser(aspnetUser, user);
        }

        public static UserModel GetUser(MembershipUser aspnetUser, User user)
        {
            var userModel = new UserModel();
            userModel.Id = user.ID;
            userModel.FullName = user.FullName;
            userModel.Address = user.Address;
            userModel.PhoneNumber = user.PhoneNumber;
            userModel.Company = user.Company;

            userModel.UserName = aspnetUser.UserName;
            userModel.AspnetUserId = (Guid)aspnetUser.ProviderUserKey;
            userModel.Email = aspnetUser.Email;
            userModel.IsLockedOut = GetMembershipUser(user.ID).IsLockedOut;
            return userModel;
        }

        public static MembershipUser GetMembershipUser(int userId)
        {
            var db = DB.GetContext();
            var user = db.User.SingleOrDefault(m => m.ID == userId);
            if (user == null)
            {
                return null;
            }

            var aspnetUser = Membership.GetUser(user.AspnetUserId);
            return aspnetUser;
        }

        public static bool IsAdmin
        {
            get
            {
                return Roles.IsUserInRole(RoleList.SuperAdmin) || Roles.IsUserInRole(RoleList.Admin);
            }
        }

        [Authorize]
        public ActionResult UserDetail()
        {
            return View(CurrentUser);
        }

        [HttpPost]
        public ActionResult UserDetail(UserModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipUser membershipUser = Membership.GetUser(User.Identity.Name);
                membershipUser.Email = model.Email;

                //Update MembershipUser
                Membership.UpdateUser(membershipUser);

                //Update Reader User
                var db = DB.GetContext();
                var user = db.User.SingleOrDefault(m => m.AspnetUserId == (Guid)membershipUser.ProviderUserKey);
                user.FullName = model.FullName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                user.Company = model.Company;
                db.SaveChanges();
            }
            return View(model);
        }

        [Authorize]
        public ActionResult Index(int? page)
        {

            int index = page.HasValue ? page.Value - 1 : 0;

            var listUserModel = new List<UserModel>();
            var aspnetUserList = Membership.GetAllUsers();
            if (aspnetUserList != null || aspnetUserList.Count > 0)
            {
                foreach (MembershipUser aspnetUser in aspnetUserList)
                {
                    string loginUserName = AccountController.CurrentUser.UserName;

                    if (Roles.IsUserInRole(aspnetUser.UserName, RoleList.SuperAdmin))
                    {
                        continue;//không hiện SuperAdmin
                    }

                    if (loginUserName == aspnetUser.UserName)
                    {
                        continue;//Không hiện chính mình
                    }

                    if (loginUserName != "sa" &&
                        loginUserName != "admin" &&
                        Roles.IsUserInRole(RoleList.Admin) &&
                        Roles.IsUserInRole(aspnetUser.UserName, RoleList.Admin))
                    {
                        continue;//Không hiện những user là Admin cùng cấp
                    }

                    var userModel = GetUser(aspnetUser);
                    listUserModel.Add(userModel);
                }
            }

            return View(listUserModel.OrderBy(m => m.Id).ToPagedList(index, PageSize));
        }

        [Authorize]
        public ActionResult Delete(int id)
        {
            var userModel = GetUser(id);
            if (userModel != null)
            {
                Membership.DeleteUser(userModel.UserName);
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Permistion(int id)
        {
            var userModel = GetUser(id);
            return View(userModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public ActionResult Permistion(int id, FormCollection collection)
        {
            var user = GetUser(id);
            if (user == null)
            {
                return View();
            }

            string userName = user.UserName;
            var roles = Roles.GetAllRoles();

            for (int i = 0; i < roles.Length; i++)
            {
                if (string.Equals(roles[i], RoleList.SuperAdmin))
                {
                    continue;
                }
                var inRole = Roles.IsUserInRole(userName, roles[i]);
                var chooseRole = collection[string.Format("ckbRole{0}", i)].Contains("true");
                if (chooseRole)
                {
                    if (!inRole)
                    {
                        Roles.AddUserToRole(userName, roles[i]);
                    }
                }
                else
                {
                    if (inRole)
                    {
                        Roles.RemoveUserFromRole(userName, roles[i]);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult EditUser(int id)
        {
            var editUser = GetUser(id);
            ViewData["Id"] = id;
            return View(editUser);
        }

        [HttpPost]
        public ActionResult EditUser(int id, UserModel model)
        {
            if (ModelState.IsValid)
            {
                //Update MembershipUser
                MembershipUser membershipUser = Membership.GetUser(GetUser(id).UserName);
                membershipUser.Email = model.Email;
                Membership.UpdateUser(membershipUser);

                var db = DB.GetContext();
                var aspNetUser = db.aspnet_Membership.SingleOrDefault(m => m.UserId == (Guid)membershipUser.ProviderUserKey);
                aspNetUser.IsLockedOut = model.IsLockedOut;
                db.SaveChanges();

                //Update Reader User
                db = DB.GetContext();
                var user = db.User.SingleOrDefault(m => m.AspnetUserId == (Guid)membershipUser.ProviderUserKey);
                user.FullName = model.FullName;
                user.PhoneNumber = model.PhoneNumber;
                user.Address = model.Address;
                user.Company = model.Company;
                db.SaveChanges();
                return RedirectToAction("EditUser", new { id = id });
            }
            else
            {
                return View(model);
            }
        }

        [Authorize]
        public ActionResult ResetPassword(int id)
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            ViewData["UserModel"] = GetUser(id);
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(int id, ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipUser user = GetMembershipUser(id);
                if (user.IsLockedOut)
                {
                    ModelState.AddModelError("", "Người dùng hiện thời đã bị khóa.");
                    ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
                    ViewData["UserModel"] = GetUser(id);
                    return View(model);
                }
                else
                {
                    var oldPassword = user.ResetPassword();
                    if (!MembershipService.ChangePassword(user.UserName, oldPassword, model.NewPassword))
                    {
                        ModelState.AddModelError("", "Mật khẩu hiện thời không đúng hoặc mật khẩu mới không hợp lệ.");
                    }
                    return RedirectToAction("EditUser", new { id = id });
                }
            }
            else
            {
                ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
                ViewData["UserModel"] = GetUser(id);
                return View(model);
            }
        }
    }
}