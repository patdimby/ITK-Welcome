
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
    
public partial class Histo_UserQcm : EntityBase<int>, IAggregateRoot
    {
        protected override void Validate()
        {
            throw new NotImplementedException();
        }

        public string Mois { get; set; }

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



    public virtual Histo_Document Histo_Document { get; set; }

    public virtual Histo_IntitekUser Histo_IntitekUser { get; set; }

}

}
