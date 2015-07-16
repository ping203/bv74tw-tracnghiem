<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<MVC_CMS.Models.UserModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Chi tiết tài khoản
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Html.BeginForm())
       { %>
    <div style="line-height: 30px; font-size: 16px;" class="fwb">
        SỬA THÔNG TIN CÁ NHÂN</div>
    <p>
        <%= Html.ActionLink("Đổi mật khẩu", "ChangePassword", "Account", new { }, new { @class = "clf0 fwb" })%>
    </p>
    <p>
        Trường đánh dấu <font style="color: Red">(*)</font> là trường bắt buộc.
    </p>
    <p>
        <font style="color: Red">
            <%= Html.ValidationSummary(true, "Sửa đổi chưa thành công. Vui lòng kiểm tra và thử lại.") %>
        </font>
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
    <br>
    <br>
    <br>
    <br>
    <div align="center">
        <input type="submit" value="Lưu thay đổi" /></div>
    <br>
    <br>
    <br>
    <% } %>
</asp:Content>
