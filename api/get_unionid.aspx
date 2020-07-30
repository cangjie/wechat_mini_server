<%@ Page Language="C#" %>


<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string encData = Util.GetSafeRequestValue(Request, "enc", 
            "toc2HVL+I5Uh8psdLz2CUOpgNBzYQIf2jVRCwGPw+q738QNazMh2UpKSPLTi/z0zyEEXk566TqeHn0Bl1LCW5cbOcqzT2Oyo5V3/cZeHfJrqar1Jn2ys6Ctuz/P7X5JtdWR1fg/3Z4IyqFtML94RJc/cB2eEnQIW4FjAwk9X7WBuaK+tpPRVvkeSrWWtXdaFoKmT4A+bCUW20FlxeYbQAw==");
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "");
        string iv = Util.GetSafeRequestValue(Request, "iv", "");
        Response.Write(Util.AES_decrypt(encData, sessionKey.Trim(), iv));
    }
</script>