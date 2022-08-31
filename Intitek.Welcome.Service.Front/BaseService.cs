using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Config;
using Intitek.Welcome.Infrastructure.Histo;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Infrastructure.UnitOfWork;

namespace Intitek.Welcome.Service.Front
{
    public abstract class BaseService: IDisposable
    {
        protected readonly WelcomeDB _context;
        protected readonly IUnitOfWork uow;
        protected ILogger _logger;
        protected IHistorize _histoActionFO;
        protected Infrastructure.Config.Config config;

        public BaseService(ILogger logger)
        {
            var xpath = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ConfigurationFilePath"]);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ConfigurationFilePath"], "config.bin");
           // var path = Path.Combine(ConfigurationManager.AppSettings["ConfigurationFilePath"], "config.bin");
            config = Infrastructure.Config.Config.Deserialize(path);
            _context = new WelcomeDB(ConfigurationEncryption.DecryptConfig(config.DBServer),
                    ConfigurationEncryption.DecryptConfig(config.DBName),
                    ConfigurationEncryption.DecryptConfig(config.DBUserID),
                    ConfigurationEncryption.DecryptConfig(config.DBPassword));
            uow = _context;
            _logger = logger;
        }

        /// <summary>
        /// path pour la classe System.Link.Dynamic pour reconnaitre la classe EdmxFunction
        /// </summary>
        protected void DynamicLinkPatch()
        {
            //path for System.Link.Dynamic pour reconnaitre la classe EdmxFunction
            var type = typeof(DynamicQueryable).Assembly.GetType("System.Linq.Dynamic.ExpressionParser");
            FieldInfo field = type.GetField("predefinedTypes", BindingFlags.Static | BindingFlags.NonPublic);
            Type[] predefinedTypes = (Type[])field.GetValue(null);
            if (!predefinedTypes.Contains(typeof(EdmxFunction)))
            {
                var length = predefinedTypes.Length + 2;
                Array.Resize(ref predefinedTypes, length);
                predefinedTypes[length - 2] = typeof(EdmxFunction);
                predefinedTypes[length - 1] = typeof(DbFunctions);
                field.SetValue(null, predefinedTypes);
            }

        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
