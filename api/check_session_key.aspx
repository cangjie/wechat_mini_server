<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessinKey = Util.GetSafeRequestValue(Request, "sessionkey", "");
        string openId = MiniUsers.CheckSessionKey(sessinKey);
        string role = "";
        if (!openId.Trim().Equals(""))
        {
            role = "staff";
        }
        Response.Write("{\"status\": 0, \"role\": \"" + role.Trim() + "\" }");
    }
</script>