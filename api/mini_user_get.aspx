<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "hKC5nig2gEKJjktmponkbA==");
        string openId = MiniUsers.CheckSessionKey(sessionKey);
        MiniUsers user = new MiniUsers(openId);
        if (!user.role.Trim().Equals("staff"))
        {
            Response.Write("{\"status\": 1, \"err_msg\": \"Staff Only!\"}");
            Response.End();
        }

        string miniOpenId = Util.GetSafeRequestValue(Request, "openid", "oHdTn5W8g2KOGPYOYFQyXaW46EVo");
        DataTable dt = DBHelper.GetDataTable(" select * from mini_users where open_id = '" + miniOpenId.Trim() + "' ");
        string miniUserInfoJson = "";
        if (dt.Rows.Count == 1)
        {
            miniUserInfoJson = Util.ConvertDataFieldsToJson(dt.Rows[0]);
        }
        dt.Dispose();
        Response.Write("{\"status\": 0, \"count\": " + dt.Rows.Count.ToString() 
            + ", \"mini_users\": [" + miniUserInfoJson.Trim() + "]}");

    }
</script>