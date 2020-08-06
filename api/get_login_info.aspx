<%@ Page Language="C#" %>


<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string code = Util.GetSafeRequestValue(Request, "code", "041ZaTll2bs7p54Ruaml2Id2SC3ZaTlI");
        string sessionKey = MiniUsers.UserLogin(code);
        Response.Write("{ \"session_key\": \"" + sessionKey.Trim() + "\", \"role\": \"staff\" }");
    }
</script>