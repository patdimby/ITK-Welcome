using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Front;

namespace Intitek.Welcome.ServiceUT
{
    [TestClass]
    public class QcmServiceUT
    {
        private ILogger logger = new FileLogger();
        private QcmService _qcmService;
        public QcmServiceUT()
        {
            _qcmService = new QcmService(logger);
        }

        [TestMethod]
        public void ShouldGetQcm()
        {
            var request = new GetQcmRequest() { Id = 1 };
            var response = _qcmService.GetQcm(request);

            Assert.IsTrue(response.Qcm.Qcm.NoteMinimal == 7 && response.Qcm.Questions.Count == 10);
        }

        [TestMethod]
        public void ShouldGetQuestion()
        {
            var request = new GetQuestionRequest() { Id = 1, QcmId = 1 };
            var response = _qcmService.GetQuestion(request);

            Assert.IsTrue(response.Question.Question != null && response.Question.Reponses.Count == 3);
        }

        [TestMethod]
        public void ShouldSaveUserQcm()
        {
        }
    }
}
