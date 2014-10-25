<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DemoUserControl.ascx.cs" Inherits="BissTalk.SitecoreSynergy.Layouts.BissTalk.Synergy.Sample.Sublayouts.DemoUserControl" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.WebControls" Assembly="Sitecore.Kernel" %>

<h1><sc:FieldRenderer ID="FieldRender" FieldName="PageTitle" runat="server"></sc:FieldRenderer></h1>
<h3><sc:FieldRenderer ID="FieldRender1" FieldName="PageDescription" runat="server"></sc:FieldRenderer></h3>
<table>
    <thead>
        <tr>
            <td>Name</td>
            <td>Path</td>
        </tr>
    </thead>
    <asp:Repeater runat="server" ID="RowRepeater">
        <ItemTemplate>
            <tr>
                <td><%#DataBinder.Eval(Container.DataItem, "Name") %></td>
                <td><%#DataBinder.Eval(Container.DataItem, "Path") %></td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</table>

<asp:Literal runat="server" ID="OutputStuff"></asp:Literal>

<br />
<br />

<asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" style="height: 26px" />