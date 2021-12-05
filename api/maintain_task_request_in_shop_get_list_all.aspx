<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        DateTime currentDate = DateTime.Parse(Util.GetSafeRequestValue(Request, "date", DateTime.Now.ToShortDateString()));

        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "CblxlJGhC0nRQ2gyiK2mTw==");

        string openId = MiniUsers.CheckSessionKey(sessionKey);

        MiniUsers miniUser = new MiniUsers(openId);

        if (!miniUser.role.Trim().Equals("staff"))
        {
            Response.Write("{\"status\": 1, \"error_message\": \"Have no right.\"}");
            Response.End();
        }

        string sql = "select maintain_in_shop_request.*, pay_state, product.[name], sale_price, (sale_price + confirmed_additional_fee) as order_real_pay_price from  order_online"
            + " left join maintain_in_shop_request on order_id = order_online.[id] "
            + " left join product on product.[id] = maintain_in_shop_request.confirmed_product_id"
            + " where maintain_in_shop_request.[id] is not null  and pay_state = 1 and pay_time >= '" + currentDate.ToShortDateString() 
            + "' and pay_time < '" + currentDate.AddDays(1).ToShortDateString() + "' order by [id] desc " ;
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