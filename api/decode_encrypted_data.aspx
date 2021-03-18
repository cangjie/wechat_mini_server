<%@ Page Language="C#" %>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionKey = Util.GetSafeRequestValue(Request, "sessionkey", "hKC5nig2gEKJjktmponkbA==");	        
        string iv = Util.GetSafeRequestValue(Request, "iv", "sBEr7Qc1tg4vVSJAP22lVg==");	       
        string encData = Util.GetSafeRequestValue(Request, "encdata", "S0xqfeuSNT/2G/onUyg7HdAFLkO0LyYUlX0KWguQi3nv7PCPJ/6+O4CqTnOzYesFfl57oizInsviE6ootV4EqOBWoO28BuDuJFpdvmRE6RsO+Knec0ovLMLE+p2uBJvGvRgYTERJzOYGbG1BQkq980ctwSx0A2Fp1tM5KFJid/+wK7egZqIlOi5vDoSha0llshtH+zDQ1y5UBUbQFYUf8w==");	       
        Response.Write(Util.AES_decrypt(encData, sessionKey, iv));
    }
</script>