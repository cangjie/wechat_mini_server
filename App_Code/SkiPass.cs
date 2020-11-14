using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for SkiPass
/// </summary>
public class SkiPass
{
    public DataRow _fields;

    public SkiPass()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public SkiPass(int id)
    {
        DataTable dt = DBHelper.GetDataTable(" select * from product_resort_ski_pass left join product on [id] = product_id where product_id = " + id.ToString());
        if (dt.Rows.Count >= 1)
        {
            _fields = dt.Rows[0];
        }

    }

    

    public SkiPass[] SameTimeSkiPass
    {
        get
        {
            DataTable dt = DBHelper.GetDataTable(" select * from product_resort_ski_pass left join product on [id] = product_id "
                + " where end_sale_time = '" + _fields["end_sale_time"].ToString().Trim() + "' and resort = '" + _fields["resort"].ToString() + "' "
                + " order by sale_price  ");
            SkiPass[] skiPassArr = new SkiPass[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                skiPassArr[i] = new SkiPass();
                skiPassArr[i]._fields = dt.Rows[i];
            }
            return skiPassArr;
        }
    } 

    public int InStockCount
    {
        get
        {
            if (_fields["stock_num"].ToString().Equals("-1"))
            {
                return int.MaxValue;
            }
            else
            {
                return int.Parse(_fields["stock_num"].ToString());
            }
        }
    }


    public bool IsAvailableDay(DateTime currentDate)
    {
        bool inAvaDays = false;
        bool inUnAvaDays = false;

        if (_fields["available_days"].ToString().Trim().Equals(""))
        {
            inAvaDays = true;
        }
        else
        {
            inAvaDays = Util.InDate(currentDate, _fields["available_days"].ToString().Trim());
        }

        if (_fields["unavailable_days"].ToString().Trim().Equals(""))
        {
            inUnAvaDays = false;
        }
        else
        {
            inUnAvaDays = Util.InDate(currentDate, _fields["unavailable_days"].ToString().Trim());
        }
        return inAvaDays && !inUnAvaDays;
            
    }

    public bool IsValid
    {
        get
        {
            return _fields["hidden"].ToString().Equals("0") && DateTime.Parse(_fields["start_date"].ToString()) < DateTime.Now
                && DateTime.Parse(_fields["end_date"].ToString()) > DateTime.Now && InStockCount > 0;
        }
    }

}