<%@ Page Language="C#" %>
<%@ Import Namespace="System.IO" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string updateApiUrl = Request.Url.Scheme.Trim() + "://" + Request.Url.Authority.Trim() + "/api/update_table.aspx";


        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "");
        string openId = MiniUsers.CheckSessionKey(sessionKey);
        int id = 0;
        MiniUsers user = new MiniUsers(openId);
        if (!user.role.Trim().Equals("staff"))
        {
            Response.Write("{\"status\": 1, \"err_msg\": \"Staff Only!\"}");
            Response.End();
        }
        StreamReader sr = new StreamReader(Request.InputStream);
        string postJson = sr.ReadToEnd();
        sr.Close();

        //postJson = "{\"fields_data\": {\"cell_number\": \"\", \"contact_name\": \"cj\", \"contact_gender\": \"男\", \"user_relation\": \"\", \"body_length\": \"\", \"boot_length\": \"\", \"hobby\": \"\", \"edge_degree\": \"89\", \"candle_temperature\": \"高\", \"service_open_id\": \"@#$current_open_id$#@\"},  \"keys\": {\"id\": \"00000002\"}}";

        postJson = postJson.Replace("@#$current_open_id$#@", openId.Trim());

        Dictionary<string, object> keysObj = Util.GetObjectFromJsonByKey(postJson, "keys");
        id = int.Parse(keysObj["id"].ToString().Trim());

        int i = DBHelper.InsertData("maintain_task_log", new string[,] {
            {"task_id", "int", id.ToString() },
            {"oper_open_id", "varchar", openId.Trim() },
            {"oper", "varchar", "assign" }
        });
        if (i == 1)
        {
            DBHelper.UpdateData("maintain_task", new string[,] { { "service_status", "int", "1" } },
                new string[,] { { "id", "int", id.ToString() } }, Util.conStr);
            string resultJson = Util.GetWebContent(updateApiUrl + "?sessionkey=" + Server.UrlEncode(sessionKey.Trim()) + "&table=maintain_task", "POST", postJson, "");
            Response.Write(resultJson);
        }



    }
</script>