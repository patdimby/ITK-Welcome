
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
    
public partial class UserQcm : EntityBase<int>, IAggregateRoot
    {
        protected override void Validate()
        {
            throw new NotImplementedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public UserQcm()
    {

        this.UserQcmReponse = new HashSet<UserQcmReponse>();

    }


    public int ID { get; set; }

    public int ID_IntitekUser { get; set; }

    public int ID_Qcm { get; set; }

    public System.DateTime DateCre { get; set; }

    public Nullable<System.DateTime> DateFin { get; set; }

    public Nullable<int> Score { get; set; }

    public Nullable<int> ScoreMinimal { get; set; }

    public Nullable<int> NbQuestions { get; set; }

    public Nullable<int> ID_Document { get; set; }

    public string Version { get; set; }



    public virtual Document Document { get; set; }

    public virtual IntitekUser IntitekUser { get; set; }

    public virtual Qcm Qcm { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<UserQcmReponse> UserQcmReponse { get; set; }

}

}
