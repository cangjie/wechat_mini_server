using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Security.Permissions;

/// <summary>
/// Summary description for EquipMaintainTaskDetail
/// </summary>
public class EquipMaintainTaskDetail
{
    public DataRow _fields;
    public EquipMaintainTaskDetail()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public EquipMaintainTaskDetail(int detailId)
    {
        DataTable dt = DBHelper.GetDataTable(" select * from maintain_task_detail where [id] = " + detailId.ToString());
        if (dt.Rows.Count > 0)
        {
            _fields = dt.Rows[0];
        }
        else
        {
            throw new Exception("Not Found.");
        }
    }

    public int ID
    {
        get
        {
            return int.Parse(_fields["id"].ToString().Trim());
        }
    }

    public string Status
    {
        get
        {
            return _fields["status"].ToString();
        }
    }

    public bool SetStatus(string status, string opneId)
    {
        if (Status.Trim().Equals(status))
        {
            return false;
        }
        bool ret = true;

        switch (status.Trim())
        {
            case "已开始":
                if (MaintainTask.Status.Trim().Equals("进行中") || MaintainTask.Status.Trim().Equals("待交付"))
                {
                    ret = false;
                }
                if (Last._fields != null && !(Last.Status.Trim().Equals("已完成") || Last.Status.Trim().Equals("强行中止")))
                {
                    ret = false;
                }
                break;
            case "强行中止":
                if (!Status.Trim().Equals("已开始"))
                {
                    ret = false;
                }
                break;
            case "已完成":
                if (!Status.Trim().Equals("强行中止") && !Status.Equals("已开始"))
                {
                    ret = false;
                }
                break;
            default:
                ret = false;
                break;
        }
        if (ret)
        {
            DBHelper.UpdateData("maintain_task_detail", new string[,] { {"status", "varchar", status.Trim() } },
                new string[,] { { "id", "int", ID.ToString() } }, Util.conStr);
            DBHelper.InsertData("maintain_task_log", new string[,] { {"task_id", "int", MaintainTask._fields["id"].ToString() },
            {"detail_id", "int", ID.ToString() }, {"oper_open_id", "varchar", opneId.Trim()}, {"oper", "varchar", status.Trim() } });
            try
            {
                if (status.Trim().Equals("已开始"))
                {
                    EquipMaintainTask.CreateSubSteps(ID);
                }
            }
            catch
            { 
            
            }

        }
        
        return ret;
    }

    public EquipMaintainTask MaintainTask
    {
        get
        {
            return new EquipMaintainTask(int.Parse(_fields["task_id"].ToString().Trim()));
        }
    }

    public EquipMaintainTaskDetail Last
    {
        get
        {
            EquipMaintainTaskDetail ret = new EquipMaintainTaskDetail();
            EquipMaintainTaskDetail[] detailArr = MaintainTask.TaskDetails;
            for (int i = 0; i < detailArr.Length; i++)
            {
                if (detailArr[i].ID == this.ID && i > 0)
                {
                    ret = detailArr[i - 1];
                    break;
                }
            }
            return ret;
        }
    }
    

    public static EquipMaintainTaskDetail GetMaintainTaskDetail(int detailSubId)
    {
        EquipMaintainTaskDetail detail = new EquipMaintainTaskDetail();
        DataTable dt = DBHelper.GetDataTable(" select * from maintain_task_detail_sub "
            + " left join maintain_task_detail on maintain_task_detail_sub.detail_id = maintain_task_detail.[id] "
            + " where  maintain_task_detail_sub.[id] = " + detailSubId.ToString());
        if (dt.Rows.Count > 0)
        {
            detail._fields = dt.Rows[0];
        }
        return detail;
    }

    
}