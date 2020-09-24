using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for Ticket
/// </summary>
public class Ticket
{

    public struct TicketTemplate
    {
        public int id;
        public double currencyValue;
        public int availableDays;
        public string memo;
        public int neetPoints;
        public string type;
        public string name;
        public DateTime expireDate;


    }

    public static TicketTemplate GetTicetTemplate(int templateId)
    {
        TicketTemplate template = new TicketTemplate();
        template.id = templateId;
        DataTable dt = DBHelper.GetDataTable(" select * from ticket_template where [id] = " + templateId.ToString());
        if (dt.Rows.Count == 1)
        {
            template.currencyValue = double.Parse(dt.Rows[0]["currency_value"].ToString().Trim());
            template.availableDays = int.Parse(dt.Rows[0]["available_days"].ToString().Trim());
            template.memo = dt.Rows[0]["memo"].ToString().Trim();
            template.type = dt.Rows[0]["type"].ToString().Trim();
            template.name = dt.Rows[0]["name"].ToString().Trim();
            if (!dt.Rows[0]["expire_date"].ToString().Equals(""))
            {
                template.expireDate = DateTime.Parse(dt.Rows[0]["expire_date"].ToString().Trim());
            }
            else
            {
                template.expireDate = DateTime.MinValue;
            }

        }
        return template;
    }

    public DataRow _fields;

    public Ticket()
    {

    }

    public Ticket(string code)
    {
        //
        // TODO: Add constructor logic here
        //
        DataTable dt = DBHelper.GetDataTable(" select * from ticket  left join ticket_template on template_id = ticket_template.[id]  where code = '" + code.Trim() + "'  ");
        if (dt.Rows.Count == 0)
            throw new Exception("Ticket is not exists.");
        else
        {
            _fields = dt.Rows[0];
        }
    }

    public bool Use(string openId, string word)
    {
        if (!Used)
        {
            string[,] updateParameters = new string[,] { { "used", "int", "1" },
                {"used_time", "datetime", DateTime.Now.ToString() },
                {"use_memo", "varchar", word }, {"oper_open_id", "varchar", openId.Trim() } };
            string[,] keyParameter = new string[,] { { "code", "varchar", Code.Trim() } };
            int i = DBHelper.UpdateData("ticket", updateParameters, keyParameter, Util.conStr);
            if (i == 1)
                return true;
            else
                return false;
        }
        else
        {
            return false;
        }
    }


    public bool Use(string word)
    {
        if (!Used)
        {
            string[,] updateParameters = new string[,] { { "used", "int", "1" },
                {"used_time", "datetime", DateTime.Now.ToString() },
                {"use_memo", "varchar", word } };
            string[,] keyParameter = new string[,] { { "code", "varchar", Code.Trim() } };
            int i = DBHelper.UpdateData("ticket", updateParameters, keyParameter, Util.conStr);
            if (i == 1)
                return true;
            else
                return false;
        }
        else
        {
            return false;
        }

    }



    public bool Use(int orderId, string word)
    {
        if (!Used)
        {
            string[,] updateParameters = new string[,] { { "used", "int", "1" },
                {"used_time", "datetime", DateTime.Now.ToString() },
                {"use_memo", "varchar", word } };
            string[,] keyParameter = new string[,] { { "code", "varchar", Code.Trim() } };
            int i = DBHelper.UpdateData("ticket", updateParameters, keyParameter, Util.conStr);
            if (i == 1)
            {
                string[,] updateParameter = new string[,] { { "ticket_code", "varchar", Code.Trim() } };
                string[,] keyParam = new string[,] { { "id", "int", orderId.ToString() } };
                i = DBHelper.UpdateData("order_online", updateParameter, keyParam, Util.conStr);
                if (i == 1)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        else
        {
            return false;
        }
    }


    public bool Transfer(string openId, string memo)
    {
        int i = DBHelper.InsertData("ticket_log",
            new string[,] { {"code", "varchar", _fields["code"].ToString() }, { "sender_open_id", "varchar", Owner.OpenId.Trim() },
                { "accepter_open_id", "varchar", openId.Trim() }, {"transact_time", "datetime", DateTime.Now.ToString() },
                {"memo", "varchar", memo.Trim() } });
        if (i == 1)
        {
            i = DBHelper.UpdateData("ticket", new string[,] { { "open_id", "varchar", openId.Trim() }, { "shared", "int", "0" } },
                new string[,] { { "code", "varchar", _fields["code"].ToString().Trim() } }, Util.conStr);
            if (i == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public bool Transfer(string openId)
    {
        if (Owner.OpenId.Trim().Equals(openId.Trim()) || !_fields["shared"].ToString().Equals("1"))
        {
            return false;
        }
        else
        {
            int i = DBHelper.InsertData("ticket_log",
                new string[,] { {"code", "varchar", _fields["code"].ToString() }, { "sender_open_id", "varchar", Owner.OpenId.Trim() },
                { "accepter_open_id", "varchar", openId.Trim() }, {"transact_time", "datetime", DateTime.Now.ToString() } });
            if (i == 1)
            {
                i = DBHelper.UpdateData("ticket", new string[,] { { "open_id", "varchar", openId.Trim() }, { "shared", "int", "0" } },
                    new string[,] { { "code", "varchar", _fields["code"].ToString().Trim() } }, Util.conStr);
                if (i == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    public string Name
    {
        get
        {
            DataTable dt = DBHelper.GetDataTable(" select * from ticket_template where [id] = " + _fields["template_id"].ToString());
            string ret = "";
            if (dt.Rows.Count > 0)
            {
                ret = dt.Rows[0]["name"].ToString().Trim();
            }
            dt.Dispose();
            return ret.Trim();
            /*
            TicketTemplate template = new TicketTemplate();

            Card card = new Card(Code.Trim());
            return card._fields["type"].ToString().Trim();
            */
            //return _fields["name"].ToString().Trim();
        }
    }

    public string Code
    {
        get
        {
            return _fields["code"].ToString();
        }
    }

    public bool IsSharing
    {
        get
        {
            return _fields["shared"].ToString().Equals("1") ? true : false;
        }
        set
        {
            int share = value ? 1 : 0;
            DBHelper.UpdateData("ticket", new string[,] { { "shared", "int", share.ToString() }, { "shared_time", "datetime", DateTime.Now.ToString() } },
                new string[,] { { "code", "varchar", _fields["code"].ToString().Trim() } }, Util.conStr.Trim());
        }


    }

    public WeixinUser Owner
    {
        get
        {
            return new WeixinUser(_fields["open_id"].ToString().Trim());
        }
    }

    public double Amount
    {
        get
        {
            return 0;// double.Parse(_fields["amount"].ToString());
        }
    }

    public DateTime ExpireDate
    {
        get
        {
            return DateTime.Parse(_fields["expire_date"].ToString());
        }
    }

    public bool Used
    {
        get
        {
            return _fields["used"].ToString().Equals("1");
        }
    }

    public string Type
    {
        get
        {
            string ret = "";
            DataTable dt = DBHelper.GetDataTable(" select * from ticket_template where [id] = " + _fields["template_id"].ToString());
            if (dt.Rows.Count == 0)
            {
                ret = "";
            }
            else
            {
                ret = dt.Rows[0]["type"].ToString().Trim();
            }


            return ret;
        }
    }

    public static int Share(string code, int type)
    {
        return DBHelper.UpdateData("ticket", new string[,] { { "shared", "int", type.ToString() }, { "shared_time", "datetime", DateTime.Now.ToString() } },
            new string[,] { { "code", "varchar", code.Trim() } }, Util.conStr.Trim());
    }

    public static TicketTemplate GetTicketTemplate(int templateId)
    {
        TicketTemplate tt = new TicketTemplate();
        DataTable dt = DBHelper.GetDataTable(" select * from ticket_template where [id] = " + templateId.ToString());
        if (dt.Rows.Count == 0)
            throw new Exception("Ticket template is not exsits.");
        else
        {
            tt.id = int.Parse(dt.Rows[0]["id"].ToString().Trim());
            tt.currencyValue = double.Parse(dt.Rows[0]["currency_value"].ToString());
            tt.availableDays = int.Parse(dt.Rows[0]["available_days"].ToString());
            tt.memo = dt.Rows[0]["memo"].ToString().Trim();
            tt.neetPoints = int.Parse(dt.Rows[0]["need_points"].ToString());
            tt.type = dt.Rows[0]["type"].ToString();
        }
        return tt;
    }

    public static int GenerateNewTicket(string code, string openId, int templateId)
    {
        DataTable dtTemplate = DBHelper.GetDataTable(" select * from ticket_template where [id] = " + templateId.ToString());
        int i = 0;
        if (dtTemplate.Rows.Count == 1)
        {
            double amount = double.Parse(dtTemplate.Rows[0]["currency_value"].ToString().Trim());
            DateTime expireDate = DateTime.Now.AddDays(int.Parse(dtTemplate.Rows[0]["available_days"].ToString().Trim()));
            DateTime templateExpireDate = DateTime.MaxValue;
            if (!dtTemplate.Rows[0]["expire_date"].ToString().Trim().Equals(""))
            {
                templateExpireDate = DateTime.Parse(dtTemplate.Rows[0]["expire_date"].ToString().Trim());
            }
            if (templateExpireDate < expireDate)
            {
                expireDate = templateExpireDate;
            }
            string memo = dtTemplate.Rows[0]["memo"].ToString().Trim();
            string[,] insertParameters = { {"code", "varchar", code },
                {"open_id", "varchar", openId.Trim() },
                {"template_id", "int", templateId.ToString() },
                {"amount", "float", Math.Round(amount,2).ToString() },
                {"expire_date", "datetime", expireDate.ToString() },
                {"memo", "varchar", memo.Trim() } };
            i = DBHelper.InsertData("ticket", insertParameters);
        }
        dtTemplate.Dispose();
        return i;
    }
    /*
    public static string GenerateNewTicket(string openId, int templateId)
    {
        string code = GenerateNewTicketCode();
        DataTable dtTemplate = DBHelper.GetDataTable(" select * from ticket_template where [id] = " + templateId.ToString());
        int i = 0;
        if (dtTemplate.Rows.Count == 1)
        {
            double amount = double.Parse(dtTemplate.Rows[0]["currency_value"].ToString().Trim());
            DateTime expireDate = DateTime.Now.AddDays(int.Parse(dtTemplate.Rows[0]["available_days"].ToString().Trim()));
            string memo = dtTemplate.Rows[0]["memo"].ToString().Trim();
            string[,] insertParameters = { {"code", "varchar", code },
                {"user_open_id", "varchar", openId.Trim() },
                {"template_id", "int", templateId.ToString() },
                {"amount", "float", Math.Round(amount,2).ToString() },
                {"expire_date", "datetime", expireDate.ToString() },
                {"memo", "varchar", memo.Trim() } };
            i = DBHelper.InsertData("ticket", insertParameters);
        }
        dtTemplate.Dispose();
        if (i == 1)
            return code.Trim();
        else
            return "";
    }
    */
    public static string GenerateNewTicketCode(int ticketTemplateId)
    {
        string templateName = "";
        DataTable dtTemplate = DBHelper.GetDataTable(" select * from ticket_template where [id] = " + ticketTemplateId.ToString());
        if (dtTemplate.Rows.Count == 1)
        {
            templateName = dtTemplate.Rows[0]["type"].ToString().Trim();
        }
        string code = Card.GenerateCardNo(9, -1);
        Card card = new Card(code);
        card.Type = templateName.Trim();
        return code.Trim();

    }

    public static string GetRandomString(int digit)
    {
        Dictionary<int, char> charHash = new Dictionary<int, char>();

        charHash.Add(0, '1');
        charHash.Add(1, '2');
        charHash.Add(2, '3');
        charHash.Add(3, '4');
        charHash.Add(4, '5');
        charHash.Add(5, '6');
        charHash.Add(6, '7');
        charHash.Add(7, '8');
        charHash.Add(8, '9');
        charHash.Add(9, '0');
        /*
        charHash.Add(9, 'A');
        charHash.Add(10, 'B');
        charHash.Add(11, 'C');
        charHash.Add(12, 'D');
        charHash.Add(13, 'E');
        charHash.Add(14, 'F');
        charHash.Add(15, 'G');
        charHash.Add(16, 'H');
        charHash.Add(17, 'I');
        charHash.Add(18, 'J');
        charHash.Add(19, 'K');
        charHash.Add(20, 'L');
        charHash.Add(21, 'M');
        charHash.Add(22, 'N');
        charHash.Add(23, 'P');
        charHash.Add(24, 'Q');
        charHash.Add(25, 'R');
        charHash.Add(26, 'R');
        charHash.Add(27, 'T');
        charHash.Add(28, 'U');
        charHash.Add(29, 'V');
        charHash.Add(30, 'W');
        charHash.Add(31, 'X');
        charHash.Add(32, 'Y');
        charHash.Add(33, 'Z');
        */
        string retCode = "";
        Random rnd = new Random();
        for (int i = 0; i < digit; i++)
        {
            retCode = retCode + charHash[rnd.Next(charHash.Count)];
        }


        return retCode;
    }

    public static TicketTemplate[] GetAllTicketTemplate()
    {
        DataTable dt = DBHelper.GetDataTable(" select * from ticket_template where valid = 1 and hide = 0 order by [id] ");
        TicketTemplate[] ticketTemplateArray = new TicketTemplate[dt.Rows.Count];
        for (int i = 0; i < ticketTemplateArray.Length; i++)
        {
            ticketTemplateArray[i] = new TicketTemplate();
            ticketTemplateArray[i].id = int.Parse(dt.Rows[i]["id"].ToString());
            ticketTemplateArray[i].name = dt.Rows[i]["name"].ToString().Trim();
            ticketTemplateArray[i].availableDays = int.Parse(dt.Rows[i]["available_days"].ToString());
            ticketTemplateArray[i].currencyValue = int.Parse(dt.Rows[i]["currency_value"].ToString());
            ticketTemplateArray[i].memo = dt.Rows[i]["memo"].ToString().Trim();
            ticketTemplateArray[i].neetPoints = int.Parse(dt.Rows[i]["need_points"].ToString());
        }
        return ticketTemplateArray;
    }

    public static Ticket[] GetUserAllTickets(string openId)
    {
        DataTable dt = DBHelper.GetDataTable(" select * from ticket where user_open_id = '" + openId + "'  order by create_date ");
        Ticket[] ticketArray = new Ticket[dt.Rows.Count];
        for (int i = 0; i < ticketArray.Length; i++)
        {
            ticketArray[i] = new Ticket();
            ticketArray[i]._fields = dt.Rows[i];
        }
        return ticketArray;
    }

    public static Ticket[] GetUserTickets(string openId, bool isUsed)
    {
        DataTable dt = DBHelper.GetDataTable(" select * from ticket where open_id = '" + openId + "' "
            + " and " + (isUsed ? " used = 1 " : " used = 0 ")
            + "  order by create_date  ");
        Ticket[] ticketArray = new Ticket[dt.Rows.Count];
        for (int i = 0; i < ticketArray.Length; i++)
        {
            ticketArray[i] = new Ticket();
            ticketArray[i]._fields = dt.Rows[i];
        }
        return ticketArray;
    }

    public static DataTable GetUserTiketSummary(string openId, bool used)
    {

        DataTable dt = DBHelper.GetDataTable(" select template_id, [name], ticket_template.memo, count(*) as [count] from ticket  "
            + "  left join ticket_template on template_id = ticket_template.[id] "
            + "  where open_id = '" + openId.Trim() + "'  and  used = " + (used ? "1" : "0")
            + " group by  template_id, [name], ticket_template.memo ");
        return dt;
    }

    public static string GetSenderOpenId(string code)
    {
        DataTable dt = DBHelper.GetDataTable(" select top 1 * from ticket_log left join users on open_id = sender_open_id where code = '"
            + code.Trim() + "' and is_admin = 1  order by transact_time   ");
        string sender = "";
        if (dt.Rows.Count == 1)
        {
            sender = dt.Rows[0]["open_id"].ToString().Trim();
        }
        return sender.Trim();
    }
}