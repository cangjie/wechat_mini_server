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
        string sql = Util.GetSafeRequestValue(Request, "sql", " select * from waybill_log");
        try
        {
            DataTable dt = DBHelper.GetDataTable(sql);
            string results = "";
            foreach (DataRow dr in dt.Rows)
            {
                results = results + ((!results.Trim().Equals("")) ? ", " : "") + Util.ConvertDataFieldsToJson(dr).Trim();
            }
            Response.Write("{\"status\": 0, \"count\": " + dt.Rows.Count.ToString() + ", \"rows\": [" + results + "] }");
        }
        catch
        {
            Response.Write("{\"status\": 1, \"error_message\": \"sql error\"}");
        }
    }
</script>