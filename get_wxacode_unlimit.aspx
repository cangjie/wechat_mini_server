<%@ Page Language="C#" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Collections" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string token = "39_2xtxAYuEMcuEx8frAweTFZUZAPTefw7Cc-p954k2CWQdkQRxiyAhIp0P9if6ZTXne1MvAcnkueeJv0S_d6urrS3jGlHsra3OcZuJuPyF4Ti30S4_2TXwfwAlSraz-bexfMfpk5vbJ-Lfz_GnZZLjAJALLX";//Util.GetToken();
        token = Util.GetToken();
        string codeUrl = "https://api.weixin.qq.com/wxa/getwxacodeunlimit?access_token=" + token;

        string page = Util.GetSafeRequestValue(Request, "page", "pages/index/index");
        string scene = Util.GetSafeRequestValue(Request, "scene", "id=1234");
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