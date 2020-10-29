<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>

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

        int detailId = int.Parse(Util.GetSafeRequestValue(Request, "detailid", "6"));
        DataTable dt = DBHelper.GetDataTable(" select * from maintain_task_detail_sub where  detail_id =" + detailId.ToString() +  " order by [sort],[id]  " );
        string results = "";
        foreach (DataRow dr in dt.Rows)
        {
            string rows = "";
            foreach (DataColumn dc in dt.Columns)
            {
                string fieldValue = dr[dc].ToString().Trim();
                if (!fieldValue.StartsWith("{") && !fieldValue.Trim().StartsWith("[")
                    && !fieldValue.Trim().StartsWith("'") && !fieldValue.Trim().StartsWith("\""))
                {
                    fieldValue = "\"" + fieldValue + "\"";
                }

                rows = rows + (rows.Trim().Equals("") ? "" : ", ") + "\"" + dc.Caption.Trim() + "\": " + fieldValue.Trim() ;
            }
            if (!rows.Trim().Equals(""))
            {
                results = results + (results.Trim().Equals("") ? "{" : ", {") + rows.Trim() + "}";
            }
        }

        Response.Write("{\"status\": 0, \"count\": " + dt.Rows.Count.ToString() + ", \"rows\": [" + results + "] }");
        dt.Dispose();

    }
</script>