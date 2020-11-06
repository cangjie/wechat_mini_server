using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

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

    public bool SetStatus(string status)
    {
        return false;
    }

    public EquipMaintainTask MaintainTask
    {
        get
        {
            return new EquipMaintainTask(int.Parse(_fields["task_id"].ToString().Trim()));
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