using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Net;
using System.IO;

/// <summary>
/// Summary description for RepliedMessage
/// </summary>
public class RepliedMessage
{
    public int id = 0;
    public string rootId = "";
    public string from = "";
    public string to = "";
    public int messageCount = 0;
    public string content = "";
    public string type = "";
    public string mediaId = "";
    public string desc = "";
    public string musicUrl = "";
    public string highQualityMusicUrl = "";
    public string thumbMediaId = "";
    public string url = "";
    public string picUrl = "";
    public bool isService = false;
    public bool hasSent = false;
    

    public  struct news
    {
        public string title;
        public string description;
        public string picUrl;
        public string url;
    }

	public RepliedMessage()
	{
		
	}

    public RepliedMessage(int messageId)
    {
        SqlDataAdapter da = new SqlDataAdapter(" select * from wxreplymsg where wxreplymsg_id = " + id.ToString(), Util.conStr);
        DataTable dt = new DataTable();
        da.Fill(dt);
        da.Dispose();
        if (dt.Rows.Count == 1)
        {
            id = messageId;
            rootId = dt.Rows[0]["wxreplymsg_rootid"].ToString().Trim();
            from = dt.Rows[0]["wxreplymsg_from"].ToString().Trim();
            to = dt.Rows[0]["wxreplymsg_to"].ToString().Trim();
            messageCount = int.Parse(dt.Rows[0]["wxreplymsg_msgcount"].ToString().Trim());
            content = dt.Rows[0]["wxreplymsg_content"].ToString().Trim();
            type = dt.Rows[0]["wxreplymsg_msgtype"].ToString().Trim();
            mediaId = dt.Rows[0]["wxreplymsg_mediaid"].ToString().Trim();
            desc = dt.Rows[0]["wxreplymsg_desc"].ToString().Trim();
            musicUrl = dt.Rows[0]["wxreplymsg_musicurl"].ToString().Trim();
            highQualityMusicUrl = dt.Rows[0]["wxreplymsg_hqmusicurl"].ToString().Trim();
            thumbMediaId = dt.Rows[0]["wxreplymsg_thumbmediaid"].ToString().Trim();
            url = dt.Rows[0]["wxreplymsg_url"].ToString().Trim();
            picUrl = dt.Rows[0]["wxreplymsg_picurl"].ToString().Trim();
            isService = (dt.Rows[0]["wxreplymsg_isservice"].ToString().Trim().Equals("1")? true:false);
            hasSent = (dt.Rows[0]["wxreplymsg_send"].ToString().Trim().Equals("1") ? true : false);
            
        }
        else
        {
            throw new Exception("Record not exists!");
        }
        dt.Dispose();
    }

    public void SendAsServiceMessage()
    {
        string url = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token=" + Util.GetToken();
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.Method = "post";
        req.ContentType = "raw";
        StreamWriter sw = new StreamWriter(req.GetRequestStream());
        sw.Write(jsonFormatData);
        sw.Close();
        HttpWebResponse res = (HttpWebResponse)req.GetResponse();
        res.Close();
        req.Abort();
    }

    public news[] newsContent
    {
        get
        {
            if (type.Trim().Equals("news"))
            {
                XmlDocument xmlD = new XmlDocument();
                xmlD.LoadXml("<root>" + content + "</root>");
                XmlNodeList nl = xmlD.SelectSingleNode("//root").ChildNodes;
                news[] newsArray = new news[nl.Count];
                for (int i = 0; i < nl.Count; i++)
                {
                    newsArray[i] = new news();
                    newsArray[i].title = nl[i].SelectSingleNode("Title").InnerText.Trim();
                    newsArray[i].description = nl[i].SelectSingleNode("Description").InnerText.Trim();
                    newsArray[i].picUrl = nl[i].SelectSingleNode("PicUrl").InnerText.Trim();
                    newsArray[i].url = nl[i].SelectSingleNode("Url").InnerText.Trim();
                }
                return newsArray;
            }
            else
            {
                return new news[0];
            }

        }
        set
        {
            string contentStr = "";
            messageCount = value.Length;
            foreach (news n in value)
            {
                contentStr = contentStr + "<item>"
                    + "<Title><![CDATA[" + n.title.Trim() + "]]></Title>"
                    + "<Description><![CDATA[" + n.description.Trim() + "]]></Description>"
                    + "<PicUrl><![CDATA[" + n.picUrl.Trim() + "]]></PicUrl>"
                    + "<Url><![CDATA[" + n.url + "]]></Url>"
                    + "</item>";
            }
            content = contentStr;
        }
    }

    public string jsonFormatData
    {
        get
        {
            string json = "{";
            json = json + "\"touser\":\"" + to.Trim() + "\",";
            json = json + "\"msgtype\":\"" + type.Trim() + "\",";
            json = json + "\"" + type.Trim() + "\":{";
            switch (type.Trim().ToLower())
            { 
                case "text":
                    json = json + "\"content\":\"" + content.Trim() + "\"";
                    break;
                case "image":
                    json = json + "\"media_id\":\"" + content.Trim() + "\"";
                    break;
                case "voice":
                    json = json + "\"media_id\":\"" + content.Trim() + "\"";
                    break;
                case "video":
                    json = json + "\"media_id\":\"" + mediaId.Trim() + "\" , ";
                    json = json + "\"thumb_media_id\":\"" + thumbMediaId.Trim() + "\" , ";
                    json = json + "\"title\":\"" + content.Trim() + "\" , ";
                    json = json + "\"description\":\"" + desc.Trim() + "\"  ";
                    break;
                case "news":
                    json = json + "\"articles\":[";
                    for (int i = 0; i < newsContent.Length; i++)
                    {
                        json = json + "{";
                        json = json + "\"title\":\"" + newsContent[i].title.Trim() + "\",";
                        json = json + "\"description\":\"" + newsContent[i].description.Trim() + "\",";
                        json = json + "\"url\":\"" + newsContent[i].url.Trim() + "\",";
                        json = json + "\"picurl\":\"" + newsContent[i].picUrl.Trim() + "\"";
                        json = json + "}";
                        if (i < newsContent.Length - 1)
                            json = json + ",";
                    }
                    json = json + "]";
                    json = json + "}";
                    break;
                default:
                    break;
            }
            json = json + "}";
            json = json + "}";
            return json;
        }
    }

    public static string CovertXmlStringFromNews(news[] newsMessageArr)
    {
        string xmlStr = "";
        foreach (news n in newsMessageArr)
        {
            xmlStr = xmlStr + "<item>"
                + "<Title><![CDATA[" + n.title.Trim().Replace("'", "") + "</Title>"
                + "<Description><![CDATA[" + n.description.Trim().Replace("'", "") + "</Description>"
                + "<PicUrl><![CDATA[" + n.picUrl.Trim().Replace("'", "") + "</PicUrl>"
                + "<Url><![CDATA[" + n.url.Trim().Replace("'", "") + "</Url>"
                + "</item>";

        }
        
        return xmlStr;
    }

    public static int AddRepliedMessage(RepliedMessage repliedMessage)
    {
        string sqlStr = " insert into wxreplymsg ( "
            + " wxreplymsg_rootid , "
            + " wxreplymsg_from , "
            + " wxreplymsg_to , "
            + " wxreplymsg_msgcount , "
            + " wxreplymsg_msgtype , "
            + " wxreplymsg_content , "
            + " wxreplymsg_mediaid , "
            + " wxreplymsg_desc , "
            + " wxreplymsg_musicurl , "
            + " wxreplymsg_hqmusicurl , "
            + " wxreplymsg_thumbmediaid , "
            + " wxreplymsg_url , "
            + " wxreplymsg_picurl , "
            + " wxreplymsg_isservice , "
            + " wxreplymsg_send ) values ("
            + " '" + repliedMessage.rootId.Trim().Replace("'", "") + "' , "
            + " '" + repliedMessage.from.Trim().Replace("'", "") + "' , "
            + " '" + repliedMessage.to.Trim().Replace("'", "") + "' , "
            + " '" + repliedMessage.messageCount.ToString().Trim().Replace("'", "") + "'  , "
            + " '" + repliedMessage.type.Trim().Replace("'", "") + "' ,  "
            + " '" + repliedMessage.content.Trim().Replace("'", "") + "'  ,  "
            + " '" + repliedMessage.mediaId.Trim().Replace("'", "") + "' ,  "
            + " '" + repliedMessage.desc.Trim().Replace("'", "") + "' ,  "
            + " '" + repliedMessage.musicUrl.Trim().Replace("'", "") + "' , "
            + " '" + repliedMessage.highQualityMusicUrl.Trim().Replace("'", "") + "' ,  "
            + " '" + repliedMessage.thumbMediaId.Trim().Replace("'", "") + "' ,  "
            + " '" + repliedMessage.url.Trim().Replace("'", "") + "' ,  "
            + " '" + repliedMessage.picUrl.Trim().Replace("'", "") + "' ,  "
            + " '" + (repliedMessage.isService ? "1" : "0") + "' , "
            + " '" + (repliedMessage.hasSent ? "1" : "0") + "' )  ";
        int maxId = 0;
        SqlConnection conn = new SqlConnection(Util.conStr.Trim());
        SqlCommand cmd = new SqlCommand(sqlStr, conn);
        conn.Open();
        int i = cmd.ExecuteNonQuery();
        if (i == 1)
        {
            cmd.CommandText = " select max(wxreplymsg_id) from wxreplymsg ";
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
                i = dr.GetInt32(0);
            dr.Close();
        }
        conn.Close();
        cmd.Dispose();
        conn.Dispose();
        repliedMessage.id = i;
        return i;

    }
}