<%@ Page Language="C#" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Data" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "");

        Stream s = Request.InputStream;
        string json = (new StreamReader(s)).ReadToEnd().Trim();
        s.Close();

        Response.Write(json.Trim());
        Response.End();


        string openId = MiniUsers.CheckSessionKey(sessionKey);
        MiniUsers miniUser = new MiniUsers(openId);
        if (!miniUser.role.Trim().Equals("staff"))
        {
            Response.Write("{\"status\": 1, \"error_message\": \"Have no right.\"}");
            Response.End();
        }

        int expierenceId = Expierence.CreateNew(openId, Util.GetSimpleJsonValueByKey(json, "shop"));
        string[,] param = DBHelper.ConvertJsonToParameterStringArray(json);
        for (int i = 0; i < param.Length / 3; i++)
        {
            string key = param[i, 0].Trim();
            switch (key)
            {
                case "start_time":
                case "end_time":
                    param[i, 1] = "datetime";
                    break;
                case "guarantee_cash":
                    param[i, 1] = "float";
                    break;
                default:
                    param[i, 1] = "varchar";
                    break;
            }
        }
        DBHelper.UpdateData("expierence_list", param, new string[,] { { "id", "int", expierenceId.ToString() } }, Util.conStr.Trim());

    }
</script>