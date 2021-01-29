<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "hKC5nig2gEKJjktmponkbA==");
        int id = int.Parse(Util.GetSafeRequestValue(Request, "id", "9"));

        string openId = MiniUsers.CheckSessionKey(sessionKey.Trim());
        MiniUsers mUser = new MiniUsers(openId);

        string sql = " select * from  expierence_list left join order_online on order_online.[id] = guarantee_order_id  where expierence_list.[id] = "
            + id.ToString() + "  " + (mUser.role.Trim().Equals("staff") ? "  " : " and guarantee_order_id <> 0 and order_online.open_id = '" + openId + "' ");

        DataTable dt = DBHelper.GetDataTable(sql);

        string dataJson = "";

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            dataJson = dataJson + ((i != 0) ? ", " : "") + Util.ConvertDataFieldsToJson(dt.Rows[i]);
        }


        Response.Write("{\"status\": 0, \"count\": " + dt.Rows.Count.ToString() + ", \"expierence_list_arr\": [" + dataJson.Trim() + "]}");
    }
</script>