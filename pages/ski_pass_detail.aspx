<%@ Page Language="C#" %>

<!DOCTYPE html>

<script runat="server">

    public string orderId = "";
    public string code = "";
    public OnlineSkiPass pass;
    public Product p;
    public OnlineOrder order;
    public OnlineOrderDetail detail;
    public MiniUsers currentUser;
    public string openId = "";
    public string userToken = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        orderId = Util.GetSafeRequestValue(Request, "orderid", "0");
        code = Util.GetSafeRequestValue(Request, "code", "");



        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "");

        if (!sessionKey.Trim().Equals(""))
        {
            Session["sessionkey"] = sessionKey.Trim();
        }

        if (sessionKey.Trim().Equals("") && Session["sessionkey"] != null && !Session["sessionkey"].ToString().Trim().Equals(""))
        {
            sessionKey = Session["sessionkey"].ToString().Trim();
        }

        openId = MiniUsers.CheckSessionKey(sessionKey);

        currentUser = new MiniUsers(openId);

        pass = new OnlineSkiPass(code);
        order = pass.associateOnlineOrder;
        detail = pass.associateOnlineOrderDetail;
        p = new Product(detail.productId);
        if (!openId.Trim().Equals(pass.owner.OpenId.Trim()))
            Response.End();

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
</head>
<body>
    <form id="form1" runat="server">
    <div>
            <div id="ticket-<%=code.Trim()%>" name="ticket" class="card" style="width:350px"  >
            <div class="card-header">
                <h3 class="card-title"><%=detail.productName.Trim() %></h3>
            </div>
            <div class="card-body">
                <% if (p._fields["intro"].ToString().Trim().Equals(""))
                  { %>
                <ul>
                    <li>价格：<font color="red" ><%=order._fields["order_price"].ToString() %></font>元，张数：
                        <%=pass.associateOnlineOrderDetail.count.ToString() %>张 
                        <%=(pass.Rent ? ",<font color='red' >租板</font>" : "") %></li>
                    <%
    if (detail.productName.IndexOf("南山") >= 0)
    {
                         %>
                    <li>到达代理商入口请拨打：13693171170，将有工作人员接您入场。</li>
                    <li>来店请出示二维码验票、取票。</li>
                    <li>此票售出后不予退换。</li>
                    <%
    }
    if (detail.productName.IndexOf("万龙八易") >= 0)
    {
        

                                %>
                    <p>雪票价格：<%=p._fields["sale_price"].ToString().Trim() %></p>
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
                    <li>滑雪时间：17:30-22:00</li>
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
                <p>雪场地址：<br />北京市丰台区射击场路甲12号万龙八易滑雪场<br />客服电话：13714100910<br />日场时间：09:00-18:00<br />夜场时间：17:30-22:00（周日-周四）；
                    <br />&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; 17:30-22:30（周五、周六、
                    <br />&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; 春节初一到初六、<font color='red' >除夕不营业</font>）</p>
                                    <%


    }
                         %>
                </ul>
                <%}
                    else
                    {
                        Response.Write(p._fields["intro"].ToString().Trim());
                    } %>
                <br />
                <div style="text-align:center" >
                    <img src="http://weixin.snowmeet.top/show_qrcode.aspx?sceneid=3<%=code %>" style="width:200px; text-align:center"  />
                    <br />
                    <b style="text-align:center" ><%=code.Substring(0,3) %>-<%=code.Substring(3,3) %>-<%=code.Substring(6,3) %></b>
                </div>
            </div>



        </div>
    </div>
    </form>
</body>
</html>
