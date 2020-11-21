<%@ Page Language="C#" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Xml" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string str = new System.IO.StreamReader(Request.InputStream).ReadToEnd();
        File.AppendAllText(DateTime.Now.ToString() + "\r\n" + Server.MapPath("payment_callback.txt"), str + "\r\n");
        XmlDocument xmlD = new XmlDocument();
        xmlD.LoadXml(str);
        if (valid(xmlD))
        {
            if (xmlD.SelectSingleNode("//xml/result_code").InnerText.Trim().ToUpper().Equals("SUCCESS") &&
            xmlD.SelectSingleNode("//xml/return_code").InnerText.Trim().ToUpper().Equals("SUCCESS"))
            {
                try
                {
                    int orderId = int.Parse(xmlD.SelectSingleNode("//xml/product_id").InnerText.Trim());
                    OnlineOrder onlineOrder = new OnlineOrder(orderId);
                    if (onlineOrder._fields["pay_state"].ToString().Trim().Equals("0"))
                    {
                        onlineOrder.SetOrderPaySuccess(DateTime.Now, xmlD.SelectSingleNode("//xml/transaction_id").InnerText.Trim());
                        if (onlineOrder.Type.Trim().Equals("雪票"))
                        {
                            onlineOrder.CreateSkiPass();
                        }
                    }

                }
                catch
                {


                }

            }
        }

    }

    public bool valid(XmlDocument xmlD)
    {
        return true;
    }
</script>
<xml>
  <return_code><![CDATA[SUCCESS]]></return_code>
  <return_msg><![CDATA[OK]]></return_msg>
</xml>