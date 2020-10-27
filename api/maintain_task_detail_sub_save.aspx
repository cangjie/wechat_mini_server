<%@ Page Language="C#" %>


<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "");
        string operOpenId = MiniUsers.CheckSessionKey(sessionKey.Trim());
        MiniUsers oper = new MiniUsers(operOpenId.Trim());
        if (!oper.role.Trim().Equals("staff"))
        {
            Response.Write("{\"status\": 1, \"err_msg\": \"Staff Only!\"}");
            Response.End();
        }
        int i = 0;
        for (; Request[i.ToString()] != null && !Request[i.ToString()].Trim().Equals(""); i++)
        {
            Response.Write(Server.UrlDecode(Request[i.ToString()])+"\r\n");
        }
    }
</script>