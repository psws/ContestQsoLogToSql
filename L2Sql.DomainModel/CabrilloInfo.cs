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

    public partial class CabrilloInfo : IEntity
    {
        public string ContestId { get; set; }
        public int CallSignId { get; set; }
        public int ClaimedScore { get; set; }
        public string Club { get; set; }
        public string Operators { get; set; }
        public string Address { get; set; }
        public string AddressCity { get; set; }
        public string AddressState { get; set; }
        public string AddressPostalCode { get; set; }
        public string AddressCountry { get; set; }

        public EntityState EntityState { get; set; }
    
        public virtual CallSign CallSign { get; set; }
        public virtual Contest Contest { get; set; }
    }
}
