<%@ Page Language="C#" %>

<!DOCTYPE html>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string scheme = Request.Url.Scheme.Trim();
        string callBack = Util.GetSafeRequestValue(Request, "callback",
            ((Request.UrlReferrer==null) ? "" : Request.UrlReferrer.ToString().Trim()));
        Response.Redirect("https://open.weixin.qq.com/connect/oauth2/authorize?appid=" 
            + System.Configuration.ConfigurationSettings.AppSettings["wxappid"].Trim()   
            + "&redirect_uri=" + Server.UrlEncode(scheme.Trim() + "://"
            + System.Configuration.ConfigurationSettings.AppSettings["domain_name"].Trim()
            + "/authorize_callback.aspx?callback=" + Server.UrlEncode(callBack))
            + "&response_type=code&scope=snsapi_userinfo&state=1000#wechat_redirect", true);
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
