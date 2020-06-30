<%@ Page Language="C#" MasterPageFile="Master.master" AutoEventWireup="true" CodeFile="Devices.aspx.cs" Inherits="_Devices" ValidateRequest="false"%>
<%@ MasterType TypeName="Master" %>
<%@ Import Namespace="mr.bBall_Lib" %>

<asp:Content ID="cph_body" ContentPlaceHolderID="cph_body" Runat="Server">
<form id="form" runat="server">
<h2>Naprave</h2>
<div class="actionbox top" style="margin-left:0;margin-top:10px">
    <div style="display:inline-block;">
        <p>Iskanje:</p>
        <input id="inp_iskanje" style="width:200px" onchange="filter()" onkeyup="filter()" />
    </div>
    <div style="float:right;display:inline-block;margin-left:10px;<%=r_naprave.Items.Count==0?"display:none;":""%>">
        <p>&nbsp;</p>
        <a href="?a=csv" class="button big colorfull" target="_blank" style="float:right;margin:0;position:relative;bottom:3px;">Izvozi</a>
        <a href="?" class="button big colorfull" style="float:right;margin:2;position:relative;bottom:3px;">Osveži</a>
    </div>
</div>
<table class="table noalt">
<tr>
    <th colspan="1">Št. zapisov: <span id="stetje"></span></th>
    <th colspan="7">
        <a href="Device.aspx" class="button colorfull right" style="margin:0;">Nova naprava</a>
    </th>
</tr>
<asp:Repeater ID="r_naprave" runat="server">
<HeaderTemplate>
<tr>
    <th style="width:20%;"><%=Splosno.Sortiranje("acTitle asc", _sort, "Ime")%></th>
    <th style="width:10%;"><%=Splosno.Sortiranje("acDevID asc", _sort, "ID aplikacije")%></th>
    <th style="width:10%;"><%=Splosno.Sortiranje("acBT_Name asc", _sort, "BT naprave")%></th>
    <th style="width:10%;"><%=Splosno.Sortiranje("acEmail asc", _sort, "E-mail uporabnika")%></th>
    <th style="width:15%;"><%=Splosno.Sortiranje("adInsetDate asc", _sort, "Datum vnosa")%></th>
    <th style="width:10%;"><%=Splosno.Sortiranje("adModificationDate asc", _sort, "Datum spremembe")%></th>
    <th></th>
    <th></th>
</tr>
</HeaderTemplate>
<ItemTemplate>
<tr class="pt all" <%--onclick="window.location='DeviceTasks.aspx?id_s=<%#Eval("acDevID")%>'"--%>>
    <td><b><%#Eval("acTitle")%></b></td>
    <td><b><%#Eval("acDevID")%></b></td>
    <td><b><%#Eval("acBT_Name")%></b></td>
    <td><b><%#Eval("acEmail")%></b></td>
    <td><b><%#Eval("adInsetDate")%></b></td>
    <td><b><%#Eval("adModificationDate")%></b></td>
    <td><%--<a href="Device.aspx?id=<%#Eval("ID")%>" class="button small">Uredi</a>--%>
    </td>
    <td><%--<a href="DeviceData.aspx?id_s=<%#Eval("acDevID")%>" class="button small greenfull">Meritve</a>--%>
    </td>
    <%--<td><a href="?id=<%#Eval("ID")%>&a=batch" class="button greenfull">Nove meritve</a></td>--%>
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
                url: "Ac.aspx?t=persistence&key=persistence_naprave&s=" + JSON.stringify(arr),
                cache: false,
                async: true
            });
        } catch (e) { }
    }
</script>
</form>
</asp:Content>
