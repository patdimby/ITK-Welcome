using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Infrastructure.UnitOfWork;

namespace Intitek.Welcome.DataAccessUT
{
    /// <summary>
    /// Summary description for UserProfileDataAccessTest
    /// </summary>
    [TestClass]
    public class UserProfileDataAccessTest
    {
        private readonly WelcomeDB _context;
        private readonly IUnitOfWork uow;
        private readonly UserProfileDataAccess _repository;
        private List<ProfileUser> docCategories = new List<ProfileUser>();
        public UserProfileDataAccessTest()
        {
            //
            // TODO: Add constructor logic here
            //
            _context = new WelcomeDB();
            uow = _context;
            _repository = new UserProfileDataAccess(uow);
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
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

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void ShouldGetAllUserProfile()
        {
            //
            // TODO: Add test logic here
            //
        }
    }
}
