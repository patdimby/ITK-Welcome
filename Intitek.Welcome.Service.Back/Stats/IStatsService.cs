using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public interface IStatsService
    {
        List<DocumentDTO> ListDocuments(int idLang, int idDefaultLang);
        List<DocumentDTO> ListDocumentsByDocs(int idLang, int idDefaultLang, List<int> docs);
        List<UserDTO> ListUsersForStat(List<int> usersId, bool bAll, bool bActivity, StatsRequestType statype);
        List<StatistiquesDTO> GetStats(GetStatsRequest request, bool isRelance);
        List<StatistiquesDTO> GetStatsEntity(GetStatsRequest request);
        List<Statistiques> GetEngineerList(GetStatsRequest request, bool onlyLatePeople);
        bool IsRelanceMail(UserDTO user);
    }
}
