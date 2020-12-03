using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
/// <summary>
/// EquipMaintainRequest 的摘要说明
/// </summary>
public class EquipMaintainRequestInshop
{
    public EquipMaintainRequestInshop(int id)
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static int CreateNew(string openId, string shop, string equipType, string brand, string scale, bool edge, bool candle, bool repair, DateTime pickDate)
    {
        int i = DBHelper.InsertData("maintain_in_shop_request", new string[,] {
            {"shop", "varchar", shop.Trim() }, {"open_id", "varchar", openId.Trim() }, {"equip_type", "varchar", equipType.Trim() },
            {"brand", "varchar", brand.Trim() }, {"scale", "varchar", scale.Trim() }, {"edge", "int", (edge? 1: 0).ToString()},
            {"candle", "varchar", (candle? 1: 0).ToString() }, {"repair_more", "int", (repair? 1: 0).ToString()}, {"pick_date", "datetime", pickDate.ToString()}
        });
        int id = 0;
        if (i == 1)
        {
            DataTable dt = DBHelper.GetDataTable(" select max([id]) from maintain_in_shop_request ");
            if (dt.Rows.Count == 1)
            {
                id = int.Parse(dt.Rows[0][0].ToString());
            }
            dt.Dispose();
        }
        return id;
    }
}