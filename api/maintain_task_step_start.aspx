<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "4lXltyNe3GrEYozN0rskag==");
        string openId = MiniUsers.CheckSessionKey(sessionKey);
        MiniUsers user = new MiniUsers(openId);
        if (!user.role.Trim().Equals("staff"))
        {
            Response.Write("{\"status\": 1, \"err_msg\": \"Staff Only!\"}");
            Response.End();
        }

        int stepId = int.Parse(Util.GetSafeRequestValue(Request, "stepid", "6"));
        int taskId = 0;
        DataTable dt = DBHelper.GetDataTable(" select * from maintain_task_detail where [id] = " + stepId.ToString());
        if (dt.Rows.Count > 0)
        {
            taskId = int.Parse(dt.Rows[0]["task_id"].ToString().Trim());
        }
        int i = EquipMaintainTask.CreateSubSteps(stepId);
        try
        {
            DBHelper.InsertData("maintain_task_log", new string[,] {
                    {"task_id", "int", taskId.ToString() },
                    {"oper_open_id", "varchar", openId.Trim() },
                    {"detail_id", "int", stepId.ToString() },
                    {"oper", "varchar", "create sub steps" }
                });
        }
        catch
        {

        }
        DBHelper.UpdateData("maintain_task_detail", new string[,] {{"oper_open_id", "varchar", openId.Trim() },
            {"start_date_time", "datetime", DateTime.Now.ToString() }, {"status", "varchar", "已开始" } },
            new string[,] { {"id", "int", stepId.ToString() } }, Util.conStr.Trim());
        try
        {
            DBHelper.InsertData("maintain_task_log", new string[,] {
                    {"task_id", "int", taskId.ToString() },
                    {"oper_open_id", "varchar", openId.Trim() },
                    {"detail_id", "int", stepId.ToString() },
                    {"oper", "varchar", "status set to start" }
                });
        }
        catch
        {

        }
    }
</script>