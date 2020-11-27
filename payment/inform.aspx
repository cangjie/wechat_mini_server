<%@ Page Language="C#" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Xml" %>
<script runat="server">

    public string signOri = "";
    public string sign = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        XmlDocument xmlD = new XmlDocument();
        xmlD.Load(Server.MapPath("c.xml"));
        signOri = xmlD.SelectSingleNode("//xml/sign").InnerText.Trim();
        XmlNode root = xmlD.SelectSingleNode("//xml");
        root.RemoveChild(xmlD.SelectSingleNode("//xml/sign"));
        string xmlStr = Util.ConverXmlDocumentToStringPair(xmlD);
        
        sign = Util.GetMd5Sign(xmlStr, "abcdefghijklmnopqrstuvwxyz123456");
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <%=signOri %><br />
            <%=sign %>
        </div>
    </form>
</body>
</html>
