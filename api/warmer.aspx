<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string customerSessionKey = Util.GetSafeRequestValue(Request, "customer", "");
        string staffSessionKey = Util.GetSafeRequestValue(Request, "staff", "");
        string action = Util.GetSafeRequestValue(Request, "action", "query");

        string customerOpenId = MiniUsers.CheckSessionKey(customerSessionKey);



        DataTable dtWarmer = DBHelper.GetDataTable(" select * from warmer where customer_open_id = '" + customerOpenId + "' order by [id] desc ");
        switch (action.Trim())
        {
            case "demand":
                string staffOpenId = MiniUsers.CheckSessionKey(staffSessionKey);
                if (dtWarmer.Rows.Count == 0)
                {
                    int i = DBHelper.InsertData("warmer", new string[,] { {"customer_open_id", "varchar", customerOpenId.Trim()},
                        {"staff_open_id", "varchar", staffOpenId.Trim()} });
                    if (i == 1)
                    {
                        Response.Write("{\"status\": 0}");
                    }
                    else
                    {
                        Response.Write("{\"status\": 1, \"error_message\": \"Can't demand warmer now.\" }");
                    }
                }
                break;
            default:
                if (dtWarmer.Rows.Count > 0)
                {
                    string itemJson = Util.ConvertDataFieldsToJson(dtWarmer.Rows[0]);
                    Response.Write("{\"status\": 0, \"have_demand\": 1, \"last_demand\": " + itemJson + "}");
                }
                else
                { 
                    Response.Write("{\"status\": 0, \"have_demand\": 0 }");
                }

                break;
        }
    }
</script>