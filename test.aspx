<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<!DOCTYPE html>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        EquipMaintainTaskDetail detail = new EquipMaintainTaskDetail(7);
        detail.SetStatus("已开始", "test");
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <%= (new EquipMaintainTask(2)).Status.Trim() %>
        </div>
    </form>
</body>
</html>
