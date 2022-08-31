namespace Intitek.Welcome.Service.Back
{
    public interface IQcmService
    {
        int GetAllCount(GetAllQcmRequest request);

        GetAllQcmResponse GetAll(GetAllQcmRequest request);

        GetAllQcmResponse GetAll(int IdLang);

        GetQcmResponse Get(GetQcmRequest request);

        DeleteQcmResponse Delete(DeleteQcmRequest request);

        SaveQcmResponse Save(SaveQcmRequest request);

        GetQuestionResponse GetQuestion(GetQuestionRequest request);

        SaveQuestionResponse SaveQuestion(SaveQuestionRequest request);

        OrderQuestionResponse OrderQuestion(OrderQuestionRequest request);

        DeleteQuestionResponse DeleteQuestion(DeleteQuestionRequest request);

        GetReponseResponse GetReponse(GetReponseRequest request);
        DeleteReponseResponse DeleteReponse(DeleteReponseRequest request);

        bool DeleteIllustration(int id, int lanId);
    }
}
