<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebFormLayout.aspx.cs" Inherits="BissTalk.SitecoreSynergy.Layouts.BissTalk.Synergy.Sample.Layouts.WebFormLayout" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.WebControls" Assembly="Sitecore.Kernel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><sc:FieldRenderer runat="server" FieldName="PageTitle"/></title>
    
</head>
<body>
    <form id="form1" runat="server">
        <sc:Placeholder ID="MainPlaceHolder" Key="main" runat="server" />
    </form>
</body>
</html>
