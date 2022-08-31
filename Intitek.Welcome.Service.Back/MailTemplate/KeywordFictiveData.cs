using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public class KeywordFictiveData
    {
        public string Keyword { get; private set; }
        public string FictiveData { get; private set; }

        public static readonly string DefaultFictiveData = "Donnée fictive";

        public static List<KeywordFictiveData> FictiveDatas
        {
            get
            {
                return new List<KeywordFictiveData>()
                {
                    new KeywordFictiveData(){Keyword = "Agence", FictiveData = "Agence LYON"},
                    new KeywordFictiveData(){Keyword = "Collaborateur", FictiveData = "John Doe"},
                    new KeywordFictiveData(){Keyword = "Entite", FictiveData = "Astek Maurice"},
                    new KeywordFictiveData(){Keyword = "jour", FictiveData = "01-01-2020"},
                    new KeywordFictiveData(){Keyword = "Stats_documents_graphique", FictiveData = "Stat graphique"},
                    new KeywordFictiveData(){Keyword = "Stats_documents_tabulaire", FictiveData = "Stat tabulaire"}
                };
            }
        }
    }
}
