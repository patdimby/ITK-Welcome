using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Log;
using System;
using System.Diagnostics;

namespace Intitek.Welcome.Service.Back
{
    public class ConfigService : BaseService, IConfigService
    {
        private readonly ConfigDataAccess _configRepository;

        public ConfigService(ILogger logger) : base(logger)
        {
            _configRepository = new ConfigDataAccess(uow);
        }

        public GetConfigResponse GetConfig(GetConfigRequest request)
        {
            throw new NotImplementedException();
        }

        public GetConfigResponse Get(GetConfigRequest request)
        {
            GetConfigResponse response = new GetConfigResponse();

            try
            {
                var config = _configRepository.FindBy(request.ConfigType);
                if (config != null)
                {
                    response = new GetConfigResponse()
                    {
                        Config = new ConfigDTO()
                        {
                            Id = config.Id,
                            Value = config.Value
                        }
                    };
                }
                return response;

            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Get",
                    ServiceName = "ConfigService",

                }, ex);
                throw ex;
            }
        }

        public UpdateConfigResponse UpdateConfig(UpdateConfigRequest request)
        {
            var response = new UpdateConfigResponse();

            try
            {
                Config config = new Config(request.Config.Id)
                {
                    Id = request.Config.Id,
                    Value = request.Config.Value
                };
                
                Trace.WriteLine($"Repository: {_configRepository}");
                Trace.WriteLine($"config: {config.Id} / {config.Value}");

                _configRepository.Save(config);

                Trace.WriteLine($"J'arrive ici");

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "UpdateConfig",
                    ServiceName = "ConfigService",

                }, ex);
                throw ex;
            }
        }
    }
}
