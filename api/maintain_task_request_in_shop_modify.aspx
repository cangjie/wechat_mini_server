<%@ Page Language="C#" %>
<%@ Import Namespace="System.IO" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "");
        int id = int.Parse(Util.GetSafeRequestValue(Request, "id", "0"));
        Stream s = Request.InputStream;
        string json = (new StreamReader(s)).ReadToEnd().Trim();
        s.Close();

        File.WriteAllText(Server.MapPath("test_json.txt"), json);

        string openId = MiniUsers.CheckSessionKey(sessionKey);
        MiniUsers user = new MiniUsers(openId);
        EquipMaintainRequestInshop request = new EquipMaintainRequestInshop(id);
        if (!user.role.Trim().Equals("staff") && !request.OwnerOpenId.Trim().Equals("") && !request.OwnerOpenId.Trim().Equals(openId))
        {
            Response.Write("{\"status\": 1, \"error_message\": \"Staff or owner only.\"}");
            Response.End();
        }

        string[] keyArr = Util.GetJsonKeys(json);

        string[,] paramArr = new string[keyArr.Length, 3];
        for (int i = 0; i < keyArr.Length; i++)
        {
            paramArr[i, 0] = keyArr[i].Trim();
            paramArr[i, 1] = "varchar";
            paramArr[i, 3] = Util.GetSimpleJsonValueByKey(json, keyArr[i]).Trim();
        }
        int j = DBHelper.UpdateData("maintain_in_shop_request", paramArr, new string[,] { { "id", "int", id.ToString() } }, Util.conStr);

        Response.Write("{\"status\": 0, \"result\": " + j.ToString() + ", \"request_json\": " + json.Trim() + " }");
    }
</script>