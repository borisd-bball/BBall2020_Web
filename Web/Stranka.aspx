<%@ Page Title="" Language="C#" MasterPageFile="Master.master" AutoEventWireup="true" CodeFile="Stranka.aspx.cs" Inherits="_Stranka" MaintainScrollPositionOnPostback="true" %>
<%@ MasterType TypeName="Master" %>
<%@ Import Namespace="mr.bBall_Lib" %>
<asp:Content ID="cph_body" ContentPlaceHolderID="cph_body" Runat="Server">
<form id="form" runat="server">
<h2>Stranka<a href="Stranke.aspx" class="button big right" style="margin:0;">Nazaj</a><%if(_id>0){%><a href="Stranka.aspx" class="button colorfull big right" style="margin-right:10px;">Nova stranka</a><%}%></h2>
<div class="actionbox top" style="margin-left:0;margin-top:10px;padding:20px 20px 10px 20px;">
    <p style="display:inline-block">
        Prikazani so vsi podatki stranke. V izbranem polju lahko podatek popravite in ga shranite s klikom na gumb "Shrani".
    </p>
</div>
<fieldset>
    <legend>Podatki stranke <%=k_naziv.Text%><span class="req_label"><input /> - obvezna polja</span></legend>
    <div class="form-horizontal">
        <div class="control-group">
            <label for="iskanje" class="control-label">Iskanje</label>
            <div class="controls">
                <asp:TextBox ID="iskanje" style="width:710px;" ReadOnly="true" runat="server"></asp:TextBox>
                <span class="wfdescription">Vnesite besede iz naziva ali kodo stranke za iskanje po centralnem registru</span>
            </div>
        </div>
        <div class="control-group">
            <label for="k_naziv" class="control-label">Kratek naziv</label>
            <div class="controls">
                <asp:TextBox ID="k_naziv" class="req" style="width:476px;margin-right:138px;" runat="server"></asp:TextBox>
                <span style="width:20px;">ID</span>
                <asp:TextBox ID="customerCode" style="width:50px;margin-right:10px;" ReadOnly="true" runat="server"></asp:TextBox>
<%--                <asp:Button ID="b_get_from_net" runat="server" CssClass="button small colorfull" Text="Poišči v spletu" style="margin-right:10px;" Visible="false" OnClick="b_get_from_net_Click"></asp:Button>--%>
            </div>
        </div>
        <div class="control-group">
            <label for="naziv" class="control-label">Dolgi naziv</label>
            <div class="controls">
                <asp:TextBox ID="naziv" class="req" style="width:710px;" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="control-group">
            <label for="naslov" class="control-label">Naslov</label>
            <div class="controls">
                <asp:TextBox ID="naslov" style="width:710px;" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="control-group">
            <label for="drzava" class="control-label">Država</label>
            <div class="controls">
                <asp:DropDownList ID="drzava" style="width:300px;margin-right:55px;" class="req" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drzava_SelectedIndexChanged"></asp:DropDownList>
                <span>Pošta</span>
                <asp:DropDownList ID="posta" style="width:300px;margin-right:10px;" class="req" runat="server"></asp:DropDownList>
            </div>
        </div>
        <div class="control-group">
            <label for="tip_zavezanca" class="control-label">Tip zavezanca</label>
            <div class="controls">
                <asp:DropDownList ID="tip_zavezanca" style="width:200px;margin-right:150px;" class="req" runat="server" AutoPostBack="true" OnSelectedIndexChanged="tip_zavezanca_SelectedIndexChanged"></asp:DropDownList>
                <span style="width:80px;">ID za DDV</span>
                <asp:TextBox ID="id_za_ddv" style="width:40px;margin-right:10px;" ReadOnly="true" runat="server"></asp:TextBox>
                <span style="width:10px;">-</span>
                <asp:TextBox ID="davcna" style="width:187px;margin-right:10px;" class="req" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="control-group">
            <label for="contact" class="control-label">Kontaktna oseba</label>
            <div class="controls">
                <asp:TextBox ID="contact" style="width:298px;margin-right:100px;" runat="server"></asp:TextBox>
                <span>Telefon</span>
                <asp:TextBox ID="telefon" style="width:228px;margin-right:10px;" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="control-group">
            <label for="email" class="control-label">Elektronska pošta</label>
            <div class="controls">
                <asp:TextBox ID="email" style="width:710px;" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="control-group">
            <label for="active" class="control-label">Aktiven</label>
            <div class="controls">
                <asp:CheckBox ID="active" class="req" style="width:110px;margin-right:30px;" runat="server" /> 
            </div>
        </div>
        <div class="control-group">
            <label for="note" class="control-label">Opomba</label>
            <div class="controls">
                <asp:TextBox ID="note" style="width:710px;" TextMode="MultiLine" Rows="4" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="form-actions">
            <asp:Button ID="shrani" runat="server" CssClass="button big colorfull" Text="Shrani" OnClick="shrani_Click"></asp:Button>
            <a href="Stranke.aspx" class="button">Nazaj</a>
            <asp:Button ID="brisi" runat="server" CssClass="button big redfull right" Text="Briši" style="margin-right:92px;" Visible="false" OnClick="brisi_Click"></asp:Button>
        </div>
    </div>
</fieldset>
<br /><br />
<h2>Transakcije</h2>
<div class="actionbox top" style="margin-left:0;margin-top:10px;padding:20px 20px 10px 20px;">
    <p style="display:inline-block">
        Prikazane so vse transakcije stranke.
    </p>
    <div style="float:right;display:inline-block;margin-left:10px">
        <p>Leto:</p>
        <select id="sel_leto" style="width:110px" onchange="filter()"><option value=".all">vsi</option><%=_sel_leto%></select>
    </div>
</div>

<%--<table class="table noalt">
<tr>
    <th colspan="2">Št. zapisov: <span id="stetje"></span></th>
    <th colspan="2"><a href="?id=<%=_id%>&a=csv" class="button big colorfull" target="_blank" style="float:right;margin:0;position:relative;bottom:3px;">Izvozi</a></th>
</tr>
<asp:Repeater ID="r_transakcije" runat="server">
<HeaderTemplate>
<tr>
    <th style="width:20%;"><a <%= _sort == "tip asc,oznaka desc" ? "class=\"act\"" : "" %> href="?id=<%=_id%>&sort=tip asc,oznaka desc">Tip</a></th>
    <th><a <%= _sort == "tip asc,oznaka desc" ? "class=\"act\"" : "" %> href="?id=<%=_id%>&sort=tip asc,oznaka desc">Oznaka</a></th>
    <th style="width:15%;text-align:center;"><a <%= _sort == "datum desc" ? "class=\"act\"" : "" %> href="?id=<%=_id%>&sort=datum desc">Datum</a></th>
    <th style="width:15%;text-align:right;"><a <%= _sort == "znesek desc" ? "class=\"act\"" : "" %> href="?id=<%=_id%>&sort=znesek desc">Znesek</a></th>
</tr>
</HeaderTemplate>
<ItemTemplate>
<tr class="pt all le<%#Convert.ToDateTime(Eval("datum")).ToString("yyyy")%>" onclick="window.location='<%#Convert.ToString(Eval("tip")) %>.aspx?id=<%#Eval("id")%>'">
    <td><%#tip_transakcije(Convert.ToString(Eval("tip")))%></td>
    <td><%#Convert.ToString(Eval("oznaka")).Trim('-')%></td>
    <td style="text-align:center;"><%#Convert.ToDateTime(Eval("datum")).ToString("dd.MM.yyyy")%></td>
    <td style="text-align:right;" class="znesek"><%#Convert.ToDouble(Eval("znesek")).ToString("#,##0.00")%></td>
</tr>
</ItemTemplate>
<FooterTemplate>
<tr>
    <th></th>
    <th></th>
    <th style="text-align:center;">Skupaj</th>
    <th style="text-align:right;" id="skupaj_znesek"></th>
</tr>
</FooterTemplate>
</asp:Repeater>
</table>--%>
</form>
<script>
    $(function () {
        $("#<%=iskanje.ClientID%>").autocomplete({
            minLength: 5,
            cache: false,
            search: function (event, ui) {
                $(this).autocomplete({
                    source: function (request, response) {
                        $.ajax({
                            url: "Ac.aspx?t=inetis&s=" + encodeURIComponent(request.term) + "&" + new Date().getDate(),
                            dataType: "json",
                            cache: false,
                            type: "get",
                            success: function (data) {
                                response(data);
                            }
                        });
                    }
                });
            },
            select: function (event, ui) {
                setTimeout(function () {
                    $.ajax({
                        url: "Ac.aspx?t=inetis&s=" + encodeURIComponent($("#<%=iskanje.ClientID%>").val()) + "&" + new Date().getDate(),
                        dataType: "json",
                        cache: false,
                        type: "get",
                        success: function (data) {
                            $("#<%=naziv.ClientID%>").val(data[0].acTitle);
                            $("#<%=k_naziv.ClientID%>").val(data[0].acShortTitle);
                            $("#<%=naslov.ClientID%>").val(data[0].acAddress);
                            $("#<%=posta.ClientID%>").val(data[0].anPostID);
                            $("#<%=email.ClientID%>").val(data[0].aceMaile);
                            $("#<%=davcna.ClientID%>").val(data[0].acVATNumber);
                            $("#<%=tip_zavezanca.ClientID%>").val(data[0].acVATTypeID);
                            $("#<%=id_za_ddv.ClientID%>").val(data[0].acVATPrefix);
<%--                            $("#<%=note.ClientID%>").val(data[0].acNote);--%>
                            $("#<%=iskanje.ClientID%>").val("");
                        }
                    });
                }, 1);
            }
        }).on("focus", function () {
            $(this).autocomplete("search", "");
        }).on("click", function () {
            $(this).autocomplete("search", "");
        });

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

        //var skupaj_znesek = 0;
        //for (var i = 0; i < trs.length; i++) {
        //    skupaj_znesek += parseFloat($(trs[i]).find(".znesek").text().replace(" ", "").replace(/\./g, "").replace(",", "."));
        //}
        //$("#skupaj_znesek").html(formatMoney(String(skupaj_znesek).replace(/\./g, ",")));

        try {
            var arr = new Array();
            var objs = $(".actionbox input,.actionbox select");
            for (var i = 0; i < objs.length; i++) arr.push({ "id": objs[i].id, "value": $(objs[i]).val() });
            $.ajax({
                url: "Ac.aspx?t=persistence&key=persistence_stranka&s=" + JSON.stringify(arr),
                cache: false,
                async: true
            });
        } catch (e) { }
    }
</script>
</asp:Content>
