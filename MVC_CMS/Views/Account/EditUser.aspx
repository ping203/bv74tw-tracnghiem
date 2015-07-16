<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MVC_CMS.Models.UserModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Thông tin chi tiết tài khoản
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Thông tin chi tiết tài khoản</h2>
    <% using (Html.BeginForm())
       { %>
    <font style="color: Red">
        <%= Html.ValidationSummary(true, "Tài khoản đã sửa không thành công. Vui lòng kiểm tra và thử lại.") %>
    </font>
    <div>
        <fieldset>
            <legend>Thông tin tài khoản</legend>
            <div class="editor-label">
                Tên tài khoản
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(m => m.UserName, new { @readonly = true })%>
            </div>
            <div class="editor-label">
                Họ và tên
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(m => m.FullName) %>
                <%= Html.ValidationMessageFor(m => m.FullName) %>
            </div>
            <div class="editor-label">
                Địa chỉ
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(m => m.Address) %>
                <%= Html.ValidationMessageFor(m => m.Address) %>
            </div>
            <div class="editor-label">
                Cơ quan
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(m => m.Company) %>
                <%= Html.ValidationMessageFor(m => m.Company) %>
            </div>
            <div class="editor-label">
                Điện thoại
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(m => m.PhoneNumber) %>
                <%= Html.ValidationMessageFor(m => m.PhoneNumber) %>
            </div>
            <div class="editor-label">
                Email
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(m => m.Email) %>
                <%= Html.ValidationMessageFor(m => m.Email) %>
            </div>
            <div class="editor-label">
                Khóa tài khoản này ?
            </div>
            <div class="editor-field">
                <%= Html.CheckBoxFor(m => m.IsLockedOut) %>
                <%= Html.ValidationMessageFor(m => m.IsLockedOut) %>
            </div>
            <p>
                <input type="submit" value="Cập nhật" />
                <%= Html.ActionLink("Đặt lại mật khẩu", "ResetPassword", "Account", new { id = ViewData["Id"] }, null)%>
            </p>
        </fieldset>
    </div>
    <% } %>
    <div>
        <a href='<%= Url.Action("Index") %>'>
            <img src="/Content/Images/back.gif" alt="Trở lại danh sách" />
        </a>
        <%= Html.ActionLink("Trở lại danh sách", "Index")%>
    </div>
</asp:Content>
