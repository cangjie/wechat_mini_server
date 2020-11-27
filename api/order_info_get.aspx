<%@ Page Language="C#" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        int orderId = int.Parse(Util.GetSafeRequestValue(Request, "orderid", "9150"));
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "hKC5nig2gEKJjktmponkbA==");

        string openId = MiniUsers.CheckSessionKey(sessionKey);
        MiniUsers user = new MiniUsers(openId);
        OnlineOrder order = new OnlineOrder(orderId);
        string orderOpenId = order._fields["open_id"].ToString().Trim();
        if (!user.role.Trim().Equals("staff") && !orderOpenId.Trim().Equals(user.OpenId.Trim())
            && !orderOpenId.Trim().Equals(user.OfficialAccountOpenId.Trim()))
        {
            Response.Write("{\"status\": 1, \"error_message\": \"Have no right to get order info.\"}");
            Response.End();
        }
        Response.Write("{\"status\": 0, \"order_online\": " + Util.ConvertDataFieldsToJson(order._fields) + "  }");

        //if (order.ow)
    }

</script>