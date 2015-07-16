<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MVC_CMS.Models.UserModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Phân quyền
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Phân quyền</h2>
    
     <% using (Html.BeginForm())
       {           
        %>
            <fieldset>
                <legend>Phân quyền cho: <b><%= Model.FullName %></b></legend>
                <table cellpadding="0" cellspacing="0">
                        <%
                            string[] roles = Roles.GetAllRoles();
           
                            for (int i = 0; i < roles.Length; i++)
                            {
                                if (!string.Equals(roles[i], MVC_CMS.Models.RoleList.SuperAdmin))
                                {
                                    %>
                                    <tr>
                                        <th style="width:5pt"><%= Html.CheckBox(string.Format("ckbRole{0}", i), Roles.IsUserInRole(Model.UserName, roles[i]))%></th>
                                        <th><%= Html.Encode(roles[i])%></th>
                                    </tr>
                                    <%
                                }
                            }%>                            
                        <tr>
                            <th></th>
                            <th><input type="submit" value="Lưu" /></th>
                        </tr>
                </table>
            </fieldset>   
     <%} %>
            <div>
                <a href='<%= Url.Action("Index") %>'>
                    <img src="/Content/Images/back.gif" alt="Trở lại danh sách" />
                </a>            
                <%= Html.ActionLink("Trở lại danh sách", "Index")%>
            </div>

</asp:Content>
