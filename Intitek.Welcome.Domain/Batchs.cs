
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
    
public partial class Batchs : EntityBase<int>, IAggregateRoot
    {
        protected override void Validate()
        {
            throw new NotImplementedException();
        }


    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Batchs()
    {

        this.HistoBatchs = new HashSet<HistoBatchs>();

        this.HistoEmails = new HashSet<HistoEmails>();

    }


    public int ID { get; set; }

    public string ProgName { get; set; }

    public string Description { get; set; }

    public string Frequency { get; set; }

    public Nullable<System.DateTime> LastExecution { get; set; }

    public int ForceExecution { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<HistoBatchs> HistoBatchs { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<HistoEmails> HistoEmails { get; set; }

}

}