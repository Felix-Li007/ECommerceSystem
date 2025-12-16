namespace UserService.API.Services;

public interface IMetricsService
{
    void RecordUserCreated();
    void RecordUserDeleted();
    void RecordLoginAttempt(bool success);
    void RecordApiCall(string endpoint, string method, int statusCode, double duration);
}
