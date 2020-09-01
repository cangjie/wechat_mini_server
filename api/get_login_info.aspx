<%@ Page Language="C#" %>


<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string code = Util.GetSafeRequestValue(Request, "code", "051rv1Ga1Ofjxz0VxmFa1A6Buw4rv1Gr");
        string sessionKey = MiniUsers.UserLogin(code);
        Response.Write("{ \"session_key\": \"" + sessionKey.Trim() + "\", \"role\": \"staff\" }");
    }
</script>