<%@ Page Language="C#" %>


<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "hKC5nig2gEKJjktmponkbA==");
        int id = int.Parse(Util.GetSafeRequestValue(Request, "id", "266"));
        string openId = MiniUsers.CheckSessionKey(sessionKey);
        MiniUsers user = new MiniUsers(openId);
        EquipMaintainRequestInshop info = new EquipMaintainRequestInshop(id);
        if (!user.role.Trim().Equals("staff") && !openId.Trim().Equals(info.OwnerOpenId.Trim())
            && !user.OfficialAccountOpenId.Trim().Equals(info.OwnerOpenId))
        {
            Response.Write("{\"status\": 1, \"error_message\": \"Have no right.\"}");
            Response.End();
        }
        string json = Util.ConvertDataFieldsToJson(info._fields);
        Response.Write("{\"status\": 0, \"maintain_in_shop_request\": " + json +  " }");
    }
</script>