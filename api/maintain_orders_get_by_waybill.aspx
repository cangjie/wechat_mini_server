<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "hKC5nig2gEKJjktmponkbA==");
        string openId = MiniUsers.CheckSessionKey(sessionKey);
        int id = 0;
        MiniUsers user = new MiniUsers(openId);
        if (!user.role.Trim().Equals("staff"))
        {
            Response.Write("{\"status\": 1, \"err_msg\": \"Staff Only!\"}");
            Response.End();
        }

        string wayBill = Util.GetSafeRequestValue(Request, "waybill", "SF1190439112999");
        DataTable dt = DBHelper.GetDataTable(" select * from maintain_task left join covid19_service on task_id = maintain_task.id "
            + " where covid19_service.waybill_no = '" + wayBill.Trim() + "' ");
        string itemJson = "";
        foreach (DataRow dr in dt.Rows)
        {
            itemJson = itemJson + (!itemJson.Trim().Equals("") ? ", " : "") + Util.ConvertDataFieldsToJson(dr);
        }
        Response.Write("{\"status\": 0, \"count\": " + dt.Rows.Count.ToString() + ", \"maintain_task_arr\": [" + itemJson.Trim() + "] }");
    }
</script>