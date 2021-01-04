<%@ Page Language="C#" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Collections" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string token = "40_-vrwPOHUy0_FLETJQT6ohF4qSDa076c3V-803ccoB1YOQapTrTaDrlPthut2PJ_qjSo_CuIvuZD-qKJBle-xk5b36iVF5v2xgObEh3IeVk4XSg_my7855FL_6zyTyJ01WfZBZu9EA28VQMz8QXRdACAAHB";//Util.GetToken();
        token = Util.GetToken();
        string codeUrl = "https://api.weixin.qq.com/wxa/getwxacodeunlimit?access_token=" + token;

        string page = Util.GetSafeRequestValue(Request, "page", "pages/maintain/in_shop_request_payment/in_shop_request_payment");
        string scene = Util.GetSafeRequestValue(Request, "scene", "161");
        scene = Server.UrlDecode(scene);
        string json = "{\"page\": \"" + page.Trim() + "\",  \"scene\": \"" + scene.Trim() + "\"}";
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(codeUrl);
        req.Method = "post";
        Stream requestStream = req.GetRequestStream();
        StreamWriter sw = new StreamWriter(requestStream);
        sw.Write(json);
        sw.Close();
        HttpWebResponse res = (HttpWebResponse)req.GetResponse();
        Stream responseStream = res.GetResponseStream();
        ArrayList arr = new ArrayList();
        int currentByte = responseStream.ReadByte();
        for (; currentByte >= 0;)
        {
            arr.Add(currentByte);
            currentByte = responseStream.ReadByte();
        }
        byte[] bArr = new byte[arr.Count];
        for (int i = 0; i < bArr.Length; i++)
        {
            bArr[i] = (byte)int.Parse(arr[i].ToString());
        }
        responseStream.Close();
        res.Close();
        req.Abort();
        Response.ContentType = "image/jpeg";
        Response.BinaryWrite(bArr);

    }
</script>