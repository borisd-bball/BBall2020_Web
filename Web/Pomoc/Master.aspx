<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Master.aspx.cs" Inherits="_Pomoc_Master" %>
<%@ Import Namespace="mr.bBall_Lib" %>
Če ob uporabi sistema Skulpture naletite na težave, se prosim obrnite na kontaktno osebo pri <%=Nastavitve.Naziv%> (<%=ConfigurationManager.AppSettings["Skupina"]=="rdljubno"?"+386 41 793 592":Nastavitve.Telefon%> ali <a href="mailto:<%=Nastavitve.EmailFrom%>"><%=Nastavitve.EmailFrom%></a>).<br />
Za vprašanja o delovanju sistema in ob napakah se lahko obrnete tudi na proizvajalca programske opreme MR Avtomatika, <a href="mailto:info@mr-avtomatika.com">info@mr-avtomatika.com</a>).<br />
<%if (Splosno.Produkcija){%><%}%>
