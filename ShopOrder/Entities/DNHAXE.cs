//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopOrder.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class DNHAXE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DNHAXE()
        {
            this.DKHACHHANGs = new HashSet<DKHACHHANG>();
            this.TDONHANGs = new HashSet<TDONHANG>();
            this.TGIAOHANGs = new HashSet<TGIAOHANG>();
        }
    
        public string ID { get; set; }
        public string NAME { get; set; }
        public string DIENTHOAI { get; set; }
        public string BIENSO { get; set; }
        public string GIOXECHAY { get; set; }
        public string BENDO { get; set; }
        public Nullable<int> XEOM { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DKHACHHANG> DKHACHHANGs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TDONHANG> TDONHANGs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TGIAOHANG> TGIAOHANGs { get; set; }
    }
}
