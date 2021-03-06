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

    public partial class Contest : IEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Contest()
        {
            this.CabrilloInfoes = new HashSet<CabrilloInfo>();
            this.CallInfoes = new HashSet<CallInfo>();
            this.Logs = new HashSet<Log>();
            this.Spots = new HashSet<Spot>();
            this.UbnSummaries = new HashSet<UbnSummary>();
        }
    
        public string ContestId { get; set; }
        public string ContestName { get; set; }
        public Logqso.mvc.common.Enum.ContestTypeEnum ContestTypeEnum { get; set; }
        public Logqso.mvc.common.Enum.QsoModeTypeEnum QsoModeTypeEnum { get; set; }
        public System.DateTime StartDateTime { get; set; }
        public System.DateTime EndDateTime { get; set; }
        public bool Active { get; set; }
        public EntityState EntityState { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CabrilloInfo> CabrilloInfoes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CallInfo> CallInfoes { get; set; }
        public virtual ContestType ContestType { get; set; }
        public virtual QsoModeType QsoModeType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Log> Logs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Spot> Spots { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UbnSummary> UbnSummaries { get; set; }
    }
}
