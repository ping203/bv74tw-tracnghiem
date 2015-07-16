<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<MVC_CMS.Models.ChangePasswordModel>" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Thay đổi mật khẩu
</asp:Content>
<asp:Content ID="changePasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Html.BeginForm())
       { %>
    <div style="line-height: 30px; font-size: 16px;" class="fwb">
        THAY ĐỔI MẬT KHẨU</div>
    <p>
        Trường đánh dấu <font style="color: Red">(*)</font> là trường bắt buộc.
    </p>
    <p>
        Mật khẩu yêu cầu độ dài nhỏ nhất là <font style="color: Red">
            <%= Html.Encode(ViewData["PasswordLength"]) %></font> ký tự.
    </p>
    <p>
        <font style="color: Red">
            <%= Html.ValidationSummary(true, "Thay đổi chưa thành công. Vui lòng kiểm tra và thử lại.") %>
        </font>
    </p>
    <p>
        <label>
            Mật khẩu cũ:</label>
        <span class="requided">(*)</span>
        <%= Html.PasswordFor(m => m.OldPassword, new { @class = "fl w250", @maxlength = "256" })%>
        <span class="requided">
            <%= Html.ValidationMessageFor(m => m.OldPassword) %></span>
    </p>
    <p>
        <label>
            Mật khẩu mới:</label>
        <span class="requided">(*)</span>
        <%= Html.PasswordFor(m => m.NewPassword, new { @class = "fl w250", @maxlength = "256" })%>
        <span class="requided">
            <%= Html.ValidationMessageFor(m => m.NewPassword) %></span>
    </p>
    <p>
        <label>
            Nhập lại mật khẩu:</label>
        <span class="requided">(*)</span>
        <%= Html.PasswordFor(m => m.ConfirmPassword, new { @class = "fl w250", @maxlength = "256" })%>
        <span class="requided">
            <%= Html.ValidationMessageFor(m => m.ConfirmPassword) %></span>
    </p>
    <br>
    <br>
    <br>
    <br>
    <div align="center">
        <input type="submit" value="Thay đổi" /></div>
    <br>
    <br>
    <br>
    <% } %>
</asp:Content>
