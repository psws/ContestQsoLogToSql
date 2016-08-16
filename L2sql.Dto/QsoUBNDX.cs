using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Microsoft.SqlServer.Server;

using L2Sql.DomainModel;

namespace L2Sql.Dto
{
    public class QsoWorkedLogDTO
    {
        public int LogId { get; set; }
        public DateTime QsoDateTime { get; set; }
        public decimal Frequency { get; set; }
        public int CallsignId { get; set; }
        public Nullable<short> QsoExchangeNumber { get; set; }
        public string QsoExchangeAlpha { get; set; }
        public Logqso.mvc.common.Enum.QsoModeTypeEnum QsoModeTypeEnum { get; set; }
        public EntityState EntityState { get; set; }

        public QsoWorkedLogDTO()
        {
            EntityState = EntityState.Unchanged;
        }

    }


    public class QsoUpdateUbnxdDTO
    {
        public short QsoNo { get; set; }
        public int LogId { get; set; }
        public bool U { get; set; }
        public bool B { get; set; }
        public string CorrectCall { get; set; }
        public bool N { get; set; }
        public bool D { get; set; }
        public bool X { get; set; }
        public string CorrectXchng {get; set; }
        public EntityState EntityState { get; set; }

        public QsoUpdateUbnxdDTO()
        {
            U = false;
            B = false;
            N = false;
            D = false;
            X = false;
            CorrectCall = null;
            CorrectXchng = null;
            EntityState = EntityState.Unchanged;
        }
    }

    public class QsoBadNilContact
    {
        public short QsoNo { get; set; }
        public int LogId { get; set; }
        public decimal Frequency { get; set; }
        public int CallsignId { get; set; }
        public string Call { get; set; }
        public System.DateTime QsoDateTime { get; set; }
        public Nullable<short> QsoExchangeNumber { get; set; }
    }


    public class QsoUpdateUniquNilDupeDTO
    {
        public short QsoNo { get; set; }
        public int LogId { get; set; }
        public EntityState EntityState { get; set; }

        public QsoUpdateUniquNilDupeDTO()
        {
            EntityState = EntityState.Unchanged;
        }

    }



    //DTO SQLTVP support class for updating QsoUpdatePoinsMultsDTO
    public class QsoUpdateUniquNilDupeDTOCollextion : List<QsoUpdateUniquNilDupeDTO>, IEnumerable<SqlDataRecord>
    {
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            var sdr = new SqlDataRecord(
                new SqlMetaData("QsoNo", SqlDbType.SmallInt),
                new SqlMetaData("LogId", SqlDbType.Int)
                );

            //.Net will call QsoUpdateQsoUpdateUniquNilDupeDTOCollextion.GetEnumerator() for each record.
            foreach (QsoUpdateUniquNilDupeDTO qupmd in this)
            {
                sdr.SetInt16(0, qupmd.QsoNo);
                sdr.SetInt32(1, qupmd.LogId);

                yield return sdr;
            }
        }
    }


    public class QsoUpdateBadCallXchngDTO
    {
        public short QsoNo { get; set; }
        public int LogId { get; set; }
        public string Corrected { get; set; }
        public EntityState EntityState { get; set; }

        public QsoUpdateBadCallXchngDTO()
        {
            Corrected = null;
            EntityState = EntityState.Unchanged;
        }
    }



    public class QsoUpdateBadCallXchngDTOCollextion : List<QsoUpdateBadCallXchngDTO>, IEnumerable<SqlDataRecord>
    {
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            var sdr = new SqlDataRecord(
                new SqlMetaData("QsoNo", SqlDbType.SmallInt),
                new SqlMetaData("LogId", SqlDbType.Int),
                new SqlMetaData("Corrected", SqlDbType.VarChar)
                );

            //.Net will call QsoUpdateQsoUpdateUniquNilDupeDTOCollextion.GetEnumerator() for each record.
            foreach (QsoUpdateBadCallXchngDTO qupmd in this)
            {
                sdr.SetInt16(0, qupmd.QsoNo);
                sdr.SetInt32(1, qupmd.LogId);
                sdr.SetSqlString(3, qupmd.Corrected);

                yield return sdr;
            }
        }
    }



}