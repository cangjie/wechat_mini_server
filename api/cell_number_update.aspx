<%@ Page Language="C#" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "GGJfvofI3+BPhgCuO80zTQ==");
        string iv = Util.GetSafeRequestValue(Request, "iv", "Q3yCCyq58SwmyROhuvUuVA==");
        string encData = Util.GetSafeRequestValue(Request, "encdata", "Uzx6PzgPEdwfqvVplMO5nv/wn8wirWUJjbo6K1gGHI0+a2QLUKeypfPNCzjxfMn4t2hXUwiMj57rv/q1+6QjvHN4HwDUJzE5RpPNltZfDI1fH0kXHE8RvNpgr61CmbbZhv4GaQ2VTxEFdgUnhoIllXpjfoBoXekfRlnV3/edOCJUVQ9zcsZqUaGAcojfNCrLSiMhwbIQG2xdvQ8ncPN7aA==");
        string json = Util.AES_decrypt(encData, sessionKey, iv);
        string cell = Util.GetSimpleJsonValueByKey(json, "phoneNumber");
        string openId = MiniUsers.CheckSessionKey(sessionKey);
        if (openId.Trim().Equals(""))
        {
            Response.Write("{\"status\": 1, \"error_message\": \"session key invalid.\" }");
            Response.End();
        }
        if (cell.Trim().Equals(""))
        { 
            Response.Write("{\"status\": 1, \"error_message\": \"cell number invalid.\" }");
            Response.End();
        }
        DBHelper.UpdateData("mini_users", new string[,] { { "cell_number", "varchar", cell.Trim() } },
            new string[,] { { "open_id", "varchar", openId.Trim() } }, Util.conStr.Trim());
        DBHelper.InsertData("mini_user_cell_number_used",
            new string[,] { { "open_id", "varchar", openId.Trim() }, { "cell_number", "varchar", cell.Trim() } });
        Response.Write(json);
    }
</script>