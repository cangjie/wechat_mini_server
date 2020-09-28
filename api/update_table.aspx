<%@ Page Language="C#" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "");
        string operOpenId = MiniUsers.CheckSessionKey(sessionKey.Trim());
        MiniUsers oper = new MiniUsers(operOpenId.Trim());
        if (!oper.role.Trim().Equals("staff"))
        {
            Response.Write("{\"status\": 1, \"err_msg\": \"Staff Only!\"}");
            Response.End();
        }
        string tableName = Util.GetSafeRequestValue(Request, "table", "");
        StreamReader sr = new StreamReader(Request.InputStream);
        string postJson = sr.ReadToEnd();
        sr.Close();


        /////////////////////////////
        //postJson = "{\"fields_data\": {\"memo\": \"111\", \"fix_board\": \"1\"}, \"keys\": {\"id\" : \"1\", \"shop\": \"万龙\" }}";
        /////////////////////////////


        Dictionary<string, object> fieldsDataObj = Util.GetObjectFromJsonByKey(postJson, "fields_data");
        Dictionary<string, object> keysObj = Util.GetObjectFromJsonByKey(postJson, "keys");

        string setClause = "";
        foreach (object key in fieldsDataObj.Keys)
        {
            setClause = setClause + ((!setClause.Trim().Equals("")) ? ", " : "") + key.ToString().Trim().Replace("'", "")
                + " = '" + fieldsDataObj[key.ToString()].ToString().Trim().Replace("'", "") + "' ";
        }

        string whereClause = "";
        foreach (object key in keysObj.Keys)
        {
            whereClause = whereClause + ((!whereClause.Trim().Equals("")) ? " and " : "") + key.ToString().Trim().Replace("'", "")
                + " = '" + keysObj[key.ToString()].ToString().Trim().Replace("'", "") + "' ";
        }

        try
        {
            string sql = " update " + tableName.Trim().Replace("'", "").Trim() + " set " + setClause + " where " + whereClause;
            SqlConnection conn = new SqlConnection(Util.conStr.Trim());
            SqlCommand cmd = new SqlCommand(sql, conn);
            conn.Open();
            int i = cmd.ExecuteNonQuery();
            conn.Close();
            cmd.Dispose();
            conn.Dispose();
            Response.Write("{\"status\": 0, \"affect_rows\": " + i.ToString() + "}");
        }
        catch
        { 
            Response.Write("{\"status\": 1}");
        }
    }
</script>