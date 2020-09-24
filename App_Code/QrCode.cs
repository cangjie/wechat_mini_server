using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for QrCode
/// </summary>
public class QrCode
{
    public DataRow _fields;

    public QrCode()
    {

    }

    public QrCode(long sceneId)
    {
        DataTable dt = DBHelper.GetDataTable(" select * from qr_code_scene where [id] = " + sceneId.ToString(), Util.conStr.Trim());
        if (dt.Rows.Count > 0)
        {
            _fields = dt.Rows[0];
        }
        else
        {
            if (sceneId >= 1000000000)
            {
                dt.Dispose();
                CreateScene(sceneId);
                dt = DBHelper.GetDataTable(" select * from qr_code_scene where [id] = " + sceneId.ToString(), Util.conStr.Trim());
                if (dt.Rows.Count > 0)
                {
                    _fields = dt.Rows[0];
                }
            }
        }
    }

    public long ID
    {
        get
        {
            return long.Parse(_fields["id"].ToString().Trim());
        }
    }

    public string Path
    {
        get
        {
            return _fields["path"].ToString().Trim();
        }
        set
        {
            string[,] updateParameters = new string[,] {
                {"path", "varchar", value.Trim()},
                {"last_update_time", "DateTime", DateTime.Now.ToString()}};
            string[,] keyParameters = new string[,] { { "id", "bigint", ID.ToString() } };
            DBHelper.UpdateData("qr_code_scene", updateParameters, keyParameters, Util.conStr);

        }
    }

    public DateTime LastUpdateTime
    {
        get
        {
            try
            {
                return DateTime.Parse(_fields["last_update_time"].ToString().Trim());
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
    }

    public static string GenerateNewQrCode(long sceneId, string qrRootPath)
    {
        string webPhysicalPath = System.Configuration.ConfigurationSettings.AppSettings["web_site_physical_path"].Trim();
        string qrCodePath = qrRootPath + "/" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0')
            + DateTime.Now.Day.ToString().PadLeft(2, '0');// + "/" + sceneId.ToString() + ".jpg";
        string qrCodePhysicalPath = webPhysicalPath + "\\" + qrCodePath.Replace("/", "\\").Trim();
        if (!Directory.Exists(qrCodePhysicalPath))
        {
            Directory.CreateDirectory(qrCodePhysicalPath.Trim());
        }
        string token = Util.GetToken();
        string ticketString = Util.GetSimpleJsonValueByKey(Util.GetWebContent("https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token=" + token.Trim(), "POST",
            "{\"expire_seconds\": 2592000, \"action_name\": \"QR_SCENE\", \"action_info\": {\"scene\": {\"scene_id\": "
            + sceneId.ToString() + " }}}", "text/html"), "ticket").Trim();
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" + ticketString.Trim());
        HttpWebResponse res = (HttpWebResponse)req.GetResponse();
        Stream s = res.GetResponseStream();
        int length = 0;
        byte[] buffer = new byte[1024 * 1024 * 10];
        for (int i = 0; i < buffer.Length; i++)
        {
            int currentByte = s.ReadByte();
            if (currentByte < 0)
            {
                length = i;
                break;
            }
            else
            {
                buffer[i] = (byte)currentByte;
            }
        }

        if (File.Exists(qrCodePhysicalPath + "\\" + sceneId.ToString() + ".jpg"))
        {
            try
            {
                File.Delete(qrCodePhysicalPath + "\\" + sceneId.ToString() + ".jpg");
            }
            catch
            {

            }
        }
        FileStream fs = File.Create(qrCodePhysicalPath + "\\" + sceneId.ToString() + ".jpg");
        for (int i = 0; i < length; i++)
        {
            fs.WriteByte(buffer[i]);
        }
        fs.Close();
        return qrCodePath + "/" + sceneId.ToString() + ".jpg";
    }

    public static string GetQrCode(long sceneId, string qrRootPath)
    {
        string path = "";
        QrCode qrCode = new QrCode(sceneId);
        TimeSpan span = new TimeSpan(0, 0, 2500000);
        if ((DateTime.Now - qrCode.LastUpdateTime) > span || !qrCode.Path.Trim().EndsWith(".jpg"))
        {
            path = GenerateNewQrCode(sceneId, qrRootPath);
            qrCode.Path = path;
        }
        else
        {
            path = qrCode.Path.Trim();
        }
        return path;
    }

    public static int CreateScene(long id)
    {
        if (id < 1000000000)
            return -2;
        int i = DBHelper.InsertData("qr_code_scene", new string[,] { { "id", "bigint", id.ToString() }, { "last_update_time", "DateTime", DateTime.Now.ToString() } });
        if (i > 0)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public static int CreateScene()
    {
        DataTable dtMax = DBHelper.GetDataTable("select max(id) from qr_code_scene where [id] < 1000000000", Util.conStr);
        int maxId = 1;
        try
        {
            maxId = int.Parse(dtMax.Rows[0][0].ToString());
            maxId++;
        }
        catch
        {

        }
        dtMax.Dispose();
        int i = DBHelper.InsertData("qr_code_scene", new string[,] { { "id", "bigint", maxId.ToString() }, { "last_update_time", "DateTime", DateTime.Now.ToString() } });
        if (i > 0)
        {
            return maxId;
        }
        else
        {
            return -1;
        }
    }

    public static string GetStaticQrCode(string sceneName, string qrRootPath)
    {
        string ret = "";
        ret = qrRootPath + "\\" + sceneName + ".jpg";
        if (!File.Exists(ret))
        {
            ret = GenerateStaticQrcode(sceneName, qrRootPath);
        }
        return ret;
    }

    public static string GetTempStaticQrCode(string sceneName, int expireSeconds, string qrRootPath)
    {
        string ret = "";
        string webPhysicalPath = System.Configuration.ConfigurationSettings.AppSettings["web_site_physical_path"].Trim();
        string qrCodePath = qrRootPath;
        string qrCodePhysicalPath = webPhysicalPath + "\\" + qrCodePath.Replace("/", "\\").Trim();
        string[] fileNameArr = Directory.GetFiles(qrCodePhysicalPath, sceneName.Trim() + "_*.jpg");
        bool found = false;
        foreach (string fileName in fileNameArr)
        {
            string realFileName = fileName.Split('\\')[fileName.Split('\\').Length - 1].Trim();
            string timeStamp = realFileName.Replace(sceneName.Trim(), "").Replace(".jpg", "").Trim();
            if (Regex.IsMatch(timeStamp, @"_\d+"))
            {
                timeStamp = timeStamp.Replace("_", "");
                long currentStamp = long.Parse(Util.GetTimeStamp(DateTime.Now));
                if (long.Parse(timeStamp) > currentStamp)
                {
                    found = true;
                    ret = qrRootPath + "/" + realFileName.Trim().Replace("\\", "/");
                    break;
                }
            }
        }
        if (!found)
        {
            ret = GenerateTempStaticQrcode(sceneName, expireSeconds, qrRootPath);
        }
        return ret;
    }

    public static string GenerateTempStaticQrcode(string sceneName, int expireSeconds, string qrRootPath)
    {
        DateTime createDateTime = DateTime.Now;
        string webPhysicalPath = System.Configuration.ConfigurationSettings.AppSettings["web_site_physical_path"].Trim();
        string qrCodePath = qrRootPath;// + "/" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0')
                                       //+ DateTime.Now.Day.ToString().PadLeft(2, '0');// + "/" + sceneId.ToString() + ".jpg";
        string qrCodePhysicalPath = webPhysicalPath + "\\" + qrCodePath.Replace("/", "\\").Trim();
        if (!Directory.Exists(qrCodePhysicalPath))
        {
            Directory.CreateDirectory(qrCodePhysicalPath.Trim());
        }
        string token = Util.GetToken();
        string ticketString = Util.GetSimpleJsonValueByKey(Util.GetWebContent("https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token=" + token.Trim(), "POST",
            "{\"expire_seconds\":" + expireSeconds.ToString() + ", \"action_name\": \"QR_STR_SCENE\", \"action_info\": {\"scene\": {\"scene_str\": \"" + sceneName.ToString() + "\" }}}", "text/html"), "ticket").Trim();
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" + ticketString.Trim());
        HttpWebResponse res = (HttpWebResponse)req.GetResponse();
        Stream s = res.GetResponseStream();
        int length = 0;
        byte[] buffer = new byte[1024 * 1024 * 10];
        for (int i = 0; i < buffer.Length; i++)
        {
            int currentByte = s.ReadByte();
            if (currentByte < 0)
            {
                length = i;
                break;
            }
            else
            {
                buffer[i] = (byte)currentByte;
            }
        }
        long expireTimeStamp = long.Parse(Util.GetTimeStamp(createDateTime)) + expireSeconds;
        string[] fileNameArr = Directory.GetFiles(qrCodePhysicalPath, sceneName.ToString() + "_*.jpg");
        foreach (string fileName in fileNameArr)
        {
            string realFileName = fileName.Split('\\')[fileName.Split('\\').Length - 1].Trim();
            string timeStamp = realFileName.Replace(sceneName.Trim(), "").Replace(".jpg", "").Trim();
            if (Regex.IsMatch(timeStamp.Trim(), @"_\d+"))
            {
                File.Delete(fileName.Trim());
            }
        }
        string qrCodeFileName = sceneName.ToString() + "_" + expireTimeStamp.ToString().Trim() + ".jpg";
        FileStream fs = File.Create(qrCodePhysicalPath + "\\" + qrCodeFileName.Trim());
        for (int i = 0; i < length; i++)
        {
            fs.WriteByte(buffer[i]);
        }
        fs.Close();
        return qrCodePath + "/" + qrCodeFileName.Trim();

    }



    public static string GenerateStaticQrcode(string sceneName, string qrRootPath)
    {
        string webPhysicalPath = System.Configuration.ConfigurationSettings.AppSettings["web_site_physical_path"].Trim();
        string qrCodePath = qrRootPath;// + "/" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0')
                                       //+ DateTime.Now.Day.ToString().PadLeft(2, '0');// + "/" + sceneId.ToString() + ".jpg";
        string qrCodePhysicalPath = webPhysicalPath + "\\" + qrCodePath.Replace("/", "\\").Trim();
        if (!Directory.Exists(qrCodePhysicalPath))
        {
            Directory.CreateDirectory(qrCodePhysicalPath.Trim());
        }
        string token = Util.GetToken();
        string ticketString = Util.GetSimpleJsonValueByKey(Util.GetWebContent("https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token=" + token.Trim(), "POST",
            "{\"action_name\": \"QR_LIMIT_STR_SCENE\", \"action_info\": {\"scene\": {\"scene_str\": \"" + sceneName.ToString() + "\" }}}", "text/html"), "ticket").Trim();
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" + ticketString.Trim());
        HttpWebResponse res = (HttpWebResponse)req.GetResponse();
        Stream s = res.GetResponseStream();
        int length = 0;
        byte[] buffer = new byte[1024 * 1024 * 10];
        for (int i = 0; i < buffer.Length; i++)
        {
            int currentByte = s.ReadByte();
            if (currentByte < 0)
            {
                length = i;
                break;
            }
            else
            {
                buffer[i] = (byte)currentByte;
            }
        }

        if (File.Exists(qrCodePhysicalPath + "\\" + sceneName.ToString() + ".jpg"))
        {
            try
            {
                File.Delete(qrCodePhysicalPath + "\\" + sceneName.ToString() + ".jpg");
            }
            catch
            {

            }
        }
        FileStream fs = File.Create(qrCodePhysicalPath + "\\" + sceneName.ToString() + ".jpg");
        for (int i = 0; i < length; i++)
        {
            fs.WriteByte(buffer[i]);
        }
        fs.Close();
        return qrCodePath + "/" + sceneName.ToString() + ".jpg";

    }
}