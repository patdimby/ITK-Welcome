using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Intitek.Welcome.Service.Front
{
    public static class Extension
    {
        public static IEnumerable<QuestionDTO> RandomizeQuestion(this IEnumerable<QuestionDTO> questions, int questionToLoad)
        {
            var randowQuestions = new List<QuestionDTO>();
            var sourceQuestions = questions.OrderBy(q => q.Question.Id).ToList();

            int nbQuestions = (sourceQuestions.Count < questionToLoad ? sourceQuestions.Count : questionToLoad);
            //Random rnd = new Random();
            for (var i = 1; i <= nbQuestions; i++)
            {
                var rnd = new byte[4];
                using (var rng = new RNGCryptoServiceProvider())
                    rng.GetBytes(rnd);
                var j = Math.Abs(BitConverter.ToInt32(rnd, 0));
                var ind = Convert.ToInt32(j % (sourceQuestions.Count));
                //var ind = rnd.Next(0, sourceQuestions.Count - 1);
                var question = sourceQuestions[ind];
                //RandomizeReponse(question.Reponses);
                randowQuestions.Add(question);
                sourceQuestions.Remove(question);

            }
            return randowQuestions;
        }

        public static IEnumerable<ReponseDTO> RandomizeReponse(this IEnumerable<ReponseDTO> reponses)
        {
            var randowReponses = new List<ReponseDTO>();
            var sourceReponses = reponses.ToArray();

            int nbReponses = reponses.Count();
            //Random rnd = new Random();
            for (var i = 1; i <= nbReponses; i++)
            {
                //var ind = rnd.Next(0, sourceReponses.Length - 1);
                var rnd = new byte[4];
                using (var rng = new RNGCryptoServiceProvider())
                    rng.GetBytes(rnd);
                var j = Math.Abs(BitConverter.ToInt32(rnd, 0));
                var ind = Convert.ToInt32(j % (sourceReponses.Length));
                var reponse = sourceReponses[ind];
                randowReponses.Add(reponse);
                sourceReponses = sourceReponses.Where((source, index) => index != ind).ToArray();

            }
            return randowReponses;
        }


    }
}
