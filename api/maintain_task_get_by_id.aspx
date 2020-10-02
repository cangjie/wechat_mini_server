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
        int orderId = int.Parse(Util.GetSafeRequestValue(Request, "orderid", "0000000108"));
        DataTable dt = DBHelper.GetDataTable(" select * from maintain_task left join waybill_log on waybill_log.waybill_no = maintain_task.waybill_no "
            + " where maintain_task.[id] = " + orderId.ToString().Trim());
        if (dt.Rows.Count == 1)
        {
            EquipMaintainTask task = new EquipMaintainTask();
            task._fields = dt.Rows[0];
            task.GetUserFilledInfo();
            Response.Write("{\"status\": 0, \"maintain_task\": " + Util.ConvertDataFieldsToJson(task._fields).Trim()
                + ", \"user_filled_info\": " + Util.ConvertDataFieldsToJson(task.userFilledEquipmentInfo._fields) + " }");
        }
        else
        { 
            Response.Write("{\"status\": 1, \"error_message\": \"Not Found.\"}");
        }
    }
</script>