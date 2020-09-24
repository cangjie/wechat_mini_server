using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
/// <summary>
/// Summary description for OrderTemp
/// </summary>
public class OrderTemp
{
    public DataRow _fields;
    public OrderTemp()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public OrderTemp(int id)
    {
        DataTable dt = DBHelper.GetDataTable(" select * from order_online_temp where [id] =  " + id.ToString());
        _fields = dt.Rows[0];
    }

    public string Type
    {
        get
        {
            return _fields["type"].ToString().Trim();
        }
        set
        {
            DBHelper.UpdateData("order_online_temp", new string[,] { { "type", "varchar", value.Trim() } },
                new string[,] { { "id", "int", _fields["id"].ToString() } }, Util.conStr.Trim());
        }
    }


    public int PlaceOnlineOrder(string openId)
    {
        try
        {
            int.Parse(_fields["online_order_id"].ToString());
            return 0;
        }
        catch
        {

        }
        string detailJson = _fields["order_detail_json"].ToString().Trim();




        OnlineOrder newOrder = new OnlineOrder();
        WeixinUser user = new WeixinUser(openId);
        string cellNumber = user.CellNumber.Trim();
        if (cellNumber.Trim().Equals(""))
        {
            cellNumber = _fields["customer_number"].ToString().Trim();
        }
        string[,] insertParam = { {"type", "varchar", _fields["type"].ToString().Trim() }, { "open_id", "varchar", openId.Trim() },
        {"cell_number", "varchar", cellNumber.Trim() }, {"name", "varchar", user.Nick.Trim() },
        {"pay_method", "varchar", _fields["pay_method"].ToString().Trim() },{ "pay_state", "int", "0" },
        {"order_price", "float", _fields["market_price"].ToString() }, {"shop", "varchar", _fields["shop"].ToString().Trim() } ,
        {"order_real_pay_price", "float", _fields["real_paid_price"].ToString() }, {"memo", "varchar", _fields["memo"].ToString().Trim() },
        {"pay_time", "datetime", DateTime.Now.ToString() }, {"ticket_amount", "float", _fields["ticket_amount"].ToString() },
        {"score_rate", "float", _fields["score_rate"].ToString() }, {"generate_score", "float", _fields["generate_score"].ToString() },
        {"order_temp_id", "float", _fields["id"].ToString() }, {"ticket_code", "varchar", _fields["ticket_code"].ToString().Trim() } };
        int i = DBHelper.InsertData("order_online", insertParam);
        if (i == 1)
        {
            i = DBHelper.GetMaxId("order_online");
        }


        try
        {
            Dictionary<string, object>[] detailDicArr = Util.GetObjectArrayFromJsonByKey(detailJson, "order_details");
            foreach (Dictionary<string, object> detail in detailDicArr)
            {
                string[,] detailInsertParam = { {"order_online_id", "int", i.ToString() }, {"product_id", "int", "0" },
                {"product_name", "varchar", detail["name"].ToString().Trim() }, {"price", "float", detail["deal_price"].ToString() },
                {"count", "int", detail["num"].ToString() }, {"retail_price", "float", detail["market_price"].ToString() } };
                DBHelper.InsertData("order_online_detail", detailInsertParam);
            }
        }
        catch
        {

        }

        string[,] updateParam = { { "online_order_id", "int", i.ToString() } };
        string[,] keyParam = { { "id", "int", _fields["id"].ToString() } };
        DBHelper.UpdateData("order_online_temp", updateParam, keyParam, Util.conStr);
        return i;
    }


    public void FinishOrder()
    {
        string[,] updateParam = { { "is_paid", "int", "1" }, { "pay_date_time", "datetime", DateTime.Now.ToString() } };
        string[,] keyParam = { { "id", "int", _fields["id"].ToString() } };
        DBHelper.UpdateData("order_online_temp", updateParam, keyParam, Util.conStr);
    }

    public static int AddNewOrderTemp(double marketPrice, double salePrice, double ticketAmount, string memo,
        string openId, string payMethod, string shop, string memberType, string recommenderNumber,
        string recommenderType, string name)
    {
        double realPayPrice = salePrice - ticketAmount;
        double scoreRate = GetScoreRate(realPayPrice, marketPrice);
        int generateScore = (int)(realPayPrice * scoreRate);
        string[,] insertParam = { { "admin_open_id", "varchar", openId }, {"market_price", "float", Math.Round(marketPrice,2).ToString() },
        {"sale_price", "float", Math.Round(salePrice, 2).ToString() }, {"real_paid_price", "float", Math.Round(realPayPrice, 2).ToString() },
        {"ticket_amount", "float", Math.Round(ticketAmount, 2).ToString() }, {"score_rate", "float", Math.Round(scoreRate, 2).ToString() },
        {"generate_score", "int", generateScore.ToString() }, {"memo", "varchar", memo.Trim() },
        {"is_paid", "int", "0" }, {"pay_date_time", "datetime", DateTime.Now.ToString() }, {"pay_method", "varchar", payMethod.Trim() },
        {"shop", "varchar", shop.Trim() }, {"member_type", "varchar", memberType.Trim() },
        {"recommender_number", "varchar", recommenderNumber.Trim() }, {"recommender_type", "varchar", recommenderType.Trim() },
         {"name", "varchar", name.Trim() }

        };
        int i = DBHelper.InsertData("order_online_temp", insertParam);
        if (i == 1)
        {
            DataTable dt = DBHelper.GetDataTable(" select max([id]) from order_online_temp ");
            i = int.Parse(dt.Rows[0][0].ToString());
            dt.Dispose();
        }
        return i;
    }

    public static int AddNewOrderTemp(string customOpenId, double marketPrice, double salePrice, double ticketAmount, string memo, string salesOpenId, string payMethod,
        string shop, string memberType, string recommenderNumber, string recommenderType, string name,
        string orderDetailJson, string tickeCode, string cell)
    {
        double realPayPrice = salePrice - ticketAmount;
        double scoreRate = GetScoreRate(realPayPrice, marketPrice);
        int generateScore = (int)(realPayPrice * scoreRate);
        string[,] insertParam = { { "admin_open_id", "varchar", salesOpenId }, {"market_price", "float", Math.Round(marketPrice,2).ToString() },
        {"sale_price", "float", Math.Round(salePrice, 2).ToString() }, {"real_paid_price", "float", Math.Round(realPayPrice, 2).ToString() },
        {"ticket_amount", "float", Math.Round(ticketAmount, 2).ToString() }, {"score_rate", "float", Math.Round(scoreRate, 2).ToString() },
        {"generate_score", "int", generateScore.ToString() }, {"memo", "varchar", memo.Trim() },
        {"is_paid", "int", "0" }, {"pay_date_time", "datetime", DateTime.Now.ToString() }, {"pay_method", "varchar", payMethod.Trim() },
        {"shop", "varchar", shop.Trim() }, {"member_type", "varchar", memberType.Trim() },
        {"recommender_number", "varchar", recommenderNumber.Trim() }, {"recommender_type", "varchar", recommenderType.Trim() },
        {"name", "varchar", name.Trim() }, {"order_detail_json", "varchar", orderDetailJson }, {"ticket_code", "varchar", tickeCode.Trim() },
        {"customer_open_id", "varchar", customOpenId.Trim() }, {"customer_number", "varchar", cell.Trim()}};
        int i = DBHelper.InsertData("order_online_temp", insertParam);
        if (i == 1)
        {
            DataTable dt = DBHelper.GetDataTable(" select max([id]) from order_online_temp ");
            i = int.Parse(dt.Rows[0][0].ToString());
            dt.Dispose();
        }
        return i;
    }

    public static int AddNewOrderTemp(double marketPrice, double salePrice, double ticketAmount, string memo,
        string openId, string payMethod, string shop)
    {
        double realPayPrice = salePrice - ticketAmount;
        double scoreRate = GetScoreRate(realPayPrice, marketPrice);
        int generateScore = (int)(realPayPrice * scoreRate);
        string[,] insertParam = { { "admin_open_id", "varchar", openId }, {"market_price", "float", Math.Round(marketPrice,2).ToString() },
        {"sale_price", "float", Math.Round(salePrice, 2).ToString() }, {"real_paid_price", "float", Math.Round(realPayPrice, 2).ToString() },
        {"ticket_amount", "float", Math.Round(ticketAmount, 2).ToString() }, {"score_rate", "float", Math.Round(scoreRate, 2).ToString() },
        {"generate_score", "int", generateScore.ToString() }, {"memo", "varchar", memo.Trim() },
        {"is_paid", "int", "1" }, {"pay_date_time", "datetime", DateTime.Now.ToString() }, {"pay_method", "varchar", payMethod.Trim() },
        {"shop", "varchar", shop.Trim() } };
        int i = DBHelper.InsertData("order_online_temp", insertParam);
        if (i == 1)
        {
            DataTable dt = DBHelper.GetDataTable(" select max([id]) from order_online_temp ");
            i = int.Parse(dt.Rows[0][0].ToString());
            dt.Dispose();
        }
        return i;
    }

    public static OrderTemp GetFinishedOrder(int orderId)
    {
        OrderTemp tempOrder = new OrderTemp();
        DataTable dt = DBHelper.GetDataTable(" select * from order_online_temp where online_order_id = "
            + orderId.ToString() + " order by [id] desc");
        if (dt.Rows.Count > 0)
        {
            tempOrder = new OrderTemp(int.Parse(dt.Rows[0]["id"].ToString()));
        }
        dt.Dispose();
        return tempOrder;
    }

    public static double GetScoreRate(double realPayPrice, double marketPrice)
    {
        double disCountRate = realPayPrice / marketPrice;
        double rate = 0;
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