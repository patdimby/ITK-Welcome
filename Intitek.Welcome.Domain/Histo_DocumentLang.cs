
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
    
public partial class Histo_DocumentLang : EntityBase<int>, IAggregateRoot
    {
        protected override void Validate()
        {
            throw new NotImplementedException();
        }

        public string Mois { get; set; }

    public int ID_Document { get; set; }

    public int ID_Lang { get; set; }

    public string Name { get; set; }

    public string NomOrigineFichier { get; set; }

    public byte[] Data { get; set; }



    public virtual Histo_Document Histo_Document { get; set; }

}

}
