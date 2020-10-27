<%@ Page Language="C#" %>
<%@ Import Namespace="System.IO" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        StreamReader sr = new StreamReader(Request.InputStream);
        string postJson = sr.ReadToEnd();
        sr.Close();
        Response.Write(postJson.Trim());
    }
</script>