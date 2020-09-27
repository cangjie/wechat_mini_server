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

        DataTable dt = DBHelper.GetDataTable(" select top 1 * from maintain_task where service_status = 0 order by create_date ");
        if (dt.Rows.Count == 1)
        {
            EquipMaintainTask task = new EquipMaintainTask();
            task._fields = dt.Rows[0];
            task.GetUserFilledInfo();
            string jsonTaskInfo = Util.ConvertDataFieldsToJson(task._fields);
            string userFilledInfo = Util.ConvertDataFieldsToJson(task.userFilledEquipmentInfo._fields);
            Response.Write("{\"status\": 0, \"count\": 1, \"maintain_task_arr\": [{\"maintain_task\": " + jsonTaskInfo.Trim() 
                + ", \"user_filled_info\": " + userFilledInfo.Trim() + "} ]}");

        }
        else
        {
            Response.Write("{\"status\": 0, \"count\": 0}");
        }
    }
</script>