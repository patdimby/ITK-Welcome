
//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Intitek.Welcome.Domain
{
    using Intitek.Welcome.Infrastructure.Domain;
    using System;
    using System.Collections.Generic;
    
public partial class UserDocument : EntityBase<int>, IAggregateRoot
    {
        protected override void Validate()
        {
            throw new NotImplementedException();
        }

        public int ID { get; set; }

    public Nullable<int> ID_IntitekUser { get; set; }

    public Nullable<int> ID_Document { get; set; }

    public Nullable<System.DateTime> UpdateDate { get; set; }

    public Nullable<System.DateTime> IsRead { get; set; }

    public Nullable<System.DateTime> IsTested { get; set; }

    public Nullable<System.DateTime> IsApproved { get; set; }



    public virtual Document Document { get; set; }

    public virtual IntitekUser IntitekUser { get; set; }

}

}
