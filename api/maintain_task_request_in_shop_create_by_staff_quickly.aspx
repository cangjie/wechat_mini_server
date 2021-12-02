<%@ Page Language="C#" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Data" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "");
        int batchId = int.Parse(Util.GetSafeRequestValue(Request, "batchid", "0").Trim());
        Stream s = Request.InputStream;
        string json = (new StreamReader(s)).ReadToEnd().Trim();
        string openId = MiniUsers.CheckSessionKey(sessionKey);
        MiniUsers user = new MiniUsers(openId);
        if (!user.role.Trim().Equals("staff"))
        {
            Response.Write("{\"status\": 1, \"error_message\": \"Staff only.\", \"input_json\": " + json.Trim() + "}");
            Response.End();
        }

        string checkParametersMessage = "";
        string pType = "";
        string pBrand = "";
        string pScale = "";
        bool pEdge = false;
        string pDegree = "89";
        bool pCandle = false;
        string pMore = "";
        DateTime pPickDate = DateTime.Now.Date.AddDays(1);
        string pShop = "万龙";

        string pMemo = "";

        string ticketCode = "";

        double pAdditionalCharge = 0;
        int pProductId = 0;
        try
        {
            pShop = Util.GetSimpleJsonValueByKey(json, "shop").Trim();
        }
        catch
        {
            checkParametersMessage = "Parameter shop is missing.";
        }

        try
        {
            Dictionary<string, object> equipInfo = Util.GetObjectFromJsonByKey(json, "equipInfo");
            try
            {
                pType = equipInfo["type"].ToString().Trim();
            }
            catch
            {
                checkParametersMessage = "Parameter type is missing.";
            }
            try
            {
                pBrand = equipInfo["brand"].ToString().Trim();
            }
            catch
            {
                checkParametersMessage = "Parameter brand is missing.";
            }
            try
            {
                pScale = equipInfo["scale"].ToString().Trim();
            }
            catch
            {

            }
        }
        catch
        {
            checkParametersMessage = "Parameter equipinfo is missing.";
        }
        try
        {
            pEdge = Util.GetSimpleJsonValueByKey(json, "edge").Trim().Equals("1")? true: false;
        }
        catch
        {

        }
        try
        {
            pDegree = Util.GetSimpleJsonValueByKey(json, "degree").Trim();
        }
        catch
        {

        }
        try
        {
            pCandle = Util.GetSimpleJsonValueByKey(json, "candle").Trim().Equals("1")? true: false;
        }
        catch
        {

        }
        try
        {
            pMore = Util.GetSimpleJsonValueByKey(json, "repair_more").Trim();
        }
        catch
        {

        }
        try
        {
            ticketCode = Util.GetSimpleJsonValueByKey(json, "ticket_code").Trim();
        }
        catch
        { 
        
        }
        try
        {
            pPickDate = DateTime.Parse(Util.GetSimpleJsonValueByKey(json, "pick_date").Trim());
        }
        catch
        {
            checkParametersMessage = "Parameter pick_date is missing.";
        }
        try
        {
            pAdditionalCharge = double.Parse(Util.GetSimpleJsonValueByKey(json, "additional_fee").Trim());
        }
        catch
        {

        }
        try
        {
            pProductId = int.Parse(Util.GetSimpleJsonValueByKey(json, "product_id").Trim());
        }
        catch
        {

        }
        try
        {
            pMemo = Util.GetSimpleJsonValueByKey(json, "memo").Trim();
        }
        catch
        {

        }
        bool urgent = false;
        try
        {
            if (int.Parse(Util.GetSimpleJsonValueByKey(json, "urgent")) == 1)
            {
                urgent = true;
            }
        }
        catch
        {

        }
        if (!checkParametersMessage.Trim().Equals(""))
        {
            Response.Write("{\"status\": 1, \"error_message\": \"" + checkParametersMessage + "\"}");
            Response.End();
        }
        if (batchId == 0)
        {
            DataTable dtBatchId = DBHelper.GetDataTable(" select max(batch_id) from  maintain_in_shop_request  ");
            if (dtBatchId.Rows.Count == 1)
            {
                batchId = int.Parse(dtBatchId.Rows[0][0].ToString()) + 1;
            }
        }
        int i = DBHelper.InsertData("maintain_in_shop_request", new string[,] {
            {"service_open_id", "varchar", openId.Trim() },
            {"confirmed_equip_type", "varchar", pType.Trim() },
            {"confirmed_brand", "varchar", pBrand.Trim() },
            {"confirmed_scale", "varchar", pScale.Trim() },
            {"confirmed_edge", "int", pEdge? "1":"0" },
            {"confirmed_degree", "varchar", pDegree.Trim() },
            {"confirmed_candle", "int", pCandle? "1" : "0" },
            {"confirmed_more", "varchar", pMore.Trim() },
            {"confirmed_pick_date", "datetime", pPickDate.ToShortDateString() },
            {"confirmed_urgent", "int", urgent? "1":"0"},
            {"confirmed_memo", "varchar", pMemo.Trim() },
            {"ticket_code", "varchar", ticketCode.Trim() },
            {"confirmed_additional_fee", "float",  pAdditionalCharge.ToString()},
            {"confirmed_product_id", "int", pProductId.ToString() },
            {"batch_id", "int", batchId.ToString() }
        });
        if (i == 1)
        {
            DataTable dt = DBHelper.GetDataTable(" select max([id]) from maintain_in_shop_request ");
            int newId = 0;
            if (dt.Rows.Count == 1)
            {
                newId = int.Parse(dt.Rows[0][0].ToString());
            }
            dt.Dispose();
            Response.Write("{\"status\": 0, \"maintain_in_shop_request_id\": " + newId.ToString() + ", \"input_json\": " + json + ", \"batch_id\": " + batchId.ToString() + " }");
        }
        else
        {
            Response.Write("{\"status\": 1, \"error_message\": \"Insert fail.\", \"input_json\": " + json + "  }");
        }
    }
</script>
