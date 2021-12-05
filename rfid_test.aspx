<%@ Page Language="C#" %>
<%@ Import Namespace="System.IO" %>
<!DOCTYPE html>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        Stream postStream = Request.InputStream;
        string inputStr = new StreamReader(postStream).ReadToEnd().Trim();
        System.IO.File.AppendAllText(Server.MapPath("rfid_post.txt"), DateTime.Now.ToString() + "\r\n" + inputStr+"\r\n");
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
    </form>
</body>
</html>
