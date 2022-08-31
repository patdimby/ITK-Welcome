using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.Infrastructure.Log;
using System.Diagnostics;

namespace Intitek.Welcome.ServiceUT
{
    /// <summary>
    /// Description résumée pour MailTemplateServiceUT
    /// </summary>
    [TestClass]
    public class MailTemplateServiceUT
    {
        private ILogger logger = new FileLogger();
        private MailTemplateService _mailTemplateService;

        public MailTemplateServiceUT()
        {
            _mailTemplateService = new MailTemplateService(logger);
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Obtient ou définit le contexte de test qui fournit
        ///des informations sur la série de tests active, ainsi que ses fonctionnalités.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Attributs de tests supplémentaires
        //
        // Vous pouvez utiliser les attributs supplémentaires suivants lorsque vous écrivez vos tests :
        //
        // Utilisez ClassInitialize pour exécuter du code avant d'exécuter le premier test de la classe
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Utilisez ClassCleanup pour exécuter du code une fois que tous les tests d'une classe ont été exécutés
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Utilisez TestInitialize pour exécuter du code avant d'exécuter chaque test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Utilisez TestCleanup pour exécuter du code après que chaque test a été exécuté
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void GetUnrecognizedKeywords()
        {
            List<string> expected = new List<string>() { "blabla", " fdsqjk[dfqsm, d!" };

            string content = "[blabla] fdqsjfdqsf [ fdsqjk[dfqsm, d!] [Collaborateur] dfq [ sfdqs";

            List<string> unrecognizedKeywords = _mailTemplateService.GetUnrecognizedKeywords(content);

            Assert.IsTrue(unrecognizedKeywords.Count == 2);
            CollectionAssert.AreEqual(expected, unrecognizedKeywords);
        }

        [TestMethod]
        public void GetMailPreview()
        {
            string expected = "Agence LYON fdqsjfdqsf [ fdsqjk[dfqsm, d!] John Doe dfq [ sfdqs";

            string content = "[Agence] fdqsjfdqsf [ fdsqjk[dfqsm, d!] [Collaborateur] dfq [ sfdqs";

            string preview = _mailTemplateService.GetMailPreview(content);

            System.Diagnostics.Debug.WriteLine(preview);

            Assert.AreEqual(expected, preview);
        }
    }
}
