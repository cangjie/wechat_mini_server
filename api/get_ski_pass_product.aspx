<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Collections" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {

        DateTime skiDate = DateTime.Parse(Util.GetSafeRequestValue(Request, "skidate", "2020-11-29"));
        int count = int.Parse(Util.GetSafeRequestValue(Request, "count", "1"));
        int productId = int.Parse(Util.GetSafeRequestValue(Request, "id", "86"));

        int totalCount = 25;

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


        if (p._fields["shop"].ToString().Trim().Equals("南山"))
        {
            try
            {
                Product p = new Product(productId);

                int type = 0;
                if (p._fields["name"].ToString().IndexOf("夜") >= 0 && p._fields["shop"].ToString().Trim().Equals("南山"))
                {
                    type = 1;
                }
                string numStr = Util.GetWebContent("/core/OrderOnlines/GetSkiPassNum/" + type.ToString() + "?dateStr=" + skiDate.Year.ToString() + "-" + skiDate.Month.ToString() + "-" + skiDate.Day.ToString());
                if (int.Parse(numStr) > totalCount)
                {
                    itemJson = "";
                }
            }
            catch
            {

            }
        }





        Response.Write("{\"results\":[" + itemJson.Trim() + "]}");

    }
</script>