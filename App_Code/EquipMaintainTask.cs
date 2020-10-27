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

    public static int CreateSteps(int taskId)
    {
        bool existsSteps = false;
        DataTable dtOri = DBHelper.GetDataTable(" select * from maintain_task_detail where task_id = " + taskId.ToString());
        if (dtOri.Rows.Count > 0)
        {
            existsSteps = true;
        }
        dtOri.Dispose();
        if (existsSteps)
        {
            return 0;
        }
        EquipMaintainTask task = new EquipMaintainTask(taskId);
        DataTable dtTemplateDetail = DBHelper.GetDataTable(" select * from maintain_template_detail where template_id = " 
            + task._fields["template_id"].ToString().Trim() + " order by sort, [id]");
        int i = 1;
        foreach (DataRow drTemplateDetail in dtTemplateDetail.Rows)
        {
            try
            {
                DBHelper.InsertData("maintain_task_detail", new string[,] {
                    {"task_id", "int", taskId.ToString() }, {"sort", "int", (i * 10).ToString() },
                    {"name", "varchar", drTemplateDetail["name"].ToString() }, {"memo", "varchar", drTemplateDetail["memo"].ToString() },
                    {"template_detail_id", "int", drTemplateDetail["id"].ToString().Trim() }
                });
            }
            catch(Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err.ToString());
            }
            i++;
        }
        return i;
    }

    public static int CreateSubSteps(int stepId)
    {
        bool existsSubSteps = false;
        DataTable dtOri = DBHelper.GetDataTable(" select * from maintain_task_detail_sub where detail_id = " + stepId.ToString());
        if (dtOri.Rows.Count > 0)
        {
            existsSubSteps = true;
        }
        dtOri.Dispose();
        if (existsSubSteps)
        {
            return 0;
        }
        int stepTemplateId = 0;
        int j = 0;
        DataTable dtStep = DBHelper.GetDataTable(" select * from maintain_task_detail where id = " + stepId.ToString());
        if (dtStep.Rows.Count > 0)
        {
            stepTemplateId = int.Parse(dtStep.Rows[0]["template_detail_id"].ToString());
            DataTable dtStepSub = DBHelper.GetDataTable(" select * from maintain_template_detail_sub   where detail_id = " + stepTemplateId.ToString()
                + " order by sort,[id] ");
            for (int i = 0; i < dtStepSub.Rows.Count; i++)
            {
                try
                {
                    j = j + DBHelper.InsertData("maintain_task_detail_sub", new string[,] {
                    {"detail_id", "int", stepId.ToString() }, {"template_detail_sub_id", "int", dtStepSub.Rows[i]["id"].ToString().Trim()},
                    {"sort", "int", (10*(i + 1)).ToString() }, {"action_type", "varchar", dtStepSub.Rows[i]["action_type"].ToString().Trim() },
                    {"action_to_do", "varchar", dtStepSub.Rows[i]["action_to_do"].ToString() }, {"oper_open_id", "varchar", dtStepSub.Rows[i]["oper_open_id"].ToString() } 
                    });
                }
                catch
                { 
                
                }
            }
        }
        return j;
    }

    



}