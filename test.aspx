<%@ Page Language="C#" %>

<!DOCTYPE html>

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {

    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <%=Util.AES_decrypt("JKzM3vPCbN/59VxFh7AuavYGaw7m9TWdZmpva72JLur2pMQWoEwzoVsLxEftqj+n0FhtTC29JRALjLSiLvg7TVty81tIP1yAIo2m3s5+EAw2EyS2mD1r6KnuaVKOg5qoDLtNLmuIp2s4RwAK0w/iMGjzkWSTgWAJBpxum0w5fSb5UgLfii+TxdZD0oTy4kNm8wAKHH9pdvQnixMkFptORQ==",
                @"4dBT7BPHbI325yUN\/VB+Pw==", "55jnuDJ/br5lzxO/T8YFgg==")%>
        </div>
    </form>
</body>
</html>
