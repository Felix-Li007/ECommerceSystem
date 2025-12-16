namespace UserService.API.Services;

using Prometheus;
public class MetricsService : IMetricsService
{
    private readonly Counter _userCreatedCounter;
    private readonly Counter _userDeletedCounter;
    private readonly Counter _loginAttempts;
    private readonly Histogram _requestDuration;
    private readonly Counter _apiCalls;

    public MetricsService()
    {
        _userCreatedCounter = Metrics.CreateCounter(
            "userservice_users_created_total",
            "Total number of users created");

        _userDeletedCounter = Metrics.CreateCounter(
            "userservice_users_deleted_total",
            "Total number of users deleted");

        _loginAttempts = Metrics.CreateCounter(
            "userservice_login_attempts_total",
            "Total number of login attempts",
            new CounterConfiguration
            {
                LabelNames = new[] { "result" }
            });

        _requestDuration = Metrics.CreateHistogram(
            "userservice_request_duration_seconds",
            "Request duration in seconds",
            new HistogramConfiguration
            {
                LabelNames = new[] { "endpoint", "method", "status_code" },
                Buckets = Histogram.ExponentialBuckets(0.001, 2, 10)
            });

        _apiCalls = Metrics.CreateCounter(
            "userservice_api_calls_total",
            "Total API calls",
            new CounterConfiguration
            {
                LabelNames = new[] { "endpoint", "method", "status_code" }
            });
    }

    public void RecordUserCreated()
    {
        _userCreatedCounter.Inc();
    }

    public void RecordUserDeleted()
    {
        _userDeletedCounter.Inc();
    }

    public void RecordLoginAttempt(bool success)
    {
        _loginAttempts.WithLabels(success ? "success" : "failure").Inc();
    }

    public void RecordApiCall(string endpoint, string method, int statusCode, double duration)
    {
        _apiCalls.WithLabels(endpoint, method, statusCode.ToString()).Inc();
        _requestDuration.WithLabels(endpoint, method, statusCode.ToString()).Observe(duration);
    }
}