<%@ Page Language="C#" %>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessinKey = Util.GetSafeRequestValue(Request, "sessionkey", "edtjZxBstdJBMCBVXeW3LQ==");
        string openId = MiniUsers.CheckSessionKey(sessinKey);
        int id = int.Parse(Util.GetSafeRequestValue(Request, "id", "8593"));

        OrderTemp temp = new OrderTemp(id);

        int orderId = temp.PlaceOnlineOrder(openId.Trim());

        Response.Write("{\"order_id\": " + orderId.ToString() + ", \"notify\": \"http://mini.snowmeet.top/call_back_wepay.aspx\" }");
    }
</script>