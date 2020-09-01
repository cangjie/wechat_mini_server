<%@ Page Language="C#" %>


<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string code = Util.GetSafeRequestValue(Request, "code", "011JaNFa1Dheyz0BTxIa1GXZwX1JaNFi");
        string sessionKey = MiniUsers.UserLogin(code);
        Response.Write("{ \"session_key\": \"" + sessionKey.Trim() + "\", \"role\": \"staff\" }");
    }
</script>