<%@ Page Language="C#" %>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string encData = Util.GetSafeRequestValue(Request, "enc", 
            "KalWr9OK5n0BYQ5C7WARr5sb+6F6O2cFtKLhB0Xcs34LeTI6RFjVC8XrI9LGMLpLTEe9exkTt7OYvewVRuzy5zPwlPk4t/9cdj3qDHqLYct/wnyMV6y4dgdjwwq4H5xDuKzWNyYkE4mfshUB4/vaOFhC8oVS+1mHb+0kDfT59g7sOKQ/5xFcRc4TmY1gOizugUk8LTr9MJWx7tD0lmnjvFbk1Oq2iVvXD9fR9OW0tZoMY1ZB67S2Fw5gipcuxGC7pBlfxt5iHs22IuJD8La0xgN6+jbCuseMsu9cfrMavuKZ8qH+PCC4dHtQMzKVrAMWuimYN6SJUTvBr8KoZHcRKrVMn6BW102PLgjI6xygTH3j9YZZxW9pcYag0HpOzNJ232PDrasAfkh3bV+R0lDPI7No3d+SeE4UocQTsR64dy0se2NJtdyRH3Yot7Qk4Q4SCk0uBCMjj6NzT36wDF7FuyPr88Y/oEE5ssUbJULXKU0=");
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "KWbv8xKT77P1hntLvs1iNg==");
        string iv = Util.GetSafeRequestValue(Request, "iv", "L/XXtaQpadBPpGWFujIosw==");
        Response.Write(Util.AES_decrypt(encData, sessionKey.Trim(), iv));
    }
</script>