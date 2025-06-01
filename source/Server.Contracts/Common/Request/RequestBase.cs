using Server.Contracts.Endpoints;

namespace Server.Contracts.Common.Request;

public abstract record RequestBase(string RouteInternal)
{
    protected virtual ApiRoute GetApiRoute()
    {
        return new ApiRoute(RouteInternal);
    }

    public string ApiRoute => GetApiRoute().ToString();

    protected const string CustomerIdSegmentParam = "{customerId}";
    protected const string JobDefinitionIdSegmentParam = "{jobDefinitionId}";
    protected const string JobOccurenceIdSegmentParam = "{jobOccurenceId}";
    protected const string JobReminderIdSegmentParam = "{jobReminderId}";
    
    // JobOccurrence
    protected const string JobOccurrenceIsSentQueryParam = "isSent";
}