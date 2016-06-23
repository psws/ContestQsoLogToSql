using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Microsoft.SqlServer.Server;

using L2Sql.DomainModel;

namespace L2Sql.Dto
{
    public class QsoInsertContact
    {
        public short QsoNo { get; set; }
        public int LogId { get; set; }
        public decimal Frequency { get; set; }
        public int CallsignId { get; set; }
        public Nullable<short> QsoExchangeNumber { get; set; }
        public Nullable<short> QsoExchangeAlpha { get; set; }
        public Logqso.mvc.common.Enum.QsoModeTypeEnum QsoModeTypeEnum { get; set; }
        public EntityState EntityState { get; set; }

        public QsoInsertContact()
        {
            EntityState = EntityState.Unchanged;
        }

    }

    public class QsoInsertContactsDTO
    {
        public short QsoNo { get; set; }
        public int LogId { get; set; }
        public bool QZoneMult { get; set; }
        public bool QCtyMult { get; set; }
        public bool QPrefixMult { get; set; }
        public bool QPts1 { get; set; }
        public bool QPts2 { get; set; }
        public bool QPts4 { get; set; }
        public bool QPts8 { get; set; }
        public EntityState EntityState { get; set; }

        public QsoInsertContactsDTO()
        {
            QZoneMult = false;
            QCtyMult = false;
            QPrefixMult = false;
            QPts1 = false;
            QPts2 = false;
            QPts4 = false;
            QPts8 = false;
            EntityState = EntityState.Unchanged;

        }
    }

    //DTO SQLTVP support class for updating QsoUpdatePoinsMultsDTO
    public class QsoInsertContactsDTOCollextion : List<QsoInsertContactsDTO>, IEnumerable<SqlDataRecord>
    {
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            var sdr = new SqlDataRecord(
                new SqlMetaData("QsoNo", SqlDbType.SmallInt),
                new SqlMetaData("LogId", SqlDbType.Int),
                new SqlMetaData("QZoneMult", SqlDbType.Bit),
                new SqlMetaData("QCtyMult", SqlDbType.Bit),
                new SqlMetaData("QPrefixMult", SqlDbType.Bit),
                new SqlMetaData("QPts1", SqlDbType.Bit),
                new SqlMetaData("QPts2", SqlDbType.Bit),
                new SqlMetaData("QPts1", SqlDbType.Bit),
                new SqlMetaData("QPts8", SqlDbType.Bit)
                );

            //.Net will call QsoUpdatePoinsMultsDTOCollextion.GetEnumerator() for each record.
            foreach (QsoInsertContactsDTO qupmd in this)
            {
                sdr.SetInt16(0, qupmd.QsoNo);
                sdr.SetInt32(1, qupmd.LogId);
                sdr.SetBoolean(2, qupmd.QZoneMult);
                sdr.SetBoolean(3, qupmd.QCtyMult);
                sdr.SetBoolean(4, qupmd.QPrefixMult);
                sdr.SetBoolean(5, qupmd.QPts1);
                sdr.SetBoolean(6, qupmd.QPts2);
                sdr.SetBoolean(7, qupmd.QPts4);
                sdr.SetBoolean(8, qupmd.QPts8);

                yield return sdr;
            }
        }


    }

}