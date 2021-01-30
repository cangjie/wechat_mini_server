<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<!DOCTYPE html>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        WeixinPaymentOrder odr = new WeixinPaymentOrder("1612004687012191");
        //WeixinPaymentOrder odr = new WeixinPaymentOrder("1611832196012035");
        odr.Refund(0.01);
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
