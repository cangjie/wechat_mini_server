<%@ Page Language="C#" %>
<%@ Import Namespace="System.IO" %>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "");
        //string uploadFileName = Util.GetSafeRequestValue(Request, "filename", Util.GetTimeStamp().ToString() + ".jpg");
        Stream s = Request.InputStream;
        if (!Directory.Exists(Server.MapPath("/upload")))
        {
            Directory.CreateDirectory(Server.MapPath("/upload"));
        }
        for (int i = 0; i < Request.Files.Count; i++)
        {
            Request.Files[i].SaveAs(Server.MapPath("/upload/" + Util.GetTimeStamp().Trim() + ".jpg"));
        }
        Response.Write("{\"status\": 0}");
    }
</script>
