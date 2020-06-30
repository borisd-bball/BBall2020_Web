<%@ Page Title="" Language="C#" MasterPageFile="Master.master" AutoEventWireup="true" CodeFile="Pregledi.aspx.cs" Inherits="_Pregledi" ValidateRequest="false" %>
<%@ MasterType TypeName="Master" %>
<%@ Import Namespace="mr.bBall_Lib" %>
<asp:Content ID="cph_body" ContentPlaceHolderID="cph_body" Runat="Server">
<h2>Pregledi</h2>
<ul id="linkboxes" class="linkboxes" style="margin-top:0;overflow:hidden;position:relative;width:100%;" runat="server">
<%--    <%if (Master.Uporabnik.PraviceL.Contains("0")){%><li class="linkbox statistika" style=""><span class="ocena">1</span><a href="PregledMatKartica.aspx">Materijalna kartica</a></li><%}%>
    <%if (Master.Uporabnik.PraviceL.Contains("0")){%><li class="linkbox statistika" style=""><span class="ocena">1</span><a href="PregledZalogaArtiklov.aspx">Zaloga artiklov na skladišču</a></li><%}%>
    <%if (Master.Uporabnik.PraviceL.Contains("0")){%><li class="linkbox statistika" style=""><span class="ocena">1</span><a href="PregledZalogaPoLok.aspx">Zaloga artiklov po lokacijah</a></li><%}%>
    <%if (Master.Uporabnik.PraviceL.Contains("0")){%><li class="linkbox statistika" style=""><span class="ocena">1</span><a href="PregledZalogaArtiklovKupci.aspx">Zaloga artiklov po kupcih</a></li><%}%>
    <%if (Master.Uporabnik.PraviceL.Contains("0")){%><li class="linkbox statistika" style=""><span class="ocena">1</span><a href="PregledMatKarticaFIFO.aspx">Materijalna kartica FIFO</a></li><%}%>
    <%if (Master.Uporabnik.PraviceL.Contains("0")){%><li class="linkbox statistika" style=""><span class="ocena">1</span><a href="PregledMatKarticaFIFORaz.aspx">Materijalna kartica FIFO - Razširjena</a></li><%}%>--%>

</ul>

</asp:Content>
