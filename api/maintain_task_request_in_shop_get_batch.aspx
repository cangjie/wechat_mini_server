<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "hKC5nig2gEKJjktmponkbA==");
        int batchId = int.Parse(Util.GetSafeRequestValue(Request, "batchid", "1"));
        string openId = MiniUsers.CheckSessionKey(sessionKey);
        MiniUsers user = new MiniUsers(openId);
        if (!user.role.Trim().Equals("staff") )
        {
            Response.Write("{\"status\": 1, \"error_message\": \"Have no right.\"}");
        }
        DataTable dt = DBHelper.GetDataTable(" select * from maintain_in_shop_request where batch_id =  " + batchId.ToString());
        string jsonArray = "";
        foreach (DataRow dr in dt.Rows)
        {
            string jsonItem = Util.ConvertDataFieldsToJson(dr);
            jsonArray = jsonArray + (!jsonArray.Trim().Equals("") ? ", " : "") + jsonItem.Trim();
        }
        Response.Write("{\"status\": 0, \"count\": " + dt.Rows.Count.ToString()
            + ", \"maintain_in_shop_request\":[" + jsonArray + "]}");
    }
</script>