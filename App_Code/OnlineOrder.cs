using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for OnlineOrder
/// </summary>
public class OnlineOrder
{
    public DataRow _fields;

    public OnlineOrderDetail[] orderDetails = new OnlineOrderDetail[0];

    private string memo = "";

    private string type = "雪票";

    public string shop = "乔波";

    public OnlineOrder()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public OnlineOrder(int orderId)
    {
        DataTable dt = DBHelper.GetDataTable(" select * from order_online where [id] = " + orderId.ToString());
        _fields = dt.Rows[0];

    }

    public int Place(string openId)
    {
        if (_fields == null)
        {
            WeixinUser user = new WeixinUser(openId.Trim());
            if (user != null)
            {
                string[,] insertParam = { {"type", "varchar", Type.Trim() },
                    {"open_id", "varchar", openId.Trim() },
                    {"cell_number", "varchar", user.CellNumber.Trim() },
                    {"name", "varchar", user.Nick.Trim() },
                    {"pay_method", "varchar", PayMethod.Trim() },
                    {"order_price", "float", OrderPrice.ToString() },
                    {"order_real_pay_price", "float", OrderPrice.ToString() },
                    {"shop", "varchar", shop.Trim() },
                    {"memo", "varchar", memo.Trim() } };
                int i = DBHelper.InsertData("order_online", insertParam);

                if (i == 1)
                {
                    int totalScore = 0;
                    DataTable dt = DBHelper.GetDataTable(" select top 1 *  from order_online order by [id] desc");
                    int maxId = int.Parse(dt.Rows[0][0].ToString());
                    _fields = dt.Rows[0];

                    foreach (OnlineOrderDetail detail in orderDetails)
                    {
                        detail.AddNew(maxId);
                        if (detail.productId > 0)
                        {
                            Product p = new Product(detail.productId);
                            totalScore = totalScore + int.Parse(p._fields["award_score"].ToString().Trim());
                        }

                    }
                    DBHelper.UpdateData("order_online", new string[,] { { "generate_score", "int", totalScore.ToString() } },
                        new string[,] { { "id", "int", maxId.ToString() } }, Util.conStr.Trim());
                    return maxId;
                }
            }
        }
        return 0;
    }



    public void AddADetail(OnlineOrderDetail onlineOrderDetail)
    {
        if (_fields == null)
        {
            OnlineOrderDetail[] newOrderDetails = new OnlineOrderDetail[orderDetails.Length + 1];
            for (int i = 0; i < newOrderDetails.Length; i++)
            {
                if (i == newOrderDetails.Length - 1)
                {
                    newOrderDetails[i] = onlineOrderDetail;
                }
                else
                {
                    newOrderDetails[i] = orderDetails[i];
                }
            }
            orderDetails = newOrderDetails;
        }
    }

    public bool HaveFinishedShopSaleOrder()
    {
        OrderTemp tempOrder = OrderTemp.GetFinishedOrder(int.Parse(_fields["id"].ToString()));
        bool ret = true;
        if (tempOrder._fields["is_paid"].ToString().Equals("0"))
        {
            tempOrder.FinishOrder();
            ret = false;
        }
        return ret;
    }

    public OnlineOrderDetail[] OrderDetails
    {
        get
        {
            if (_fields == null)
                return orderDetails;
            else
                return OnlineOrderDetail.GetOnlineOrderDetails(int.Parse(_fields["id"].ToString().Trim()));
        }
    }

    public string Type
    {
        get
        {
            if (_fields != null)
            {
                return _fields["type"].ToString().Trim();
            }
            else
                return type;
        }
        set
        {
            type = value;
        }
    }

    public string PayMethod
    {
        get
        {
            if (_fields != null)
                return _fields["pay_method"].ToString().Trim();
            else
                return "微信";
        }
    }

    /*
    public double OrderPrice
    {
        get
        {
            if (Type.Trim().Equals("雪票") || Type.Trim().Equals("打赏") 
                || Type.Trim().Equals("秒杀") || Type.Trim().Equals("卡券"))
            {
                double price = 0;
                foreach (OnlineOrderDetail detail in OrderDetails)
                {
                    price = price + detail.summary;
                }
                return price;
            }
            else
            {
                return double.Parse(_fields["order_real_pay_price"].ToString());
            }
            
        }
    }
    */
    public double OrderPrice
    {
        get
        {
            double price = 0;
            foreach (OnlineOrderDetail detail in OrderDetails)
            {
                price = price + detail.summary;
            }
            if (price == 0)
            {
                return double.Parse(_fields["order_real_pay_price"].ToString());
            }
            return price;
        }
    }



    public string Memo
    {
        get
        {
            if (_fields == null)
                return memo;
            else
                return _fields["memo"].ToString();
        }
        set
        {
            memo = value.Trim();
        }
    }

    public bool SetOrderPaySuccess(DateTime successTime, string syssn)
    {
        bool ret = false;
        if (_fields["pay_state"].ToString().Equals("0"))
        {
            string[,] updateParam = { { "pay_state", "int", "1" }, { "pay_time", "datetime", successTime.ToString() },
                {"syssn", "varchar", syssn.Trim() } };
            string[,] keyParam = { { "id", "int", _fields["id"].ToString() } };
            int i = DBHelper.UpdateData("order_online", updateParam, keyParam, Util.conStr.Trim());
            if (i == 1)
            {
                ret = true;
            }
        }
        return ret;
    }

    public string CreateSkiPass()
    {
        if (_fields["code"] == null || _fields["code"].ToString().Trim().Equals(""))
        {
            string type = "";

            foreach (OnlineOrderDetail detail in OrderDetails)
            {
                Product p = new Product(detail.productId);
                if (p.Type.Trim().Equals("雪票") || p.Type.Trim().Equals("课程"))
                {
                    type = p.Type.Trim();
                }
            }


            string code = Card.GenerateCardNo(9, 0, type.Trim());
            string[,] updateParam = { { "code", "varchar", code } };
            string[,] keyParam = { { "id", "int", _fields["id"].ToString() } };
            DBHelper.UpdateData("order_online", updateParam, keyParam, Util.conStr.Trim());

            foreach (OnlineOrderDetail orderDetail in OrderDetails)
            {
                try
                {
                    Product p = new Product(orderDetail.productId);
                    if (p._fields["type"].ToString().Trim().Equals("雪票") && int.Parse(p._fields["stock_num"].ToString()) != -1)
                    {
                        int stockNum = int.Parse(p._fields["stock_num"].ToString());
                        stockNum--;
                        DBHelper.UpdateData("product", new string[,] { { "stock_num", "int", stockNum.ToString() } },
                            new string[,] { { "id", "int", orderDetail.productId.ToString() } }, Util.conStr.Trim());

                    }
                }
                catch
                {

                }
            }


            return code;
        }
        else
        {
            return "";
        }

    }

    public void UpdateMchId(string mchid)
    {
        DBHelper.UpdateData("order_online", new string[,] { { "mchid", "varchar", mchid.Trim() } },
            new string[,] { { "id", "int", _fields["id"].ToString().Trim() } }, Util.conStr.Trim());
    }

    public int ID
    {
        get
        {
            return int.Parse(_fields["id"].ToString().Trim());
        }
    }

    public static void SetOutTrdeNo(int orderId, string outTradeNo)
    {
        DBHelper.UpdateData("order_online", new string[,] { { "out_trade_no", "varchar", outTradeNo.Trim() } },
            new string[,] { { "id", "int", orderId.ToString() } }, Util.conStr.Trim());
    }

    public int Refund(double amount, string operOpenId)
    {
        int ret = 0;
        bool result = false;
        if (int.Parse(_fields["pay_state"].ToString()) != 1)
        {
            return 0;
        }
        int i = DBHelper.InsertData("order_online_refund", new string[,] {
            {"order_id", "int", _fields["id"].ToString()},
            {"amount", "float", Math.Round(amount, 2).ToString() },
            {"oper", "varchar", operOpenId.Trim() }
        });
        if (i == 1)
        {
            DataTable dt = DBHelper.GetDataTable(" select top 1 * from  order_online_refund where order_id = " + ID.ToString()
                + " and amount = " + Math.Round(amount, 2).ToString() + "  and oper = '" + operOpenId.Trim() + "'  order by [id] desc");
            if (dt.Rows.Count == 1)
            {
                ret = int.Parse(dt.Rows[0]["id"].ToString());
            }
            dt.Dispose();

        }
        if (ret > 0)
        {
            switch (_fields["pay_method"].ToString().Trim())
            {
                case "微信":
                    WeixinPaymentOrder weixinOrder = new WeixinPaymentOrder(_fields["out_trade_no"].ToString().Trim());
                    result = weixinOrder.Refund(amount);

                    break;
                default:
                    break;
            }
        }
        if (result)
        {
            DBHelper.UpdateData("order_online_refund", new string[,] { { "status", "int", "1" } },
                new string[,] { { "id", "int", ret.ToString() } }, Util.conStr);
        }
        return ret;
    }


}