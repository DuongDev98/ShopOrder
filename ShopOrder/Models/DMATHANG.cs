﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopOrder.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class DMATHANG
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DMATHANG()
        {
            this.DANHs = new HashSet<DANH>();
            this.DMAUCHITIETs = new HashSet<DMAUCHITIET>();
            this.DSIZECHITIETs = new HashSet<DSIZECHITIET>();
        }
    
        public string ID { get; set; }
        public string CODE { get; set; }
        [Required(ErrorMessage = "Tên không được trống")]
        public string NAME { get; set; }
        public string DNHOMHANGID { get; set; }
        public string DPHANLOAIID { get; set; }
        public string DDANGID { get; set; }
        public string DTHOIGIANDATID { get; set; }
        public Nullable<long> GIABAN { get; set; }
        public Nullable<long> GIABAN2 { get; set; }
        public Nullable<long> GIABAN3 { get; set; }
        public Nullable<long> GIABAN4 { get; set; }
        public Nullable<long> GIABAN5 { get; set; }
        public Nullable<long> GIABAN6 { get; set; }
        public Nullable<long> GIABAN7 { get; set; }
        public Nullable<long> GIABAN8 { get; set; }
        public Nullable<int> HANGMOIVE { get; set; }
        public Nullable<int> HANGSAPVE { get; set; }
        public Nullable<int> HANGSALE { get; set; }
        public Nullable<int> BANAMKHO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DANH> DANHs { get; set; }
        public virtual DDANG DDANG { get; set; }
        public virtual DNHOMHANG DNHOMHANG { get; set; }
        public virtual DPHANLOAI DPHANLOAI { get; set; }
        public virtual DTHOIGIANDAT DTHOIGIANDAT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DMAUCHITIET> DMAUCHITIETs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DSIZECHITIET> DSIZECHITIETs { get; set; }
    }
}