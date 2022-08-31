using Intitek.Welcome.Domain;
using System;
using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public class QcmDTO
    {

        public int Id { get; set; }
        public int IdLang { get; set; }
        public string Name { get; set; }
        public bool IsDefaultTradName { get; set; }
        public System.DateTime DateCreation { get; set; }
        public Nullable<int> NoteMinimal { get; set; }
        public bool Inactif { get; set; }
        public int NbQuestions { get; set; }
        public bool IsRemovable { get; set; }
        public bool IsUpdatable { get; set; }
        public QcmLang QcmTrad { get; set; }
        public QcmLang DefaultTrad { get; set; }
        public List<QuestionDTO> Questions { get; set; }

    }
}
