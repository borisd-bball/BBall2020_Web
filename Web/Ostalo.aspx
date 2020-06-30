<%@ Page Title="" Language="C#" MasterPageFile="Master.master" AutoEventWireup="true" CodeFile="Ostalo.aspx.cs" Inherits="_Ostalo" ValidateRequest="false" %>
<%@ MasterType TypeName="Master" %>
<%@ Import Namespace="mr.bBall_Lib" %>
<asp:Content ID="cph_body" ContentPlaceHolderID="cph_body" Runat="Server">
<h2>Ostalo</h2>
<ul id="linkboxes" class="linkboxes" style="margin-top:0;overflow:hidden;position:relative;width:100%;" runat="server">
    <!--<%if (Master.Uporabnik.Pravice.Contains("drzave")){%><li class="linkbox statistika" style=""><span class="ocena"><%=_countrys_stetje%></span><a href="Countrys.aspx">Države</a></li><%}%>
    <%if (Master.Uporabnik.Pravice.Contains("poste")){%><li class="linkbox statistika" style=""><span class="ocena"><%=_posts_stetje%></span><a href="Posts.aspx">Pošte</a></li><%}%>
    <%if (Master.Uporabnik.Pravice.Contains("tipi_zavezancev")){%><li class="linkbox statistika" style=""><span class="ocena"><%=_subtypes_stetje%></span><a href="SubTypes.aspx">Tipi zavezancev</a></li><%}%>
    <%if (Master.Uporabnik.Pravice.Contains("stranke")){%><li class="linkbox statistika" style=""><span class="ocena"><%=_stranke_stetje%></span><a href="Stranke.aspx">Stranke</a></li><%}%>-->

    <%if(Master.Uporabnik.Pravice.Contains("uporabniki")){%><li class="linkbox statistika" style=""><span class="ocena"><%=_uporabniki_stetje%></span><a href="Uporabniki.aspx">Uporabniki</a></li><%}%>
    <%if (Master.Uporabnik.Pravice.Contains("nastavitve")){%><li class="linkbox statistika" style=""><span class="ocena">&nbsp;</span><a href="Nastavitve.aspx">Nastavitve</a></li><%}%>
</ul>

</asp:Content>
