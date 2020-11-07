<%@ Page Language="C#" %>

<!DOCTYPE html>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {

    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <script type="text/javascript" >
                function callpay() {
                    if (typeof WeixinJSBridge == "undefined") {
                        if (document.addEventListener) {
                            document.addEventListener('WeixinJSBridgeReady', jsApiCall, false);
                        } else if (document.attachEvent) {
                            document.attachEvent('WeixinJSBridgeReady', jsApiCall);
                            document.attachEvent('onWeixinJSBridgeReady', jsApiCall);
                        }
                        document.write((typeof WeixinJSBridge) + "<br/>");
                    } else {
                        jsApiCall();
                    }
                }


                function jsApiCall() {
            

                    WeixinJSBridge.invoke('getBrandWCPayRequest',
                        {
                            "appId": "343434",
                            "timeStamp": "3343434",
                            "nonceStr": "34343434",
                            "package": "prepay_id=ererer",
                            "signType": "effddd",
                            "paySign": "ereee"
                        },
                        function (res) {
                    //alert(res.err_code + "!" + res.err_desc + "!" + res.err_msg);
                            document.write(res.err_msg);

                        
                        }
                    

                      );
                }

                document.onload = callpay();
            </script>
        </div>
    </form>
</body>
</html>
