<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<!DOCTYPE html>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        DataTable dt = DBHelper.GetDataTable(" select card_no from covid19_service ");
        foreach (DataRow dr in dt.Rows)
        {
            EquipMaintainTask.CreateTaskFromCovid19Service(dr[0].ToString().Trim());
        }
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <%=Util.conStr%>
        </div>
    </form>
</body>
</html>
