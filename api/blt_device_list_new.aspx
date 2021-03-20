<%@ Page Language="C#" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Write("{\"status\": 0, \"blt_devices\":[{\"scene\": \"maintain_on_site_label_print\", \"device_name\": \"Printer_1048\" }]}");
    }
</script>