<%@ Page Language="C#" %>

<!DOCTYPE html>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessinKey = Util.GetSafeRequestValue(Request, "sessionkey", "edtjZxBstdJBMCBVXeW3LQ==");
        string openId = MiniUsers.CheckSessionKey(sessinKey);
        int id = int.Parse(Util.GetSafeRequestValue(Request, "id", "8593"));

        OrderTemp temp = new OrderTemp(id);

        int orderId = temp.PlaceOnlineOrder(openId.Trim());

        Response.Write("{\"order_id\": " + orderId.ToString() + " }");

    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
    </form>
</body>
</html>
