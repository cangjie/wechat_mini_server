using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for EquipMaintainTask
/// </summary>
public class EquipMaintainTask
{
    public EquipMaintainTask()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static int CreateTaskFromCovid19Service(string cardNo)
    {
        int maxId = 0;
        DataTable dt = DBHelper.GetDataTable(" select * from covid19_service where card_no = '" 
            + cardNo.Trim().Replace("'", "").Trim() + "' ");
        if (dt.Rows.Count == 1 && dt.Rows[0]["task_id"].ToString().Equals("0"))
        {
            Card card = new Card(cardNo.Trim());
            string openId = card.Owner.OpenId.Trim();
            int i = DBHelper.InsertData("maintain_task", new string[,] {
                {"open_id", "varchar", openId.Trim() },
                {"card_no", "varchar", cardNo.Trim()}
            });
            if (i == 1)
            {
                DataTable dtMax = DBHelper.GetDataTable(" select max([id]) from maintain_task ");
                maxId = int.Parse(dtMax.Rows[0][0].ToString().Trim());
                dtMax.Dispose();
                DBHelper.UpdateData("covid19_service", new string[,] { { "task_id", "int", maxId.ToString() } },
                    new string[,] { { "card_no", "varchar", cardNo.Trim() } }, Util.conStr.Trim());
            }
        }
        dt.Dispose();
        return maxId;
    }
}