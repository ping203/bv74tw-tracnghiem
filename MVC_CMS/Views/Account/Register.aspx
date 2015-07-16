<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<MVC_CMS.Models.RegisterUserModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Đăng ký thành viên
</asp:Content>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Html.BeginForm())
       { %>
    <div style="line-height: 30px; font-size: 16px;" class="fwb">
        ĐĂNG KÝ THÀNH VIÊN</div>
    <p>
        Trường đánh dấu <font style="color: Red">(*)</font> là trường bắt buộc.
    </p>
    <p>
        Mật khẩu yêu cầu độ dài nhỏ nhất là <font style="color: Red">
            <%= Html.Encode(ViewData["PasswordLength"]) %></font> ký tự.
    </p>
    <p>
        <font style="color: Red">
            <%= Html.ValidationSummary(true, "Đăng ký chưa thành công. Vui lòng kiểm tra và thử lại.") %>
        </font>
    </p>
    <p>
        <label>
            Họ và tên:</label>
        <span class="requided">(*)</span>
        <%= Html.TextBoxFor(m => m.FullName, new { @class = "fl w250", @maxlength = "256" })%>
        <span class="requided">
            <%= Html.ValidationMessageFor(m => m.FullName) %></span>
    </p>
    <p>
        <label>
            Địa chỉ:</label>
        <span class="requided"></span>
        <%= Html.TextBoxFor(m => m.Address, new { @class = "fl w250", @maxlength = "256" })%>
        <span class="requided">
            <%= Html.ValidationMessageFor(m => m.Address) %></span>
    </p>
    <p>
        <label>
            Cơ quan:</label>
        <span class="requided"></span>
        <%= Html.TextBoxFor(m => m.Company, new { @class = "fl w250", @maxlength = "256" })%>
        <span class="requided">
            <%= Html.ValidationMessageFor(m => m.Company) %></span>
    </p>
    <p>
        <label>
            Điện thoại:</label>
        <span class="requided"></span>
        <%= Html.TextBoxFor(m => m.PhoneNumber, new { @class = "fl w250", @maxlength = "256" })%>
        <span class="requided">
            <%= Html.ValidationMessageFor(m => m.PhoneNumber) %></span>
    </p>
    <p>
        <label>
            Email:</label>
        <span class="requided">(*)</span>
        <%= Html.TextBoxFor(m => m.Email, new { @class = "fl w250", @maxlength = "256" })%>
        <span class="requided">
            <%= Html.ValidationMessageFor(m => m.Email) %></span>
    </p>
    <p>
        <label>
            Tên tài khoản:</label>
        <span class="requided">(*)</span>
        <%= Html.TextBoxFor(m => m.UserName, new { @class = "fl w250", @maxlength = "256" })%>
        <span class="requided">
            <%= Html.ValidationMessageFor(m => m.UserName) %></span>
    </p>
    <p>
        <label>
            Mật khẩu:</label>
        <span class="requided">(*)</span>
        <%= Html.PasswordFor(m => m.Password, new { @class = "fl w250", @maxlength = "256" })%>
        <span class="requided">
            <%= Html.ValidationMessageFor(m => m.Password) %></span>
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
        <input type="submit" value="Đăng ký" /></div>
    <br>
    <br>
    <br>
    <% } %>
</asp:Content>
