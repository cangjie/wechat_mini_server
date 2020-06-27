<%@ Page Language="C#" %>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string appId = System.Configuration.ConfigurationSettings.AppSettings["appid"].Trim();
        string appSecret = System.Configuration.ConfigurationSettings.AppSettings["appsecret"].Trim();
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "");
        string iv = Util.GetSafeRequestValue(Request, "iv", "");
        string encData = Util.GetSafeRequestValue(Request, "encdata", "");
        Response.Write(Util.AES_decrypt(encData, sessionKey, iv));
    }
</script>