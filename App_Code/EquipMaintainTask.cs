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

    public class UserFilledEquipmentInfo
    {
        public string equipType = "";
        public string equipBrand = "";
        public string equipScale = "";
        public string binderBrand = "";
        public string binderColor = "";
        public string sendItem = "板牌";
        public string itemNo = "";
        public string associateItems = "";
        public string expressCompany = "";
        public string wayBillNo = "";
        public string contactName = "";
        public string address = "";
        public string cell = "";
        public DataRow _fields;
    }

    public DataRow _fields;

    public UserFilledEquipmentInfo userFilledEquipmentInfo = new UserFilledEquipmentInfo();

    public EquipMaintainTask()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public EquipMaintainTask(int id)
    {
        DataTable dt = DBHelper.GetDataTable("select * from maintain_task where [id] = " + id.ToString());
        if (dt.Rows.Count == 1)
        {
            _fields = dt.Rows[0];
        }
        else 
        {
            throw new Exception("Not Found!");
        }
    }

    public void GetUserFilledInfo()
    {
        switch (_fields["source_table"].ToString().Trim())
        {
            default:
                DataTable dtCovid = DBHelper.GetDataTable(" select * from covid19_service where task_id = " 
                    + _fields["id"].ToString().Trim());
                if (dtCovid.Rows.Count > 0)
                {
                    userFilledEquipmentInfo._fields = dtCovid.Rows[0];
                    userFilledEquipmentInfo.equipType = userFilledEquipmentInfo._fields["equip_type"].ToString().Trim();
                    userFilledEquipmentInfo.equipBrand = userFilledEquipmentInfo._fields["equip_brand"].ToString().Trim();
                    userFilledEquipmentInfo.equipScale = userFilledEquipmentInfo._fields["equip_scale"].ToString().Trim();
                    userFilledEquipmentInfo.binderBrand = userFilledEquipmentInfo._fields["board_binder_brand"].ToString().Trim();
                    userFilledEquipmentInfo.binderColor = userFilledEquipmentInfo._fields["board_binder_color"].ToString().Trim();
                    userFilledEquipmentInfo.sendItem = userFilledEquipmentInfo._fields["send_item"].ToString().Trim();
                    userFilledEquipmentInfo.itemNo = userFilledEquipmentInfo._fields["send_item"].ToString().Trim();
                    userFilledEquipmentInfo.associateItems = userFilledEquipmentInfo._fields["others_in_wanlong"].ToString().Trim();
                    userFilledEquipmentInfo.expressCompany = userFilledEquipmentInfo._fields["express_company"].ToString().Trim();
                    userFilledEquipmentInfo.wayBillNo = userFilledEquipmentInfo._fields["waybill_no"].ToString().Trim();
                    userFilledEquipmentInfo.contactName = userFilledEquipmentInfo._fields["contact_name"].ToString().Trim();
                    userFilledEquipmentInfo.address = userFilledEquipmentInfo._fields["address"].ToString().Trim();
                    userFilledEquipmentInfo.cell = userFilledEquipmentInfo._fields["cell"].ToString().Trim();
                }
                break;
        }
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