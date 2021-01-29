<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Xml" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        int id = int.Parse(Util.GetSafeRequestValue(Request, "id", "10"));
        double amount = double.Parse(Util.GetSafeRequestValue(Request, "amount", "0.01"));

        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "hKC5nig2gEKJjktmponkbA==");
        string openId = MiniUsers.CheckSessionKey(sessionKey);
        MiniUsers user = new MiniUsers(openId);
        if (!user.role.Trim().Equals("staff"))
        {
            Response.Write("{\"status\": 1, \"error_message\": \"Staff Only!\"}");
            Response.End();
        }
        Expierence expierence = new Expierence(id);
        int orderId = int.Parse(expierence._fields["guarantee_order_id"].ToString());
        if (orderId == 0)
        {
            Response.Write("{\"status\": 1, \"error_message\": \"Guarantee cash not paid.\"}");
            Response.End();
        }
        OnlineOrder order = new OnlineOrder(orderId);
        double orderTotalFee = double.Parse(order._fields["order_real_pay_price"].ToString());
        if (amount > orderTotalFee)
        {
            Response.Write("{\"status\": 1, \"error_message\": \"Refund amount greater than order price.\"}");
            Response.End();
        }

        string outTradeNo = order._fields["out_trade_no"].ToString();
        WeixinPaymentOrder payOrder = new WeixinPaymentOrder(outTradeNo);
        bool ret = payOrder.Refund(amount);
        Response.Write("{\"status\": 0, \"refund_status\": " + (ret ? "1" : "0") + "}");
    }

</script>