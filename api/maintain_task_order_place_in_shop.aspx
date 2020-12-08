<%@ Page Language="C#" %>
<%@ Import Namespace="System.IO" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "");
        string action = Util.GetSafeRequestValue(Request, "action", "");
        Stream s = Request.InputStream;
        string json = (new StreamReader(s)).ReadToEnd().Trim();

        string openId = MiniUsers.CheckSessionKey(sessionKey);


        MiniUsers user = new MiniUsers(openId);
        if (!user.role.Trim().Equals("staff"))
        {
            Response.Write("{\"status\": 1, \"error_message\": \"Staff only.\"}");
            Response.End();
        }

        int productId = 0;
        bool edge = false;
        bool candle = false;
        double additionalFee = 0;
        string shop = "万龙";

        if (Util.GetSimpleJsonValueByKey(json, "edge").Trim().Equals("1"))
        {
            edge = true;
        }
        if (Util.GetSimpleJsonValueByKey(json, "candle").Trim().Equals("1"))
        {
            candle = true;
        }

        try
        {
            additionalFee = double.Parse(Util.GetSimpleJsonValueByKey(json, "additional_fee").Trim());
        }
        catch
        {

        }

        try
        {
            shop = Util.GetSimpleJsonValueByKey(json, "shop").Trim();
        }
        catch
        {

        }

        DateTime pickDate = DateTime.Now.Date;

        try
        {
            pickDate = DateTime.Parse(Util.GetSimpleJsonValueByKey(json, "pick_date"));
        }
        catch
        {

        }

        bool pickImmediately = true;
        if (pickDate.Date > DateTime.Now.Date)
        {
            pickImmediately = false;
        }

        switch (shop)
        {
            case "万龙":
                if (pickImmediately)
                {
                    if (edge && candle)
                    {
                        productId = 137;
                    }
                    else
                    {
                        if (edge)
                        {
                            productId = 138;
                        }
                        if (candle)
                        {
                            productId = 142;
                        }
                    }
                }
                else
                {
                    if (edge && candle)
                    {
                        productId = 139;
                    }
                    else
                    {
                        if (edge)
                        {
                            productId = 140;
                        }
                        if (candle)
                        {
                            productId = 143;
                        }
                    }
                }
                break;
            default:
                break;
        }
        int orderId = 0;

        double productFee = 0;
        if (productId > 0)
        {
            Product product = new Product(productId);
            productFee = product.SalePrice;
            if (action.Trim().Equals("placeorder"))
            {
                int requestId = int.Parse(Util.GetSimpleJsonValueByKey(json, "request_id"));
                EquipMaintainRequestInshop request = new EquipMaintainRequestInshop(requestId);
                orderId = request.PlaceOrder(openId, productId);
            }
        }


        Response.Write("{\"status\": 0, \"product_fee\": " + Math.Round(productFee, 2).ToString() + ", \"additional_fee\": "
            + Math.Round(additionalFee, 2).ToString()+ ", \"total_fee\": " + Math.Round(productFee + additionalFee, 2).ToString()
            + ", \"product_id\": " + productId.ToString() + ", \"order_id\": " + orderId.ToString() + ", \"request_detail\": " + json + " }");



    }
</script>