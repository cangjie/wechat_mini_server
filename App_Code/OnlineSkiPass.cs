using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for OnlineSkiPass
/// </summary>
public class OnlineSkiPass
{
    public string cardCode = "";
    public string productName = "";
    public MiniUsers owner;
    public int count = 0;
    public bool used = false;
    public DateTime useDate;
    public OnlineOrder associateOnlineOrder;
    public OnlineOrderDetail associateOnlineOrderDetail;
    public Card associateCard;

    public DataRow _fields;

    public OnlineSkiPass(string code)
    {
        cardCode = code;
        DataTable dtOrder = DBHelper.GetDataTable(" select [id] from order_online where type='雪票' and code = '" + code.Trim() + "' ");
        associateOnlineOrder = new OnlineOrder(int.Parse(dtOrder.Rows[0][0].ToString()));
        associateOnlineOrderDetail = associateOnlineOrder.OrderDetails[0];
        productName = associateOnlineOrderDetail.productName.Trim();
        count = associateOnlineOrderDetail.count;
        associateCard = new Card(code);

        owner = new MiniUsers(associateOnlineOrder._fields["open_id"].ToString());

        if (associateCard._fields["type"].Equals("雪票"))
        {
            if (!associateCard._fields["used"].ToString().Equals("0"))
            {
                used = true;
                try
                {
                    useDate = DateTime.Parse(associateCard._fields["use_date"].ToString());
                }
                catch
                {

                }
            }

        }
    }



    public OnlineSkiPass()
    {

    }

    public string CardCode
    {
        get
        {
            if (cardCode.Trim().Equals(""))
            {
                return _fields["code"].ToString().Trim();
            }
            else
            {
                return cardCode.Trim();
            }
        }

    }

    public bool Used
    {
        get
        {
            if (_fields["used"].ToString().Equals("1"))
            {
                used = true;
                try
                {
                    useDate = DateTime.Parse(_fields["use_date"].ToString());
                }
                catch
                {

                }
            }
            else
            {
                used = false;
            }
            return used;
        }
    }

    public bool Rent
    {
        get
        {
            bool ret = false;
            foreach (OnlineOrderDetail detail in AssociateOnlineOrder.OrderDetails)
            {
                if (detail.productName.IndexOf("租板") >= 0)
                {
                    ret = true;
                }
            }
            return ret;
        }
    }

    public DateTime AppointDate
    {
        get
        {
            try
            {
                return DateTime.Parse(Util.GetSimpleJsonValueByKey(AssociateOnlineOrder._fields["memo"].ToString(), "use_date"));
            }
            catch
            {
                return DateTime.Parse(DateTime.Parse(AssociateOnlineOrder._fields["create_date"].ToString()).AddDays(1).ToShortDateString());
            }

        }
    }

    public MiniUsers Owner
    {
        get
        {
            if (owner != null)
            {
                return owner;
            }
            else
            {
                if (_fields != null)
                {
                    return new MiniUsers(_fields["open_id"].ToString().Trim());
                }
                else
                {
                    return null;
                }
            }
        }
    }

    public OnlineOrder AssociateOnlineOrder
    {
        get
        {
            if (associateOnlineOrder != null)
            {
                return associateOnlineOrder;
            }
            else
            {
                OnlineOrder order = new OnlineOrder();
                order._fields = this._fields;
                return order;
            }
        }
    }

    public OnlineOrderDetail AssociateOnlineOrderDetail
    {
        get
        {
            return AssociateOnlineOrder.OrderDetails[0];
        }
    }

    public static OnlineSkiPass[] GetOnlieSkiPassByOwnerOpenId(string openId)
    {
        DataTable dt = DBHelper.GetDataTable(" select * from order_online where type = '雪票' and code <> '' and code is not null and open_id = '"
            + openId.Trim() + "'  and pay_state = 1 "
            + " and exists ( select 'a' from [card] where card_no = order_online.code and [type]='雪票' ) order by [id] desc ");
        OnlineSkiPass[] passArr = new OnlineSkiPass[dt.Rows.Count];
        for (int i = 0; i < passArr.Length; i++)
        {
            passArr[i] = new OnlineSkiPass(dt.Rows[i]["code"].ToString());
        }
        dt.Dispose();
        return passArr;
    }

    public static OnlineSkiPass[] GetUnusedOnlineSkiPass()
    {
        DataTable dt = DBHelper.GetDataTable(" select code from order_online left join card on card_no = code where   card.type = '雪票' and code <> '' and code is not null   and pay_state = 1 and used = 0  order by [id] desc ");
        OnlineSkiPass[] passArr = new OnlineSkiPass[dt.Rows.Count];
        for (int i = 0; i < passArr.Length; i++)
        {
            passArr[i] = new OnlineSkiPass(dt.Rows[i]["code"].ToString());
        }
        dt.Dispose();
        return passArr;
    }

    public static OnlineSkiPass[] GetLastWeekOnlineSkiPass1()
    {
        DataTable dt = DBHelper.GetDataTable(" select code from order_online left join card on card_no = code where   card.type = '雪票' and code <> '' and code is not null   and pay_state = 1   and card.create_date >= '" + DateTime.Now.AddDays(-30).ToShortDateString() + "' order by [id] desc ");
        OnlineSkiPass[] passArr = new OnlineSkiPass[dt.Rows.Count];
        for (int i = 0; i < passArr.Length; i++)
        {
            passArr[i] = new OnlineSkiPass(dt.Rows[i]["code"].ToString());
        }
        dt.Dispose();
        return passArr;
    }

    public static OnlineSkiPass[] GetLastWeekOnlineSkiPass()
    {
        DataTable dt = DBHelper.GetDataTable(" select * from order_online left join card on card_no = code where   card.type = '雪票' and code <> '' and code is not null   and pay_state = 1   and card.create_date >= '" + DateTime.Now.AddDays(-30).ToShortDateString() + "' order by [id] desc ");
        OnlineSkiPass[] passArr = new OnlineSkiPass[dt.Rows.Count];
        for (int i = 0; i < passArr.Length; i++)
        {
            passArr[i] = new OnlineSkiPass();
            passArr[i]._fields = dt.Rows[i];
        }
        dt.Dispose();
        return passArr;
    }

}