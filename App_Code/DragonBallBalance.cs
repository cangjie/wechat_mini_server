using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for DragonBallBalance
/// </summary>
public class DragonBallBalance
{
    public DragonBallBalance()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static int Add(string openId, int ballCount, string memo, DateTime transactDate)
    {
        string[,] insertParam = { {"user_open_id", "varchar", openId.Trim() },
            {"points", "int", ballCount.ToString() },
            {"memo", "varchar", memo.Trim() },
            {"transact_date", "datetime", transactDate.ToShortDateString() } };
        int i = DBHelper.InsertData("user_point_balance", insertParam);
        if (i == 1)
        {
            DataTable dt = DBHelper.GetDataTable(" select max([id]) from user_point_balance ");
            i = int.Parse(dt.Rows[0][0].ToString());
            dt.Dispose();
        }
        return i;
    }
}