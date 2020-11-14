<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Collections" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {

        DateTime skiDate = DateTime.Parse(Util.GetSafeRequestValue(Request, "skidate", "2020-11-6"));
        int count = int.Parse(Util.GetSafeRequestValue(Request, "count", "1"));
        int productId = int.Parse(Util.GetSafeRequestValue(Request, "id", "86"));

        //Dictionary<SkiPass, int> skiPassPair = new Dictionary<SkiPass, int>(); ;
        KeyValuePair<SkiPass, int> skiPassPair;
        ArrayList arr = new ArrayList();

        SkiPass skiPass = new SkiPass(productId);
        foreach (SkiPass s in skiPass.SameTimeSkiPass)
        {
            if (s.IsAvailableDay(skiDate) && s.IsValid)
            {
                if (s.InStockCount < count)
                {
                    skiPassPair = new KeyValuePair<SkiPass, int>(s, s.InStockCount);
                    arr.Add(skiPassPair);
                    count = count - s.InStockCount;
                }
                else
                {
                    skiPassPair = new KeyValuePair<SkiPass, int>(s, count);
                    arr.Add(skiPassPair);
                    count = 0;
                }
                if (count == 0)
                {
                    break;
                }
            }
        }

        string itemJson = "";

        foreach (Object o in arr)
        {
            skiPassPair = (KeyValuePair<SkiPass, int>)o;
            itemJson = itemJson + (itemJson.Trim().Equals("")? "" : ", ") + "{\"count\": " + skiPassPair.Value.ToString()
                + ", \"product_info\": " + Util.ConvertDataFieldsToJson(skiPassPair.Key._fields) +"} ";
        }
        Response.Write("{\"results\":[" + itemJson.Trim() + "]}");

    }
</script>