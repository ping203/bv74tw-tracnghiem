<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MVC_CMS.Models.ResetPasswordModel>" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
   Thiết lập lại mật khẩu
</asp:Content>

<asp:Content ID="resetPasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Reset Password</h2>
    <% var userModel = (MVC_CMS.Models.UserModel)ViewData["UserModel"]; %>
    <p>
        Sử dụng form dưới đây để <b><%= Html.Encode(userModel.UserName) %></b>'s password. 
    </p>
    <p>
       Mật khẩu mới có chiều dài nhỏ nhất là <%= Html.Encode(ViewData["PasswordLength"]) %> ký tự .
    </p>

    <% using (Html.BeginForm()) { %>
        <%= Html.ValidationSummary(true, "Thiết lập lại mật khẩu chưa thành công .Vui lòng kiểm tra và thử lại .") %>
        <div>
            <fieldset>
                <legend>Thông tin tài khoản</legend>
                
                <div class="editor-label">
                    Mật khẩu mới
                </div>
                <div class="editor-field">
                    <%= Html.PasswordFor(m => m.NewPassword) %>
                    <%= Html.ValidationMessageFor(m => m.NewPassword) %>
                </div>
                
                <div class="editor-label">
                   Nhập lại mật khẩu mới
                </div>
                <div class="editor-field">
                    <%= Html.PasswordFor(m => m.ConfirmPassword) %>
                    <%= Html.ValidationMessageFor(m => m.ConfirmPassword) %>
                </div>
                
                <p>
                    <input type="submit" value="Thay đổi" />
                </p>
            </fieldset>
        </div>
    <% } %>
            <div>
                <a href='<%= Url.Action("Index") %>'>
                    <img src="/Content/Images/back.gif" alt="Trở lại" />
                </a>            
                <%= Html.ActionLink("Trở lại", "EditUser", new { id = userModel.Id })%>
            </div>
</asp:Content>
