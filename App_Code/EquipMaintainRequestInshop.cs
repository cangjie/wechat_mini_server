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
    public DataRow _fields;

    public EquipMaintainRequestInshop(int id)
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
        DataTable dt = DBHelper.GetDataTable(" select * from maintain_in_shop_request where [id] = " + id.ToString());
        if (dt.Rows.Count == 0)
        {
            throw new Exception("Not found.");
        }
        else
        {
            _fields = dt.Rows[0];
        }
    }

    public string OwnerOpenId
    {
        get
        {
            return _fields["open_id"].ToString().Trim();
        }
    }

    public int PlaceOrder(string operOpenId, int productId)
    {
        OnlineOrderDetail detail = new OnlineOrderDetail();
        Product p = new Product(productId);
        detail.productId = int.Parse(p._fields["id"].ToString());
        detail.productName = p._fields["name"].ToString();
        detail.price = double.Parse(p._fields["sale_price"].ToString());
        detail.count = 1;

        OnlineOrder newOrder = new OnlineOrder();
        newOrder.AddADetail(detail);
        newOrder.Type = p._fields["type"].ToString();
        newOrder.shop = p._fields["shop"].ToString();
        int orderId = newOrder.Place(_fields["open_id"].ToString().Trim());

        DBHelper.UpdateData("maintain_in_shop_request", new string[,] { { "service_open_id", "varchar", operOpenId.Trim() }, {"order_id", "int", orderId.ToString() } },
            new string[,] { {"id", "int", _fields["id"].ToString() } }, Util.conStr);

        return orderId;
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