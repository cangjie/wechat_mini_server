<%@ Page Language="C#" %>
<%@ Import Namespace="System.Xml" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string nonceStr = GetNonceString(32);
        string appId = System.Configuration.ConfigurationSettings.AppSettings["wxappid"].Trim();
        string appSecret = System.Configuration.ConfigurationSettings.AppSettings["wxappsecret"].Trim();
        string mch_id = System.Configuration.ConfigurationSettings.AppSettings["mch_id"].Trim();
        XmlDocument xmlD = new XmlDocument();
        xmlD.LoadXml("<xml/>");
        XmlNode rootXmlNode = xmlD.SelectSingleNode("//xml");
        XmlNode n = xmlD.CreateNode(XmlNodeType.Element, "appid", "");
        n.InnerText = appId.Trim();
        rootXmlNode.AppendChild(n);

        n = xmlD.CreateNode(XmlNodeType.Element, "mch_id", "");
        n.InnerText = mch_id.Trim();
        rootXmlNode.AppendChild(n);

        string nonce_str = Util.GetNonceString(32);

        //nonceStr = "jihuo";

        nonce_str = nonceStr.Trim();
        n = xmlD.CreateNode(XmlNodeType.Element, "nonce_str", "");
        n.InnerText = nonceStr;
        rootXmlNode.AppendChild(n);

        n = xmlD.CreateNode(XmlNodeType.Element, "notify_url", "");
        n.InnerText = Request.Url.ToString().Trim().Replace("get_prepay_id.aspx", "payment_callback.aspx").Trim();
        rootXmlNode.AppendChild(n);

        n = xmlD.CreateNode(XmlNodeType.Element, "openid", "");
        try
        {
            n.InnerText = Util.GetSafeRequestValue(Request, "openid", "oRY1ws3bQFILrsUJtR5mKy6e2kQI").Trim();
        }
        catch
        {
            n.InnerText = "";
        }
        rootXmlNode.AppendChild(n);

        n = xmlD.CreateNode(XmlNodeType.Element, "spbill_create_ip", "");
        n.InnerText = Request.UserHostAddress.Trim();
        rootXmlNode.AppendChild(n);

        n = xmlD.CreateNode(XmlNodeType.Element, "trade_type", "");
        n.InnerText = "JSAPI";
        rootXmlNode.AppendChild(n);

        n = xmlD.CreateNode(XmlNodeType.Element, "out_trade_no", "");
        string timeStamp = Util.GetTimeStamp();
        
        n.InnerText = timeStamp;
        string out_trade_no = n.InnerText.Trim();
        rootXmlNode.AppendChild(n);

        n = xmlD.CreateNode(XmlNodeType.Element, "body", "");
        n.InnerText = "test";
        rootXmlNode.AppendChild(n);

        n = xmlD.CreateNode(XmlNodeType.Element, "detail", "");
        n.InnerText = "test_detail";
        rootXmlNode.AppendChild(n);

        n = xmlD.CreateNode(XmlNodeType.Element, "total_fee", "");
        n.InnerText = Util.GetSafeRequestValue(Request, "total_fee", "1");
        rootXmlNode.AppendChild(n);
        
        string s = Util.ConverXmlDocumentToStringPair(xmlD);
        //s = Util.GetMd5Sign(s, "jihuowangluoactivenetworkjarrodc");
        s = Util.GetMd5Sign(s, "ubsyrgj6wy1fn8qbyjx68lgmvli6eod0");

        n = xmlD.CreateNode(XmlNodeType.Element, "sign", "");
        n.InnerText = s.Trim();
        rootXmlNode.AppendChild(n);

        string prepayXml = Util.GetWebContent("https://payapi.mch.weixin.semoor.cn/4.0/pay/unifiedorder", "post", xmlD.InnerXml.Trim(), "raw");

        Response.Write(xmlD.InnerXml.Trim().Replace("<", "&lt;").Replace(">", "&gt;") 
            + "<br/>" + prepayXml.Trim().Replace("<", "&lt;").Replace(">", "&gt;"));

    }



    public static string GetNonceString(int length)
    {
        string chars = "0123456789abcdefghijklmnopqrstuvwxyz";
        char[] charsArr = chars.ToCharArray();
        int charsCount = chars.Length;
        string str = "";
        Random rnd = new Random();
        for (int i = 0; i < length - 1; i++)
        {
            str = str + charsArr[rnd.Next(charsCount)].ToString();
        }
        return str;
    }
</script>