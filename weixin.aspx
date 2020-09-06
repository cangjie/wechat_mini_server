<%@ Page Language="C#" %>
<%@ Import Namespace="System.Security.Cryptography" %>
<%@ Import Namespace="System.Text" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="System.Net" %>
<script runat="server">

    public string token = "snowmeetfreeski";
    public string validResult = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (valid())
        {
            //Response.Write(Request["echostr"].Trim());
            //Response.End();
            Stream s = Request.InputStream;
            StreamReader sr = new StreamReader(s);
            string xmlContent = sr.ReadToEnd();
            sr.Close();
            XmlDocument xmlD = new XmlDocument();
            xmlD.LoadXml(xmlContent);
            
            ReceivedMessage receiveMessage = new ReceivedMessage(xmlD);
            ReceivedMessage.SaveReceivedMessage(receiveMessage);
            
            
        }
        else
        {
            Response.Write(validResult);
        }
    }
    

 
    
    public bool valid()
    {
        string sign = Request["signature"].Trim();
        string time = Request["timestamp"].Trim();
        string nonce = Request["nonce"].Trim();
        string[] strArr = new string[3];
        strArr[0] = token;
        strArr[1] = time;
        strArr[2] = nonce;
        Array.Sort(strArr);
        string arrStr = String.Join("", strArr);
        SHA1 sha = SHA1.Create();
        ASCIIEncoding enc = new ASCIIEncoding();
        byte[] bArr = enc.GetBytes(arrStr);
        bArr = sha.ComputeHash(bArr);
        validResult = "";
        for (int i = 0; i < bArr.Length; i++)
        {
            validResult = validResult + bArr[i].ToString("x").PadLeft(2,'0');
        }
        if (validResult == sign)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static string GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }

    public static string GetMd5(string str)
    {
        System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
        byte[] bArr = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
        string ret = "";
        foreach (byte b in bArr)
        {
            ret = ret + b.ToString("x").PadLeft(2, '0');
        }
        return ret;
    }


    public static bool CheckIfUserBind(string openId)
    {
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://www.luqinwenda.com/index.php?app=public&mod=Api&act=getUserOpenID&openID=" + openId);
        HttpWebResponse res = (HttpWebResponse)req.GetResponse();
        string str = (new StreamReader(res.GetResponseStream())).ReadToEnd();
        res.Close();
        req.Abort();
      //if (str.Trim().Equals("false"))
        if (str.Trim().Equals("﻿false"))
            return false;
        else
            return true;
    }


    public string[] GetHotQuestion()
    {
        string jsonStr = GetJsonFromUrl("http://www.luqinwenda.com/index.php?app=public&mod=api&act=getHotQuestion");
        return GetQuestions(jsonStr);
    }


    public string[] GetNewQuestion()
    {
        string jsonStr = GetJsonFromUrl("http://www.luqinwenda.com/index.php?app=public&mod=api&act=getNewQuestion");
        return GetQuestions(jsonStr);
    }

    public string[] GetQuestions(string jsonStr)
    {
        //File.AppendAllText(Server.MapPath("log/json.txt"), jsonStr + "\r\n");
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        Dictionary<string, object> json = (Dictionary<string, object>)serializer.DeserializeObject(jsonStr);
        object v;
        json.TryGetValue("data", out v);
        object[] vArr = (object[])v;
        string[] qArr = new string[vArr.Length];
        for (int i = 0; i < qArr.Length; i++)
        {
            object o = vArr[i];
            Dictionary<string, object> d = (Dictionary<string, object>)o;
            qArr[i] = d["body"].ToString().Trim();
        }
        return qArr;
    }



    public string GetJsonFromUrl(string url)
    {
        string s = GetContentFromUrl(url);
        /*
        byte[] bArr = Encoding.UTF8.GetBytes(s);
        byte[] bArrNew = new byte[bArr.Length - 3];
        for (int i = 0; i < bArrNew.Length; i++)
        {
            bArrNew[i] = bArr[i + 3];
        }
        s = Encoding.UTF8.GetString(bArrNew);
         */ 
        return s;

    }

    public string GetContentFromUrl(string url)
    {
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        HttpWebResponse res = (HttpWebResponse)req.GetResponse();
        string s = (new StreamReader(res.GetResponseStream())).ReadToEnd();
        res.Close();
        req.Abort();
        return s;
    }
    
</script>