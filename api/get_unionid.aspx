<%@ Page Language="C#" %>


<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string encData = Util.GetSafeRequestValue(Request, "enc", 
            "7Ea5/tcdKWwyX++BB3w3pg1IHUygfM8+6A00QSdy9PbHzVX6sVdG+7kY37MgvcLnfD0W5peowgQsCKqcxhbmp+jlDTuCzgMF52E/tJktlhCMtF8eFTLVG6grbhHMTx7ATeXaQurpWKUatMMgBhdPphkc4jfT92o6JA3RjEvgFpEHFV2zD6qXaZ8aHubq7VZJSLGrvqh9kjyKHm9rngZRI2lFoRcZYtR9leMiilQQ9IkGxRYyJrOzdmYLgCdSi4sBc2tu9Fb0l8SVpOoFSMj8A2PMUvAcbaPbAr6bs27WNg4FqeiMtigpPE5BkW21gPsPcM4nf4qmiKpMoioOTazHHfVBOGZKh7P0SG4ni5qPbt14z/xy3Yb57cB5gq/Yp9oYxUGUQKMK8JK/9PIupxIHlP2oAvgKW63cj9Gl+6t11Wsd1ogb9mYksYbJ6HHSVoE+NHS7JJWEnf0g5HGJOod9BiAGxPYqJIMRQbf0cm/eIXQ=");
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "5Kt1GLyrm/UkJ1zcbIXkVw==");
        string iv = Util.GetSafeRequestValue(Request, "iv", "Cp/TT2j5Z+QE0XnxXhDMxw==");
        Response.Write(Util.AES_decrypt(encData, sessionKey.Trim(), iv));
    }
</script>