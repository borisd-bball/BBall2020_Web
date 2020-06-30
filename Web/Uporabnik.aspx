<%@ Page Title="" Language="C#" MasterPageFile="Master.master" AutoEventWireup="true" CodeFile="Uporabnik.aspx.cs" Inherits="_Uporabnik" MaintainScrollPositionOnPostback="true" %>
<%@ MasterType TypeName="Master" %>
<%@ Import Namespace="mr.bBall_Lib" %>
<asp:Content ID="cph_body" ContentPlaceHolderID="cph_body" Runat="Server">
<form id="form" runat="server">
<h2>Uporabnik<a href="Uporabniki.aspx" class="button big right" style="margin:0;">Nazaj</a><%if(_id>0){%><a href="Uporabnik.aspx" class="button colorfull big right" style="margin-right:10px;">Nov uporabnik</a><%}%></h2>
<div class="actionbox top" style="margin-left:0;margin-top:10px;padding:20px 20px 10px 20px;">
    <p style="display:inline-block">
        Prikazani so vsi podatki uporabnika. V izbranem polju lahko podatek popravite in ga shranite s klikom na gumb "Shrani".
    </p>
</div>
<fieldset>
    <legend>Podatki uporabnika <%=username.Text%> | <%=ime.Text%> <%=priimek.Text%><span class="req_label"><input /> - obvezna polja</span></legend>
    <div class="form-horizontal">
        <div class="control-group">
            <label for="username" class="control-label">Uporabniško ime</label>
            <div class="controls">
                <asp:TextBox ID="username" class="req" style="width:162px;margin-right:30px;" runat="server"></asp:TextBox>
                <span>Ime</span>
                <asp:TextBox ID="ime" style="width:161px;margin-right:30px;" runat="server"></asp:TextBox>
                <span>Priimek</span>
                <asp:TextBox ID="priimek" style="width:161px;" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="control-group">
            <label for="email" class="control-label">Elektronska pošta</label>
            <div class="controls">
                <asp:TextBox ID="email" style="width:293px;margin-right:40px;" runat="server"></asp:TextBox>
                <span>Administrator</span>
                <%--<asp:TextBox ID="anAdmin" class="req" style="width:110px;margin-right:30px;" runat="server"></asp:TextBox>--%>
                <asp:CheckBox ID="anAdmin" class="req" style="width:110px;margin-right:30px;" runat="server" /> 
<%--                <span>Davčna</span>
                <asp:TextBox ID="davcna" class="req" style="width:80px;margin-right:30px;" runat="server"></asp:TextBox>--%>
            </div>
        </div>
<%--        <div class="control-group">
            <label for="davcne" class="control-label">Davčne številke</label>
            <div class="controls">
                <asp:TextBox ID="davcne" style="width:720px;" runat="server"></asp:TextBox>
                <span class="wfdescription">
                    Davčne številke prodajalcev (ločene s vejico) - davčna številka uporabnika se doda samodejno
                </span>
            </div>
        </div>--%>
<%--        <div class="control-group">
            <label for="tiskalnik" class="control-label">Tiskalnik</label>
            <div class="controls">
                <asp:TextBox ID="tiskalnik" style="width:720px;" runat="server"></asp:TextBox>
                <span class="wfdescription">
                    Ime pos tiskalnika - če je prazno se tiska v pdf
                </span>
            </div>
        </div>--%>
        <div class="control-group">
            <label for="password" class="control-label">Novo geslo</label>
            <div class="controls">
                <asp:TextBox ID="password" TextMode="Password" style="width:162px;margin-right:30px;" runat="server"></asp:TextBox>
<%--                <asp:CheckBox ID="prodajalec" runat="server" /> <span style="margin-right:30px;">Prodajalec</span>
                <asp:CheckBox ID="racuni" runat="server" /> <span style="margin-right:40px;">Računi</span>
                <asp:CheckBox ID="popravljanje" runat="server" /> <span style="margin-right:50px;">Popravljanje</span>
                <asp:CheckBox ID="reprezentanca" runat="server" /> <span style="margin-right:50px;">Reprezentanca</span>
                <span class="wfdescription">
                    Prodajalec - ali je uporabnik prodajalec izven društva - ima omejen nabor artiklov (skupine)<br />
                    Računi - ali uporabnik poleg dovolilnice izda tudi račun društva<br />
                    Popravljanje - ali uporabnik lahko na rubriki prodaja popravlja ceno, količino...<br />
                    Reprezentanca - ali uporabnik lahko izdaja reprezentančne dovolilnice
                </span>--%>
            </div>
        </div>
<%--        <div class="control-group">
            <label for="poslovni_prostor" class="control-label">Poslovni prostor</label>
            <div class="controls">
                <asp:DropDownList ID="poslovni_prostor" style="width:724px;" class="req" runat="server" AutoPostBack="true" OnSelectedIndexChanged="poslovni_prostor_SelectedIndexChanged"></asp:DropDownList>
            </div>
        </div>
        <div class="control-group">
            <label for="elektronska_naprava" class="control-label">Elektronska naprava</label>
            <div class="controls">
                <asp:DropDownList ID="elektronska_naprava" style="width:724px" class="req" runat="server"></asp:DropDownList>
            </div>
        </div>
        <div class="control-group prodajalec">
            <label for="skupine" class="control-label">Skupine</label>
            <div class="controls">
                <asp:TextBox ID="skupine" style="width:710px;" runat="server"></asp:TextBox>
                <span class="wfdescription">Prodajalec ima na voljo samo tiste artikle, ki spadajo v izbrane skupine</span>
            </div>
        </div>
        <div class="control-group prodajalec rd">
            <label for="revirji" class="control-label">Revirji</label>
            <div class="controls">
                <asp:TextBox ID="revirji" style="width:710px;" runat="server"></asp:TextBox>
                <span class="wfdescription">Prodajalec ima na voljo samo tiste dovolilnice, ki spadajo v izbrane revirje</span>
            </div>
        </div>--%>
        <div class="control-group">
            <label class="control-label"><b>Pravice</b></label>
            <div class="controls">
                <table class="table noalt" style="width:800px;">
                    <tr>
                        <th style="padding-left:0;">Naziv</th>
                        <th style="text-align:right;width:60px;">&nbsp;<input name="vrstica" id="vrstica" type="hidden" /></th>
                    </tr>
                    <tr>
                        <th style="padding-left:0;"><asp:DropDownList ID="u_roles" style="width:710px;" class="req" runat="server" AutoPostBack="true"></asp:DropDownList></th>
                        <%--<th style="padding-left:0;"><asp:TextBox ID="vrstica_naziv" class="req" style="width:710px;" Text="" runat="server"></asp:TextBox></th>--%>
                        <th style="text-align:right;"><asp:Button ID="dodaj" style="margin-right:0;" runat="server" CssClass="button" Text="Dodaj" Visible="true" OnClick="dodaj_Click"></asp:Button></th>
                    </tr>
                    <%=s_vrstice%>
                </table>
            </div>
        </div>
        <div class="form-actions">
            <asp:Button ID="shrani" runat="server" CssClass="button big colorfull" Text="Shrani" OnClick="shrani_Click"></asp:Button>
            <a href="Uporabniki.aspx" class="button">Nazaj</a>
            <asp:Button ID="brisi" runat="server" CssClass="button big redfull right" Text="Briši" style="margin-right:92px;" Visible="false" OnClick="brisi_Click"></asp:Button>
        </div>
    </div>
</fieldset> 
</form>
<script>
    $(function () {
<%--        $("#<%=prodajalec.ClientID%>").change(function () {
            if ($("#<%=prodajalec.ClientID%>").prop("checked")) $(".prodajalec").show();
            else $(".prodajalec").hide();
            <%if(!ConfigurationManager.AppSettings["Skupina"].StartsWith("rd")){%>
            $(".rd").hide();
            <%}%>
        });
        $("#<%=prodajalec.ClientID%>").change();
--%>
<%--        $("#<%=vrstica_naziv.ClientID%>").autocomplete({
            minLength: 0,
            cache: false,
            search: function (event, ui) {
                $(this).autocomplete({
                    source: function (request, response) {
                        $.ajax({
                            url: "Ac.aspx?t=pravice&s=" + encodeURIComponent($(".pravica").text().replace(/\s/g, ",")) + "&" + new Date().getDate(),
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
                    $("#<%=dodaj.ClientID%>").click();
                }, 1);
            }
        }).on("focus", function () {
            $(this).autocomplete("search", "");
        }).on("click", function () {
            $(this).autocomplete("search", "");
        });--%>

        function split(val) { return val.split(/,/); }
<%--        function extractLast(term) { return split(term).pop();}
            $("#<%=skupine.ClientID%>").bind("keydown", function (event) {
            if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).autocomplete("instance").menu.active) {
                event.preventDefault();
            }
        }).autocomplete({
            minLength: 0,
            cache: false,
            source: function (request, response) {
                $.getJSON("Ac.aspx?t=skupine&s=" + encodeURIComponent($("#<%=skupine.ClientID%>").val()) + "&" + new Date().getDate(), {
                    term: extractLast(request.term)
                }, response);
            },
            search: function () {
                var term = extractLast(this.value);
            },
            focus: function () {
                return false;
            },
            select: function (event, ui) {
                var terms = split(this.value);
                terms.pop();
                terms.push(ui.item.value);
                terms.push("");
                this.value = terms.join(",");
                return false;
            }
        });--%>

<%--        $("#<%=revirji.ClientID%>").bind("keydown", function (event) {
            if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).autocomplete("instance").menu.active) {
                event.preventDefault();
            }
        }).autocomplete({
            minLength: 0,
            cache: false,
            source: function (request, response) {
                $.getJSON("Ac.aspx?t=revirji&s=" + encodeURIComponent($("#<%=revirji.ClientID%>").val()) + "&" + new Date().getDate(), {
                    term: extractLast(request.term)
                }, response);
            },
            search: function () {
                var term = extractLast(this.value);
            },
            focus: function () {
                return false;
            },
            select: function (event, ui) {
                var terms = split(this.value);
                terms.pop();
                terms.push(ui.item.value);
                terms.push("");
                this.value = terms.join(",");
                return false;
            }
        });--%>
        <%if(!ConfigurationManager.AppSettings["Skupina"].StartsWith("rd")){%>
        $(".rd").hide();
        <%}%>
    });
</script>
</asp:Content>
