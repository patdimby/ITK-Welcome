//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Intitek.Welcome.Domain
{
    using Intitek.Welcome.Infrastructure.Domain;
    using System;
    using System.Collections.Generic;

    public partial class HistoActions : EntityBase<long>, IAggregateRoot
    {
        protected override void Validate()
        {
            throw new NotImplementedException();
        }

        public long ID { get; set; }
        public int ID_Object { get; set; }
        public int ID_IntitekUser { get; set; }
        public string ObjectCode { get; set; }
        public string Action { get; set; }
        public System.DateTime DateAction { get; set; }
        public string LinkedObjects { get; set; }

        public virtual IntitekUser IntitekUser { get; set; }
    }
}