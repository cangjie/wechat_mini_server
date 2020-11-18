<%@ Page Language="C#" %>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string accessToken = Util.GetToken();
        string urlGetProducts = "https://api.weixin.qq.com/product/spu/get_list?access_token=" + accessToken.Trim();
        string returnJson = Util.GetWebContent(urlGetProducts, "POST", "{\"status\": 5,\"page\": 1, \"page_size\": 1000}", "application/json");
        Response.Write(returnJson);
    }
</script>
