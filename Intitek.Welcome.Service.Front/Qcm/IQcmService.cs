namespace Intitek.Welcome.Service.Front
{
    public interface IQcmService
    {
        GetQcmResponse GetQcm(GetQcmRequest request);

        GetQuestionResponse GetQuestion(GetQuestionRequest request);
        SaveUserQcmResponse SaveUserQcm(SaveUserQcmRequest request);

        //GetUserQcmResultResponse GetUserQcmResult(GetUserQcmResultRequest request);
        GetUserQcmResultResponse GetUserQcmResult(GetUserQcmResultRequest request, int userId);

        CheckUserQcmResponse CheckUserQcm(CheckUserQcmRequest request);
    }
}
