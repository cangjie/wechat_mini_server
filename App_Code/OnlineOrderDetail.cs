using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for OnlineOrderDetail
/// </summary>
public class OnlineOrderDetail
{
    public int id = 0;
    public int productId = 0;
    public int orderId = 0;
    public string productName = "";
    public double price = 0;
    public int count = 0;


    public OnlineOrderDetail()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public void AddNew(int orderId)
    {
        if (id == 0)
        {
            string[,] insertParam = { {"order_online_id", "int", orderId.ToString() },
            { "product_id", "int", productId.ToString()},
            { "product_name", "varchar", productName.Trim() },
            { "price", "float", price.ToString()},
            { "count", "int", count.ToString()} };
            int i = DBHelper.InsertData("order_online_detail", insertParam);
            if (i == 1)
            {
                DataTable dt = DBHelper.GetDataTable("select max(id) from order_online_detail ");
                id = int.Parse(dt.Rows[0][0].ToString());
                dt.Dispose();
            }
        }
    }

    public double summary
    {
        get
        {
            return price * count;
        }
    }

    public static OnlineOrderDetail[] GetOnlineOrderDetails(int orderId)
    {
        DataTable dt = DBHelper.GetDataTable(" select * from order_online_detail where order_online_id = " + orderId.ToString());
        OnlineOrderDetail[] detailArr = new OnlineOrderDetail[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            detailArr[i] = new OnlineOrderDetail();
            detailArr[i].id = int.Parse(dt.Rows[i]["id"].ToString());
            detailArr[i].orderId = int.Parse(dt.Rows[i]["order_online_id"].ToString().Trim());
            detailArr[i].productId = int.Parse(dt.Rows[i]["product_id"].ToString().Trim());
            detailArr[i].productName = dt.Rows[i]["product_name"].ToString().Trim();
            detailArr[i].price = double.Parse(dt.Rows[i]["price"].ToString());
            detailArr[i].count = int.Parse(dt.Rows[i]["count"].ToString());
        }
        dt.Dispose();
        return detailArr;
    }
}