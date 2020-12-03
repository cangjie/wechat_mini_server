<%@ Page Language="C#" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "");
        string shop = Util.GetSafeRequestValue(Request, "shop", "");
        string type = Util.GetSafeRequestValue(Request, "type", "");
        string brand = Util.GetSafeRequestValue(Request, "brand", "");
        string scale = Util.GetSafeRequestValue(Request, "scale", "");
        bool edge = Util.GetSafeRequestValue(Request, "edge", "0").ToString().Equals("1") ? true : false;
        bool candle = Util.GetSafeRequestValue(Request, "candle", "0").ToString().Equals("1") ? true : false;
        bool repair = Util.GetSafeRequestValue(Request, "repair", "0").ToString().Equals("1") ? true : false;
        DateTime pickDate = DateTime.Parse(Util.GetSafeRequestValue(Request, "pickdate", DateTime.Now.AddDays(10).ToShortDateString()));

        string openId = MiniUsers.CheckSessionKey(sessionKey);
        if (openId.Trim().Equals(""))
        {
            Response.Write("{\"status\": 1, \"error_message\": \"Session key is not valid.\"}");
            Response.End();
        }
        int id = 0;
        if (!shop.Trim().Equals("") && !brand.Trim().Equals("") && (edge || candle || repair) && !type.Equals(""))
        {
            id = EquipMaintainRequestInshop.CreateNew(openId, shop, type.Trim(), brand.Trim(), scale.Trim(), edge, candle, repair, pickDate);
            Response.Write("{\"status\": 0, \"id\": " + id.ToString() + "}");
        }
        else
        { 
            Response.Write("{\"status\": 1, \"error_message\": \"Filled info is incorrect.\"}");
        }
    }
</script>