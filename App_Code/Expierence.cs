using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
/// <summary>
/// Expierence 的摘要说明
/// </summary>
public class Expierence
{
    public DataRow _fields;

    public Expierence(int id)
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
        DataTable dt = DBHelper.GetDataTable(" select * from expierence_list where [id] = " + id.ToString());
        if (dt.Rows.Count > 0)
        {
            _fields = dt.Rows[0];
        }
        else
        {
            throw new Exception("Not found.");
        }
    }

    public int PlaceOrder(string customerOpenId)
    {
        int productId = 147;
        int orderId = 0;
        double amount = double.Parse(_fields["guarantee_cash"].ToString().Trim());
        OnlineOrderDetail detail = new OnlineOrderDetail();
        Product p = new Product(productId);
        detail.productId = int.Parse(p._fields["id"].ToString());
        detail.productName = p._fields["name"].ToString();
        detail.price = double.Parse(p._fields["sale_price"].ToString());
        detail.count = (int)(amount / detail.price);

        OnlineOrder newOrder = new OnlineOrder();
        newOrder.Type = "押金";
        newOrder.shop = _fields["shop"].ToString().Trim();
        newOrder.AddADetail(detail);
        try
        {
            orderId = newOrder.Place(customerOpenId);
        }
        catch
        {

        }
        DBHelper.UpdateData("expierence_list", new string[,] { { "guarantee_order_id", "int", orderId.ToString() } },
            new string[,] { { "id", "int", _fields["id"].ToString() } }, Util.conStr);
        return orderId;
    }

    public static int CreateNew(string staffOpenId, string shop)
    {
        DBHelper.InsertData("expierence_list", new string[,] { { "staff_open_id", "varchar", staffOpenId.Trim() },
            {"shop", "varchar", shop.Trim() } });
        DataTable dt = DBHelper.GetDataTable(" select top 1 * from expierence_list where staff_open_id = '" + staffOpenId.Trim() + "' order by [id] desc ");
        int retId = 0;
        if (dt.Rows.Count > 0)
        {
            retId = int.Parse(dt.Rows[0]["id"].ToString());
        }
        dt.Dispose();
        return retId;
    }
}