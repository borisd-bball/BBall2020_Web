<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
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
                                <!--<div id="dpomoc"><a onclick="pomoc('Default')">Pomoč</a></div>-->
                                <img src="Img/login<%=string.IsNullOrWhiteSpace(_error) ? "" : "x"%>.png" style="margin-top:20px;" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" class="napaka">
                                <h2>Prijava</h2>
                                <%=_error%>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:TextBox ID="username" placeholder="Uporabniško ime" AutoCompleteType="None" runat="server"></asp:TextBox><br />
                                <asp:TextBox ID="password" placeholder="Geslo" AutoCompleteType="None" TextMode="Password" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr class="small">
                            <td style="text-align:left;padding-left:30px;"><asp:CheckBox ID="remember" runat="server" Checked="false" />Zapomni si prijavo</td>
                            <td style="text-align:right;padding-right:30px;"><a href="#" onclick="pozabljeno()">Pozabljeno geslo</a></td>
                        </tr>
                        <tr>
                            <td colspan="2"><asp:Button ID="prijava" OnClientClick="showSpinner()" runat="server" CssClass="button big colorfull" Text="Prijava"></asp:Button></td>
                        </tr>
                    </table>
                    <!--<div id="dverzija">Ver: <%=mr.bBall_Lib.Splosno.Verzija%> <%=ConfigurationManager.AppSettings["Skupina"]%><%if (!mr.bBall_Lib.Splosno.Produkcija){%><span onclick="dialog('Demo način','Trenutno aplikacija deluje v demonstracijskem načinu. To pomeni, da se za potrjevanje gotovinskih računov uporablja testno razvijalsko potrdilo in testna storitev FURS.','i')">TEST</span><%}else{%> | <a href="http://mr-avtomatika.com/<%=ConfigurationManager.AppSettings["Skupina"]%>test" target="_blank">povezava na test</a><%}%></div> -->
                    <!--<div id="dspock"><a href="http://mr-avtomatika.com" target="_blank"><img src="Img/mr-avtomatika.png" /></a></div>-->
                    <div id="dialog" class="login" style="display:none;"><center><img src="Img/x.png" /><br /><br /><h3></h3></center><br /><div style="text-align:justify;"></div></div>
                </div>    
            </div>
        </div>
    </div>
</section>
<script type="text/javascript">
$(function () {
    $("#username").watermark("Uporabniško ime");
    $("#password").watermark("Geslo");
});
function pozabljeno() {
    dialog("Pozabljeno geslo", "<input type='text' style='text-align:center;' placeholder='Uporabniško ime' /><br /><br />Vnesite uporabniško ime in kliknite gumb Pozabljeno geslo. Na vaš elektronski naslov vam bomo poslali povezavo, kjer boste lahko določili novo geslo. Če ste pozabili tudi svoje uporabniško ime, se obrnite po pomoč na elektronski naslov <a href='mailto:info@bball.si'>info@bball.si</a>.", "e");
    $("#dialog").find("input").val($("#username").val());
    $("#dialog").dialog("option", "buttons", {
        "Pozabljeno geslo": function () {
            $.ajax({
                url: "Ac.aspx?t=geslo&s=" + encodeURIComponent($("#dialog").find("input").val()) + "&" + new Date().getDate(),
                dataType: "json",
                cache: false,
                type: "get",
                success: function (data) {
                    $("#dialog").dialog("close");
                    dialog(data.title, data.message, data.type);
                    if (data.type == "e") {
                        $("#dialog").dialog("option", "buttons", {
                            Ok: function () {
                                $(this).dialog("close");
                                pozabljeno();
                            }
                        });
                    }
                }
            });
        }
    });
}
</script>
</form>
</body>
</html>
