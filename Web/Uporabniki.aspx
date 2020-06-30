<%@ Page Title="" Language="C#" MasterPageFile="Master.master" AutoEventWireup="true" CodeFile="Uporabniki.aspx.cs" Inherits="_Uporabniki" ValidateRequest="false" %>
<%@ MasterType TypeName="Master" %>
<%@ Import Namespace="mr.bBall_Lib" %>
<asp:Content ID="cph_body" ContentPlaceHolderID="cph_body" Runat="Server">
<form id="form" runat="server">
<h2>Uporabniki</h2>
<div class="actionbox top" style="margin-left:0;margin-top:10px">
    <div style="display:inline-block;">
        <p>Iskanje:</p>
        <input id="inp_iskanje" style="width:200px" onchange="filter()" onkeyup="filter()" />
    </div>
    <div style="float:right;display:inline-block;margin-left:10px;<%=r_uporabniki.Items.Count==0?"display:none;":""%>">
        <p>&nbsp;</p>
        <a href="?a=csv" class="button big colorfull" target="_blank" style="float:right;margin:0;position:relative;bottom:3px;">Izvozi</a>
    </div>
</div>
<table class="table noalt">
<tr>
    <th colspan="4">Št. zapisov: <span id="stetje"></span></th>
    <th colspan="5">
        <a href="Uporabnik.aspx" class="button colorfull right" style="margin:0;">Nov uporabnik</a>
    </th>
</tr>
<asp:Repeater ID="r_uporabniki" runat="server">
<HeaderTemplate>
<tr>
    <th><%=Splosno.Sortiranje("acUserName asc", _sort, "Uporabniško&nbsp;ime")%></th>
    <th style="width:150px;"><%=Splosno.Sortiranje("acFirstName asc", _sort, "Ime")%></th>
    <th style="width:200px;"><%=Splosno.Sortiranje("acLastName asc", _sort, "Priimek")%></th>
    <th><%=Splosno.Sortiranje("acEmail asc", _sort, "Email")%></th>
    <th style="width:40px;text-align:center;"><%=Splosno.Sortiranje("anAdmin asc", _sort, "Administrator")%></th>
<%--    <th style="width:50px;text-align:center;">Davčna</th>
    <th style="width:40px;text-align:right;"><%=Splosno.Sortiranje("prodajalec desc", _sort, "Prodajalec")%></th>
    <th style="width:30px;text-align:right;"><%=Splosno.Sortiranje("racuni desc", _sort, "Računi")%></th>
    <th style="width:40px;text-align:right;"><%=Splosno.Sortiranje("popravljanje desc", _sort, "Popravljanje")%></th>--%>
</tr>
</HeaderTemplate>
<ItemTemplate>
<tr class="pt all" onclick="window.location='Uporabnik.aspx?id=<%#Eval("anUserID")%>'">
    <td><b><%#Eval("acUserName")%></b></td>
    <td><%#Eval("acFirstName")%></td>
    <td><%#Eval("acLastName")%></td>
    <td><%#Eval("acEmail")%></td>
    <td><i class="icon-<%#Convert.ToInt32(Eval("anAdmin")) == 1 ? "ok" : "remove"%>"></i></td>
<%--    <td style="text-align:center;"><%#Eval("davcna")%></td>
    <td style="text-align:right;"><i class="icon-<%#Convert.ToBoolean(Eval("prodajalec")) ? "ok" : "remove"%>"></i></td>
    <td style="text-align:right;"><i class="icon-<%#Convert.ToBoolean(Eval("racuni")) ? "ok" : "remove"%>"></i></td>
    <td style="text-align:right;"><i class="icon-<%#Convert.ToBoolean(Eval("popravljanje")) ? "ok" : "remove"%>"></i></td>--%>
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
                url: "Ac.aspx?t=persistence&key=persistence_uporabniki&s=" + JSON.stringify(arr),
                cache: false,
                async: true
            });
        } catch (e) { }
    }
</script>
</form>
</asp:Content>
