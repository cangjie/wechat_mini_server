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
    public MiniUsers()
    {
        //
        // TODO: Add constructor logic here
        //
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
            dt.Dispose();
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