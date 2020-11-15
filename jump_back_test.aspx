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
    <script type="text/javascript" >
        function goback() {
            document.write(wx.miniProgram);
            //wx.miniProgram.navigateTo({ url: '/logs/logs' });
        }
    </script>
</head>
<body>
    <div>
        <input type="button" value="Go To Log" onclick="goback()" />
    </div>
</body>
</html>
