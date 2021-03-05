<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        int id = int.Parse(Util.GetSafeRequestValue(Request, "id", "314"));
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "CblxlJGhC0nRQ2gyiK2mTw==");
        string openId = MiniUsers.CheckSessionKey(sessionKey);
        MiniUsers user = new MiniUsers(openId);
        if (!user.role.Trim().Equals("staff"))
        {
            Response.Write("{\"status\": 1, \"error_message\": \"Staff only.\"}");
            Response.End();
        }
        EquipMaintainRequestInshop inShopTask = new EquipMaintainRequestInshop(id);
        if (!inShopTask._fields["task_flow_num"].ToString().Trim().Equals(""))
        {
            Response.Write("{\"status\": 1, \"error_message\": \"Task flow num has already allocate.\"}");
            Response.End();
        }
        DateTime orderDate = DateTime.Parse(inShopTask._fields["create_date"].ToString()).Date;
        DataTable dt = DBHelper.GetDataTable(" select * from maintain_in_shop_request where order_id > 0 "
            + " and exists ( select 'a' from order_online where pay_state = 1 and order_online.[id] = order_id  ) "
            + " and create_date >= '" + orderDate.ToShortDateString() + "' and task_flow_num is not null  ");
        string taskFlowNum = orderDate.Year.ToString().Substring(2, 2) + orderDate.Month.ToString().PadLeft(2, '0')
            + orderDate.Day.ToString().PadLeft(2, '0') + "-" + (dt.Rows.Count + 1).ToString().PadLeft(5, '0');
        int i = DBHelper.UpdateData("maintain_in_shop_request", new string[,] { { "task_flow_num", "varchar", taskFlowNum } },
            new string[,] { { "id", "int", id.ToString() } }, Util.conStr);
        if (i == 1)
        {
            Response.Write("{\"status\": 0, \"task_flow_num\": \"" + taskFlowNum.Trim() + "\"}");
        }
        else
        { 
            Response.Write("{\"status\": 1, \"error_message\": \"Can't update record.\"}");
        }
       
    }
</script>