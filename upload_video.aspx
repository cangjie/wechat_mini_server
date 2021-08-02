<%@ Page Language="C#" %>
<%@ Import Namespace="System.IO" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "");
        string openId = MiniUsers.CheckSessionKey(sessionKey);
        string timeStamp = Util.GetSafeRequestValue(Request, "timestamp", "").Trim();
        MiniUsers user = new MiniUsers(openId);
        if (!user.role.Trim().Equals("staff"))
        {
            Response.Write("{ \"status\": 1, \"error_message\": \"Have no permission to upload file.\" }");
            Response.End();

        }
        //string uploadFileName = Util.GetSafeRequestValue(Request, "filename", Util.GetTimeStamp().ToString() + ".jpg");
        Stream s = Request.InputStream;
        if (!Directory.Exists(Server.MapPath("/upload")))
        {
            Directory.CreateDirectory(Server.MapPath("/upload"));
        }
        string dateStr = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0')
            + DateTime.Now.Day.ToString().PadLeft(2, '0');
        if (!Directory.Exists(Server.MapPath("/upload/" + dateStr)))
        {
            Directory.CreateDirectory(Server.MapPath("/upload/" + dateStr));
        }
        string filesJson = "";
        for (int i = 0; i < Request.Files.Count; i++)
        {
            string fileName = Util.GetTimeStamp().Trim();
            string oriFileName = Request.Files[i].FileName.Trim();
            string ext = oriFileName.Split('.')[oriFileName.Split('.').Length - 1].Trim();
            string fullFileName = "/upload/" + dateStr + "/" + fileName.Trim() + "." + ext.Trim();
            if (ext.Trim().Equals("mp4") && !timeStamp.Trim().Equals(""))
            { 
                fullFileName = "/upload/" + dateStr + "/" + timeStamp.Trim() + "." + ext.Trim();
            }
            try
            {
                Request.Files[i].SaveAs(Server.MapPath(fullFileName));
            }
            catch
            {
                continue;
            }
            DBHelper.InsertData("mini_upload", new string[,] { { "owner", "varchar", openId.Trim() },
                { "file_path_name", "varchar", fullFileName.Trim() } });
            filesJson = filesJson + ((i != 0) ? "," : "") + fullFileName.Trim();
        }
        //Response.Write("{\"status\": 0, \"files\": [" + filesJson + "]}");
        Response.Write(filesJson.Trim());
    }
</script>
