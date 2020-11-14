<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string type = Util.GetSafeRequestValue(Request, "type", "resort_ski_pass");
        string id = Util.GetSafeRequestValue(Request, "id", "65");
        DataTable dt = DBHelper.GetDataTable(" select * from product "
            + (type.Trim().Equals("") ? " " : (" left join product_" + type.Trim() + " on product_id = [id] "))
            + " where product.[id] = " + id.Trim());
        if (dt.Rows.Count == 1)
        {
            Response.Write("{\"status\": 0, \"" + (type.Trim().Equals("") ? "product" : type.Trim()) + "\":" + Util.ConvertDataFieldsToJson(dt.Rows[0]) + " }");
        }
        else
        {
            Response.Write("{\"status\": 1}");
        }
    }
</script>