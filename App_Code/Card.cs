using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for Card
/// </summary>
public class Card
{

    public struct CardPackageUsage
    {
        public int productDetailId;
        public string firstAvaliableCardCode;
        public string name;
        public int totalCount;
        public int avaliableCount;
    }

    public class CardDetail
    {
        public DataRow _fields;

        public CardDetail(string code)
        {
            DataTable dt = DBHelper.GetDataTable(" select * from card_detail where card_no = '" + code.Substring(0, 9)
                + "' and detail_no = '" + code.Substring(9, 3).Trim() + "' ");
            _fields = dt.Rows[0];
        }

        public int Use(DateTime useDateTime, string memo)
        {
            int i = DBHelper.UpdateData("card_detail", new string[,] { { "used", "int", "1" },
                { "use_memo", "varchar", memo.Trim() }, {"use_date", "datetime", useDateTime.ToString() } },
                new string[,] { {"card_no", "varchar", _fields["card_no"].ToString().Trim() },
                {"detail_no", "varchar", _fields["detail_no"].ToString().Trim() } }, Util.conStr);
            return i;
        }


        public string Name
        {
            get
            {
                string name = "";
                DataTable dt = DBHelper.GetDataTable(" select * from product_service_card_detail where [id] = " + _fields["product_detail_id"].ToString());
                if (dt.Rows.Count == 1)
                {
                    name = dt.Rows[0]["name"].ToString().Trim();
                }
                dt.Dispose();
                return name.Trim();
            }
        }

        public bool Used
        {
            get
            {
                bool ret = false;
                if (_fields["used"].ToString().Trim().Equals("1"))
                {
                    ret = true;
                }
                return ret;
            }
        }
    }

    public DataRow _fields;

    public Ticket associateTicket;

    public Card()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public Card(string code)
    {
        DataTable dt = DBHelper.GetDataTable(" select * from card where card_no = '" + code + "' ");
        _fields = dt.Rows[0];
    }

    public void Use(DateTime useDateTime)
    {
        string[,] updateParam = { { "used", "int", "1" }, { "use_date", "datetime", useDateTime.ToString() } };
        string[,] keyParam = { { "card_no", "varchar", _fields["card_no"].ToString() } };
        DBHelper.UpdateData("card", updateParam, keyParam, Util.conStr);
    }

    public void Use(DateTime useDateTime, string memo)
    {
        string[,] updateParam = { { "used", "int", "1" }, { "use_date", "datetime", useDateTime.ToString() },
            {"use_memo", "varchar", memo } };
        string[,] keyParam = { { "card_no", "varchar", _fields["card_no"].ToString() } };
        DBHelper.UpdateData("card", updateParam, keyParam, Util.conStr);
    }

    public string Code
    {
        get
        {
            return _fields["card_no"].ToString().Trim();

        }
    }

    public bool Used
    {
        get
        {
            if (_fields["used"].ToString().Equals("0"))
                return false;
            else
                return true;

        }
    }

    public bool IsTicket
    {
        get
        {
            DataTable dtTicket = DBHelper.GetDataTable("select * from ticket where code = '" + _fields["card_no"].ToString().Trim() + "' ");
            bool ret = false;
            if (dtTicket.Rows.Count > 0)
            {
                ret = true;
                associateTicket = new Ticket(_fields["card_no"].ToString().Trim());
            }
            return ret;
        }
    }

    public CardPackageUsage[] CardPackageUsageList
    {
        get
        {
            DataTable dtCardDetail = DBHelper.GetDataTable(" select distinct product_detail_id, [name] from card_detail "
                + " left join product_service_card_detail on product_service_card_detail.[id] = product_detail_id where card_no = '" + Code.Trim() + "' ");
            CardPackageUsage[] usageList = new CardPackageUsage[dtCardDetail.Rows.Count];
            for (int i = 0; i < usageList.Length; i++)
            {
                usageList[i] = new CardPackageUsage();
                usageList[i].productDetailId = int.Parse(dtCardDetail.Rows[i]["product_detail_id"].ToString());
                usageList[i].name = dtCardDetail.Rows[i]["name"].ToString().Trim();
                DataTable dtSubCard = DBHelper.GetDataTable(" select * from card_detail where card_no = '" + Code
                    + "' and  product_detail_id = " + usageList[i].productDetailId.ToString() + " order by used ");
                usageList[i].totalCount = dtSubCard.Rows.Count;
                usageList[i].avaliableCount = dtSubCard.Select(" used = 0 ").Length;
                if (usageList[i].totalCount > 0 && dtSubCard.Rows[0]["used"].ToString().Equals("0"))
                {
                    usageList[i].firstAvaliableCardCode = Code.Trim() + dtSubCard.Rows[0]["detail_no"].ToString();
                }
                else
                {
                    usageList[i].firstAvaliableCardCode = "";
                }
                dtSubCard.Dispose();
            }
            return usageList;
        }
    }





    public WeixinUser Owner
    {
        get
        {
            switch (_fields["type"].ToString().Trim())
            {
                case "课程":
                case "雪票":
                    OnlineSkiPass pass = new OnlineSkiPass(_fields["card_no"].ToString().Trim());
                    return pass.owner;
                    break;
                default:
                    string ownerOpenId = "";
                    if (IsTicket)
                    {
                        ownerOpenId = associateTicket.Owner.OpenId.Trim();
                    }
                    else
                    {
                        ownerOpenId = _fields["owner_open_id"].ToString().Trim();
                    }
                    return new WeixinUser(ownerOpenId);
                    break;
            }
            return null;
        }
    }

    public string Type
    {
        get
        {
            return _fields["type"].ToString().Trim();
        }
        set
        {
            DBHelper.UpdateData("card", new string[,] { { "type", "varchar", value.Trim() } },
                new string[,] { { "card_no", "varchar", _fields["card_no"].ToString().Trim() } }, Util.conStr.Trim());
        }
    }

    public static string GenerateCardNo(int digit, int batchId)
    {

        string no = Ticket.GetRandomString(digit);
        for (; ExsitsCardNo(no);)
        {
            no = Ticket.GetRandomString(digit);
        }
        string[,] insertParam = { { "card_no", "varchar", no.Trim() }, { "batch_id", "int", batchId.ToString() } };
        int i = 0;
        try
        {
            i = DBHelper.InsertData("card", insertParam);
        }
        catch
        {

        }
        if (i == 1)
            return no;
        else
            return "";
    }

    public static string GenerateCardNoWithPassword(int digit, int batchId, int passwordDigit, string cardType)
    {
        string no = Ticket.GetRandomString(digit);
        for (; ExsitsCardNo(no);)
        {
            no = Ticket.GetRandomString(digit);
        }
        string password = Ticket.GetRandomString(passwordDigit);
        string[,] insertParam = { { "card_no", "varchar", no.Trim() }, { "batch_id", "int", batchId.ToString() },
            {"password", "varchar", password.Trim() }, {"type", "varchar", cardType.Trim() } };
        int i = 0;
        try
        {
            i = DBHelper.InsertData("card", insertParam);
        }
        catch (Exception err)
        {
            Console.WriteLine(err.ToString());
        }
        if (i == 1)
            return no;
        else
            return "";
    }



    public static string GenerateCardNo(int digit, int batchId, string cardType)
    {

        string no = Ticket.GetRandomString(digit);
        for (; ExsitsCardNo(no);)
        {
            no = Ticket.GetRandomString(digit);
        }
        string[,] insertParam = { { "card_no", "varchar", no.Trim() },
            { "batch_id", "int", batchId.ToString() }, {"type", "varchar", cardType.Trim() } };
        int i = DBHelper.InsertData("card", insertParam);
        if (i == 1)
            return no;
        else
            return "";
    }

    public static string GenerateCardNo(int digit, string cardType, string ownerOpenId, bool isPackage, int productId)
    {
        string no = Ticket.GetRandomString(digit);
        for (; ExsitsCardNo(no);)
        {
            no = Ticket.GetRandomString(digit);
        }
        string[,] insertParam = { { "card_no", "varchar", no.Trim() },
            { "batch_id", "int", "0" }, {"type", "varchar", cardType.Trim() }, {"owner_open_id", "varchar", ownerOpenId.Trim() },
            { "is_package", "int", (isPackage?"1":"0")  }, {"product_id", "int", productId.ToString() } };
        int i = DBHelper.InsertData("card", insertParam);
        if (i == 1)
            return no;
        else
            return "";
    }


    public static void CreatePackageCard(string cardNo)
    {
        Card card = new Card(cardNo.Trim());
        Product product = new Product(int.Parse(card._fields["product_id"].ToString()));
        Product.ServiceCard serviceCard = product.cardInfo;
        if (serviceCard.isPackage)
        {
            int j = 0;
            foreach (Product.ServiceCardDetail detail in serviceCard.detail)
            {
                for (int i = 0; i < detail.count; i++)
                {
                    string[,] insertParam = { {"card_no", "varchar", cardNo.Trim() }, {"detail_no", "varchar", j.ToString().PadLeft(3,'0') },
                        {"product_detail_id", "int", detail.id.ToString() } };
                    DBHelper.InsertData("card_detail", insertParam);
                    j++;
                }
            }
        }
    }


    public static bool ExsitsCardNo(string no)
    {
        DataTable dt = DBHelper.GetDataTable(" select 'a' from card where card_no = '" + no.Trim().Replace("'", "") + "' ");
        bool ret = false;
        if (dt.Rows.Count >= 1)
            ret = true;
        dt.Dispose();
        return ret;
    }

    public static Card[] GetCardList(string openId)
    {
        DataTable dt = DBHelper.GetDataTable(" select * from card where owner_open_id = '" + openId.Trim() + "' and [type] = '服务卡' ");
        Card[] cardArray = new Card[dt.Rows.Count];
        for (int i = 0; i < cardArray.Length; i++)
        {
            cardArray[i] = new Card();
            cardArray[i]._fields = dt.Rows[i];
        }
        return cardArray;
    }
}