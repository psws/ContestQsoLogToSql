//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace L2Sql.DomainModel
{
    using System;
    using System.Collections.Generic;

    public partial class CallSign : IEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CallSign()
        {
            this.CabrilloInfoes = new HashSet<CabrilloInfo>();
            this.CallInfoes = new HashSet<CallInfo>();
            this.Logs = new HashSet<Log>();
            this.Qsoes = new HashSet<Qso>();
            this.Spots = new HashSet<Spot>();
            this.Spots1 = new HashSet<Spot>();
            this.UbnSummaries = new HashSet<UbnSummary>();
        }
    
        public int CallSignId { get; set; }
        public string Call { get; set; }
        public string Prefix { get; set; }
        public Logqso.mvc.common.Enum.ContinentEnum ContinentEnum { get; set; }
        public int Accuracy { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
        public Nullable<bool> GeoCodeChk { get; set; }
        public EntityState EntityState { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CabrilloInfo> CabrilloInfoes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CallInfo> CallInfoes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Log> Logs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Qso> Qsoes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Spot> Spots { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Spot> Spots1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UbnSummary> UbnSummaries { get; set; }
    }
}
