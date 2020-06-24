<%@ Page Language="C#" %>


<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string code = Util.GetSafeRequestValue(Request, "code", "011or7R60eIL1D19BvR601DTQ60or7R6");
        string appId = System.Configuration.ConfigurationSettings.AppSettings["appid"].Trim();
        string appSecret = System.Configuration.ConfigurationSettings.AppSettings["appsecret"].Trim();
        string sessionKeyJson = Util.GetWebContent("https://api.weixin.qq.com/sns/jscode2session?appid="
            + appId.Trim() + "&secret=" + appSecret.Trim() + "&js_code=" + code.Trim() + "&grant_type=authorization_code");
        Response.Write(sessionKeyJson);
    }
</script>