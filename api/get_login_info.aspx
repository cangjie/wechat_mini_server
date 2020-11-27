<%@ Page Language="C#" %>


<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string code = Util.GetSafeRequestValue(Request, "code", "0110Cj000GwpGK1PCM200I7ZoW10Cj0l");
        string sessionKey = MiniUsers.UserLogin(code);
        Response.Write("{ \"session_key\": \"" + sessionKey.Trim() + "\", \"role\": \"staff\" }");
    }
</script>