<%@ Page Language="C#" %>

<!DOCTYPE html>

<script runat="server">

    public string used = "0";

    public MiniUsers currentUser;

    public string userToken = "";

    public string openId = "";

    public OnlineSkiPass[] passArr;

    protected void Page_Load(object sender, EventArgs e)
    {
        used = Util.GetSafeRequestValue(Request, "used", "0");
        /*
        string currentPageUrl = Server.UrlEncode("/pages/ski_pass_list.aspx");
        if (Session["user_token"] == null || Session["user_token"].ToString().Trim().Equals(""))
        {
            Response.Redirect("../authorize.aspx?callback=" + currentPageUrl, true);
        }
        userToken = Session["user_token"].ToString();
        openId = WeixinUser.CheckToken(userToken);
        if (openId.Trim().Equals(""))
        {
            Response.Redirect("../authorize.aspx?callback=" + currentPageUrl, true);
        }
        */
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "");

        if (!sessionKey.Trim().Equals(""))
        {
            Session["sessionkey"] = sessionKey.Trim();
        }

        if (sessionKey.Trim().Equals("") && Session["sessionkey"] != null && !Session["sessionkey"].ToString().Trim().Equals(""))
        {
            sessionKey = Session["sessionkey"].ToString().Trim();
        }

        try
        {
            string openId = MiniUsers.CheckSessionKey(sessionKey);

            currentUser = new MiniUsers(openId);

            passArr = OnlineSkiPass.GetOnlieSkiPassByOwnerOpenId(openId);
        }
        catch
        {
            Response.End();
        }
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <!-- 新 Bootstrap 核心 CSS 文件 -->
    <link rel="stylesheet" href="css/bootstrap.min.css">
    <link rel="stylesheet" href="css/normalize.css" />
    <!-- 可选的Bootstrap主题文件（一般不用引入） -->
    <link rel="stylesheet" href="css/bootstrap-theme.min.css">
    <!-- jQuery文件。务必在bootstrap.min.js 之前引入 -->
    <script src="js/jquery.min.js"></script>
    <!-- 最新的 Bootstrap 核心 JavaScript 文件 -->
    <script src="js/bootstrap.min.js"></script>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <script type="text/javascript" >
        function go_to_detail(order_id, card_code) {
            window.location.href = "ski_pass_detail.aspx?orderid=" + order_id + "&code=" + card_code;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <ul class="nav nav-tabs" >
            <li class="nav-item"  ><a class="nav-link<%if (used.Trim().Equals("0"))
                { %> active<%}  %>" href="ski_pass_list.aspx?used=0">未使用</a></li>
            <li class="nav-item"  ><a  class="nav-link<%if (used.Trim().Equals("1"))
                { %> active<%}  %>"  href="ski_pass_list.aspx?used=1">已使用</a></li>
        </ul>
        <%
            foreach (OnlineSkiPass pass in passArr)
            {
                bool valid = false;
                if (used.Trim().Equals("1") == pass.used)
                    valid = true;
                if (valid)
                {
                    Product p = new Product(pass.associateOnlineOrderDetail.productId);
                    if (!p.Type.Trim().Equals("雪票"))
                    {
                        continue;
                    }
                    %>
        <br />
        <div id="ticket-1" name="ticket" class="card" style="width:350px" onclick="go_to_detail('<%=pass.associateOnlineOrder._fields["id"].ToString().Trim() %>','<%=pass.cardCode %>')" >
            <div class="card-header">
                <h3 class="card-title"><%=p._fields["name"].ToString() %></h3>
            </div>
            <div class="card-body">
                <p>张数：<%=pass.associateOnlineOrderDetail.count.ToString() %>张 <%=(pass.Rent? ",<font color='red' >租板</font>":"") %></p>
                        <%
    if (p._fields["intro"].ToString().Trim().Equals(""))
    {
        if (p._fields["name"].ToString().IndexOf("南山") >= 0)
        {


                     %>
                <p>如租板，押金200元。</p>
                <p>价格包括：门票、滑雪、缆车、拖牵、魔毯费用、（如租板，则包含雪具使用）。</p>
                <p>如需租用雪板、雪鞋、雪杖以外的物品，如头盔、雪镜、雪服等物品，请额外准备现金，押金 100元/件。</p>
                <p>使用说明：</p>
                <ul>
                    <li><font color="red" >出票日：<%=pass.AppointDate.ToShortDateString() %>，将于该日自动出票。</font></li>
                    <li>到达代理商入口请拨打：13521733301，将有工作人员接您入场。</li>
                    <li>来店请出示二维码验票、取票。</li>
                    <li>此票售出后不予退换。</li>
                </ul>
                <p>雪场地址：<br />北京市密云区河南寨镇圣水头村南山滑雪场<br />客服电话：13521733301</p>
                <%}
    else
    {
        if (p._fields["name"].ToString().IndexOf("八易") >= 0)
        {
                        %>


                <p>价格包括：滑雪、缆车、拖牵、魔毯费用。（不包含保险，保险请在窗口另行购买）</p>
                <p>使用说明：</p>
                <ul>
                    <li><font color="red" >出票日自动出票。</font></li>
                    <li>前往易龙雪聚八易店出示二维码验票、取票。</li>
                    <li>此票售出后不予退换。</li>
                    <%
    if (p._fields["name"].ToString().IndexOf("半天") >= 0)
    {
                            %>
                    <li>滑雪时间：刷第一次门禁（缆车/魔毯）开始计时</li>
                                <%
    }
    if (p._fields["name"].ToString().IndexOf("全天") >= 0)
    {
                                        %>
                    <li>滑雪时间：9:00-18:00</li>
                    <%
    }
    if (p._fields["name"].ToString().Trim().IndexOf("夜场") >= 0)
    {
                            %>
                    <li>滑雪时间：平日17:00-24:00 节假日通宵</li>
                                <%
    }
    if (p._fields["name"].ToString().IndexOf("自助餐") >= 0)
    {
                                        %>
                    <li>用餐时间：17:00-21:00</li>
                                            <%
    }
                         %>

                    
                </ul>
                <p>雪场地址：<br />北京市丰台区射击场路甲12号万龙八易滑雪场<br />客服电话：13693171170<br />日场时间：09:00-18:00<br />夜场时间：17:00-24:00（周日-周四）；
                    <br />&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; 周末节假日通宵
                    <br />&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; </p>


                <%
    }
    else if (p._fields["name"].ToString().IndexOf("乔波") >= 0)
    {
                            %>
                <p>价格包括：门票，滑雪，魔毯，拖牵，雪板，雪鞋，雪杖等费用。</p>
                <p>注：如需租用雪服，头盔，雪镜等物品需单独从押金里面扣除相关费用。</p>
                <p>预定须知：提前一天预定。</p>
                <p>使用说明：</p>
                <ul>
                    <li><font color="red" >出票日自动出票。</font></li>
                    <li>来店请出示二维码验票、取票。</li>
                    <li>滑雪结束后凭押金单在雪馆前台办理退押金手续。</li>
                    <li>此票售出后不予退换。</li>
                </ul>
                <p>雪场地址：<br />北京市顺义区顺安路6号<br />客服电话：15701179221</p>
                <%
                            }
                        }
                    }
                    else
                    {
                        Response.Write(p._fields["intro"].ToString());
                    }
                     %>
            </div>














        </div>
        
        <%
                }
            }
             %>
    </div>
    </form>
</body>
</html>
