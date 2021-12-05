<%@ Page Language="C#" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "hKC5nig2gEKJjktmponkbA==");
        string headImage = Util.GetSafeRequestValue(Request, "headimage", "http://");
        string nick = Util.GetSafeRequestValue(Request, "nick", "sssd");
        string gender = Util.GetSafeRequestValue(Request, "gender", "男");
        string openId = MiniUsers.CheckSessionKey(sessionKey);
        if (nick.Trim().Equals("") || nick.Trim().Equals("微信用户"))
        {
            DBHelper.UpdateData("mini_users", new string[,] {  { "head_image", "varchar", headImage.Trim() }, { "gender", "varchar", gender.Trim() } },
                new string[,] { { "open_id", "varchar", openId.Trim() } }, Util.conStr.Trim());
        }
        else
        {
            DBHelper.UpdateData("mini_users", new string[,] { { "nick", "varchar", nick.Trim() }, { "head_image", "varchar", headImage.Trim() }, { "gender", "varchar", gender.Trim() } },
                new string[,] { { "open_id", "varchar", openId.Trim() } }, Util.conStr.Trim());
        }


    }
</script>