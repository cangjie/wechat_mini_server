<%@ Page Language="C#" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string imagePath = Util.GetSafeRequestValue(Request, "img", "'show_wechat_temp_qrcode.aspx?scene=oper_ticket_code_001001001");
        imagePath = Server.UrlDecode(imagePath);
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://weixin.snowmeet.top/" + imagePath.Trim());
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        byte[] buf = new byte[1024 * 1024 * 100];
        Stream s = response.GetResponseStream();
        int i = s.ReadByte();
        int j = 0;
        while (i >= 0)
        {
            buf[j] = (byte)i;
            i = s.ReadByte();
            j++;
        }
        byte[] buff = new byte[j];
        for (int k = 0; k < j; k++)
        {
            buff[k] = buf[k];
        }
        Response.ContentType = "image/jpeg";
        Response.BinaryWrite(buff);
        response.Close();
        request.Abort();

    }
</script>