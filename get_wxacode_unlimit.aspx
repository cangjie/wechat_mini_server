<%@ Page Language="C#" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Collections" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string token = "39_Zok6W_lpI5dQU5V2-ZOlPzpDiNsZc9zb1x03R4dT4d7ZZVwoF0Hy1KB_-R1NwGIGIt77j-elf5zomAG7WBUHajKh6_hm2huXwe76ZqYwkE-NYuy6nTG7c1Gu3z1u19xqlbqmrAgzJSK-ydRmACMeABAMOY";//Util.GetToken();
        token = Util.GetToken();
        string codeUrl = "https://api.weixin.qq.com/wxa/getwxacodeunlimit?access_token=" + token;
        string json = new StreamReader(Request.InputStream).ReadToEnd();
        //json = "{\"page\": \"pages/admin/equip_maintain/in_shop_order_confirm/in_shop_order_detail/in_shop_order_detail\",  \"scene\": \"id=1234\"}";
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