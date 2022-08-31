namespace Intitek.Welcome.Service.Back
{
    public interface IConfigService
    {
        GetConfigResponse Get(GetConfigRequest request);
        UpdateConfigResponse UpdateConfig(UpdateConfigRequest request);
    }
}
