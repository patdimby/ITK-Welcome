using System;
using System.Collections.Generic;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Infrastructure.Histo;
using Intitek.Welcome.Service.Back;
using Astek.Welcome.Batch.Service;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using Intitek.Welcome.UI.Resources;
using System.IO;
using Intitek.Welcome.Infrastructure.Config;
using System.Configuration;

namespace Astek.Welcome.Batch
{
    class Program
    {
        private static BatchService _batchService;
        private static IBatch _service;
       
        static void Main(string[] args)
        {
            var xpath = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ConfigurationFilePath"]);
            var txtfilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ConfigurationFilePath"], "config.txt");
            var binfilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ConfigurationFilePath"], "config.bin");
            if (!File.Exists(binfilename)
               /*&& !File.Exists(Server.MapPath("~/Configs/Config.bin"))*/)
            {
                ConfigurationEncryption.EncryptConfigurationFile(txtfilename, binfilename);
                //File.Delete(txtfilename);
                //log.Info("App_Start...");
            }

            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr-FR");
            string productVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            var logger = new FileLogger();
            _batchService = new BatchService(logger);

            var _batchHisto = new List<BatchBO>();
            var startedAt = DateTime.Now;
            var errMessage = string.Empty;
            var batchId = 0;
            var batchName = string.Empty;
            var reponse = new BatchResponse();
            Synthese synthese = new Synthese();
            //batchName = BatchName.REMINDEMPLOYEES;           
            try
            {
                var templateName = string.Empty;
                if (args.Length > 0)
                {
                    batchName = !string.IsNullOrEmpty(args[0]) ? args[0] : string.Empty;
                }
                if (!(BatchName.SYNCHRONIZEAD.Equals(batchName) || BatchName.REMINDEMPLOYEES.Equals(batchName)
                     || BatchName.REMINDMANAGERS.Equals(batchName) || BatchName.SENDSTATISTICS.Equals(batchName)
                     || BatchName.CRESNAPSHOT.Equals(batchName)))
                {
                    logger.Info(string.Format("Welcome Batch version {0}", productVersion));
                    logger.Error("Le premier argument doit être SynchronizeAD ou RemindEmployees ou RemindManagers ou CreateSnapshot");
                    Environment.Exit(-1);
                }
                var theBatch = _batchService.Get(new GetBatchRequest() { ProgName = batchName });
                if (theBatch.Batch != null)
                {
                    logger.Info(string.Format("Welcome Batch version {0} option {1}", productVersion, batchName));
                    batchId = theBatch.Batch.ID;
                    logger.Info(string.Format("Batch {0} started at {1}....", theBatch.Batch.ProgName, DateTime.Now));

                    switch (batchName)
                    {
                        case BatchName.SYNCHRONIZEAD:
                            _service = new SyncronizeAD();
                            synthese.Subject = string.Format(Resource.mail_synthese_subject, "Synchronisation AD") ;
                            break;
                        case BatchName.REMINDEMPLOYEES:
                            var emailList = string.Empty;
                            //emailList = "aalqouch@groupeastek.fr";
                            //templateName = "TemplateRemindEmployees";
                            if (args.Length > 1 && !string.IsNullOrEmpty(args[1]))
                            {
                                templateName = args[1];
                            }
                            if (args.Length > 2 && !string.IsNullOrEmpty(args[2]))
                            {
                                emailList = args[2];
                            }
                            if (string.IsNullOrEmpty(templateName))
                            {
                                logger.Error("Le deuxième argument (nom du template) ne doit pas être vide");
                                Environment.Exit(-1);
                            }
                            _service = new RemindEmployees(templateName, emailList);
                            synthese.Subject = string.Format(Resource.mail_synthese_subject, "Relance employés");
                            break;
                        case BatchName.SENDSTATISTICS:
                            _service = new SendStatistics();
                            break;
                        case BatchName.REMINDMANAGERS:

                            emailList = string.Empty;
                            //emailList = "christophe.illgen@intitek.fr";
                            //templateName = "TemplateRemindManagers";
                            if (args.Length > 1 && !string.IsNullOrEmpty(args[1]))
                            {
                                templateName = args[1];
                            }
                            if (args.Length > 2 && !string.IsNullOrEmpty(args[2]))
                            {
                                emailList = args[2];
                            }
                            if (string.IsNullOrEmpty(templateName))
                            {
                                logger.Error("Le deuxième argument (nom du template) ne doit pas être vide");
                                Environment.Exit(-1);
                            }
                            _service = new RemindManagers(templateName, emailList);
                            synthese.Subject = string.Format(Resource.mail_synthese_subject, "Relance managers");
                            break;
                        case BatchName.CRESNAPSHOT:
                            var snapshotMonth = string.Empty;
                            if (args.Length > 1 && !string.IsNullOrEmpty(args[1]))
                            {
                                snapshotMonth = args[1];
                            }


                            _service = new CreateSnapshot(snapshotMonth);
                            break;

                    }

                    if (_service != null)
                    {
                        reponse = _service.Execute(new BatchRequest() { ID_Batch = batchId }, synthese);
                    }

                    var endedAt = DateTime.Now;
                    _batchService.HistoBatch._batches.Add(new BatchBO()
                    {
                        ID_Batch = batchId,
                        Start = startedAt,
                        Finish = endedAt,
                        Message = string.Join("<br/>", reponse.Errors),
                        ReturnCode = reponse.Result,
                    });
                }

            }
            catch (Exception ex)
            {
                reponse.Errors.Add(ex.Message);
                var abortedAt = DateTime.Now;
                _batchService.HistoBatch._batches.Add(new BatchBO()
                {
                    ID_Batch = batchId,
                    Start = startedAt,
                    Finish = abortedAt,
                    Message = string.Join("<br/>", ex.Message),
                    ReturnCode = reponse.Result,
                });
                synthese.Errors.Add(string.Format("Erreur serveur : {0}", ex.Message));
            }
            finally
            {
                _batchService.Historize();
                logger.Info(string.Format("Batch {3} execution ended at {0} with error code: {1} - {2}....", DateTime.Now, reponse.Result, string.Join("\n", reponse.Errors), batchName));
                synthese.Debut = startedAt;
                synthese.Fin = DateTime.Now;
           }
            //Send synthese
            if (BatchName.REMINDEMPLOYEES.Equals(batchName)
                    || BatchName.REMINDMANAGERS.Equals(batchName))
            {
                try
                {
                    synthese.SendMail(batchId);
                }
                catch (Exception ex)
                {
                    var msg = string.Format("Erreur d'envoi mail synthese : {0}", ex.Message);
                    logger.Info(msg);
                }
                finally
                {
                    logger.Info(string.Format("========================SYNTHESE============================{0}{1}", Environment.NewLine, synthese.GetString(false)));
                }
            }
            else if (BatchName.SYNCHRONIZEAD.Equals(batchName))
            {
                try
                {
                    synthese.SendMailSynchronizeAD(batchId);
                }
                catch (Exception ex)
                {
                    var msg = string.Format("Erreur d'envoi mail synthese : {0}", ex.Message);
                    logger.Info(msg);
                }
            }
            
        }

       
    }
}
