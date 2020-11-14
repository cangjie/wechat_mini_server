<%@ Page Language="C#" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        string token = Util.GetSafeRequestValue(Request, "token", "db7f7360ec8994d1c710bc6de207ee7292e5383de08ced03a7303882c6a5df27179dd45d");
        string cartJson = Util.GetSafeRequestValue(Request, "cart", "{\"cart_array\" : [{ \"product_id\": 123, \"count\": 1 }]}");
        string source = Util.GetSafeRequestValue(Request, "source", "");
        string openId = WeixinUser.CheckToken(token);
        if (openId.Trim().Equals(""))
        {
            Response.Write("{\"status\" : \"1\", \"error_message\":\"token is invalid.\"}");
        }
        else
        {
            Dictionary<string, object>[] cartItemArr = Util.GetObjectArrayFromJsonByKey(cartJson, "cart_array");
            OnlineOrder newOrder = new OnlineOrder();
            foreach (Dictionary<string, object> item in cartItemArr)
            {
                OnlineOrderDetail detail = new OnlineOrderDetail();
                Product p = new Product(int.Parse(item["product_id"].ToString()));
                detail.productId = int.Parse(p._fields["id"].ToString());
                detail.productName = p._fields["name"].ToString();
                detail.price = double.Parse(p._fields["sale_price"].ToString()) + double.Parse(p._fields["deposit"].ToString()) ;
                detail.count = int.Parse(item["count"].ToString());
                newOrder.AddADetail(detail);
                newOrder.Type = p._fields["type"].ToString();
                newOrder.shop = p._fields["shop"].ToString();
            }
            Dictionary<string, object> memoJsonObject = (Dictionary<string, object>)Util.GetObjectFromJsonByKey(cartJson, "memo");
            if (memoJsonObject != null)
            {
                newOrder.Memo = Util.GetSimpleJsonStringFromKeyPairArray(memoJsonObject.ToArray());
            }
            else
                newOrder.Memo = "";
            int orderId = newOrder.Place(openId);
            if (!source.Trim().Equals(""))
            {
                //OnlineOrder order = new OnlineOrder(orderId);
                DBHelper.UpdateData("order_online", new string[,] { { "source", "varchar", source.Trim() } },
                    new string[,] { { "id", "int", orderId.ToString() } }, Util.conStr);


            }
            Response.Write("{\"status\" : \"0\", \"order_id\" : \"" + orderId.ToString() + "\" }");
        }
    }
</script>