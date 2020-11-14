<%@ Page Language="C#" %>
<!DOCTYPE html>
<script runat="server">

    public string currentResort = "nanshan";

    public Product[] prodArr;

    public WeixinUser currentUser;

    public string openId = "";

    public string userToken = "";

    public KeyValuePair<DateTime, string>[] selectedDate = new KeyValuePair<DateTime, string>[5];



    protected void Page_Load(object sender, EventArgs e)
    {
        

        string currentPageUrl = Request.Url.ToString().Split('?')[0].Trim();
        if (!Request.QueryString.ToString().Trim().Equals(""))
        {
            currentPageUrl = currentPageUrl + "?" + Request.QueryString.ToString().Trim();
        }
        if (Session["user_token"] == null || Session["user_token"].ToString().Trim().Equals(""))
        {
            Response.Redirect("../authorize.aspx?callback=" + currentPageUrl, true);
        }
        
        userToken = Session["user_token"].ToString();

        //userToken = "efa86b2cb53ff14b4500298208effda1652c863ac117668953d4ef93f807351b4ff11040";
        openId = WeixinUser.CheckToken(userToken);
        if (openId.Trim().Equals(""))
        {
            Response.Redirect("../authorize.aspx?callback=" + currentPageUrl, true);
        }
        currentUser = new WeixinUser(WeixinUser.CheckToken(userToken));

        /*
        if (currentUser.CellNumber.Trim().Equals("") || currentUser.VipLevel < 1)
            Response.Redirect("register_cell_number.aspx?refurl=" + currentPageUrl, true);
            */

        string resort = Util.GetSafeRequestValue(Request, "resort", "南山");
        if (!resort.Trim().Equals(""))
        {
            currentResort = resort;
            Session["default_resort"] = currentResort;
        }
        else
        {
            if (Session["default_resort"] != null && !Session["default_resort"].ToString().Equals(""))
            {
                currentResort = Session["default_resort"].ToString().Trim();
            }
            else
            {
                currentResort = "qiaobo";
                Session["default_resort"] = currentResort;
            }
        }

        prodArr = Product.GetSkiPassList(currentResort);
    }

    


</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <!-- 新 Bootstrap 核心 CSS 文件 -->
    <link rel="stylesheet" href="css/bootstrap.min.css">
    <link rel="stylesheet" href="css/normalize.css" />
    <!-- 可选的Bootstrap主题文件（一般不用引入） -->
    <link rel="stylesheet" href="css/bootstrap-theme.min.css">

    <script type="text/javascript" src="js/popper.min.js" ></script>
    <!-- jQuery文件。务必在bootstrap.min.js 之前引入 -->
    <script src="js/jquery.min.js"></script>
    <!-- 最新的 Bootstrap 核心 JavaScript 文件 -->
    <script src="js/bootstrap.min.js"></script>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <script type="text/javascript" >

        var pre_order_product_obj_arr;

        var current_product_id = 0;

        var can_book = false;

        function launch_book_modal(product_id) {
            current_product_id = product_id;
            fill_modal_new(product_id);
            select_date_num(product_id, document.getElementById("current_date").innerText,
                document.getElementById("current_num").innerText);
            $("#booking_modal").modal();
        }

        function select_date_num(product_id, ski_date, count) {
            $.ajax({
                url: "/api/get_ski_pass_product.aspx?id=" + product_id + "&count=" + count + "&skidate=" + ski_date,
                method: "GET",
                async: false,
                success: function (msg, status) {
                    can_book = false;
                    var obj = eval("(" + msg + ")");
                    pre_order_product_obj_arr = obj.results;
                    var div_summary = document.getElementById("summary");
                    div_summary.innerHTML = "";
                    var summary_price = 0;
                    for (var i = 0; i < pre_order_product_obj_arr.length; i++) {
                        var stock_num = parseInt(pre_order_product_obj_arr[i].product_info.stock_num);
                        if (stock_num == -1 || stock_num >= count) {
                            summary_price = summary_price
                                + (parseFloat(pre_order_product_obj_arr[i].product_info.sale_price)
                                + parseFloat(pre_order_product_obj_arr[i].product_info.deposit)) * parseFloat(pre_order_product_obj_arr[i].count);
                            div_summary.innerHTML = div_summary.innerHTML + '(' + pre_order_product_obj_arr[i].product_info.name +
                                ((pre_order_product_obj_arr[i].product_info.deposit == 0) ? '' : ' + 押金：' + pre_order_product_obj_arr[i].product_info.deposit.toString())
                                + ')' + ' x '
                                + pre_order_product_obj_arr[i].count.toString() + ' = '
                                + summary_price.toString()
                                + "<br/>";
                            current_product_id = pre_order_product_obj_arr[i].product_info.product_id;
                            can_book = true;
                        }
                    }
                    div_summary.innerHTML = div_summary.innerHTML + "小计：" + summary_price.toString();
                }
            });
        }

       

        function book_ski_pass() {

            if (!can_book) {
                return;
            }

            var cart_json = '';

            var pass_json = '{ "product_id": "' + current_product_id + '", "count": "' + document.getElementById("current_num").innerHTML.trim() + '" }';
            var rent_json = '';
            

            cart_json = '{"cart_array" : [' + pass_json + ((rent_json != '') ? (', ' + rent_json) : '') + '], "memo" : { "use_date" : "'
                + document.getElementById("current_date").innerText.trim() + '"   }}';

            

            $.ajax({
                url: "/api/place_online_order.aspx",
                async: false,
                type: "GET",
                data: { "cart": cart_json, "token": "<%=userToken%>" },
                success: function(msg, status) {
                    var msg_object = eval("(" + msg + ")");
                    window.location.href = "../payment/payment.aspx?product_id=" + msg_object.order_id;
                }
            });
        }

      
        function fill_modal_new(product_id) {

            $.ajax({
                url: "/api/get_product_info.aspx?type=resort_ski_pass&id=" + product_id,
                method: "GET",
                async: false,
                success: function (msg, status) {
                    var msg_obj = eval("(" + msg + ")");
                    if (msg_obj.status == 0) {
                        product_obj = msg_obj.resort_ski_pass;
                        

                    }
                }
            });
            var today_is_available = false;
            var current_date_time = new Date();
            var end_sale_time_string_arr = product_obj.end_sale_time.split(':');
            var end_sale_time = new Date();
            end_sale_time.setHours(parseInt(end_sale_time_string_arr[0]));// + parseInt(end_sale_time_string_arr[1]));
            end_sale_time.setMinutes(parseInt(end_sale_time_string_arr[1]));
            if (current_date_time < end_sale_time) {
                today_is_available = true;
            }
            var start_selected_date = current_date_time;
            if (!today_is_available) {
                start_selected_date.setDate(current_date_time.getDate() + 1);
            }
            document.getElementById("current_date").innerHTML = start_selected_date.getFullYear().toString() + '-'
                + (start_selected_date.getMonth() + 1).toString() + '-' + start_selected_date.getDate().toString();
            var temp_inner_html = '';
            var start_selected_date_str = start_selected_date.getFullYear().toString() + '-'
                    + (start_selected_date.getMonth() + 1).toString() + '-' + start_selected_date.getDate().toString();
            for (var i = 0; i < 5; i++) {
                var date_str = start_selected_date.getFullYear().toString() + '-'
                    + (start_selected_date.getMonth() + 1).toString() + '-' + start_selected_date.getDate().toString();
                temp_inner_html = temp_inner_html + '<a href="#" class="dropdown-item" onclick="select_date(\'' +
                    date_str + '\')" >' + date_str  + '</a>';
                start_selected_date.setDate(start_selected_date.getDate() + 1);
            }
            document.getElementById("drop-down-date-menu").innerHTML = temp_inner_html;
            select_date(start_selected_date_str);
        }

        function select_date(date) {
            document.getElementById("current_date").innerHTML = date;
            select_date_num(current_product_id,
                document.getElementById("current_date").innerText,
                document.getElementById("current_num").innerText);
        }

        function select_num(num) {
            document.getElementById("current_num").innerHTML = num;
            select_date_num(current_product_id,
                document.getElementById("current_date").innerText,
                document.getElementById("current_num").innerText);
        }
     
    </script>
</head>
<body>
    <div>
        <ul class="nav nav-tabs" >
            <!--li class="nav-item">
                <a class=nav-link" href="ski_pass_product_list.aspx?resort=<%=Server.UrlEncode("万龙") %>" >万龙</a>
            </li-->
            <!--li class="nav-item">
                <a class="nav-link" href="ski_pass_product_list_new.aspx?resort=<%=Server.UrlEncode("南山") %>" >南山</a>
            </li-->
            <li class="nav-item">
                <a class="nav-link" href="ski_pass_product_list_new.aspx?resort=<%=Server.UrlEncode("八易自带") %>" >八易自带</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="ski_pass_product_list_new.aspx?resort=<%=Server.UrlEncode("八易租单板") %>" >八易租单板</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="ski_pass_product_list_new.aspx?resort=<%=Server.UrlEncode("八易租双板") %>" >八易租双板</a>
            </li>
        </ul>
        <%
            foreach (Product p in prodArr)
            {
             %>
        <br />
        <div id="ticket-1" name="ticket" class="panel panel-success" style="width:350px" onclick="launch_book_modal('<%=p._fields["id"].ToString().Trim() %>')" >
            <div class="panel-heading">
                <h3 class="panel-title"><%=p._fields["name"].ToString() %></h3>
            </div>
            <div class="panel-body">
                价格：<%=p.SalePrice.ToString()%> <%if (!p._fields["stock_num"].ToString().Trim().Equals("-1")) {
                                                       %>剩余：<%=p._fields["stock_num"].ToString().Trim() %>张<%
                                                   } %><br />
                <%=p._fields["rules"].ToString().Trim() %>
            </div>
        </div>
        <%} %>

        <div id="booking_modal" class="modal fade bs-example-modal-lg" tabindex="-1" role="dialog" aria-labelledby="myLargeModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header" id="modal-header" ><%=currentResort %></div>
                    <div class="modal-body" >
                        <div>日期：<span class="dropdown">
                                <button class="btn btn-secondary btn-sm dropdown-toggle" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" id="dropdownSelectDate" >
                                    <span id="current_date" ></span>
                                    <span class="caret"></span>
                                </button>
                                <div class="dropdown-menu" aria-labelledby="dropdownMenuButton" id="drop-down-date-menu" ></div>
                            </span>
                        </div>
			            <br/>
                        <div>
                            人数：<span class="dropdown" >
                                <button class="btn btn-default dropdown-toggle" type="button" id="dropdownSelectNum" data-toggle="dropdown">
                                    <span id="current_num" >1</span>
                                    <span class="caret"></span>
                                </button>
                                <ul class="dropdown-menu" role="menu" aria-labelledby="dropdownMenu1"  >
                                    <li role="presentation"><a role="menuitem" tabindex="-1" href="#" onclick="select_num(1)" >1</a></li>
                                    <li role="presentation"><a role="menuitem" tabindex="-1" href="#" onclick="select_num(2)" >2</a></li>
                                    <li role="presentation"><a role="menuitem" tabindex="-1" href="#" onclick="select_num(3)" >3</a></li>
                                    <li role="presentation"><a role="menuitem" tabindex="-1" href="#" onclick="select_num(4)" >4</a></li>
                                    <li role="presentation"><a role="menuitem" tabindex="-1" href="#" onclick="select_num(5)" >5</a></li>
                                </ul>
                            </span>
                            
                        </div>
			            <br/>
                      
                       
                        <div id="summary" >小计：</div>
                    </div>
                    <div class="modal-footer" ><button type="button" class="btn btn-default" onclick="book_ski_pass()"> 确 认 预 定 </button></div>
                </div>
            </div>
        </div>
    </div>
</body>
</html>
