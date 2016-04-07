using System;
using System.Collections.Generic;
using System.Text;

namespace L2Sql.BusinessLayer
{
    class CabrilloLTagInfos
    {
        public string Callsign;
        public string CatAssisted;
        public string CatBand;
        public string CatMode;
        public string CatOperator;
        public string CatPower;
        public string CatStation;
        public string CatTx;
        public string CatOverlay;
        public int ClaimedScore;
        public string Club;
        public string Contest;
        public string CreatedBy;
        public string Operators;
        public byte CallTxZone;
        
        public CabrilloLTagInfos()
        {
            CallTxZone = 0;
        }
    }

    class CabrilloQSOCQWWTagInfos
    {
        public int QFreq { get; set; }
        public string Mode { get; set; }
        public string QDate { get; set; }
        public string QTime { get; set; }
        public string Call { get; set; }
        public short TxRpt { get; set; }
        public byte TxZone { get; set; }
        public string QCall { get; set; }
        public short QRpt { get; set; }
        public byte QZone { get; set; }
        public byte Radio { get; set; }

        public CabrilloQSOCQWWTagInfos()
        {
            QFreq = 0;
            TxRpt = 0;
            TxZone = 0;
            QRpt = 0;
            QZone = 0;
        }
    }

    class CabrilloQSOCQWPXTagInfos
    {
        public int QFreq  {get; set;}
        public string Mode { get; set; }
        public string QDate { get; set; }
        public string QTime { get; set; }
        public string Call { get; set; }
        public short TxRpt { get; set; }
        public short TxNum { get; set; }
        public string QCall { get; set; }
        public short QRpt { get; set; }
        public short QNum { get; set; }
        public byte Radio { get; set; }

        public CabrilloQSOCQWPXTagInfos()
        {
            QFreq = 0;
            TxRpt = 0;
            TxNum = 0;
            QRpt = 0;
            QNum = 0;
        }
    }

    class ConvertFreqToBands
    {

        public static string ConvertFreqToBand(int freq)
        {
            string band = "";
            if (freq >= 1800 && freq <= 2000)
            {
                band = "160M";
            }
            else if (freq >= 3500 && freq <= 4000)
            {
                band = "80M";
            }
            else if (freq >= 7000 && freq <= 7300)
            {
                band = "40M";
            }
            else if (freq >= 14000 && freq <= 14350)
            {
                band = "20M";
            }
            else if (freq >= 21000 && freq <= 21450)
            {
                band = "15M";
            }
            else if (freq >= 28000 && freq <= 29700)
            {
                band = "10M";
            } 
            
            return band;
        }
    }
}
