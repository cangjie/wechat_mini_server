﻿<%@ Page Language="C#" %>
<%@ Import Namespace="System.Xml" %>
<!DOCTYPE html>

<script runat="server">

    public string prepayId = "";
    public string timeStampStr = "";
    public string nonceString = "";
    public string sign = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        int orderId = int.Parse(Util.GetSafeRequestValue(Request, "product_id", "16387"));
        string appId = System.Configuration.ConfigurationSettings.AppSettings["appid"].Trim();
        string mch_id = System.Configuration.ConfigurationSettings.AppSettings["mch_id"].Trim();
        string key = "snowmeetsnowmeetsnowmeetsnowmeet";
        string nonce_str = Util.GetNonceString(32);



        OnlineOrder order = new OnlineOrder(orderId);

        XmlDocument xmlD = new XmlDocument();
        xmlD.LoadXml("<xml/>");
        XmlNode rootXmlNode = xmlD.SelectSingleNode("//xml");
        XmlNode n = xmlD.CreateNode(XmlNodeType.Element, "appid", "");
        n.InnerText = appId.Trim();
        rootXmlNode.AppendChild(n);
        n = xmlD.CreateNode(XmlNodeType.Element, "mch_id", "");
        n.InnerText = mch_id.Trim();
        rootXmlNode.AppendChild(n);
        n = xmlD.CreateNode(XmlNodeType.Element, "nonce_str", "");
        n.InnerText = nonce_str;
        rootXmlNode.AppendChild(n);

        n = xmlD.CreateNode(XmlNodeType.Element, "sign_type", "");
        n.InnerText = "MD5";
        rootXmlNode.AppendChild(n);

        n = xmlD.CreateNode(XmlNodeType.Element, "notify_url", "");
        n.InnerText = Request.Url.ToString().Trim().Replace("payment.aspx", "callback.aspx").Trim();
        rootXmlNode.AppendChild(n);

        n = xmlD.CreateNode(XmlNodeType.Element, "openid", "");
        try
        {
            n.InnerText = order._fields["open_id"].ToString().Trim();
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
        n.InnerText = order._fields["id"].ToString();
        rootXmlNode.AppendChild(n);

        n = xmlD.CreateNode(XmlNodeType.Element, "body", "");
        n.InnerText = order.OrderDetails[0].productName.Trim();
        rootXmlNode.AppendChild(n);

        n = xmlD.CreateNode(XmlNodeType.Element, "detail", "");
        n.InnerText = order._fields["type"].ToString().Trim();
        rootXmlNode.AppendChild(n);

        n = xmlD.CreateNode(XmlNodeType.Element, "total_fee", "");
        n.InnerText = Math.Round(100 * float.Parse(order._fields["order_real_pay_price"].ToString()), 0).ToString();
        rootXmlNode.AppendChild(n);

        string s = Util.ConverXmlDocumentToStringPair(xmlD);
        //s = Util.GetMd5Sign(s, "jihuowangluoactivenetworkjarrodc");
        s = Util.GetMd5Sign(s, key);

        n = xmlD.CreateNode(XmlNodeType.Element, "sign", "");
        n.InnerText = s.Trim();
        rootXmlNode.AppendChild(n);

        string prepayXml = Util.GetWebContent("https://api.mch.weixin.qq.com/pay/unifiedorder", "post", xmlD.InnerXml.Trim(), "raw");

        XmlDocument xmlPrepay = new XmlDocument();
        xmlPrepay.LoadXml(prepayXml);
        prepayId = xmlPrepay.SelectSingleNode("//xml/prepay_id").InnerText.Trim();

        timeStampStr = Util.GetTimeStamp();
        nonceString = Util.GetNonceString(32);

        XmlDocument xmlPayClient = new XmlDocument();
        xmlPayClient.LoadXml("<xml/>");
        rootXmlNode = xmlPayClient.SelectSingleNode("//xml");

        n = xmlPayClient.CreateNode(XmlNodeType.Element, "appId", "");
        n.InnerText = appId;
        rootXmlNode.AppendChild(n);

        n = xmlPayClient.CreateNode(XmlNodeType.Element, "timeStamp", "");
        n.InnerText = timeStampStr.Trim();
        rootXmlNode.AppendChild(n);

        n = xmlPayClient.CreateNode(XmlNodeType.Element, "signType", "");
        n.InnerText = "MD5";
        rootXmlNode.AppendChild(n);

        n = xmlPayClient.CreateNode(XmlNodeType.Element, "nonceStr", "");
        n.InnerText = nonceString.Trim();
        rootXmlNode.AppendChild(n);

        n = xmlPayClient.CreateNode(XmlNodeType.Element, "package", "");
        n.InnerText = "prepay_id=" + prepayId.Trim();
        rootXmlNode.AppendChild(n);
        s = Util.ConverXmlDocumentToStringPair(xmlPayClient);
        //s = Util.GetMd5Sign(s, "jihuowangluoactivenetworkjarrodc");
        sign = Util.GetMd5Sign(s, key);

        //Response.Write("<br>" + s + "<br>" + sign);
        //Response.End();
        //sign = s.Trim();


    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script type="text/javascript" src="https://res.wx.qq.com/open/js/jweixin-1.3.2.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            
        </div>
    </form>
</body>
<script type="text/javascript" >
    wx.miniProgram.navigateTo({ url: '/pages/payment/payment?orderid=<%=Util.GetSafeRequestValue(Request, "product_id", "0")%>&prepayid=<%=prepayId.Trim()%>&timestamp=<%=timeStampStr%>&nonce=<%=nonceString%>&sign=<%=sign%>' });
</script>
</html>
