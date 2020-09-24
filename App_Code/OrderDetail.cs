using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for OrderDetail
/// </summary>
public class OrderDetail
{

    public double unitPrice = 0;
    public double count = 0;
    public double salePrice = 0;
    public double saleSummary = 0;
    public int usedDragonBallCount = 0;
    public double usedTicketAmount = 0;
    public string cellNumber = "";
    public string memberName = "";
    public string flowNumber = "";
    public string goodName = "";
    public DateTime orderDate = DateTime.MinValue;
    public string orderType = "";
    public DataRow _fields;

    public OrderDetail()
    {
        //
        // TODO: Add constructor logic here
        //
    }


    public string OrderType
    {
        get
        {
            if (_fields == null)
                return orderType.Trim();
            else
                return _fields["type"].ToString().Trim();
        }
    }

    public bool IsValid
    {
        get
        {
            if (goodName.Trim().IndexOf("押金") < 0 && !goodName.Trim().Equals(""))
                return true;
            else
            {
                return false;
            }

        }
    }

    /*
    public bool CanImport
    {
        get
        {
            bool can = false;
            if (orderType.Trim().Equals("现货补收"))
            {
                Order order = new Order(flowNumber);
                if (order.RealPaidAmount < order.OrderShouldPaidAmount)
                    can = true;
                if (can)
                {
                    foreach (OrderDetail dtl in order.orderDetails)
                    {
                        if (DateTime.Parse(dtl._fields["order_date"].ToString()) == orderDate)
                        {
                            can = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                can = !SalesFlowSheet.HasImported(flowNumber.Trim());
            }
            if (orderType.Trim().IndexOf("押金") >= 0)
            {
                can = false;
            }
            return can && IsValid;
        }
    }
    */
    public int Save()
    {
        string[,] insertParams = { { "flow_number", "varchar", flowNumber.Trim()},
            {"type", "varchar", orderType.Trim() },
            {"good_name", "varchar", goodName.Trim()  },
            {"count", "int", count.ToString() },
            {"unit_price", "float", unitPrice.ToString() },
            {"deal_price", "float", saleSummary.ToString() },
            {"order_date", "datetime", orderDate.ToShortDateString() } };
        int i = DBHelper.InsertData("order_details", insertParams);
        return i;
    }



}