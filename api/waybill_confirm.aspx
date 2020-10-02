<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "hKC5nig2gEKJjktmponkbA==");
        string openId = MiniUsers.CheckSessionKey(sessionKey);
        MiniUsers user = new MiniUsers(openId);
        if (!user.role.Trim().Equals("staff"))
        {
            Response.Write("{\"status\": 1, \"err_msg\": \"Staff Only!\"}");
            Response.End();
        }

        string waybillNo = Util.GetSafeRequestValue(Request, "waybillno", "SF1190439112999").Trim();
        int waybillStatus = int.Parse(Util.GetSafeRequestValue(Request, "state", "1").Trim());
        string strOrderIds = Util.GetSafeRequestValue(Request, "orderids", "108,109,110").Trim();
        string memo = Util.GetSafeRequestValue(Request, "memo", "");
        int i = DBHelper.UpdateData("waybill_log", new string[,] { { "valid", "int", waybillStatus.ToString() } },
            new string[,] { { "waybill_no", "varchar", waybillNo.Trim() },
            {"oper", "varchar", openId.Trim() } }, Util.conStr.Trim());
        if (i == 0)
        {
            DBHelper.InsertData("waybill_log", new string[,] { {"waybill_no", "varchar", waybillNo.Trim() },
                {"valid", "int", waybillStatus.ToString().Trim() }, {"oper", "varchar", openId.Trim() },
                {"memo", "varchar",  memo.Trim()} });
        }
        if (waybillStatus == 1)
        {
            string sql = "update maintain_task set waybill_no = '" + waybillNo.Trim() + "' where [id] in (" + strOrderIds.Trim() + ")";
            SqlConnection conn = new SqlConnection(Util.conStr);
            SqlCommand cmd = new SqlCommand(sql, conn);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            cmd.Dispose();
            conn.Dispose();
            foreach (string strOrderId in strOrderIds.Split(','))
            {
                try
                {
                    DBHelper.InsertData("maintain_task_log", new string[,] {
                        {"task_id", "int", strOrderId.Trim() }, {"oper_open_id", "varchar", openId.Trim() }, 
                        {"oper", "varchar", "waybill_confirm" }
                    });
                    EquipMaintainTask.CreateSteps(int.Parse(strOrderId.Trim()));
                }
                catch
                {

                }
            }
        }
        
        Response.Write("{\"status\": 0}");
    }
</script>