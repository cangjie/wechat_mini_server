<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        DataTable dt = DBHelper.GetDataTable("select * from brand_list order by brand_type, brand_name");
        string arrayJson = "";
        foreach(DataRow dr in dt.Rows)
        {
            string itemJson = Util.ConvertDataFieldsToJson(dr);
            arrayJson = arrayJson + (!arrayJson.Trim().Equals("") ? ", " : "") + itemJson;
        }
        Response.Write("{\"status\": 0, \"brand_list\": [" + arrayJson.Trim() + "]}");
    }
</script>
