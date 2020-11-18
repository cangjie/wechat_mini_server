<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "cTMplhK6yvRFA4H77gKAyw==");
        string openId = MiniUsers.CheckSessionKey(sessionKey);
        if (openId.Trim().Equals(""))
        {
            Response.Write("{\"status\": 1, \"error_message\": \"session key invalid.\" }");
            Response.End();
        }
        MiniUsers user = new MiniUsers(openId);
        string cellNumber = user._fields["cell_number"].ToString();
        DataTable dt = DBHelper.GetDataTable(" select top 1 * from mini_user_cell_number_used where open_id = '"
            + openId.Trim() + "' order by [id] desc ");
        DateTime lastUpdateDate = DateTime.Parse("2020-1-1");
        if (dt.Rows.Count > 0)
        {
            lastUpdateDate = DateTime.Parse(dt.Rows[0]["create_date"].ToString());
        }
        bool needUpdate = false;
        if (DateTime.Now - lastUpdateDate > new TimeSpan(10, 0, 0, 0) || cellNumber.Trim().Equals(""))
        {
            needUpdate = true;
        }
        Response.Write("{\"status\": 0, \"number\": \"" + cellNumber.Trim() + "\", \"need_update\":"
            + (needUpdate?"1":"0") + "  }");

    }
</script>
