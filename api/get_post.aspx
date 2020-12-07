<%@ Page Language="C#" %>
<%@ Import Namespace="System.IO" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        Stream s = Request.InputStream;
        string json = (new StreamReader(s)).ReadToEnd().Trim();
        json = "{\"status\": 0, \"content\": { \"txt\": \"aa\" }}";
        Dictionary<string, object> equipInfo = Util.GetObjectFromJsonByKey(json, "content");   //.GetObjectArrayFromJsonByKey(json, "content");
        foreach (string k in equipInfo.Keys)
        { 
        
        }

    }
</script>