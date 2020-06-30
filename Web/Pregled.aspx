<%@ Page Title="" Language="C#" MasterPageFile="Master.master" AutoEventWireup="true" CodeFile="Pregled.aspx.cs" Inherits="_Pregled" %>
<%@ MasterType TypeName="Master" %>
<%@ Import Namespace="mr.bBall_Lib" %>
<asp:Content ID="cph_body" ContentPlaceHolderID="cph_body" Runat="Server">
<form id="form" runat="server">
<h1 style="margin-left:20px;">Pregled</h1>
<%if(!string.IsNullOrWhiteSpace(_racuni)){%>
<div class="grid-4 box" style="height:360px;margin-top:20px;">
    <div>
        <h2 class="pt" style="display:inline-block;" onclick="window.location='Racuni.aspx'">Računi</h2><a href="Racun.aspx" class="button pt" style="float:right;margin-right:5px;font-weight:bold;">+</a><br />
        <div class="wrap"><table class="table noalt" style="margin-top:0;"><%=_racuni%></table></div>
    </div>
</div>
<%}%>
<%if(!string.IsNullOrWhiteSpace(_blagajne)){%>
<div class="grid-4 box" style="height:360px;margin-top:20px;">
    <div>
        <h2 class="pt" style="display:inline-block;" onclick="window.location='Blagajne.aspx'">Blagajne</h2><a href="Blagajna.aspx" class="button pt" style="float:right;margin-right:5px;font-weight:bold;">+</a><br />
        <%--<div class="wrap"><table class="table noalt" style="margin-top:0;"><%=_blagajne%></table></div>--%>
    </div>
</div>
<%}%>
<%if(!string.IsNullOrWhiteSpace(_prihodkiodhodki)){%>
<div class="grid-4 box" style="height:360px;margin-top:20px;">
    <div>
        <h2 class="pt" style="display:inline-block;" onclick="window.location='PrihodkiOdhodki.aspx'">Prihodki in odhodki</h2><a href="PrihodekOdhodek.aspx" class="button pt" style="float:right;margin-right:5px;font-weight:bold;">+</a><br />
        <%--<div class="wrap"><table class="table noalt" style="margin-top:0;"><%=_prihodkiodhodki%></table></div>--%>
    </div>
</div>
<%}%>
<%if(!string.IsNullOrWhiteSpace(_clani)){%>
<div class="grid-4 box" style="height:360px;margin-top:20px;">
    <div>
        <h2 class="pt" style="display:inline-block;" onclick="window.location='Clani.aspx'">Člani</h2><a href="Clan.aspx" class="button pt" style="float:right;margin-right:5px;font-weight:bold;">+</a><br />
        <%--<div class="wrap"><table class="table noalt" style="margin-top:0;"><%=_clani%></table></div>--%>
    </div>
</div>
<%}%>
<%if(!string.IsNullOrWhiteSpace(_Skulpture)){%>
<div class="grid-4 box" style="height:360px;margin-top:20px;">
    <div>
<%--        <h2 class="pt" style="display:inline-block;" onclick="window.location='Clani.aspx'">Skulpture</h2><a href="Clan.aspx" class="button pt" style="float:right;margin-right:5px;font-weight:bold;">+</a><br />--%>
        <h2 class="pt" style="display:inline-block;" onclick="window.location='Skulpture.aspx'">Skulpture</h2><a href="" class="button pt" style="float:right;margin-right:5px;font-weight:bold;">+</a><br />
        <%--<div class="wrap"><table class="table noalt" style="margin-top:0;"><%=_Skulpture%></table></div>--%>
    </div>
</div>
<%}%>
</form>
<script type="text/javascript">
    $(function () { $(".pregled").css("margin-bottom", $(window).height() - $(".pregled").height() - 243); });
</script>
</asp:Content>
