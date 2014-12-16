using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Finance.Core.Configuration
{
    public static class Constants
    {
        // symbol, month, day, year
        public const string YahooUrl = "http://ichart.finance.yahoo.com/table.csv?s={0}&a={1}&b={2}&c={3}&d={4}&e={5}&f={6}";
        //public const string FiUrl = "http://insynsok.fi.se/SearchPage.aspx?reporttype=0&culture=sv-SE&publdate={0}";
        public const string FiUrl =
            "http://insynsok.fi.se/SearchPage.aspx?reporttype=0&culture=sv-SE&fromdate={0}&tomdate={1}";



        public static Dictionary<string, string> Ticker = new Dictionary<string, string>
                                                             {
                                                                 { "ACAN-B.ST", "ACANDO AB (PUBL)" },
                                                                 { "ANOD-B.ST", "ADDNODE GROUP AB (PUBL)" },
                                                                 { "ANOT.ST", "ANOTO GROUP AB (PUBL)" },
                                                                 { "ADDT-B.ST", "ADDTECH AB (PUBL)" },
                                                                 { "ACTI-B.ST", "ACTIVE BIOTECH AB (PUBL)" },
                                                                 { "ALFA.ST ", "ALFA LAVAL AB (PUBL)"},
                                                                 { "AERO-B.ST", "AEROCRINE AB (PUBL)" },
                                                                 { "ATCO-A.ST", "ATLAS COPCO AKTIEBOLAG (PUBL)"},
                                                                 { "AZA.ST", "AVANZA BANK HOLDING AB (PUBL)" },
                                                                 { "BBTO-B.ST", "B&B TOOLS AB (PUBL)" },
                                                                 { "BEIA-B.ST", "BEIJER ALMA AB (PUBL)" },
                                                                 { "BELE.ST", "BEIJER ELECTRONICS AKTIEBOLAG (PUBL)" },
                                                                 { "BETS-B.ST", "BETSSON AB (PUBL)" },
                                                                 { "BILI-A.ST", "BILIA AB (PUBL)" },
                                                                 { "BTS-B.ST", "BTS GROUP AB (PUBL)"},
                                                                 { "ELUX-B.ST", "AKTIEBOLAGET ELECTROLUX (PUBL)"},
                                                                 { "NETI-B.ST", "NET INSIGHT AB (PUBL)" },
                                                                 { "NDA-SEK.ST", "NORDEA BANK AB (PUBL)" },
                                                                 { "PART.ST", "PARTNERTECH AB (PUBL)" },
                                                                 { "TRMO.ST", "TRANSMODE AB"},
                                                                 { "VOLVO-B.ST", "AKTIEBOLAGET VOLVO (PUBL)" }
                                                             };
                                                            

    }
}