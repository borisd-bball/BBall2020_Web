<%@ Page Title="" Language="C#" MasterPageFile="Master.master" AutoEventWireup="true" CodeFile="SubType.aspx.cs" Inherits="_SubType" MaintainScrollPositionOnPostback="true" %>
<%@ MasterType TypeName="Master" %>
<%@ Import Namespace="mr.bBall_Lib" %>
<asp:Content ID="cph_body" ContentPlaceHolderID="cph_body" Runat="Server">
<form id="form" runat="server">
<h2>Tip zavezanca<a href="SubTypes.aspx" class="button big right" style="margin:0;">Nazaj</a><%if(!String.IsNullOrEmpty(_id)){%><a href="SubType.aspx" class="button colorfull big right" style="margin-right:10px;">Nov tip zavezanca</a><%}%></h2>
<div class="actionbox top" style="margin-left:0;margin-top:10px;padding:20px 20px 10px 20px;">
    <p style="display:inline-block">
        Prikazani so vsi podatki tipa zavezanca. V izbranem polju lahko podatek popravite in ga shranite s klikom na gumb "Shrani".
    </p>
</div>
<fieldset>
    <legend>Podatki tipa zavezanca: <%=naziv.Text%><span class="req_label"><input /> - obvezna polja</span></legend>
    <div class="form-horizontal">
        <div class="control-group">
            <label for="naziv" class="control-label">Naziv</label>
            <div class="controls">
                <asp:TextBox ID="naziv" class="req" style="width:476px;margin-right:138px;" runat="server"></asp:TextBox>
                <span style="width:20px;">ID</span>
                <asp:TextBox ID="TypeID" class="req" style="width:50px;margin-right:10px;" ReadOnly="false" MaxLength="1" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="control-group">
            <label for="drzava" class="control-label">Država</label>
            <div class="controls">
                <asp:DropDownList ID="drzava" style="width:250px;margin-right:55px;" class="req" runat="server"></asp:DropDownList>
            </div>
        </div>
        <div class="form-actions">
            <asp:Button ID="shrani" runat="server" CssClass="button big colorfull" Text="Shrani" OnClick="shrani_Click"></asp:Button>
            <a href="SubTypes.aspx" class="button">Nazaj</a>
            <asp:Button ID="brisi" runat="server" CssClass="button big redfull right" Text="Briši" style="margin-right:92px;" Visible="false" OnClick="brisi_Click"></asp:Button>
        </div>
    </div>
</fieldset>
<br /><br />

</form>
<script>
    $(function () {

        <%if(!string.IsNullOrWhiteSpace(_persistence)){%>
        setTimeout(function () {
            try {
                var arr = eval(<%=_persistence%>);
                for (var i = 0; i < arr.length; i++) $("#" + arr[i].id).val(arr[i].value);
            } catch (e) { }
            setTimeout(function () {
                filter();
            }, 1);
        }, 1);
        <%}else{%>
        filter();
        <%}%>
    });
    function filter() {
        var val_leto = $("#sel_leto").val();
        var trs = $(val_leto);
        $(".all").hide();
        trs.show();
        $("#stetje").html(trs.length);

        try {
            var arr = new Array();
            var objs = $(".actionbox input,.actionbox select");
            for (var i = 0; i < objs.length; i++) arr.push({ "id": objs[i].id, "value": $(objs[i]).val() });
            $.ajax({
                url: "Ac.aspx?t=persistence&key=persistence_subtype&s=" + JSON.stringify(arr),
                cache: false,
                async: true
            });
        } catch (e) { }
    }
</script>
</asp:Content>
