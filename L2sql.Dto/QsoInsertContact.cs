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
        public string StationName { get; set; }
        public decimal Frequency { get; set; }
        public int CallsignId { get; set; }
        public System.DateTime QsoDateTime { get; set; }
        public short RxRst { get; set; }
        public short TxRst { get; set; }
        public Nullable<short> QsoExchangeNumber { get; set; }
        public Logqso.mvc.common.Enum.QsoModeTypeEnum QsoModeTypeEnum { get; set; }
        public Logqso.mvc.common.Enum.QsoRadioTypeEnum QsoRadioTypeEnum { get; set; }
        public EntityState EntityState { get; set; }

        public QsoInsertContactsDTO()
        {
            EntityState = EntityState.Unchanged;

        }
    }

    //DTO SQLTVP support class for updating QsoInsertContactsDTO
    public class QsoInsertContactsDTOCollextion : List<QsoInsertContactsDTO>, IEnumerable<SqlDataRecord>
    {
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            var sdr = new SqlDataRecord(
                new SqlMetaData("QsoNo", SqlDbType.SmallInt),
                new SqlMetaData("LogId", SqlDbType.Int),
                new SqlMetaData("StationName", SqlDbType.VarChar, 20),
                new SqlMetaData("Frequency", SqlDbType.Decimal),
                new SqlMetaData("CallsignId", SqlDbType.Int),
                new SqlMetaData("QsoDateTime", SqlDbType.DateTime ),
                new SqlMetaData("RxRst", SqlDbType.SmallInt),
                new SqlMetaData("TxRst", SqlDbType.SmallInt),
                new SqlMetaData("QsoExchangeNumber", SqlDbType.SmallInt),
                new SqlMetaData("QsoModeTypeEnum", SqlDbType.Int),
                new SqlMetaData("QsoRadioTypeEnum", SqlDbType.Int)
                );

            //.Net will call QsoUpdatePoinsMultsDTOCollextion.GetEnumerator() for each record.
            foreach (QsoInsertContactsDTO qinmd in this)
            {
                sdr.SetInt16(0, qinmd.QsoNo);
                sdr.SetInt32(1, qinmd.LogId);
                if (qinmd.StationName == null || qinmd.StationName == string.Empty)
                {
                    sdr.SetDBNull(2);
                }
                else
                {
                    System.Data.SqlTypes.SqlChars StationName = new System.Data.SqlTypes.SqlChars(qinmd.StationName.ToCharArray());
                    sdr.SetSqlChars(2, StationName);
                }

                sdr.SetDecimal(3, qinmd.Frequency);
                sdr.SetInt32(4, qinmd.CallsignId);
                sdr.SetDateTime(5, qinmd.QsoDateTime);
                sdr.SetInt16(6, qinmd.RxRst);
                sdr.SetInt16(7, qinmd.TxRst);
                if (qinmd.QsoExchangeNumber == null)
	            {
                    sdr.SetDBNull(8);
	            }else
	            {
                    sdr.SetInt16(8, (short)qinmd.QsoExchangeNumber);
	            }
                sdr.SetInt32(9, (int)qinmd.QsoModeTypeEnum);
                sdr.SetInt32(10, (int)qinmd.QsoRadioTypeEnum);

                yield return sdr;
            }
        }


    }

}