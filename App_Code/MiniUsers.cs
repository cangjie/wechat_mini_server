using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for MiniUsers
/// </summary>
public class MiniUsers
{
    public DataRow _fields;

    public MiniUsers()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public MiniUsers(string openId)
    {
        DataTable dt = DBHelper.GetDataTable(" select * from mini_users where open_id = '" + openId.Trim() + "' ");
        if (dt.Rows.Count > 0)
        {
            _fields = dt.Rows[0];
        }
        else
        {

            throw new Exception("Key " + openId.Trim() + " Not Found!");
        }
    }

    public string role
    {
        get 
        {
            if (_fields["is_admin"].ToString().Equals("1"))
            {
                return "staff";
            }
            else
            {
                return "customer";
            }
        }
    }

    public string OpenId
    {
        get
        {
            return _fields["open_id"].ToString().Trim();
        }
    }

    public string OfficialAccountOpenId
    {
        get
        {
            string openId = "";
            DataTable dt = DBHelper.GetDataTable(" select * from unionids where source = 'snowmeet_official_account' and union_id = '" + OpenId.Trim().Replace("'", "") + "' ");
            if (dt.Rows.Count > 0)
            {
                openId = dt.Rows[0]["open_id"].ToString();
            }
            return openId;
        }
    }

    public string CellNumber
    {
        get
        {
            return _fields["cell_number"].ToString().Trim();
        }
    }

    public string Nick
    {
        get
        {
            return _fields["nick"].ToString().Trim();
        }
    }

    public string RealName
    {
        get
        {
            return _fields["real_name"].ToString().Trim();
        }
    }
    public static string CheckSessionKey(string sessionKey)
    {
        string openId = "";
        DataTable dt = DBHelper.GetDataTable(" select * from mini_session where session_key = '" + sessionKey.Trim() + "' order by create_date desc ");
        if (dt.Rows.Count == 1)
        {
            openId = dt.Rows[0]["open_id"].ToString().Trim();
        }
        dt.Dispose();
        return openId.Trim();
    }
    public static string UserLogin(string code)
    {
        string sessionKeyJson = Util.GetWebContent("https://api.weixin.qq.com/sns/jscode2session?appid="
            + Util.appId.Trim() + "&secret=" + Util.appSecret.Trim() + "&js_code=" + code.Trim() + "&grant_type=authorization_code");
        string openId = "";
        string unionId = "";
        string sessionKey = "";
        try
        {
            openId = Util.GetSimpleJsonValueByKey(sessionKeyJson, "openid").Trim();
        }
        catch
        {

        }
        try
        {
            unionId = Util.GetSimpleJsonValueByKey(sessionKeyJson, "unionid").Trim();
        }
        catch
        {

        }
        try
        {
            sessionKey = Util.GetSimpleJsonValueByKey(sessionKeyJson, "session_key").Trim();
        }
        catch
        {

        }
        if (!openId.Trim().Equals("") && !sessionKey.Trim().Equals(""))
        {
            DataTable dt = DBHelper.GetDataTable(" select * from mini_users where open_id = '" + openId.Trim() + "' ");
            if (dt.Rows.Count == 0)
            {
                DBHelper.InsertData("mini_users", new string[,] { {"open_id", "varchar", openId.Trim() },
                    {"union_id", "varchar", unionId.Trim() } });
            }
            else
            {
                if (!unionId.Trim().Equals(""))
                {
                    DBHelper.UpdateData("mini_users", new string[,] { { "union_id", "varchar", unionId.Trim() } },
                        new string[,] { { "open_id", "varchar", openId.Trim()} }, Util.conStr.Trim());
                    //DBHelper.InsertData("")
                    
                }
                
            }
            dt.Dispose();
            if (!unionId.Trim().Equals("") && !openId.Trim().Equals(""))
            {
                DataTable dtUnionIdOpenId = DBHelper.GetDataTable(" select * from unionids where union_id = '" + unionId.Trim()
                            + "' and open_id = '" + openId.Trim() + "' ");
                if (dtUnionIdOpenId.Rows.Count == 0)
                {
                    DBHelper.InsertData("unionids", new string[,] { {"union_id", "varchar", unionId.Trim() },
                            {"open_id", "varchar", openId.Trim() }, {"source", "varchar", "snowmeet_mini" } });

                }
                dtUnionIdOpenId.Dispose();
            }
            dt = DBHelper.GetDataTable(" select * from mini_session where session_key = '" + sessionKey.Trim()
                + "' and open_id = '" + openId.Trim() + "' ");
            if (dt.Rows.Count == 0)
            {
                DBHelper.InsertData("mini_session", new string[,] { {"session_key", "varchar", sessionKey.Trim() },
                    {"open_id", "varchar", openId.Trim() } });
            }
        }
        return sessionKey.Trim();
    }
}