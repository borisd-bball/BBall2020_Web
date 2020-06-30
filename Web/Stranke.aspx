<%@ Page Title="" Language="C#" MasterPageFile="Master.master" AutoEventWireup="true" CodeFile="Stranke.aspx.cs" Inherits="_Stranke" ValidateRequest="false" %>
<%@ MasterType TypeName="Master" %>
<%@ Import Namespace="mr.bBall_Lib" %>
<asp:Content ID="cph_body" ContentPlaceHolderID="cph_body" Runat="Server">
<form id="form" runat="server">
<h2>Stranke<a onclick="showSpinner()" href="Ostalo.aspx" class="button big right" style="margin:0;">Nazaj</a></h2>
<div class="actionbox top" style="margin-left:0;margin-top:10px">
    <div style="display:inline-block;">
        <p>Iskanje:</p>
        <input id="inp_iskanje" style="width:200px" onchange="filter()" onkeyup="filter()" />
    </div>
    <div style="float:right;display:inline-block;margin-left:10px;<%=r_stranke.Items.Count==0?"display:none;":""%>">
        <p>&nbsp;</p>
        <a href="?a=csv" class="button big colorfull" target="_blank" style="float:right;margin:0;position:relative;bottom:3px;">Izvozi</a>
    </div>
    <div style="float:right;display:inline-block;margin-left:10px;<%=r_stranke.Items.Count==0?"display:none;":""%>">
        <p>&nbsp;</p>
        <a onclick="showSpinner()" href="Stranke.aspx" class="button big colorfull" style="float:right;margin:0;position:relative;bottom:3px;">Osveži</a>
    </div>
</div>
<table class="table noalt">
<tr>
    <th colspan="3">Št. zapisov: <span id="stetje"></span></th>
    <th colspan="3">
        <a href="Stranka.aspx" class="button colorfull right" style="margin:0;">Nova stranka</a>
    </th>
</tr>
<asp:Repeater ID="r_stranke" runat="server">
<HeaderTemplate>
<tr>
    <th style="width:250px;"><%=Splosno.Sortiranje("acShortTitle asc", _sort, "Naziv")%></th>
    <th style="width:100px;"><%=Splosno.Sortiranje("anCustomerID asc", _sort, "ID")%></th>
    <th>Naslov</th>
    <th style="width:200px;"><%=Splosno.Sortiranje("acPostTitle asc", _sort, "Pošta")%></th>
    <th style="width:200px;"><%=Splosno.Sortiranje("aceMaile asc", _sort, "Email")%></th>
    <th style="width:50px;text-align:right;">Kontakt</th>
</tr>
</HeaderTemplate>
<ItemTemplate>
<tr class="pt all" onclick="window.location='Stranka.aspx?id=<%#Eval("anCustomerID")%>'">
    <td><b><%#Eval("acShortTitle")%></b></td>
    <td><%#Eval("anCustomerID")%></td>
    <td><%#Eval("acAddress")%></td>
    <td><%#Eval("acPostTitle")%></td>
    <td><%#Eval("aceMaile")%></td>
    <td style="text-align:right;"><%#Eval("acContactName")%></td>
</tr>
</ItemTemplate>
</asp:Repeater>
</table>
<script type="text/javascript">
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
        var val_iskanje = $("#inp_iskanje").val().toLowerCase();
        var trs = $(".all");
        if (val_iskanje != "") {
            trs = trs.filter(function (index) {
                return $(trs[index]).text().toLowerCase().indexOf(val_iskanje) > -1;
            });
        }
        $(".all").hide();
        trs.show();
        $("#stetje").html(trs.length);

        try {
            var arr = new Array();
            var objs = $(".actionbox input,.actionbox select");
            for (var i = 0; i < objs.length; i++) arr.push({ "id": objs[i].id, "value": $(objs[i]).val() });
            $.ajax({
                url: "Ac.aspx?t=persistence&key=persistence_stranke&s=" + JSON.stringify(arr),
                cache: false,
                async: true
            });
        } catch (e) { }
    }
</script>
</form>
</asp:Content>
