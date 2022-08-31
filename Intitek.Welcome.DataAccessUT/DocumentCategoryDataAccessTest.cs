using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Infrastructure.UnitOfWork;

namespace Intitek.Welcome.DataAccessUT
{
    [TestClass]
    public class DocumentCategoryDataAccessTest
    {
        private readonly WelcomeDB _context;
        private readonly IUnitOfWork uow;
        private readonly DocumentCategoryDataAccess _repository;
        private List<DocumentCategory> docCategories = new List<DocumentCategory>();
        public DocumentCategoryDataAccessTest()
        {
            _context = new WelcomeDB();
            uow =  _context;
            _repository = new DocumentCategoryDataAccess(uow);
           
        }

        [TestMethod]
        public void ShouldGetAllDocumentCategory()
        {
            var all = _repository.FindAll();
            Assert.IsTrue(all.Count() == 0);
        
        }

        [TestMethod]
        public void ShouldAddNewDocumentCategory()
        {
            var docCategaory = new DocumentCategory()
            {
                //Name = "Sécurité",
                OrdreCategory = 1,             
                //ID = 1
            };

            _repository.Add(docCategaory);
            docCategories = _repository.FindAll().ToList();
            Assert.IsTrue(docCategories.Contains(docCategaory));
        }

    }
}
