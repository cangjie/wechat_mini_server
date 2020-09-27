<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string openId = Util.GetSafeRequestValue(Request, "openid", "oZBHkjtfBjSy-xN2Akza7LM_r3GE");
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "75/7isywMN23jcOmMpk2YQ==");
        string adminOpenId = MiniUsers.CheckSessionKey(sessionKey);
        MiniUsers adminUser = new MiniUsers(adminOpenId);
        if (!adminUser.role.Trim().Equals("staff"))
        {
            Response.Write("{\"status\": 1, \"err_msg\": \"Staff Only!\"}");
            Response.End();
        }
        DataTable dtUsers = DBHelper.GetDataTable(" select * from users where open_id = '" + openId + "' ");
        if (dtUsers.Rows.Count > 0)
        {
            Response.Write(Util.ConvertDataFieldsToJson(dtUsers.Rows[0]));
        }
        else
        {
            Response.Write("{\"status\": 1, \"err_msg\": \"User not found.\"}");
        }
        dtUsers.Dispose();
    }
</script>