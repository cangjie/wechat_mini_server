using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Message
/// </summary>
public class ReceivedMessage
{
    public string id = "";
    public string to = "";
    public string from = "";
    public int time = 0;
    public string type = "";
    public string picUrl = "";
    public string mediaId = "";
    public string format = "";
    public string thumbMediaId = "";
    public string lat = "";
    public string lon = "";
    public string scale = "";
    public string label = "";
    public string title = "";
    public string description = "";
    public string url = "";
    public string content = "";
    public string userEvent = "";
    public string eventKey = "";
    public string recognition = "";
    public bool hasReplied = false;
    public bool hasDealed = false;
    public DateTime createDate = DateTime.Now;
    public DateTime updateDate = DateTime.Now;

    public ReceivedMessage()
    { 
    
    }

    public ReceivedMessage(string id)
    {
        SqlDataAdapter da = new SqlDataAdapter(" select * from wxreceivemsg where wxreceivemsg_id = '" + id.Trim().Replace("'", "") + "'  ", Util.conStr);
        DataTable dt = new DataTable();
        da.Fill(dt);
        da.Dispose();
        if (dt.Rows.Count == 0)
        {
            throw new Exception("Record not exists!");
        }
        else
        {
            id = dt.Rows[0]["wxreceivemsg_id"].ToString().Trim();
            to = dt.Rows[0]["wxreceivemsg_to"].ToString().Trim();
            from = dt.Rows[0]["wxreceivemsg_from"].ToString().Trim();
            time = int.Parse(dt.Rows[0]["wxreceivemsg_time"].ToString().Trim());
            type = dt.Rows[0]["wxreceivemsg_type"].ToString().Trim();
            picUrl = dt.Rows[0]["wxreceivemsg_picurl"].ToString().Trim();
            mediaId = dt.Rows[0]["wxreceivemsg_mediaid"].ToString().Trim();
            format = dt.Rows[0]["wxreceivemsg_format"].ToString().Trim();
            thumbMediaId = dt.Rows[0]["wxreceivemsg_thumbmediaid"].ToString().Trim();
            lat = dt.Rows[0]["wxreceivemsg_locationx"].ToString().Trim();
            lon = dt.Rows[0]["wxreceivemsg_locationy"].ToString().Trim();
            scale = dt.Rows[0]["wxreceivemsg_scale"].ToString().Trim();
            label = dt.Rows[0]["wxreceivemsg_label"].ToString().Trim();
            title = dt.Rows[0]["wxreceivemsg_title"].ToString().Trim();
            description = dt.Rows[0]["wxreceivemsg_description"].ToString().Trim();
            url = dt.Rows[0]["wxreceivemsg_url"].ToString().Trim();
            url = dt.Rows[0]["wxreceivemsg_content"].ToString().Trim();
            userEvent = dt.Rows[0]["wxreceivemsg_event"].ToString().Trim();
            eventKey = dt.Rows[0]["wxreceivemsg_eventkey"].ToString().Trim();
            recognition = dt.Rows[0]["wxreceivemsg_recognition"].ToString().Trim();
            hasReplied = (dt.Rows[0]["wxreceivemsg_isreply"].ToString().Trim().Equals("0")? false:true);
            hasDealed = (dt.Rows[0]["wxreceivemsg_deal"].ToString().Trim().Equals("0")?false:true);
            createDate = DateTime.Parse(dt.Rows[0]["wxreceivemsg_crt"].ToString().Trim());
            try
            {
                updateDate = DateTime.Parse(dt.Rows[0]["wxreceivemsg_upd"].ToString().Trim());
            }
            catch
            { 
            
            }
        }
    }

    public ReceivedMessage(XmlDocument xmlD)
	{
        to = xmlD.SelectSingleNode("//xml/ToUserName").InnerText.Trim();
        from = xmlD.SelectSingleNode("//xml/FromUserName").InnerText.Trim();
        try
        {
            time = int.Parse(xmlD.SelectSingleNode("//xml/CreateTime").InnerText.Trim());
        }
        catch
        { 
        
        }
        type = xmlD.SelectSingleNode("//xml/MsgType").InnerText.Trim();
        id = "";
        if (type == "event")
        {
            DateTime nowDate = DateTime.Now;
            id = "event_" + nowDate.Year.ToString() + nowDate.Month.ToString().PadLeft(2, '0') + nowDate.Day.ToString().PadLeft(2, '0')
                + nowDate.Hour.ToString().PadLeft(2, '0') + nowDate.Minute.ToString().PadLeft(2, '0') + nowDate.Second.ToString().PadLeft(2, '0')
                + nowDate.Millisecond.ToString().PadLeft(3, '0');
        }
        else
        {

            id = xmlD.SelectSingleNode("//xml/MsgId").InnerText.Trim();
        }

        switch (type.ToLower().Trim())
        { 
            case "text":
                content = xmlD.SelectSingleNode("//xml/Content").InnerText.Trim();
                break;
            case "event":
                userEvent = xmlD.SelectSingleNode("//xml/Event").InnerText.Trim();
                try
                {
                    XmlNode keyNode = xmlD.SelectSingleNode("//xml/EventKey");
                    if (keyNode != null)
                    {
                        eventKey = keyNode.InnerText.Trim();
                    }
                }
                catch
                { 
                
                }
                if (userEvent.ToLower().Trim().Equals("location"))
                {
                    lat = xmlD.SelectSingleNode("//xml/Latitude").InnerText.Trim();
                    lon = xmlD.SelectSingleNode("//xml/Longitude").InnerText.Trim();
                    scale = xmlD.SelectSingleNode("//xml/Precision").InnerText.Trim();
                }
                break;
            case "image":
                picUrl = xmlD.SelectSingleNode("//xml/PicUrl").InnerText.Trim();
                mediaId = xmlD.SelectSingleNode("//xml/MediaId").InnerText.Trim();
                break;
            case "voice":
                format = xmlD.SelectSingleNode("//xml/Format").InnerText.Trim();
                mediaId = xmlD.SelectSingleNode("//xml/MediaId").InnerText.Trim();
                try
                {
                    recognition = xmlD.SelectSingleNode("//xml/Recognition").InnerText.Trim();
                }
                catch
                { 
                
                }
                break;
            case "video":
                thumbMediaId = xmlD.SelectSingleNode("//xml/ThumbMediaId").InnerText.Trim();
                mediaId = xmlD.SelectSingleNode("//xml/MediaId").InnerText.Trim();
                break;
            case "location":
                lat = xmlD.SelectSingleNode("//xml/Location_X").InnerText.Trim();
                lon = xmlD.SelectSingleNode("//xml/Location_Y").InnerText.Trim();
                scale = xmlD.SelectSingleNode("//xml/Scale").InnerText.Trim();
                label = xmlD.SelectSingleNode("//xml/Label").InnerText.Trim();
                break;
            case "link":
                title = xmlD.SelectSingleNode("//xml/Title").InnerText.Trim();
                description = xmlD.SelectSingleNode("//xml/Description").InnerText.Trim();
                url = xmlD.SelectSingleNode("//xml/Url").InnerText.Trim();
                break;
            default:
                break;
        }
	}

    public bool isEvent
    {
        get
        {
            if (type.Trim().ToLower().Equals("event"))
                return true;
            else
                return false;
        }
    }

    public bool isMenuClick
    {
        get
        {
            bool ret = false;
            if (isEvent)
            {
                if (userEvent.ToLower().Trim().Equals("click"))
                    ret = true;
            }
            return ret;
        }
    }

    public bool isMenuView
    {
        get
        {
            bool ret = false;
            if (isEvent)
            {
                if (userEvent.ToLower().Trim().Equals("view"))
                    ret = true;
            }
            return ret;
        }
    }

    public static bool HasBeenSaved(ReceivedMessage receivedMessage)
    {
        try
        {
            new ReceivedMessage(receivedMessage.id);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static int SaveReceivedMessage(ReceivedMessage receivedMessage)
    {
        if (HasBeenSaved(receivedMessage))
        {
            UpdateDatabase(receivedMessage);
        }
        else
        {
            SaveToDatabase(receivedMessage);
        }
        return 0;
    }

    public static bool SaveToDatabase(ReceivedMessage receivedMessage)
    {
        string sqlStr = " insert into wxreceivemsg ("
            + " wxreceivemsg_id, "
            + " wxreceivemsg_to, "
            + " wxreceivemsg_from, "
            + " wxreceivemsg_time, "
            + " wxreceivemsg_type, "
            + " wxreceivemsg_picurl, "
            + " wxreceivemsg_mediaid, "
            + " wxreceivemsg_format, "
            + " wxreceivemsg_thumbmediaid, "
            + " wxreceivemsg_locationx, "
            + " wxreceivemsg_locationy, "
            + " wxreceivemsg_scale, "
            + " wxreceivemsg_label, "
            + " wxreceivemsg_title, "
            + " wxreceivemsg_description, "
            + " wxreceivemsg_url, "
            + " wxreceivemsg_content, "
            + " wxreceivemsg_event, "
            + " wxreceivemsg_eventkey, "
            + " wxreceivemsg_recognition, "
            + " wxreceivemsg_isreply, "
            + " wxreceivemsg_deal )  "
            + " values ("
            + "'" + receivedMessage.id.Trim().Replace("'", "") + "'  , "
            + "'" + receivedMessage.to.Trim().Replace("'", "") + "'  , "
            + "'" + receivedMessage.from.Trim().Replace("'", "") + "'  , "
            + "'" + receivedMessage.time.ToString().Replace("'", "") + "'  , "
            + "'" + receivedMessage.type.Trim().Replace("'", "") + "'  , "
            + "'" + receivedMessage.picUrl.Trim().Replace("'", "") + "'  , "
            + "'" + receivedMessage.mediaId.Trim().Replace("'", "") + "'  , "
            + "'" + receivedMessage.format.Trim().Replace("'", "") + "'  , "
            + "'" + receivedMessage.thumbMediaId.Trim().Replace("'", "") + "'  , "
            + "'" + receivedMessage.lon.Trim().Replace("'", "") + "'  , "
            + "'" + receivedMessage.lat.Trim().Replace("'", "") + "'  , "
            + "'" + receivedMessage.scale.Trim().Replace("'", "") + "'  , "
            + "'" + receivedMessage.label.Trim().Replace("'", "") + "'  , "
            + "'" + receivedMessage.title.Trim().Replace("'", "") + "'  , "
            + "'" + receivedMessage.description.Trim().Replace("'", "") + "'  , "
            + "'" + receivedMessage.url.Trim().Replace("'", "") + "'  , "
            + "'" + receivedMessage.content.Trim().Replace("'", "") + "'  , "
            + "'" + receivedMessage.userEvent.Trim().Replace("'", "") + "'  , "
            + "'" + receivedMessage.eventKey.Trim().Replace("'", "") + "'  , "
            + "'" + receivedMessage.recognition.Trim().Replace("'", "") + "'  , "
            + "'0'  , "
            + "'0'  ) ";
        SqlConnection conn = new SqlConnection(Util.conStr.Trim());
        SqlCommand cmd = new SqlCommand(sqlStr, conn);
        conn.Open();
        int i = 0;
        try
        {
            i = cmd.ExecuteNonQuery();
        }
        catch
        { 
        
        }
        conn.Close();
        cmd.Dispose();
        conn.Dispose();
        if (i == 0)
            return false;
        else
            return true;
    }

    public static bool UpdateDatabase(ReceivedMessage receivedMessage)
    {
        string sqlStr = "  update wxreceivemsg set  "
            + " wxreceivemsg_to = '" + receivedMessage.to.Trim().Replace("'", "") + "'  , "
            + " wxreceivemsg_from = '" + receivedMessage.from.Trim().Replace("'", "") + "'  , "
            + " wxreceivemsg_time = '" + receivedMessage.time.ToString().Trim().Replace("'", "") + "'  , "
            + " wxreceivemsg_type = '" + receivedMessage.type.Trim().Replace("'", "") + "'  , "
            + " wxreceivemsg_picurl = '" + receivedMessage.picUrl.Trim().Replace("'", "") + "'  , "
            + " wxreceivemsg_mediaid = '" + receivedMessage.mediaId.Trim().Replace("'", "") + "'  , "
            + " wxreceivemsg_format = '" + receivedMessage.format.Trim().Replace("'", "") + "'  , "
            + " wxreceivemsg_thumbmediaid = '" + receivedMessage.thumbMediaId.Trim().Replace("'", "") + "'  , "
            + " wxreceivemsg_locationx = '" + receivedMessage.lat.Trim().Replace("'", "") + "'  , "
            + " wxreceivemsg_locationy = '" + receivedMessage.lon.Trim().Replace("'", "") + "'  , "
            + " wxreceivemsg_scale = '" + receivedMessage.scale.Trim().Replace("'", "") + "'  , "
            + " wxreceivemsg_label = '" + receivedMessage.label.Trim().Replace("'", "") + "'  , "
            + " wxreceivemsg_title = '" + receivedMessage.title.Trim().Replace("'", "") + "'  , "
            + " wxreceivemsg_description = '" + receivedMessage.description.Trim().Replace("'", "") + "'  , "
            + " wxreceivemsg_url = '" + receivedMessage.url.Trim().Replace("'", "") + "'  , "
            + " wxreceivemsg_content = '" + receivedMessage.content.Trim().Replace("'", "") + "'  , "
            + " wxreceivemsg_event = '" + receivedMessage.userEvent.Trim().Replace("'", "") + "'  , "
            + " wxreceivemsg_eventkey = '" + receivedMessage.eventKey.Trim().Replace("'", "") + "'  , "
            + " wxreceivemsg_recognition = '" + receivedMessage.recognition.Trim().Replace("'", "") + "'  , "
            + " wxreceivemsg_isreply = '" + (receivedMessage.hasReplied ? "1" : "0") + "'  , "
            + " wxreceivemsg_deal = '" + (receivedMessage.hasDealed ? "1" : "0") + "'  , "
            + " wxreceivemsg_upd = dbo.GetLocalDate(DEFAULT)   "
            + " where  wxreceivemsg_id = '" + receivedMessage.id.Trim().Replace("'", "") + "'   ";
        SqlConnection conn = new SqlConnection(Util.conStr.Trim());
        SqlCommand cmd = new SqlCommand(sqlStr, conn);
        conn.Open();
        int i = 0;
        try
        {
            i = cmd.ExecuteNonQuery();
        }
        catch
        {

        }
        conn.Close();
        cmd.Dispose();
        conn.Dispose();
        if (i == 0)
            return false;
        else
            return true;
        
    }
}