﻿<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "hKC5nig2gEKJjktmponkbA==");
        string openId = MiniUsers.CheckSessionKey(sessionKey.Trim());
        MiniUsers mUser = new MiniUsers(openId.Trim());
        if (!mUser.role.Trim().Equals("staff"))
        {
            Response.Write("{\"status\": 0, \"error_message\": \"Staff only.\" }");
            Response.End();
        }
        string sql = " select distinct expierence_list.*, order_online.*, order_out_trade_no from expierence_list  "
            + " left join order_online on order_online.[id] = guarantee_order_id   "
            + " left join  weixin_payment_orders on (order_product_id = guarantee_order_id and  order_prepay_id is not null )"
            + " where pay_state = 1 and not exists( select 'a' from weixin_payment_orders_refund where out_trade_no = order_out_trade_no ) "
            + " and not exists ( select 'a' from wepay_order_refund where  ISNUMERIC(status) = 1 and wepay_out_trade_no = order_online.out_trade_no ) "
            + " order by expierence_list.[id] desc ";
        DataTable dt = DBHelper.GetDataTable(sql);

        string subJson = "";

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            subJson = subJson + ((i > 0) ? ", " : "") + Util.ConvertDataFieldsToJson(dt.Rows[i]);
        }
        Response.Write("{\"status\": 0, \"count\": " + dt.Rows.Count + ", \"expierence_list_arr\":[" + subJson + "]}");
    }
</script>