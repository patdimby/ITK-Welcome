using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Infrastructure.UnitOfWork;

namespace Intitek.Welcome.DataAccessUT
{
    [TestClass]
    public class QcmDataAccessTest
    {
        private readonly WelcomeDB _context;
        private readonly IUnitOfWork uow;
        private readonly QcmDataAccess _repository;
        private List<Qcm> docCategories = new List<Qcm>();
        public QcmDataAccessTest()
        {
            _context = new WelcomeDB();
            uow = _context;
            _repository = new QcmDataAccess(uow);

        }

        [TestMethod]
        public void ShouldGetAllQcm()
        {
            var all = _repository.FindAll().ToList();
            Assert.IsTrue(all.Count() == 5);
        }

        [TestMethod]
        public void ShouldGetQcm()
        {
            var qcm = _repository.FindBy(2);
            Assert.IsTrue(true);
        }
    }
}
