<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "CblxlJGhC0nRQ2gyiK2mTw==");

        string openId = MiniUsers.CheckSessionKey(sessionKey);

        MiniUsers miniUser = new MiniUsers(openId);

        string sql = "select maintain_in_shop_request.*, pay_state from  order_online"
            + " left join maintain_in_shop_request on order_id = order_online.[id] "
            + " where maintain_in_shop_request.[id] is not null and order_online.open_id in  ('" + openId + "', '" + miniUser.OfficialAccountOpenId + "', '" + miniUser.NewOfficialAccountOpenId + "' ) and pay_state = 1 order by [id] desc " ;
        DataTable dt = DBHelper.GetDataTable(sql);
        string jsonArray = "";
        foreach (DataRow dr in dt.Rows)
        {
            string jsonItem = Util.ConvertDataFieldsToJson(dr);
            jsonArray = jsonArray + (!jsonArray.Trim().Equals("") ? ", " : "") + jsonItem.Trim();
        }
        Response.Write("{\"status\": 0, \"count\": " + dt.Rows.Count.ToString()
            + ", \"maintain_in_shop_request\":[" + jsonArray + "]}");

    }

</script>