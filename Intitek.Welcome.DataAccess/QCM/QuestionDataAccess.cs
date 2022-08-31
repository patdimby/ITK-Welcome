using System.Data.Entity;
using System.Linq;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;


namespace Intitek.Welcome.DataAccess
{
    public class QuestionDataAccess : Repository<Question, int>, IQuestionDataAccess
    {
        public QuestionDataAccess(IUnitOfWork uow) : base(uow)
        {
        }

        //public int CountRightResponse(int id)
        //{
        //    Question question = base.Context.Question.Include("Reponse").First(q => q.Id == id);
        //    return question.Reponse.Count(r => r.IsRight);

        //}

        public int CountRightResponse(int id)
        {
            Question question = base.Context.Question.Include("Reponse")
                .Where(q => q.inactif == 0)
                .First(q => q.Id == id);
            return question.Reponse.Count(r => r.IsRight && r.inactif == null);

        }

        public void Update(Question question, int idLang = 0)
        {
            MergeEntityToContext(question, idLang);
            UnitOfWork.SaveChanges();
        }

        private void MergeEntityToContext(Question question, int idLang)
        {

            Question questionToUpdate = base.Context.Question.FirstOrDefault(q => q.Id == question.Id);

            if (questionToUpdate != null)
            {
                questionToUpdate.Id_Qcm = question.Id_Qcm;
                questionToUpdate.OrdreQuestion = question.OrdreQuestion;
                questionToUpdate.Photo = question.Photo;               
                questionToUpdate.inactif = question.inactif;
                questionToUpdate.QuestionLang = base.Context.QuestionLang.Where(r => r.ID_Question == question.Id).ToList();
                questionToUpdate.Reponse = base.Context.Reponse.Where(r => r.ID_Question == question.Id).ToList();

                foreach (QuestionLang lang in question.QuestionLang)
                {
                    //Comparaison SOurce du context par rapport à l'objet envoyé             
                    if (questionToUpdate.QuestionLang.Any(q => q.ID_Question == lang.ID_Question && q.ID_Lang == idLang))
                    {
                        QuestionLang qlToUpdate = base.Context.QuestionLang.FirstOrDefault(q => q.ID_Question == lang.ID_Question && q.ID_Lang == idLang);
                        if (qlToUpdate != null)
                        {
                            qlToUpdate.ID_Question = lang.ID_Question;
                            qlToUpdate.TexteJustification = lang.TexteJustification;
                            qlToUpdate.TexteQuestion = lang.TexteQuestion;
                            //reponseToUpdate.Texte = rep.Texte;
                            if(lang.Illustration != null) qlToUpdate.Illustration = lang.Illustration;
                        }
                    }
                    else
                    {
                        this.Context.Entry<QuestionLang>(lang).State = EntityState.Added;
                    }
                }


                foreach (Reponse rep in question.Reponse)
                {

                    //Comparaison SOurce du context par rapport à l'objet envoyé             
                    if (questionToUpdate.Reponse.Any(r => r.Id == rep.Id && rep.Id > 0))
                    //if (base.Context.Reponse.Any(r => r.Id == rep.Id))
                    {
                        Reponse reponseToUpdate = base.Context.Reponse.FirstOrDefault(r => r.Id == rep.Id);
                        if (reponseToUpdate != null)
                        {
                            reponseToUpdate.ID_Question = rep.ID_Question;
                            reponseToUpdate.IsRight = rep.IsRight;
                            reponseToUpdate.OrdreReponse = rep.OrdreReponse;
                            foreach (var rl in rep.ReponseLang)
                            {

                                ReponseLang rlToUpdate = base.Context.ReponseLang.FirstOrDefault(r => r.ID_Reponse == rep.Id && r.ID_Lang == idLang);
                                if (rlToUpdate != null)
                                {
                                    rlToUpdate.ID_Lang = idLang;
                                    rlToUpdate.ID_Reponse = reponseToUpdate.Id;
                                    rlToUpdate.Texte = rl.Texte;
                                }
                                else
                                {
                                    this.Context.Entry<ReponseLang>(rl).State = EntityState.Added;
                                }
                            }
                        }

                    }
                    else if (rep.Id == 0)
                    {

                        this.Context.Entry<Reponse>(rep).State = EntityState.Added;
                    }
                    else
                    {
                        this.Context.Entry<Reponse>(rep).State = EntityState.Added;
                    }
                }

            }
        }
    }
}
