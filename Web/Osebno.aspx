<%@ Page Title="" Language="C#" MasterPageFile="Master.master" AutoEventWireup="true" CodeFile="Osebno.aspx.cs" Inherits="_Osebno" MaintainScrollPositionOnPostback="true" %>
<%@ MasterType TypeName="Master" %>
<%@ Import Namespace="mr.bBall_Lib" %>
<asp:Content ID="cph_body" ContentPlaceHolderID="cph_body" Runat="Server">
<form id="form" runat="server">
<h2><%=Master.Uporabnik.Ime%> <%=Master.Uporabnik.Priimek%></h2>
<div class="actionbox top" style="margin-left:0;margin-top:10px;padding:20px 20px 10px 20px;">
    <p style="display:inline-block">
        Prikazani so vsi vaši podatki. V izbranem polju lahko podatek popravite in ga shranite s klikom na gumb "Shrani".
    </p>
</div>
<fieldset>
    <legend>Uporabniško ime <%=Master.Uporabnik.Username%><span class="req_label"><input /> - obvezna polja</span></legend>
    <div class="form-horizontal">
        <div class="control-group">
            <label for="ime" class="control-label">Ime</label>
            <div class="controls">
                <asp:TextBox ID="ime" style="width:298px;margin-right:30px;" runat="server"></asp:TextBox>
                <span>Priimek</span>
                <asp:TextBox ID="priimek" style="width:298px;" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="control-group">
            <label for="email" class="control-label">Elektronska pošta</label>
            <div class="controls">
                <asp:TextBox ID="email" style="width:710px;" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="control-group">
            <label for="password" class="control-label pt" onclick="$('.geslo').toggle()"><b>Zamenjava gesla</b></label>
        </div>
        <div class="control-group geslo" style="display:none;">
            <label for="password" class="control-label">Staro geslo</label>
            <div class="controls">
                <asp:TextBox ID="password" TextMode="Password" style="width:298px;" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="control-group geslo" style="display:none;">
            <label for="password" class="control-label">Novo geslo</label>
            <div class="controls">
                <asp:TextBox ID="password_new1" TextMode="Password" style="width:298px;" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="control-group geslo" style="display:none;">
            <label for="password" class="control-label">Novo geslo (ponovi)</label>
            <div class="controls">
                <asp:TextBox ID="password_new2" TextMode="Password" style="width:298px;" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="form-actions">
            <asp:Button ID="shrani" runat="server" CssClass="button big colorfull" Text="Shrani" OnClick="shrani_Click"></asp:Button>
        </div>
    </div>
</fieldset> 
</form>
</asp:Content>
