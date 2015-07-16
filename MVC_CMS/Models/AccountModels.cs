using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVC_CMS.Models
{

    #region Models

    [PropertiesMustMatch("NewPassword", "ConfirmPassword", ErrorMessage = "Mật khẩu mới và mật khẩu xác nhận không khớp nhau.")]
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        [DisplayName("Old password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "*")]
        [ValidatePasswordLength(ErrorMessage="trên 6 ký tự")]
        [DataType(DataType.Password)]
        [StringLength(128, ErrorMessage = "< 128 ký tự")]
        [DisplayName("New password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        [DisplayName("Confirm new password")]
        public string ConfirmPassword { get; set; }
    }

    [PropertiesMustMatch("NewPassword", "ConfirmPassword", ErrorMessage = "Mật khẩu mới và mật khẩu xác nhận không khớp nhau.")]
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "*")]
        [ValidatePasswordLength(ErrorMessage="trên 6 ký tự")]
        [DataType(DataType.Password)]
        [StringLength(128, ErrorMessage = "< 128 ký tự")]
        [DisplayName("New password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        [DisplayName("Confirm new password")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required(ErrorMessage = "Bạn phải nhập vào tên tài khoản")]
        [DisplayName("User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập vào mật khẩu")]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("Remember me?")]
        public bool RememberMe { get; set; }
    }

    [PropertiesMustMatch("Password", "ConfirmPassword", ErrorMessage = "Mật khẩu mới và mật khẩu xác nhận không khớp nhau.")]
    public class RegisterUserModel
    {
        [Required(ErrorMessage = "*")]
        [StringLength(50, ErrorMessage = "< 50 ký tự")]
        [DisplayName("User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "*")]
        [StringLength(50, ErrorMessage = "< 50 ký tự")]
        [DisplayName("Full name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.EmailAddress)]
        [StringLength(256, ErrorMessage = "< 256 ký tự")]
        [DisplayName("Email address")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [StringLength(50, ErrorMessage = "< 50 ký tự")]
        [DisplayName("Phone number")]
        public string PhoneNumber { get; set; }

        [StringLength(250, ErrorMessage = "< 250 ký tự")]
        [DisplayName("Address")]
        public string Address { get; set; }

        [StringLength(250, ErrorMessage = "< 250 ký tự")]
        [DisplayName("Company")]
        public string Company { get; set; }

        [Required(ErrorMessage = "*")]
        [ValidatePasswordLength(ErrorMessage="trên 6 ký tự")]
        [DataType(DataType.Password)]
        [StringLength(128, ErrorMessage = "< 128 ký tự")]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        [DisplayName("Confirm password")]
        public string ConfirmPassword { get; set; }
    }
    
    public class UserModel
    {
        public int Id { get; set; }
        public Guid AspnetUserId { get; set; }

        [Required(ErrorMessage = "*")]
        [StringLength(50, ErrorMessage = "< 50 ký tự")]
        [DisplayName("User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "*")]
        [StringLength(50, ErrorMessage = "< 50 ký tự")]
        [DisplayName("Full name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.EmailAddress)]
        [StringLength(256, ErrorMessage = "< 256 ký tự")]
        [DisplayName("Email address")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [StringLength(50, ErrorMessage = "< 50 ký tự")]
        [DisplayName("Phone number")]
        public string PhoneNumber { get; set; }

        [StringLength(250, ErrorMessage = "< 250 ký tự")]
        [DisplayName("Address")]
        public string Address { get; set; }

        [StringLength(250, ErrorMessage = "< 250 ký tự")]
        [DisplayName("Company")]
        public string Company { get; set; }

        [DisplayName("Locked")]
        public bool IsLockedOut { get; set; }
    }

    public class UserLogin
    {
        [Required(ErrorMessage = "*")]
        public string SBD { get; set; }

        [Required(ErrorMessage = "*")]
        public string FullName { get; set; }
    }

    #endregion

    #region Services
    // The FormsAuthentication type is sealed and contains static members, so it is difficult to
    // unit test code that calls its members. The interface and helper class below demonstrate
    // how to create an abstract wrapper around such a type in order to make the AccountController
    // code unit testable.

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
    }

    public class AccountMembershipService : IMembershipService
    {
        private readonly MembershipProvider _provider;

        public AccountMembershipService()
            : this(null)
        {
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Không thể để trống.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Không thể để trống.", "password");

            return _provider.ValidateUser(userName, password);
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Không thể để trống.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Không thể để trống.", "password");
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("Không thể để trống.", "email");

            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Không thể để trống.", "userName");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("Không thể để trống.", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("Không thể để trống.", "newPassword");

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
                return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }
    }

    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Không thể để trống.", "userName");

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
    #endregion

    #region Validation
    public static class AccountValidation
    {
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Tên người dùng này đã tồn tại. Vui lòng chọn tên khác.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Có 1 người dùng đã sử dụng địa chỉ email này. Vui lòng nhập địa chỉ email khác.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Mật khẩu không hợp lệ. Vui lòng nhập mật khẩu hợp lệ.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Địa chỉ email không hợp lệ. Vui lòng kiểu tra và thử lại.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "Câu trả lời để lấy lại mật khẩu không hợp lệ. Vui lòng kiểm tra và thử lại.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "Câu hỏi để lấy lại mật khẩu không hợp lệ. Vui lòng kiểm tra và thử lại.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Tên người dùng cung cấp không hợp lệ. Vui lòng kiểm tra và thử lại.";

                case MembershipCreateStatus.ProviderError:
                    return "Chứng thực trả về một lỗi. Vui lòng kiểm tra lại toàn bộ đầu vào và thử lại. Nếu bạn cho rằng hệ thống có vấn đề xin vui lòng liên hệ với quản trị hệ thống.";

                case MembershipCreateStatus.UserRejected:
                    return "Yêu cầu tạo tài khoản đã bị hủy bỏ. Vui lòng kiểm tra lại toàn bộ  đầu vào và thử lại. Nếu bạn cho rằng hệ thống có vấn đề xin vui lòng liên hệ với quản trị hệ thống.";

                default:
                    return "Một lỗi không rõ đã xuất hiện. Vui lòng kiểm tra lại toàn bộ đầu vào và thử lại. Nếu bạn cho rằng hệ thống có vấn đề xin vui lòng liên hệ với quản trị hệ thống.";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class PropertiesMustMatchAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "'{0}' và '{1}' không khớp.";
        private readonly object _typeId = new object();

        public PropertiesMustMatchAttribute(string originalProperty, string confirmProperty)
            : base(_defaultErrorMessage)
        {
            OriginalProperty = originalProperty;
            ConfirmProperty = confirmProperty;
        }

        public string ConfirmProperty { get; private set; }
        public string OriginalProperty { get; private set; }

        public override object TypeId
        {
            get
            {
                return _typeId;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                OriginalProperty, ConfirmProperty);
        }

        public override bool IsValid(object value)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
            object originalValue = properties.Find(OriginalProperty, true /* ignoreCase */).GetValue(value);
            object confirmValue = properties.Find(ConfirmProperty, true /* ignoreCase */).GetValue(value);
            return Object.Equals(originalValue, confirmValue);
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "'{0}' phải ít nhất {1} ký tự.";
        private readonly int _minCharacters = Membership.Provider.MinRequiredPasswordLength;

        public ValidatePasswordLengthAttribute()
            : base(_defaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                name, _minCharacters);
        }

        public override bool IsValid(object value)
        {
            string valueAsString = value as string;
            return (valueAsString != null && valueAsString.Length >= _minCharacters);
        }
    }
    #endregion

}
