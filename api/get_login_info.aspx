<%@ Page Language="C#" %>


<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string code = Util.GetSafeRequestValue(Request, "code", "071EXqlD0msA9h2Ws2kD0kLzlD0EXql1");
        string appId = System.Configuration.ConfigurationSettings.AppSettings["appid"].Trim();
        string appSecret = System.Configuration.ConfigurationSettings.AppSettings["appsecret"].Trim();
        string sessionKeyJson = Util.GetWebContent("https://api.weixin.qq.com/sns/jscode2session?appid="
            + appId.Trim() + "&secret=" + appSecret.Trim() + "&js_code=" + code.Trim() + "&grant_type=authorization_code");
        Response.Write(sessionKeyJson);
    }
</script>