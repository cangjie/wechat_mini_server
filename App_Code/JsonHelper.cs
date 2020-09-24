using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for JsonHelper
/// </summary>
public class JsonHelper
{
    public static JavaScriptSerializer serializer = new JavaScriptSerializer();

    public Dictionary<string, object> json;

    public JsonHelper(string jsonStr)
    {
        json = (Dictionary<string, object>)serializer.DeserializeObject(jsonStr);
    }

    public string GetValue(string key)
    {
        object ret;
        json.TryGetValue(key, out ret);
        return ret.ToString();
    }


}