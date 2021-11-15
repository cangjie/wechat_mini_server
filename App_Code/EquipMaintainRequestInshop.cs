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
            string openId = _fields["open_id"].ToString().Trim();
            if (openId.Length < 5)
            {
                openId = "";
            }
            return openId;
        }
    }

    public int ProductId
    {
        get
        {
            return int.Parse(_fields["confirmed_product_id"].ToString());
        }
    }

    public double AddtionalFee
    {
        get
        {
            return double.Parse(_fields["confirmed_additional_fee"].ToString());
        }
    }

    public int AddtionalFeeProductId
    {
        get
        {
            return 146;
        }
    }

    public string ServiceOpenId
    {
        get
        {
            return _fields["service_open_id"].ToString().Trim();
        }
    }

    

    public int Confirm(string type, string brand, string serial, string scale, string year, string cell, string name, string gender,
        bool edge, int degree, bool candle, string more, double additionalFee, string memo, DateTime pickDate, int productId, string serviceOpenId, bool urgent)
    {
        return DBHelper.UpdateData("maintain_in_shop_request", new string[,] {
            {"confirmed_equip_type", "varchar", type.Trim() },
            {"confirmed_brand", "varchar", brand.Trim() },
            {"confirmed_serial", "varchar", serial.Trim() },
            {"confirmed_scale", "varchar", scale.Trim() },
            {"confirmed_year", "varchar", year.Trim() },
            {"confirmed_edge", "int", edge?"1":"0" },
            {"confirmed_degree", "int", degree.ToString() },
            {"confirmed_candle", "int", candle?"1":"0" },
            {"confirmed_more", "varchar", more.Trim() },
            {"confirmed_memo", "varchar", memo.Trim() },
            {"confirmed_pick_date", "datetime", pickDate.ToShortDateString() },
            {"confirmed_urgent", "int", (urgent?"1":"0") },
            {"confirmed_additional_fee", "float", additionalFee.ToString()},
            {"confirmed_cell", "varchar", cell.Trim() },
            {"confirmed_name", "varchar", name.Trim() },
            {"confirmed_gender", "varchar", gender.Trim() },
            {"service_open_id", "varchar", serviceOpenId.Trim() },
            {"confirmed_product_id", "int", productId.ToString() },
        }, new string[,] { {"id", "int", _fields["id"].ToString() } }, Util.conStr.Trim());
    }

    public static int PlaceOrder(int id)
    {
        EquipMaintainRequestInshop request = new EquipMaintainRequestInshop(id);
        if (request.ProductId == 0 && request.AddtionalFee == 0)
        {
            return 0;
        }
        OnlineOrder newOrder = new OnlineOrder();
        
        
        if (request.ProductId != 0)
        {
            OnlineOrderDetail detail = new OnlineOrderDetail();
            Product p = new Product(request.ProductId);
            detail.productId = int.Parse(p._fields["id"].ToString());
            detail.productName = p._fields["name"].ToString();
            detail.price = double.Parse(p._fields["sale_price"].ToString());
            detail.count = 1;
            newOrder.Type = p.Type.Trim();
            newOrder.shop = request._fields["shop"].ToString().Trim();
            newOrder.AddADetail(detail);
        }
        if (request.AddtionalFee != 0)
        {
            OnlineOrderDetail detail = new OnlineOrderDetail();
            Product p = new Product(request.AddtionalFeeProductId);
            detail.productId = int.Parse(p._fields["id"].ToString());
            detail.productName = p._fields["name"].ToString();
            detail.price = double.Parse(p._fields["sale_price"].ToString());
            detail.count = (int)(request.AddtionalFee/p.SalePrice);
            newOrder.Type = p.Type.Trim();
            newOrder.shop = request._fields["shop"].ToString().Trim();
            newOrder.AddADetail(detail);
        }
        int orderId = newOrder.Place(request.OwnerOpenId.Trim());
        DBHelper.UpdateData("maintain_in_shop_request", new string[,] { {"order_id", "int", orderId.ToString() } },
            new string[,] { {"id","int", id.ToString() } }, Util.conStr);
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