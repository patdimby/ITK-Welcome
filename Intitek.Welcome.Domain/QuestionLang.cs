
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
    
public partial class QuestionLang : EntityBase<int>, IAggregateRoot
    {
        protected override void Validate()
        {
            throw new NotImplementedException();
        }

        public int ID_Question { get; set; }

    public int ID_Lang { get; set; }

    public string TexteQuestion { get; set; }

    public string TexteJustification { get; set; }

    public byte[] Illustration { get; set; }



    public virtual Lang Lang { get; set; }

    public virtual Question Question { get; set; }

}

}
