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

    public partial class QsoExchangeType : IEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public QsoExchangeType()
        {
            this.ContestTypes = new HashSet<ContestType>();
        }
    
        public Logqso.mvc.common.Enum.QsoExchangeTypeEnum QsoExchangeTypeEnum { get; set; }
        public string QsoExchangeTypeEnumName { get; set; }

        public EntityState EntityState { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContestType> ContestTypes { get; set; }
    }
}