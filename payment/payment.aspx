<%@ Page Language="C#" %>

<!DOCTYPE html>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {

    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script type="text/javascript" src="https://res.wx.qq.com/open/js/jweixin-1.3.2.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <%=Request.Url.ToString() %>
        </div>
    </form>
</body>
<script type="text/javascript" >
    wx.miniProgram.navigateTo({ url: '/pages/payment/payment?id=<%=Util.GetSafeRequestValue(Request, "product_id", "0")%>' });
</script>
</html>
