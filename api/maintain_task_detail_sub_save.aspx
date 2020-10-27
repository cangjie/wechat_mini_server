<%@ Page Language="C#" %>
<%@ Import Namespace="System.Text.RegularExpressions" %>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        /*
        string json1 = "{\"id\":\"5\",\"content\":[{\"url\":\"https://mini.snowmeet.top/upload/20201027/1603786016.jpg\"}]}";
        string json2 = "{\"id\":\"26\",\"content\":{\"type\":\"双板\",\"brand\":\"Fischer\",\"serial\":\"RC4Booster\",\"scale\":\"165\",\"year\":\"18-19\"}}";
        string json3 = "{\"id\":\"31\",\"content\":\"s\"}";
        Response.Write(GetContent(json1)+"<br>");
        Response.Write(GetContent(json2)+"<br>");
        Response.Write(GetContent(json3));
        */
        
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "");
        string operOpenId = MiniUsers.CheckSessionKey(sessionKey.Trim());
        MiniUsers oper = new MiniUsers(operOpenId.Trim());
        if (!oper.role.Trim().Equals("staff"))
        {
            Response.Write("{\"status\": 1, \"err_msg\": \"Staff Only!\"}");
            Response.End();
        }
        int i = 0;
        for (; Request[i.ToString()] != null && !Request[i.ToString()].Trim().Equals(""); i++)
        {
            Response.Write(GetContent(Server.UrlDecode(Request[i.ToString()]))+"\r\n");
        }
        
    }

    public static string GetContent(string json)
    {
        Match m = Regex.Match(json, "\"content\":.+}");
        string str = m.Value.Trim();
        str = str.Replace("\"content\":", "").Trim();
        if (str.EndsWith("}"))
        {
            str = str.Substring(0, str.Length - 1);
        }
        return str.Trim();
    }
</script>