using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Config;
using Intitek.Welcome.Infrastructure.Helpers;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Astek.Welcome.Batch.Service
{
    public class CreateSnapshot : IBatch, IDisposable
    {
        private string _month;
        private WelcomeDB Context;
        private readonly ILogger _logger;

        public CreateSnapshot(string month = "")
        {
            _month = month;
            if(string.IsNullOrEmpty(month))
            {
                _month = DateTime.Now.ToString("yyyy-MM");
            }

            Intitek.Welcome.Infrastructure.Config.Config config;
            var binfilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ConfigurationFilePath"], "config.bin");
            if (!File.Exists(binfilename))
            {
                var txtfilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ConfigurationFilePath"], "config.txt");

                if (File.Exists(txtfilename)
                   /*&& !File.Exists(Server.MapPath("~/Configs/Config.bin"))*/)
                {

                    ConfigurationEncryption.EncryptConfigurationFile(txtfilename, binfilename);
                }
            }

            config = Intitek.Welcome.Infrastructure.Config.Config.Deserialize(binfilename);

            Context = new WelcomeDB(ConfigurationEncryption.DecryptConfig(config.DBServer),
                ConfigurationEncryption.DecryptConfig(config.DBName),
                ConfigurationEncryption.DecryptConfig(config.DBUserID),
                ConfigurationEncryption.DecryptConfig(config.DBPassword));

            this._logger = new FileLogger();
        }

        public BatchResponse Execute(BatchRequest request, Synthese synthese)
        {
            var response = new BatchResponse()
            {
                Result = 0,
                Errors = new List<string>()
            };

            var monthT = new SqlParameter("@Mois", _month);

            var nbUsers = new SqlParameter("@NB_Users", SqlDbType.Int);
            nbUsers.Direction = ParameterDirection.Output;

            var nbDocs = new SqlParameter("@NB_Docs", SqlDbType.Int);
            nbDocs.Direction = ParameterDirection.Output;

            var sql = "EXEC p_CreSnapshot @Mois, @NB_Users OUT, @NB_Docs OUT";
            var data = Context.Database.SqlQuery<object>(sql, monthT, nbUsers, nbDocs);

            // Read the results so that the output variables are accessible
            var item = data.FirstOrDefault();

            var retNbDocs = (int)nbDocs.Value;
            var retNbUsers = (int)nbUsers.Value;

            _logger.Info(string.Format("Nombre d'utilisateurs historisés : {0}, Nombre de documents historisés : {1}", retNbUsers, retNbDocs));

            response.Errors.Add(string.Format("Nombre de documents historisés : {0}", retNbDocs));
            response.Errors.Add(string.Format("Nombre d'utilisateurs historisés : {0}", retNbUsers));

            return response;
        }

        public void Dispose()
        {
            ((IDisposable)Context).Dispose();
        }
    }
}
