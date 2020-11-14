<%@ Page Language="C#" %>
<%@ Import Namespace="System.Runtime.Serialization" %>
<%@ Import Namespace="System.Runtime.Serialization.Json" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="System.Data" %>
<!DOCTYPE html>

<script runat="server">

    public int tryGetOpenIdTimes = 0;

    public string GetOpenId(string code)
    {
        if (tryGetOpenIdTimes > 0)
        {
            System.Threading.Thread.Sleep(1000);
        }
        if (tryGetOpenIdTimes > 10)
        {
            return "";
        }
        tryGetOpenIdTimes++;

        string openIdStr = "";

        try
        {

            string jsonStr = "";
            jsonStr = Util.GetWebContent("https://api.weixin.qq.com/sns/oauth2/access_token?appid="
                + System.Configuration.ConfigurationSettings.AppSettings["wxappid"].Trim()
                + "&secret=" + System.Configuration.ConfigurationSettings.AppSettings["wxappsecret"].Trim()
                + "&code=" + code + "&grant_type=authorization_code", "GET", "", "text/htm");
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> json = (Dictionary<string, object>)serializer.DeserializeObject(jsonStr);
            object openId;
            json.TryGetValue("openid", out openId);
            openIdStr = openId.ToString().Trim();

            try
            {

                DataTable dt = DBHelper.GetDataTable(" select * from unionids  where open_id = '" + openIdStr.Trim()
                    + "' and  source = 'snowmeet_offical_account'");
                string unionId = "";
                if (dt.Rows.Count > 0)
                {
                    unionId = dt.Rows[0]["union_id"].ToString().Trim();
                }
                dt.Dispose();

                if (unionId.Trim().Equals(""))
                {
                    object userAccessToken;
                    json.TryGetValue("access_token", out userAccessToken);
                    string url = "https://api.weixin.qq.com/sns/userinfo?access_token=" + userAccessToken.ToString().Trim()
                        + "&openid=" + openIdStr.Trim() + "&lang=zh_CN";
                    string jsonResult = Util.GetWebContent(url);
                    //Response.Write(code + "<br/>" + url + "<br/>" + jsonResult);
                    //Response.End();
                    unionId = Util.GetSimpleJsonValueByKey(jsonStr, "unionid");
                    if (!unionId.Trim().Equals(""))
                    {
                        DBHelper.InsertData("unionids", new string[,] { {"union_id", "varchar", unionId.Trim() },
                            {"open_id", "varchar", openIdStr.Trim() }, {"source", "varchar", "snowmeet_official_account" } });
                       
                    }

                }
            }
            catch
            {

            }




        }
        catch
        {

        }
        if (openIdStr.Trim().Equals(""))
        {
            return GetOpenId(code);
        }
        else
        {
            return openIdStr.Trim();
        }

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string code = Util.GetSafeRequestValue(Request, "code", "011991e1f9087a38af2d965e8f7cfa3A");
        string state = Util.GetSafeRequestValue(Request, "state", "1000");
        string openId = GetOpenId(code);

        if (!openId.Trim().Equals(""))
        {
            string callBack = Util.GetSafeRequestValue(Request, "callback", "pages/home_page.aspx");
            callBack = Server.UrlDecode(callBack);
            string token = WeixinUser.CreateToken(openId, DateTime.Now.AddMinutes(100));
            Session["user_token"] = token;
            Response.Redirect(callBack, true);
        }
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
