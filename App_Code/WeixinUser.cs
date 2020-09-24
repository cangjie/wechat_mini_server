using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for User
/// </summary>
public class WeixinUser : ObjectHelper
{
    public WeixinUser()
    {
        tableName = "users";
        primaryKeyName = "open_id";
        //primaryKeyValue = openId.Trim();
    }





    public WeixinUser(string openId)
    {
        tableName = "users";
        primaryKeyName = "open_id";
        primaryKeyValue = openId.Trim();
        DataTable dt = DBHelper.GetDataTable(" select * from users where open_id = '" + openId.Trim() + "' ");
        if (dt.Rows.Count == 0)
        {
            //throw new Exception("not found");
            string json = Util.GetWebContent("https://api.weixin.qq.com/cgi-bin/user/info?access_token="
            + Util.GetToken() + "&openid=" + openId + "&lang=zh_CN");
            if (json.IndexOf("errocde") >= 0)
            {
                throw new Exception("not found");
            }
            else
            {
                try
                {
                    JsonHelper jsonObject = new JsonHelper(json);
                    string nick = jsonObject.GetValue("nickname");
                    string headImageUrl = jsonObject.GetValue("headimgurl");

                    KeyValuePair<string, KeyValuePair<SqlDbType, object>>[] parameters = new KeyValuePair<string, KeyValuePair<SqlDbType, object>>[5];
                    parameters[0] = new KeyValuePair<string, KeyValuePair<SqlDbType, object>>(
                        "open_id", new KeyValuePair<SqlDbType, object>(SqlDbType.VarChar, (object)openId));
                    parameters[1] = new KeyValuePair<string, KeyValuePair<SqlDbType, object>>(
                        "nick", new KeyValuePair<SqlDbType, object>(SqlDbType.VarChar, (object)nick.Trim()));
                    parameters[2] = new KeyValuePair<string, KeyValuePair<SqlDbType, object>>(
                        "head_image", new KeyValuePair<SqlDbType, object>(SqlDbType.VarChar, (object)headImageUrl.Trim()));
                    parameters[3] = new KeyValuePair<string, KeyValuePair<SqlDbType, object>>(
                        "vip_level", new KeyValuePair<SqlDbType, object>(SqlDbType.VarChar, (object)0));
                    parameters[4] = new KeyValuePair<string, KeyValuePair<SqlDbType, object>>(
                        "is_admin", new KeyValuePair<SqlDbType, object>(SqlDbType.VarChar, (object)0));

                    int i = DBHelper.InsertData(tableName, parameters);

                    if (i == 0)
                        throw new Exception("not inserted");
                    else
                    {
                        dt.Dispose();
                        dt = DBHelper.GetDataTable(" select * from users where open_id = '" + openId.Trim() + "' ");
                        _fields = dt.Rows[0];
                    }
                }
                catch
                {

                }

            }
        }
        else
        {
            _fields = dt.Rows[0];
        }

    }

    public string OpenId
    {
        get
        {
            return _fields["open_id"].ToString().Trim();
        }
    }

    public int Points
    {
        get
        {
            DataTable dt = DBHelper.GetDataTable(" select sum(points) from user_point_balance where user_open_id = '" + OpenId.Trim() + "'  ");
            int points = 0;
            if (dt.Rows.Count > 0)
            {
                if (Util.IsNumeric(dt.Rows[0][0].ToString().Trim()))
                    points = int.Parse(dt.Rows[0][0].ToString().Trim());
            }
            dt.Dispose();
            return points;
        }
    }

    public string Memo
    {
        get
        {
            return _fields["memo"].ToString().Trim();
        }
        set
        {
            KeyValuePair<string, KeyValuePair<SqlDbType, object>> memo
                = new KeyValuePair<string, KeyValuePair<SqlDbType, object>>("memo",
                    new KeyValuePair<SqlDbType, object>(SqlDbType.VarChar, (object)value));

            KeyValuePair<string, KeyValuePair<SqlDbType, object>>[] updateDataArr
                = new KeyValuePair<string, KeyValuePair<SqlDbType, object>>[] { memo };

            KeyValuePair<string, KeyValuePair<SqlDbType, object>>[] keyDataArr
                = new KeyValuePair<string, KeyValuePair<SqlDbType, object>>[] {
                    new KeyValuePair<string , KeyValuePair<SqlDbType, object>>( "open_id",
                        new KeyValuePair<SqlDbType,object>(SqlDbType.VarChar, _fields["open_id"]))};
            int i = DBHelper.UpdateData(tableName.Trim(), updateDataArr, keyDataArr);
            if (i == 0)
                throw new Exception("update failed");
        }
    }

    public int VipLevel
    {
        get
        {
            return int.Parse(_fields["vip_level"].ToString().Trim());
        }
        set
        {
            KeyValuePair<string, KeyValuePair<SqlDbType, object>> vipLevel
                = new KeyValuePair<string, KeyValuePair<SqlDbType, object>>("vip_level",
                    new KeyValuePair<SqlDbType, object>(SqlDbType.Int, (object)value));
            KeyValuePair<string, KeyValuePair<SqlDbType, object>>[] updateDataArr
                = new KeyValuePair<string, KeyValuePair<SqlDbType, object>>[] { vipLevel };
            KeyValuePair<string, KeyValuePair<SqlDbType, object>>[] keyDataArr
                = new KeyValuePair<string, KeyValuePair<SqlDbType, object>>[] {
                    new KeyValuePair<string , KeyValuePair<SqlDbType, object>>( "open_id",
                        new KeyValuePair<SqlDbType,object>(SqlDbType.VarChar, _fields["open_id"]))};
            int i = DBHelper.UpdateData(tableName.Trim(), updateDataArr, keyDataArr);
            if (i == 0)
                throw new Exception("update failed");
        }
    }

    public string StaffResort
    {
        get
        {
            if (_fields["is_resort_staff"].ToString().Equals("1"))
            {
                return "万龙";
            }
            else
            {
                return "";
            }
        }
    }

    public bool IsAdmin
    {
        get
        {
            if (_fields["is_admin"].ToString().Trim().Equals("1"))
                return true;
            else
                return false;
            //return bool.Parse(_fields["is_admin"].ToString().Trim());
        }
    }

    public bool IsBetaUser
    {
        get
        {
            bool ret = false;
            DataTable dt = DBHelper.GetDataTable(" select * from beta_user where cell_number =  '" + CellNumber.Trim() + "' ");
            if (dt.Rows.Count > 0)
                ret = true;
            dt.Dispose();
            return ret;
        }
    }

    public string HeadImage
    {
        get
        {
            return _fields["head_image"].ToString().Trim();
        }
    }

    public string Nick
    {
        get
        {
            if (_fields == null)
            {
                return "";
            }
            return _fields["nick"].ToString().Trim();
        }
    }

    public string CellNumber
    {
        get
        {
            if (_fields == null)
            {
                return "";
            }
            if (_fields["cell_number"] == null)
            {
                return "";
            }
            else
            {
                return _fields["cell_number"].ToString().Trim();
            }
        }
        set
        {
            _fields["cell_number"] = value.Trim();
            if (WeixinUser.CheckCellNumberHasNotBeenBinded(value.Trim()))
            {
                string[,] updateParameter = { { "cell_number", "varchar", value.Trim() } };
                string[,] keyParameter = { { "open_id", "varchar", _fields["open_id"].ToString().Trim() } };
                DBHelper.UpdateData("users", DBHelper.ConvertStringArryToKeyValuePairArray(updateParameter),
                    DBHelper.ConvertStringArryToKeyValuePairArray(keyParameter));
            }
        }
    }

    public int QrCodeSceneId
    {
        get
        {
            int currentSceneId = int.Parse(_fields["qr_code_scene"].ToString().Trim());
            if (currentSceneId == 0)
            {
                currentSceneId = QrCode.CreateScene();
                string[,] updateParameter = new string[,] { { "qr_code_scene", "int", currentSceneId.ToString() } };
                string[,] keyParameter = new string[,] { { "open_id", "varchar", OpenId.Trim() } };
                DBHelper.UpdateData("users", updateParameter, keyParameter, Util.conStr.Trim());
                return currentSceneId;

            }
            else
            {
                return currentSceneId;
            }

        }
    }

    public string FatherOpenId
    {
        get
        {
            return _fields["father_open_id"].ToString().Trim();
        }
        set
        {
            string[,] updateParameter = { { "father_open_id", "varchar", value.Trim() } };
            string[,] keyParameter = { { "open_id", "varchar", _fields["open_id"].ToString().Trim() } };
            DBHelper.UpdateData("users", DBHelper.ConvertStringArryToKeyValuePairArray(updateParameter),
                DBHelper.ConvertStringArryToKeyValuePairArray(keyParameter));
        }
    }

    public string LastScanedOpenId
    {
        get
        {
            string openId = "";
            int sceneId = 0;
            DataTable dt = DBHelper.GetDataTable(" select top 1 * from wxreceivemsg where wxreceivemsg_from = '" + OpenId.Trim().Replace("'", "").Trim()
                + "' and wxreceivemsg_event in ('SCAN', 'subscribe') and wxreceivemsg_eventkey <> '' order by wxreceivemsg_crt desc  ");
            if (dt.Rows.Count > 0)
            {
                string temp = dt.Rows[0]["wxreceivemsg_eventkey"].ToString().Trim();
                if (temp.StartsWith("qrscene_"))
                    temp = temp.Replace("qrscene_", "");
                try
                {
                    sceneId = int.Parse(temp);
                    if (sceneId != 0)
                    {
                        DataTable dtUser = DBHelper.GetDataTable(" select * from users where qr_code_scene = " + sceneId.ToString());
                        if (dtUser.Rows.Count > 0)
                        {
                            openId = dtUser.Rows[0]["open_id"].ToString().Trim();
                        }
                        dtUser.Dispose();
                    }
                }
                catch
                {


                }
            }
            dt.Dispose();
            return openId;
        }
    }

    public string TempOpenId
    {
        get
        {
            string ret = "";
            DataTable dt = DBHelper.GetDataTable(" select * from users where cell_number = '" + CellNumber.Trim() + "' and ISNUMERIC(open_id) = 1 and vip_level = 0  ");
            if (dt.Rows.Count > 0)
            {
                ret = dt.Rows[0]["open_id"].ToString().Trim();
            }
            dt.Dispose();
            return ret;
        }
    }

    public void TransferOrderAndPointsFromTempAccount()
    {
        //string tempOpenId = TempOpenId.Trim();

        DataTable dtOriPoints = DBHelper.GetDataTable(" select * from point_prepare_imported where deal = 0 and cell_number = '" + CellNumber.Trim() + "' ");
        foreach (DataRow drOriPoints in dtOriPoints.Rows)
        {
            try
            {
                int i = DBHelper.InsertData("user_point_balance",
                    new string[,] { { "user_open_id", "varchar", OpenId.Trim() }, {"points", "int", drOriPoints["score"].ToString() },
                    {"memo", "varchar", drOriPoints["source"].ToString() }, {"transact_date", "datetime", DateTime.Now.ToShortDateString() } },
                    Util.conStr);
                if (i == 1)
                {
                    DBHelper.UpdateData("point_prepare_imported", new string[,] { { "deal", "int", "1" } },
                        new string[,] { { "id", "int", drOriPoints["id"].ToString() } }, Util.conStr.Trim());
                }
            }
            catch
            {

            }
        }

    }

    public static WeixinUser[] GetAllUsers()
    {
        DataTable dt = DBHelper.GetDataTable(" select * from users order by crt desc ");
        WeixinUser[] usersArr = new WeixinUser[dt.Rows.Count];
        for (int i = 0; i < usersArr.Length; i++)
        {
            usersArr[i] = new WeixinUser();
            usersArr[i]._fields = dt.Rows[i];
        }
        return usersArr;
    }

    public static string CheckToken(string token)
    {
        DataTable dt = DBHelper.GetDataTable(" select * from m_token where expire > dbo.GetLocalDate(DEFAULT) and isvalid = 1 and token = '" + token.Trim().Replace("'", "").Trim() + "'  ");
        string ret = "";
        if (dt.Rows.Count > 0)
            ret = dt.Rows[0]["open_id"].ToString().Trim();
        dt.Dispose();
        return ret;
    }

    public static string CreateToken(string openId, DateTime expireDate)
    {
        string stringWillBeToken = openId.Trim() + Util.GetLongTimeStamp(DateTime.Now)
            + Util.GetLongTimeStamp(expireDate)
            + (new Random()).Next(10000).ToString().PadLeft(4, '0');
        string token = Util.GetMd5(stringWillBeToken) + Util.GetSHA1(stringWillBeToken);

        SqlConnection conn = new SqlConnection(Util.conStr);
        SqlCommand cmd = new SqlCommand(" update m_token set isvalid = 0 where open_id = '" + openId.Trim() + "'  ", conn);
        conn.Open();
        cmd.ExecuteNonQuery();
        cmd.CommandText = " insert m_token (token,isvalid,expire,open_id) values  ('" + token.Trim() + "' "
            + " , 1 , '" + expireDate.ToString() + "' , '" + openId.Trim() + "' ) ";
        cmd.ExecuteNonQuery();
        conn.Close();
        cmd.Dispose();
        conn.Dispose();
        return token;
    }
    /*
    public static string GetOpenIdByToken(string token)
    { 
       DataTable dt = DBHelper.GetDataTable(" select * from tokens where token = '" 
        + token.Replace("'","").Trim() + "'  and expire_date <= dbo.GetLocalDate(DEFAULT) and valid = 1 order by crt desc ", 
        new KeyValuePair<string, KeyValuePair<SqlDbType, object>>[0]);

        //bool ret = false;
       string openId = "";

        foreach (DataRow dr in dt.Rows)
        {
            if (dr["valid"].ToString().Equals("1")
                && DateTime.Parse(dr["expire_date"].ToString()) < DateTime.Now)
            {
                //ret = true;
                openId = dr["weixin_open_id"].ToString().Trim();
                break;
            }
        }
        return openId.Trim();
        //return true;
    }
    */
    public static string[] GetOpenIdByCellNumber(string number)
    {
        DataTable dt = DBHelper.GetDataTable(" select open_id from users where cell_number = '" + number.Trim().Replace("'", "").Trim() + "'  ");
        string[] cellNumber = new string[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            cellNumber[i] = dt.Rows[i]["open_id"].ToString().Trim();
        }
        dt.Dispose();
        return cellNumber;
    }

    public static bool CheckCellNumberHasNotBeenBinded(string number)
    {
        DataTable dt = DBHelper.GetDataTable("select * from users where cell_number = '" + number.Trim().Replace("'", "").Trim() + "' and vip_level > 0  ");
        bool ret = true;
        if (dt.Rows.Count > 0)
            ret = false;
        dt.Dispose();
        return ret;
    }

    public static string GetVipUserOpenIdByNumber(string number)
    {
        DataTable dt = DBHelper.GetDataTable(" select * from users where cell_number = '" + number.Replace("'", "") + "' and vip_level > 0");
        string openId = "";
        if (dt.Rows.Count > 0)
        {
            openId = dt.Rows[0]["open_id"].ToString().Trim();
        }
        dt.Dispose();
        return openId.Trim();
    }

    public static string[] GetTagList()
    {
        DataTable dt = DBHelper.GetDataTable(" select distinct tag from users_tag order by tag ");
        string[] tagArr = new string[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            tagArr[i] = dt.Rows[i][0].ToString().Trim();
        }
        dt.Dispose();
        return tagArr;
    }

    public static void SaveUserTag(string openId, string tag, string tagValue)
    {
        if (!tag.Trim().Equals("") && !tagValue.Trim().Equals(""))
        {
            DataTable dt = DBHelper.GetDataTable(" select * from users_tag where open_id = '" + openId.Trim().Replace("'", "").Trim() + "' and tag = '" + tag.Replace("'", "").Trim() + "' ");
            if (dt.Rows.Count == 0)
            {
                DBHelper.InsertData("users_tag", new string[,] { { "tag", "varchar", tag.Trim() }, { "tag_value", "varchar", tagValue.Trim() }, { "open_id", "varchar", openId.Trim() } });
            }
            else
            {
                DBHelper.UpdateData("users_tag", new string[,] { { "tag_value", "varchar", tagValue.Trim() } },
                    new string[,] { { "open_id", "varchar", openId.Trim() }, { "tag", "varchar", tag.Trim() } }, Util.conStr.Trim());
            }
        }
    }

    public static string GetUserTagValue(string openId, string tag)
    {
        DataTable dt = DBHelper.GetDataTable(" select * from users_tag where open_id = '" + openId.Trim() + "' and  tag = '" + tag.Replace("'", "").Trim() + "' order by tag ");
        string ret = "";
        if (dt.Rows.Count == 1)
        {
            ret = dt.Rows[0]["tag_value"].ToString().Trim();
        }
        dt.Dispose();
        return ret;
    }

    public static DataTable GetUserTagTable(string openId)
    {
        return DBHelper.GetDataTable(" select tag, tag_value from users_tag where open_id = '" + openId.Trim().Replace("'", "") + "' ");
    }

    public static void DeleteUserTag(string openId, string tag)
    {
        DBHelper.DeleteData("users_tag", new string[,] { { "open_id", "varchar", openId.Trim() }, { "tag", "varchar", tag.Trim() } }, Util.conStr.Trim());
    }

    public static WeixinUser GetTempWeixinUser(string cell)
    {
        WeixinUser user;
        DataTable dtUser = DBHelper.GetDataTable(" select * from users where cell_number = '" + cell.Trim() + "' and ISNUMERIC(open_id) = 1 ");
        if (dtUser.Rows.Count == 0)
        {
            string tempTimeStampOpenId = Util.GetTimeStamp();
            DBHelper.InsertData("users", new string[,]{ {"open_id", "varchar", tempTimeStampOpenId }, { "nick", "varchar", "" },
            {"cell_number", "varchar", cell.Trim() },{"vip_level", "int", "0" },{"head_image", "varchar", "" }});
            user = new WeixinUser(tempTimeStampOpenId);
        }
        else
        {
            user = new WeixinUser(dtUser.Rows[0]["open_id"].ToString().Trim());
        }
        dtUser.Dispose();
        return user;
    }

    public static void MergeUser(string cellNumber)
    {
        string[] openIdArr = GetOpenIdByCellNumber(cellNumber);
        string realOpenId = "";
        string numericOpenIdArr = "";
        foreach (string openId in openIdArr)
        {
            try
            {
                long s = long.Parse(openId);
                numericOpenIdArr = numericOpenIdArr + (numericOpenIdArr.Trim().Equals("") ? "" : ", ")
                    + " '" + s.ToString() + "' ";
            }
            catch
            {
                realOpenId = openId;
            }
        }
        if (!realOpenId.Trim().Equals("") && !numericOpenIdArr.Trim().Equals(""))
        {
            string sql = "update order_online set open_id = '" + realOpenId.Trim() + "' where open_id in (" + numericOpenIdArr + ")";
            SqlConnection conn = new SqlConnection(Util.conStr.Trim());
            SqlCommand cmd = new SqlCommand(sql, conn);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            sql = "update user_point_balance set user_open_id = '" + realOpenId.Trim() + "' where user_open_id in (" + numericOpenIdArr + ")";
            cmd.CommandText = sql;
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            cmd.Dispose();
            conn.Dispose();

        }
    }

}