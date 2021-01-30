<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Xml" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        int id = int.Parse(Util.GetSafeRequestValue(Request, "id", "10"));
        double amount = double.Parse(Util.GetSafeRequestValue(Request, "amount", "0.01"));
        string memo = Util.GetSafeRequestValue(Request, "memo", "");

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

        DataTable dtRefund = DBHelper.GetDataTable(" select top 1 * from weixin_payment_orders_refund where out_trade_no = '"
            + outTradeNo.Trim() + "' order by [id] desc ");
        int refundId = 0;
        if (dtRefund.Rows.Count > 0)
        {
            refundId = int.Parse(dtRefund.Rows[0]["id"].ToString());
        }

        try
        {
            DBHelper.UpdateData("expierenct_list", new string[,] { {"return_memo", "varchar", memo.Trim() }, {"refund_amount", "float", amount.ToString() },
                {"refund_id", "int", refundId.ToString() } }, new string[,] { { "id", "int", id.ToString() } }, Util.conStr.Trim());
        }
        catch
        { 
        
        }

        Response.Write("{\"status\": 0, \"refund_status\": " + (ret ? "1" : "0") + ", \"refund_id\": " + refundId.ToString() + " }");
    }

</script>