﻿<%@ Page Title="" Language="C#" MasterPageFile="Master.master" AutoEventWireup="true" CodeFile="SubTypes.aspx.cs" Inherits="_SubTypes" ValidateRequest="false" %>
<%@ MasterType TypeName="Master" %>
<%@ Import Namespace="mr.bBall_Lib" %>
<asp:Content ID="cph_body" ContentPlaceHolderID="cph_body" Runat="Server">
<form id="form" runat="server">
<h2>Tipi zavezanca<a onclick="showSpinner()" href="Ostalo.aspx" class="button big right" style="margin:0;">Nazaj</a></h2>
<div class="actionbox top" style="margin-left:0;margin-top:10px">
    <div style="display:inline-block;">
        <p>Iskanje:</p>
        <input id="inp_iskanje" style="width:200px" onchange="filter()" onkeyup="filter()" />
    </div>
    <div style="float:right;display:inline-block;margin-left:10px;<%=r_subtypes.Items.Count==0?"display:none;":""%>">
        <p>&nbsp;</p>
        <a href="?a=csv" class="button big colorfull" target="_blank" style="float:right;margin:0;position:relative;bottom:3px;">Izvozi</a>
    </div>
    <div style="float:right;display:inline-block;margin-left:10px;<%=r_subtypes.Items.Count==0?"display:none;":""%>">
        <p>&nbsp;</p>
        <a onclick="showSpinner()" href="SubTypes.aspx" class="button big colorfull" style="float:right;margin:0;position:relative;bottom:3px;">Osveži</a>
    </div>
</div>
<table class="table noalt">
<tr>
    <th colspan="1">Št. zapisov: <span id="stetje"></span></th>
    <th colspan="2">
        <a href="SubType.aspx" class="button colorfull right" style="margin:0;">Nova tip zavezanca</a>
    </th>
</tr>
<asp:Repeater ID="r_subtypes" runat="server">
<HeaderTemplate>
<tr>
    <th style="width:60px;"><%=Splosno.Sortiranje("acTypeID asc", _sort, "ID")%></th>
    <th style="width:200px;"><%=Splosno.Sortiranje("acTitle asc", _sort, "Naziv")%></th> 
    <th style="width:100px;"><%=Splosno.Sortiranje("CountryTitle asc", _sort, "Država")%></th>
</tr>
</HeaderTemplate>
<ItemTemplate>
<tr class="pt all" onclick="window.location='SubType.aspx?id=<%#Eval("acTypeID")%>'">
    <td><b><%#Eval("acTypeID")%></b></td>
    <td><%#Eval("acTitle")%></td>
    <td><%#Eval("CountryTitle")%></td>
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
                url: "Ac.aspx?t=persistence&key=persistence_subtypes&s=" + JSON.stringify(arr),
                cache: false,
                async: true
            });
        } catch (e) { }
    }
</script>
</form>
</asp:Content>
