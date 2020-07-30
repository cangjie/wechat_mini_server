<%@ Page Language="C#" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string code = Util.GetSafeRequestValue(Request, "code", "011azXRm03mmbn1KRzUm02HjMR3azXRt");
        string appId = System.Configuration.ConfigurationSettings.AppSettings["appid"].Trim();
        string appSecret = System.Configuration.ConfigurationSettings.AppSettings["appsecret"].Trim();
        string sessionKeyJson = Util.GetWebContent("https://api.weixin.qq.com/sns/jscode2session?appid="
            + appId.Trim() + "&secret=" + appSecret.Trim() + "&js_code=" + code.Trim() + "&grant_type=authorization_code");
        string newJsonStr = sessionKeyJson.Substring(0, sessionKeyJson.Length - 2);
        newJsonStr = newJsonStr + ", \"role\": \"staff\"}";
        Response.Write(newJsonStr.Trim());
    }
</script>