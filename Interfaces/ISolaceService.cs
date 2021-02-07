using SolaceSystems.Solclient.Messaging;

namespace SiteWebApi.Interfaces
{
    public interface ISolaceService
    {
         void PublishMessage(string messageText);
    }
}