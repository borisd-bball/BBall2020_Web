namespace mr.bBall_Lib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;


    public static class cTranslate
    {
        static public KeyValuePair<int, string>[] lTransValues = new KeyValuePair<int, string>[]
        {
            new KeyValuePair<int, string>(999, "Neznana napaka!"),
            new KeyValuePair<int, string>(998, "Registracija ni pravilna!"),
            new KeyValuePair<int, string>(997, "Registracija je potekla!"),
            new KeyValuePair<int, string>(100, "Prijava ni uspela!"),
            new KeyValuePair<int, string>(101, "Prijava ni uspela - Uporabnik ni aktiven!"),
            new KeyValuePair<int, string>(102, "Prijava ni uspela - Prekoračeno število prijavljenih uporabnikov!"),
            new KeyValuePair<int, string>(103, "Uporabnik je že prijavljen!"),
            new KeyValuePair<int, string>(104, "Dostop zavrnjen!"),
            new KeyValuePair<int, string>(105, "Operacija ni uspela!"),
            new KeyValuePair<int, string>(106, "Uporabnik nima pravice za prijavo!"),
            new KeyValuePair<int, string>(107, "Napaka pri pridobivanju podatkov!"),
            new KeyValuePair<int, string>(108, "Dokument ne obstaja ali pa ni dodeljen uporabniku!"),
            new KeyValuePair<int, string>(109, "Dokument je zaključen!"),
            new KeyValuePair<int, string>(110, "Uporabnik ni aktiven na opravilu!"),
            new KeyValuePair<int, string>(111, "Uporabnik ni dodeljen opravilu!"),
            new KeyValuePair<int, string>(112, "Vsi skladiščniki se še niso odjavili!"),
            new KeyValuePair<int, string>(113, "Skladiščniki še niso iz svojih lokacij izpraznili zaloge!"),
            new KeyValuePair<int, string>(114, "Artikel ne obstaja!"),
            new KeyValuePair<int, string>(115, "Artikel na prevzemu ne obstaja!"),
            new KeyValuePair<int, string>(116, "Uporabnik nima dodeljene lokacije!"),
            new KeyValuePair<int, string>(117, "Zapis v popisno listo prevzema ni uspel!"),
            new KeyValuePair<int, string>(118, "Knjiženje zaloge ni uspelo. Na lokaciji manjka: "),
            new KeyValuePair<int, string>(119, "Status dokumenta ni pravilen. Popis ni možen!"),
            new KeyValuePair<int, string>(120, "Sledilna koda že obstaja v sistemu. Popis ni možen!"),
            new KeyValuePair<int, string>(121, "Artikel se ne ujema s artiklom iz liste ali dokumenta!"),
            new KeyValuePair<int, string>(122, "Lokacija ne obstaja!"),
            new KeyValuePair<int, string>(123, "Knjiženje zaloge ni uspelo! Verjetno ni dovolj zaloge."),
            new KeyValuePair<int, string>(124, "Status dokumenta ni pravilen!"),
            new KeyValuePair<int, string>(125, "Lokacija ni končna lokacija!"),
            new KeyValuePair<int, string>(126, "Količina nič, ni dovoljena!"),
            new KeyValuePair<int, string>(127, "Uporabnik ni več lastnik rekorda!"),
            new KeyValuePair<int, string>(128, "Ni dovoljeno več popisat kot pa je naročeno!"),
            new KeyValuePair<int, string>(129, "Lokacija se ne ujema s popisano!"),
            new KeyValuePair<int, string>(130, "Pozicija je že popisana!"),
            new KeyValuePair<int, string>(131, "Napaka pri Split!"),
            new KeyValuePair<int, string>(132, "Popis ne obstaja!"),
            new KeyValuePair<int, string>(133, "Napaka pri potrditvi pozicij. Napaka pri prenosu!"),
            new KeyValuePair<int, string>(134, "Dokument še ni zaključen na terminalu!"),
            new KeyValuePair<int, string>(135, "Ni podatkov za prenos!"),
            new KeyValuePair<int, string>(136, "Opravilo je že bilo potrjeno v ERP!"),
            new KeyValuePair<int, string>(137, "Ni pozicij za ponovno najavo!"),
            new KeyValuePair<int, string>(138, "Izvorna in ciljna lokacija sta enaki. To ni dovoljeno!"),
            new KeyValuePair<int, string>(139, "Izvorna lokacija ne sme biti tipa izhodna!"),
            new KeyValuePair<int, string>(140, "Sledilna koda se ne ujema s popisano!"),
            new KeyValuePair<int, string>(141, "Sledilna koda na tej lokaciji ne obstaja!"),
            new KeyValuePair<int, string>(142, "Sledilna koda ne obstaja!"),
            new KeyValuePair<int, string>(143, "Na voljo ni nove verzije!"),
            new KeyValuePair<int, string>(144, "Ni artiklov za prenos (iz lokacije skladiščnika)!"),
            new KeyValuePair<int, string>(145, "Lokacija ni vhodna lokacija!"),
            new KeyValuePair<int, string>(146, "Dokument že ima določeno izhodno lokacijo. Druga izhodna lokacija ni možna!"),
            new KeyValuePair<int, string>(147, "Lokacija ni aktivna!"),
            new KeyValuePair<int, string>(148, "Za maloprodajo je dovoljena samo lokacija tipa maloprodaja!"),
            new KeyValuePair<int, string>(149, "Maloprodajna lokacija ni prosta!"),
            new KeyValuePair<int, string>(150, "Uporabnik je že registriran!")

        };

        public static string GetTransByID(int pTransID)
        {
            string lResut = "";
            try
            {
                foreach (KeyValuePair<int, string> element in lTransValues)
                {
                    if (element.Key == pTransID)
                    { lResut = element.Value; }
                }
                
            }
            catch (Exception)
            { }

            return lResut;
        }

    }
}