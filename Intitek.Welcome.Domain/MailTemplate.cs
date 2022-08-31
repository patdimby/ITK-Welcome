
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
    
public partial class MailTemplate : EntityBase<int>, IAggregateRoot
    {
        protected override void Validate()
        {
            throw new NotImplementedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public MailTemplate()
    {

        this.HistoEmails = new HashSet<HistoEmails>();

        this.DocumentCategory = new HashSet<DocumentCategory>();

        this.SubCategory = new HashSet<SubCategory>();

    }


    public int ID { get; set; }

    public System.DateTime Date { get; set; }

    public string Name { get; set; }

    public string Commentaire { get; set; }

    public string Content { get; set; }

    public string Object { get; set; }

    public int isGlobal { get; set; }

    public int isDocNoCategory { get; set; }

    public int isDocNoSubCategory { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<HistoEmails> HistoEmails { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<DocumentCategory> DocumentCategory { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<SubCategory> SubCategory { get; set; }

}

}
