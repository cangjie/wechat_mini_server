<%@ Page Language="C#" %>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        int id = int.Parse(Util.GetSafeRequestValue(Request, "id", "198"));
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "MFwiXRJpsjzr3cJ9m5tGTw==");

        string openId = MiniUsers.CheckSessionKey(sessionKey);
        EquipMaintainRequestInshop request = new EquipMaintainRequestInshop(id);
        MiniUsers user = new MiniUsers(openId);

        if (!user.role.Trim().Equals("staff") && !request.OwnerOpenId.Trim().Equals(openId.Trim()))
        {
            Response.Write("{\"status\": 1, \"error_message\": \"Staff only.\"}");
        }

        int orderId = EquipMaintainRequestInshop.PlaceOrder(id);
        Response.Write("{\"status\": 0, \"order_id\": " + orderId.ToString() + " }");
    }
</script>