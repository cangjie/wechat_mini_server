﻿<%@ Page Language="C#" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
     
        Response.Write("{\"status\": 0, "
            + "\"blt_devices\":[{\"scene\": \"maintain_on_site_label_print\", \"device_name\": \"Printer_1048\" }, "
            + "{\"scene\": \"maintain_on_site_label_print\", \"device_name\": \"Printer_51EA\" }, "
            + "{\"scene\": \"maintain_on_site_label_print\", \"device_name\": \"Printer_73E7\" }, "
            + "{\"scene\": \"maintain_on_site_label_print\", \"device_name\": \"Printer_CA10\" }, "
            + "{\"scene\": \"maintain_on_site_label_print\", \"device_name\": \"Printer_7371\" } "
            + " ]}");
        
    }
</script>