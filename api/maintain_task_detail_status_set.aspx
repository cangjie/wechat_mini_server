<%@ Page Language="C#" %>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "hKC5nig2gEKJjktmponkbA==");
        string openId = MiniUsers.CheckSessionKey(sessionKey);
        //int id = 0;
        MiniUsers user = new MiniUsers(openId);
        if (!user.role.Trim().Equals("staff"))
        {
            Response.Write("{\"status\": 1, \"err_msg\": \"Staff Only!\"}");
            Response.End();
        }
        int detailId = int.Parse(Util.GetSafeRequestValue(Request, "id", "7"));
        string status = Server.UrlDecode(Util.GetSafeRequestValue(Request, "status", "已开始"));
        EquipMaintainTaskDetail detail = new EquipMaintainTaskDetail(detailId);
        bool ret = detail.SetStatus(status.Trim(), openId.Trim());
        if (ret)
        {
            Response.Write("{\"status\": 0, \"success\": 1 }");
        }
        else
        { 
            Response.Write("{\"status\": 0, \"success\": 0 }");
        }
    }
</script>