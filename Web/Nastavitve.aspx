<%@ Page Title="" Language="C#" MasterPageFile="Master.master" AutoEventWireup="true" CodeFile="Nastavitve.aspx.cs" Inherits="_Nastavitve" MaintainScrollPositionOnPostback="true" %>
<%@ MasterType TypeName="Master" %>
<%@ Import Namespace="mr.bBall_Lib" %>
<asp:Content ID="cph_body" ContentPlaceHolderID="cph_body" Runat="Server">
<form id="form" runat="server">
<h2>Nastavitve</h2>
<div class="actionbox top" style="margin-left:0;margin-top:10px;padding:20px 20px 10px 20px;">
    <p style="display:inline-block">
        Prikazani so vse nastavitve. V izbranem polju lahko podatek popravite in ga shranite s klikom na gumb "Shrani".
    </p>
</div>
<fieldset>
    <legend>Nastavitve za <%=naziv.Text%><span class="req_label"><input /> - obvezna polja</span></legend>
    <div class="form-horizontal">
        <div class="control-group">
            <label for="naziv" class="control-label">Naziv</label>
            <div class="controls">
                <asp:TextBox ID="naziv" class="req" style="width:298px;margin-right:30px;" runat="server"></asp:TextBox>
                <span>Dolg naziv</span>
                <asp:TextBox ID="naziv_dolg" class="req" style="width:298px;" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="control-group">
            <label for="naslov" class="control-label">Naslov</label>
            <div class="controls">
                <asp:TextBox ID="naslov" class="req" style="width:710px;" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="control-group">
            <label for="telefon" class="control-label">Telefon</label>
            <div class="controls">
                <asp:TextBox ID="telefon" style="width:298px;margin-right:30px;" runat="server"></asp:TextBox>
                <span>E-pošta</span>
                <asp:TextBox ID="email" style="width:298px;" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="control-group">
            <label for="telefon" class="control-label">Spletna stran</label>
            <div class="controls">
                <asp:TextBox ID="spletna_stran" style="width:298px;margin-right:30px;" runat="server"></asp:TextBox>
                <span>Davčna</span>
                <asp:TextBox ID="davcna" style="width:125px;margin-right:30px;" runat="server"></asp:TextBox>
                <asp:CheckBox ID="zavezanec" runat="server" /> Zavezanec za DDV
            </div>
        </div>
        <div class="control-group">
            <label for="trr" class="control-label">TRR</label>
            <div class="controls">
                <asp:TextBox ID="trr" style="width:298px;margin-right:30px;" runat="server"></asp:TextBox>
                <span>Banka</span>
                <asp:TextBox ID="banka" style="width:125px;margin-right:30px;" runat="server"></asp:TextBox>
                <asp:CheckBox ID="osvezevanje" runat="server" /> Osveževanje
            </div>
        </div>
        <div class="control-group rd">
            <label for="naslov" class="control-label">Obveščanje</label>
            <div class="controls">
                <asp:TextBox ID="obvescanje" style="width:710px;" runat="server"></asp:TextBox>
                <span class="wfdescription">Uporabniki, ki dobijo obvestila o dovolilnicah za vse revirje - vnesite gsm številke ločene z vejico</span>
            </div>
        </div>
        <div class="control-group rd">
            <label for="naslov" class="control-label">Povzetek</label>
            <div class="controls">
                <asp:TextBox ID="povzetek" style="width:710px;" runat="server"></asp:TextBox>
                <span class="wfdescription">Uporabniki, ki dobijo povzetek o dovolilnicah za vse revirje - vnesite gsm številke ločene z vejico</span>
            </div>
        </div>
        <div class="control-group rd">
            <label for="naslov" class="control-label">Promet</label>
            <div class="controls">
                <asp:TextBox ID="promet" style="width:710px;" runat="server"></asp:TextBox>
                <span class="wfdescription">Uporabniki, ki dobijo povzetek o prometu - vnesite gsm številke ločene z vejico</span>
            </div>
        </div>
        <div class="control-group rd">
            <label for="naslov" class="control-label">Kvote</label>
            <div class="controls">
                <asp:TextBox ID="kvote" style="width:710px;" runat="server"></asp:TextBox>
                <span class="wfdescription">Uporabniki, ki dobijo podatek o kodi za kvote - vnesite gsm številke ločene z vejico</span>
            </div>
        </div>
        <div class="control-group">
            <label for="opombe" class="control-label">Glava računa</label>
            <div class="controls">
                <asp:TextBox ID="izdajatelj" style="width:710px;margin-right:30px;" TextMode="MultiLine" Rows="8" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="control-group rd">
            <label for="klen_uporabnik" class="control-label">Klen uporabnik</label>
            <div class="controls">
                <asp:TextBox ID="klen_uporabnik" style="width:298px;margin-right:30px;" runat="server"></asp:TextBox>
                <span>Geslo</span>
                <asp:TextBox ID="klen_geslo" style="width:298px;" TextMode="Password" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="form-actions">
            <asp:Button ID="shrani" runat="server" CssClass="button big colorfull" Text="Shrani"></asp:Button>
            <a href="Ostalo.aspx" class="button">Nazaj</a>
        </div>
    </div>
</fieldset> 
</form>
<script>
$(function () {

});
</script>
</asp:Content>
