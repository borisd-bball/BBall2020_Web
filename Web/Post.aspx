<%@ Page Title="" Language="C#" MasterPageFile="Master.master" AutoEventWireup="true" CodeFile="Post.aspx.cs" Inherits="_Post" MaintainScrollPositionOnPostback="true" %>
<%@ MasterType TypeName="Master" %>
<%@ Import Namespace="mr.bBall_Lib" %>
<asp:Content ID="cph_body" ContentPlaceHolderID="cph_body" Runat="Server">
<form id="form" runat="server">
<h2>Pošta<a href="Posts.aspx" class="button big right" style="margin:0;">Nazaj</a><%if(_id>0){%><a href="Post.aspx" class="button colorfull big right" style="margin-right:10px;">Nova pošta</a><%}%></h2>
<div class="actionbox top" style="margin-left:0;margin-top:10px;padding:20px 20px 10px 20px;">
    <p style="display:inline-block">
        Prikazani so vsi podatki pošte. V izbranem polju lahko podatek popravite in ga shranite s klikom na gumb "Shrani".
    </p>
</div>
<fieldset>
    <legend>Podatki pošte: <%=naziv.Text%><span class="req_label"><input /> - obvezna polja</span></legend>
    <div class="form-horizontal">
        <div class="control-group">
            <label for="naziv" class="control-label">Naziv</label>
            <div class="controls">
                <asp:TextBox ID="naziv" class="req" style="width:476px;margin-right:138px;" runat="server"></asp:TextBox>
                <span style="width:20px;">ID</span>
                <asp:TextBox ID="postCode" style="width:50px;margin-right:10px;" ReadOnly="true" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="control-group">
            <label for="drzava" class="control-label">Država</label>
            <div class="controls">
                <asp:DropDownList ID="drzava" style="width:250px;margin-right:55px;" class="req" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drzava_SelectedIndexChanged"></asp:DropDownList>
                <span style="width:80px;">ISO koda</span>
                <asp:TextBox ID="iso_code" style="width:50px;margin-right:72px;" ReadOnly="true" class="req" runat="server"></asp:TextBox>
                <span style="width:50px;">Koda</span>
                <asp:TextBox ID="code" style="width:120px;margin-right:30px;" class="req" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="form-actions">
            <asp:Button ID="shrani" runat="server" CssClass="button big colorfull" Text="Shrani" OnClick="shrani_Click"></asp:Button>
            <a href="Posts.aspx" class="button">Nazaj</a>
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
                url: "Ac.aspx?t=persistence&key=persistence_post&s=" + JSON.stringify(arr),
                cache: false,
                async: true
            });
        } catch (e) { }
    }
</script>
</asp:Content>
