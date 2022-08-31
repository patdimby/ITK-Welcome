using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public class SaveProfilRequest
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public List<int> DocsList { get; set; }
        public List<DocCheckState> DocsAffected { get; set; }
        public int ID_IntitekUser { get; set; }
        public void InitAffectDocs()
        {
            this.DocsAffected = new List<DocCheckState>();
        }
        public void ToAffectDocs(DocCheckState state)
        {
            if (DocsAffected == null)
                DocsAffected = new List<DocCheckState>();
            if (!DocsAffected.Contains(state))
            {
                DocsAffected.Add(state);
            }
            else
            {
                if (state.OldCheckState == state.NewCheckState)
                {
                    DocsAffected.Remove(state);
                }
                else
                {
                    DocsAffected.Find(x => x.DocId == state.DocId).NewCheckState = state.NewCheckState;
                }
            }
        }
    }
    
}
