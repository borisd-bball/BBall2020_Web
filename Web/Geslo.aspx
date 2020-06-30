<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Geslo.aspx.cs" Inherits="_Geslo" %>
<!DOCTYPE html>
<!--[if lt IE 7]> <html class="no-js lt-ie9 lt-ie8 lt-ie7" lang="en"> <![endif]-->
<!--[if IE 7]>    <html class="no-js lt-ie9 lt-ie8" lang="en"> <![endif]-->
<!--[if IE 8]>    <html class="no-js lt-ie9" lang="en"> <![endif]-->
<!--[if gt IE 8]><!-->
<html lang="en">
<!--<![endif]-->
<head>
    <title><%=mr.bBall_Lib.Nastavitve.Naziv%></title>
    <link rel="shortcut icon" href="Img/icon.png?<%=DateTime.Now.Ticks%>" />
    <link rel="icon" type="image/png" href="Img/icon.png?<%=DateTime.Now.Ticks%>" />
    <!--[if lt IE 9]><link href="Css/ie.css" media="screen" rel="stylesheet" type="text/css" /><script src="Js/ie.js" type="text/javascript"></script><![endif]-->
    <link href="Css/all.css?<%=DateTime.Now.Ticks%>" type="text/css" rel="stylesheet" />
    <link href="Css/jquery-ui.min.css?<%=DateTime.Now.Ticks%>" type="text/css" rel="stylesheet" />
    <script type="text/javascript" src="Js/jquery.min.js?<%=DateTime.Now.Ticks%>"></script>
    <script type="text/javascript" src="Js/jquery-ui.min.js?<%=DateTime.Now.Ticks%>"></script>
    <script type="text/javascript" src="Js/jquery.watermark.min.js?<%=DateTime.Now.Ticks%>"></script>
    <script type="text/javascript" src="Js/all.js?<%=DateTime.Now.Ticks%>"></script>
</head>
<body class="landing zap">
<form id="form" runat="server">
<section class="container" style="height:100vh;width:100vw;padding:0;margin:0;position:fixed;top:0;">
    <div id="content">
        <div class="boxes">
            <div class="row">
                <div class="centered login" style="background-color:#FFF;border:solid 1px #EAEAEA;">
                    <table>
                        <tr>
                            <td colspan="2">
                                <div id="dpomoc"><a onclick="pomoc('Geslo')">Pomoč</a></div>
                                <img src="Img/login<%=string.IsNullOrWhiteSpace(_error) ? "" : "x"%>.png" style="margin-top:20px;" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" class="napaka">
                                <h2>Zamenjava gesla</h2>
                                <%=_error%>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:TextBox ID="username" Enabled="false" placeholder="Uporabniško ime" AutoCompleteType="None" runat="server"></asp:TextBox><br />
                                <asp:TextBox ID="password_new1" placeholder="Novo geslo" AutoCompleteType="None" TextMode="Password" runat="server"></asp:TextBox><br />
                                <asp:TextBox ID="password_new2" placeholder="Novo geslo (ponovi)" AutoCompleteType="None" TextMode="Password" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2"><asp:Button ID="zamenjaj" runat="server" CssClass="button big colorfull" Text="Zamenjaj"></asp:Button></td>
                        </tr>
                    </table>
                    <div id="dverzija">Ver: <%=mr.bBall_Lib.Splosno.Verzija%> <%=ConfigurationManager.AppSettings["Skupina"]%></div> 
                    <div id="dspock"><a href="http://mr-avtomatika.com" target="_blank"><img src="Img/spock.png" /> mr-avtomatika.com</a></div>
                    <div id="dialog" class="login" style="display:none;"><center><img src="Img/x.png" /><br /><br /><h3></h3></center><br /><div style="text-align:justify;"></div></div>
                </div>    
            </div>
        </div>
    </div>
</section>
<script type="text/javascript">
    $(function () {
        $("#password_new1").watermark("Novo geslo");
        $("#password_new2").watermark("Novo geslo (ponovi)");
        <%=_js%>
    });
</script>
</form>
</body>
</html>
