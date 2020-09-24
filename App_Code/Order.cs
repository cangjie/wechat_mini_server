using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for Order
/// </summary>
public class Order
{
    public OrderDetail[] orderDetails;
    public int startIndex = 0;
    public string flowNumber = "";
    private double orderPrice = 0;
    private double orderShouldPaidAmount = 0;
    private double usedDragonBallCount = 0;
    private double usedTicketAmount = 0;
    private double realPaidAmount = 0;
    private double disCountRate = 0;
    private double dragonBallRate = 0;
    private int generateDragonBallCount = 0;
    //public string openId = "";
    //public string cellNumber = "";
    public DataRow _fields;

    public Order()
    {

    }

    public Order(string flowNumber)
    {
        DataTable dt = DBHelper.GetDataTable(" select * from orders where flow_number = '" + flowNumber.Trim().Replace("'", "") + "' ");
        if (dt.Rows.Count == 1)
        {
            _fields = dt.Rows[0];
        }

        DataTable dtDetails = DBHelper.GetDataTable(" select * from order_details where flow_number = '" + flowNumber.Trim().Replace("'", "") + "' order by detail_id ");
        orderDetails = new OrderDetail[dtDetails.Rows.Count];
        for (int i = 0; i < orderDetails.Length; i++)
        {
            orderDetails[i] = new OrderDetail();
            orderDetails[i]._fields = dtDetails.Rows[i];
        }

    }

    public int Save()
    {
        int i = 0;
        if (!HaveImported)
        {
            int payStatus = 1;
            if (OrderType.Trim().Equals("现货未付"))
            {
                payStatus = 0;
            }
            string[,] insertParam = { {"flow_number", "varchar", flowNumber.Trim() },
                {"type", "varchar", OrderType.Trim() },
                {"open_id", "varchar", "" },
                {"member_name", "varchar", MemberName.Trim() },
                {"cell_number", "varchar", CellNumber.Trim() },
                {"price", "float", OrderPrice.ToString() },
                {"dragon_ball_used", "int", UsedDragonBallCount.ToString() },
                {"ticket_used_amount", "float", UsedTicketAmount.ToString().Trim() },
                {"real_paid_summary", "float", RealPaidAmount.ToString() },
                {"dragon_ball_rate", "float", DragonBallRate.ToString().Trim() },
                {"dragon_ball_generated", "int", GenerateDraonBallCount.ToString() },
                {"pay_status", "int", payStatus.ToString() },
                {"order_date", "datetime", Date.ToShortDateString() } };
            i = DBHelper.InsertData("orders", insertParam);
            if (i > 0)
            {
                foreach (OrderDetail dtl in orderDetails)
                {
                    dtl.Save();
                }
            }
        }
        return i;

    }

    public DateTime Date
    {
        get
        {
            DateTime orderDate = DateTime.MinValue;
            foreach (OrderDetail dtl in orderDetails)
            {
                if (dtl.orderDate != DateTime.MinValue)
                    orderDate = dtl.orderDate;
                break;
            }
            return orderDate;
        }
    }

    public bool HaveImported
    {
        get
        {
            bool imported = false;
            DataTable dt = DBHelper.GetDataTable(" select * from orders where flow_number = '" + flowNumber.Trim() + "' ");
            if (dt.Rows.Count > 0)
                imported = true;
            dt.Dispose();
            return imported;
        }
    }

    public string CellNumber
    {
        get
        {
            string cellNumber = "";
            foreach (OrderDetail dtl in orderDetails)
            {
                if (!dtl.cellNumber.Trim().Equals("") && dtl.IsValid)
                {
                    cellNumber = dtl.cellNumber;
                    break;
                }
            }
            return cellNumber;
        }
    }

    public string MemberName
    {
        get
        {
            string memberName = "";
            foreach (OrderDetail dtl in orderDetails)
            {
                if (dtl.IsValid && !dtl.memberName.Trim().Equals(""))
                {
                    memberName = dtl.memberName.Trim();
                    break;
                }
            }
            return memberName.Trim();
        }
    }

    public double OrderPrice
    {
        get
        {
            orderPrice = 0;
            foreach (OrderDetail detail in orderDetails)
            {
                orderPrice = orderPrice + detail.unitPrice * detail.count;
            }
            return orderPrice;
        }
    }

    public double OrderShouldPaidAmount
    {
        get
        {
            orderShouldPaidAmount = 0;
            foreach (OrderDetail detail in orderDetails)
            {
                if (!detail.OrderType.Trim().ToString().Trim().Equals("现货补收"))
                    orderShouldPaidAmount = orderShouldPaidAmount + detail.saleSummary;
            }
            return orderShouldPaidAmount;
        }
    }

    public double UsedDragonBallCount
    {
        get
        {
            usedDragonBallCount = 0;
            foreach (OrderDetail detail in orderDetails)
            {
                usedDragonBallCount = usedDragonBallCount + detail.usedDragonBallCount;
            }
            return usedDragonBallCount;
        }
    }

    public double UsedTicketAmount
    {
        get
        {
            usedTicketAmount = 0;
            foreach (OrderDetail detail in orderDetails)
            {
                usedTicketAmount = usedTicketAmount + detail.usedTicketAmount;
            }
            return usedTicketAmount;
        }
    }

    public double RealPaidAmount
    {
        get
        {
            if (OrderType.Trim().Equals("现货未付"))
            {
                double paidAmount = 0;
                foreach (OrderDetail dtl in orderDetails)
                {
                    if (dtl.orderType.Equals("现货补收"))
                    {
                        paidAmount = paidAmount + double.Parse(dtl._fields["deal_price"].ToString().Trim());
                    }
                }
                return paidAmount + UsedTicketAmount + UsedDragonBallCount / 10;
            }
            else
            {
                return OrderShouldPaidAmount - UsedTicketAmount - UsedDragonBallCount / 10;
            }

        }
    }

    public double DisCountRate
    {
        get
        {
            return Math.Round(OrderShouldPaidAmount / OrderPrice, 4);
        }
    }

    public double DragonBallRate
    {
        get
        {
            double rate = 0;
            double disCountRate = DisCountRate;
            if (disCountRate == 1)
                rate = 1;
            else if (disCountRate >= 0.95)
                rate = 0.925;
            else if (disCountRate >= 0.9)
                rate = 0.85;
            else if (disCountRate >= 0.85)
                rate = 0.775;
            else if (disCountRate >= 0.8)
                rate = 0.7;
            else if (disCountRate >= 0.75)
                rate = 0.625;
            else if (disCountRate >= 0.7)
                rate = 0.55;
            else if (disCountRate >= 0.65)
                rate = 0.475;
            else if (disCountRate >= 0.6)
                rate = 0.4;
            else if (disCountRate >= 0.55)
                rate = 0.325;
            else if (disCountRate >= 0.5)
                rate = 0.25;
            else if (disCountRate >= 0.45)
                rate = 0.175;
            else if (disCountRate >= 0.4)
                rate = 0.1;
            else
                rate = 0;
            return rate;
        }
    }



    public int GenerateDraonBallCount
    {
        get
        {
            return (int)(RealPaidAmount * DragonBallRate);
        }
    }

    public string OrderType
    {
        get
        {
            string type = "";
            foreach (OrderDetail dtl in orderDetails)
            {
                if (type.Equals(""))
                    type = dtl.orderType.Trim();
                break;
            }
            return type;
        }
    }

    public void AddItem(OrderDetail orderDetail)
    {
        orderDetail.flowNumber = flowNumber.Trim();
        OrderDetail[] newOrderDetails;// = new OrderDetail[orderDetails.Length + 1];
        if (orderDetails == null)
            newOrderDetails = new OrderDetail[1];
        else
            newOrderDetails = new OrderDetail[orderDetails.Length + 1];
        for (int i = 0; i < newOrderDetails.Length; i++)
        {
            if (i < newOrderDetails.Length - 1)
            {
                newOrderDetails[i] = orderDetails[i];
            }
            else
            {
                newOrderDetails[i] = orderDetail;
            }
        }
        orderDetails = newOrderDetails;
    }

    public static int SetPayStatus(string flowNumber)
    {
        int i = 0;
        Order order = new Order(flowNumber);
        i = int.Parse(order._fields["pay_status"].ToString());
        if (order._fields["type"].ToString().Trim().Equals("现货未付"))
        {
            i = 0;
        }
        if (order.RealPaidAmount > 0)
        {
            i = 2;
        }
        else
        {
            if (order.RealPaidAmount >= order.OrderShouldPaidAmount)
            {
                i = 3;
            }
        }

        string[,] updateParam = { { "pay_status", "int", i.ToString() } };
        string[,] keyParam = { { "flow_number", "varchar", flowNumber.Trim() } };
        int r = DBHelper.UpdateData("orders", updateParam, keyParam, Util.conStr);
        if (r == 1)
            return i;
        else
            return -1;
    }

    public static int ImportUserOrderDragonBall(string cellNumber)
    {
        DataTable dtOrder = DBHelper.GetDataTable(" select * from orders where deal = 0 and cell_number = '" + cellNumber.Replace(",", "").Trim() + "' ");
        int i = 0;
        foreach (DataRow dr in dtOrder.Rows)
        {
            i += ImportOrderDragonBall(dr["flow_number"].ToString());
        }
        return i;
    }

    public static int ImportOrderDragonBall(string flowNumber)
    {
        int i = 0;
        Order order = new Order(flowNumber);
        if (order._fields["type"].ToString().Trim().Equals("现货未付"))
            SetPayStatus(flowNumber);
        string openId = WeixinUser.GetVipUserOpenIdByNumber(order._fields["cell_number"].ToString().Trim());

        if (!openId.Trim().Equals("")
            && (order._fields["pay_status"].ToString().Equals("1") || order._fields["pay_status"].ToString().Equals("3"))
            && order._fields["deal"].ToString().Equals("0"))
        {
            i = DragonBallBalance.Add(openId.Trim(), int.Parse(order._fields["dragon_ball_generated"].ToString().Trim()),
                order._fields["flow_number"].ToString(), DateTime.Parse(order._fields["order_date"].ToString()));
        }
        if (i > 0)
        {
            string[,] updateParam = { { "deal", "int", "1" } };
            string[,] keyParam = { { "flow_number", "varchar", flowNumber } };
            int r = DBHelper.UpdateData("orders", updateParam, keyParam, Util.conStr);
            if (r != 1)
            {
                string[,] keyParamDel = { { "id", "int", i.ToString() } };
                DBHelper.DeleteData("user_point_balance", keyParamDel, Util.conStr);
                i = 0;
            }
        }

        return i;
    }

}