<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionKey", "WD0/+sc6blmyX0ZwmzgYDg==");
        string openId = MiniUsers.CheckSessionKey(sessionKey);
        MiniUsers user = new MiniUsers(openId);
        if (!user.role.Trim().Equals("staff"))
        {
            Response.Write("{\"status\": 1, \"err_msg\": \"Staff Only!\"}");
            Response.End();
        }

        DataTable dt = DBHelper.GetDataTable(" select top 1 * from maintain_task where service_status = 0 order by create_date ");
        if (dt.Rows.Count != 1)
        {
            EquipMaintainTask task = new EquipMaintainTask();
            task._fields = dt.Rows[0];
            task.GetUserFilledInfo();

        }
        else
        {
            Response.Write("{\"status\": 0, \"count\": 0}");
        }
    }
</script>