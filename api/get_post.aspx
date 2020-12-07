<%@ Page Language="C#" %>
<%@ Import Namespace="System.IO" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        Stream s = Request.InputStream;
        Response.Write((new StreamReader(s)).ReadToEnd().Trim());
    }
</script>