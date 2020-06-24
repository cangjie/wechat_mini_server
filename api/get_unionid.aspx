<%@ Page Language="C#" %>


<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string code = Util.GetSafeRequestValue(Request, "code", "011or7R60eIL1D19BvR601DTQ60or7R6");
        string appId = System.Configuration.ConfigurationSettings.AppSettings["appid"].Trim();
        string appSecret = System.Configuration.ConfigurationSettings.AppSettings["appsecret"].Trim();
        string sessionKeyJson = Util.GetWebContent("https://api.weixin.qq.com/sns/jscode2session?appid="
            + appId.Trim() + "&secret=" + appSecret.Trim() + "&js_code=" + code.Trim() + "&grant_type=authorization_code");
        string encData = "toc2HVL+I5Uh8psdLz2CUOpgNBzYQIf2jVRCwGPw+q738QNazMh2UpKSPLTi/z0zyEEXk566TqeHn0Bl1LCW5cbOcqzT2Oyo5V3/cZeHfJrqar1Jn2ys6Ctuz/P7X5JtdWR1fg/3Z4IyqFtML94RJc/cB2eEnQIW4FjAwk9X7WBuaK+tpPRVvkeSrWWtXdaFoKmT4A+bCUW20FlxeYbQAw==";
        string iv = "HcTOecfdudgql5ow99ngYQ==";
        Response.Write(Util.AES_decrypt(encData, Util.GetSimpleJsonValueByKey(sessionKeyJson, "session_key").Trim(), iv));

    }
</script>