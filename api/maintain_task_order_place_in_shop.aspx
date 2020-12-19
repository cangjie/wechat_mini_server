<%@ Page Language="C#" %>
<%@ Import Namespace="System.IO" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "");
        string action = Util.GetSafeRequestValue(Request, "action", "placeorder");
        Stream s = Request.InputStream;
        string json = (new StreamReader(s)).ReadToEnd().Trim();

        //File.WriteAllText(Server.MapPath("test_json.txt"), json);

        //json = "{\"request_id\":\"63\",\"cell_number\":\"13501177897\",\"real_name\":\"\",\"gender\":\"男\",\"equipInfo\":{\"type\":\"双板\",\"brand\":\"Fischer\",\"serial\":\"\",\"scale\":\"165\",\"year\":\"\"},\"edge\":\"0\",\"degree\":\"89\",\"candle\":\"0\",\"repair_more\":\"0\",\"shop\":\"万龙\",\"additional_fee\":\"0.01\"}";

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
        if (productId > 0 || additionalFee > 0)
        {
            if (productId > 0)
            {
                Product product = new Product(productId);
                productFee = product.SalePrice;
            }

            if (action.Trim().Equals("placeorder"))
            {
                try
                {
                    Dictionary<string, object> equipInfo = Util.GetObjectFromJsonByKey(json, "equipInfo");
                    string type = equipInfo["type"].ToString().Trim();
                    string brand = equipInfo["brand"].ToString().Trim();
                    string serial = equipInfo["serial"].ToString().Trim();
                    string year = equipInfo["year"].ToString().Trim();
                    string scale = equipInfo["scale"].ToString().Trim();
                    string cell = Util.GetSimpleJsonValueByKey(json, "cell_number").ToString().Trim();
                    string name = Util.GetSimpleJsonValueByKey(json, "real_name").ToString().Trim();
                    string gender = Util.GetSimpleJsonValueByKey(json, "gender").ToString().Trim();
                    int degree = int.Parse(Util.GetSimpleJsonValueByKey(json, "degree").ToString().Trim());
                    int id = int.Parse(Util.GetSimpleJsonValueByKey(json, "request_id"));
                    string more = "";
                    try
                    {
                        more = Util.GetSimpleJsonValueByKey(json, "repair_more").Trim();
                    }
                    catch
                    {

                    }
                    string memo = "";
                    try
                    {
                        memo = Util.GetSimpleJsonValueByKey(json, "memo").Trim();
                    }
                    catch
                    {

                    }

                    EquipMaintainRequestInshop req = new EquipMaintainRequestInshop(id);
                    MiniUsers customer = new MiniUsers(req.OwnerOpenId.Trim());
                    if (name.Trim().Equals(""))
                    {
                        if (!customer.Nick.Trim().Equals(""))
                        {
                            name = customer.Nick.Trim();
                        }
                        if (!customer.RealName.Trim().Equals(""))
                        {
                            name = customer.RealName.Trim();
                        }
                    }
                    if (cell.Trim().Equals(""))
                    {
                        cell = customer.CellNumber.Trim();
                    }
                    if (gender.Trim().Equals(""))
                    {
                        gender = customer._fields["gender"].ToString().Trim();
                    }
                    int r = req.Confirm(type, brand, serial, scale, year, cell, name, gender, edge, degree, candle, more,
                        additionalFee, memo, pickDate.Date, productId, openId.Trim());
                    if (r == 1)
                    {
                        orderId = EquipMaintainRequestInshop.PlaceOrder(id);
                    }
                    else
                    {
                        orderId = -1;
                    }

                }
                catch
                {

                }


                int requestId = int.Parse(Util.GetSimpleJsonValueByKey(json, "request_id"));
                EquipMaintainRequestInshop request = new EquipMaintainRequestInshop(requestId);
                //request.Confirm()
            }
        }


        Response.Write("{\"status\": 0, \"product_fee\": " + Math.Round(productFee, 2).ToString() + ", \"additional_fee\": "
            + Math.Round(additionalFee, 2).ToString()+ ", \"total_fee\": " + Math.Round(productFee + additionalFee, 2).ToString()
            + ", \"product_id\": " + productId.ToString() + ", \"order_id\": " + orderId.ToString() + ", \"request_detail\": " + json + " }");



    }
</script>