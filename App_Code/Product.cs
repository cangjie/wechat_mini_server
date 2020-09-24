using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


/// <summary>
/// Summary description for Product
/// </summary>
public class Product
{
    public DataRow _fields;



    public Product()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public struct ServiceCard
    {
        public bool isPackage;
        public string rules;
        public ServiceCardDetail[] detail;
    }

    public struct ServiceCardDetail
    {
        public int id;
        public string name;
        public int count;
    }

    public Product(int id)
    {
        DataTable dt = DBHelper.GetDataTable("select * from product where [id] = " + id.ToString());
        _fields = dt.Rows[0];
    }

    public double SalePrice
    {
        get
        {
            return Math.Round(double.Parse(_fields["sale_price"].ToString()), 2);
        }
    }

    public string Type
    {
        get
        {
            string type = _fields["type"].ToString();
            if (type.Trim().Equals("雪票"))
            {
                DataTable dt = DBHelper.GetDataTable("select * from product_resort_ski_pass where product_id = " + _fields["id"].ToString());
                if (dt.Rows.Count == 0)
                {
                    type = "课程";
                }
            }
            return type.Trim();
        }
    }

    public ServiceCard cardInfo
    {
        get
        {

            if (_fields["type"].ToString().Trim().Equals("服务卡"))
            {
                try
                {
                    ServiceCard card = new ServiceCard();
                    DataTable dtCard = DBHelper.GetDataTable(" select * from product_service_card where product_id = " + _fields["id"].ToString());
                    if (dtCard.Rows.Count == 1)
                    {
                        card.isPackage = (dtCard.Rows[0]["is_package"].ToString().Equals("0") ? false : true);
                        card.rules = dtCard.Rows[0]["rules"].ToString().Trim();
                        if (card.isPackage)
                        {
                            DataTable dtCardDetail = DBHelper.GetDataTable(" select * from product_service_card_detail where product_id = " + _fields["id"].ToString());
                            ServiceCardDetail[] cardDetail = new ServiceCardDetail[dtCardDetail.Rows.Count];
                            for (int i = 0; i < cardDetail.Length; i++)
                            {
                                cardDetail[i] = new ServiceCardDetail();
                                cardDetail[i].id = int.Parse(dtCardDetail.Rows[i]["id"].ToString());
                                cardDetail[i].count = int.Parse(dtCardDetail.Rows[i]["count"].ToString());
                                cardDetail[i].name = dtCardDetail.Rows[i]["name"].ToString().Trim();
                            }
                            card.detail = cardDetail;
                        }
                    }
                    dtCard.Dispose();
                    return card;
                }
                catch
                {
                    return new ServiceCard();
                }
            }
            else
            {
                return new ServiceCard();
            }
        }
    }


    public static Product[] GetInstructorProduct()
    {
        DataTable dt = DBHelper.GetDataTable(" select * from product where [name] like '%教学场地费%' order by sort desc, [id] ");
        Product[] productArr = new Product[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            productArr[i] = new Product();
            productArr[i]._fields = dt.Rows[i];
        }
        return productArr;
    }

    public static Product[] GetSkiPassList(string resort)
    {
        string sqlStr = " select * from product_resort_ski_pass left join product on product.[id] = product_id   where type = '雪票' and  hidden = 0 and ";
        if (resort.Trim().Equals("nanshan"))
        {
            sqlStr = sqlStr + " name like '南山%' ";
        }
        if (resort.Trim().Equals("bayi"))
        {
            sqlStr = sqlStr + " name like '万龙八易%' and shop = '八易' ";
        }
        if (resort.Trim().Equals("bayidan"))
        {
            sqlStr = sqlStr + " name like '万龙八易%' and shop = '八易租单板' ";
        }
        if (resort.Trim().Equals("bayishuang"))
        {
            sqlStr = sqlStr + " name like '万龙八易%' and shop = '八易租双板' ";
        }
        if (resort.Trim().Equals("haixuan"))
        {
            sqlStr = sqlStr + "  shop = '单板海选' ";
        }

        if (resort.Trim().Equals("qiaobo"))
        {
            sqlStr = sqlStr + " name like '乔波%' ";
        }
        sqlStr = sqlStr + " resort = '" + resort.Trim() + "' ";

        sqlStr = sqlStr + "   order by sort desc ";
        DataTable dt = DBHelper.GetDataTable(sqlStr);
        Product[] productArr = new Product[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            productArr[i] = new Product();
            productArr[i]._fields = dt.Rows[i];
        }
        return productArr;
    }
}