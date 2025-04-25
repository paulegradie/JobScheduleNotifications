using Server.Client.Base;
using Server.Contracts.Client.Endpoints;
using Server.Contracts.Client.Endpoints.ScheduledJobs;
using Server.Contracts.Client.Endpoints.ScheduledJobs.Contracts;

namespace Server.Client.Endpoints;

  /// <summary>
    /// HTTP-based implementation of <see cref="IScheduledJobsEndpoint"/> for calling the Scheduled Jobs API.
    /// </summary>
    public class ScheduledJobsEndpoint : EndpointBase, IScheduledJobsEndpoint
    {
        private readonly HttpClient _http;

        public ScheduledJobsEndpoint(HttpClient http)
        {
            _http = http;
        }

        public async Task<OperationResult<ListJobDefinitionsByCustomerIdResponse>> ListJobDefinitionsAsync(
            ListJobDefinitionsByCustomerIdRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _http.GetAsync(request.GetApiRoute().ToString(), cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<ListJobDefinitionsByCustomerIdResponse>(cancellationToken: cancellationToken);
                return OperationResult<ListJobDefinitionsByCustomerIdResponse>.Success(data!);
            }
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: cancellationToken);
            return OperationResult<ListJobDefinitionsByCustomerIdResponse>.Failure(error!);
        }

        public async Task<OperationResult<GetScheduledJobDefinitionByIdResponse>> GetScheduledJobDefinitionByIdAsync(
            GetScheduledJobDefinitionByIdRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _http.GetAsync(request.GetApiRoute().ToString(), cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<GetScheduledJobDefinitionByIdResponse>(cancellationToken: cancellationToken);
                return OperationResult<GetScheduledJobDefinitionByIdResponse>.Success(data!);
            }
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: cancellationToken);
            return OperationResult<GetScheduledJobDefinitionByIdResponse>.Failure(error!);
        }

        public async Task<OperationResult<GetNextScheduledJobRunResponse>> GetNextScheduledJobRunAsync(
            GetNextScheduledJobRunRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _http.GetAsync(request.GetApiRoute().ToString(), cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<GetNextScheduledJobRunResponse>(cancellationToken: cancellationToken);
                return OperationResult<GetNextScheduledJobRunResponse>.Success(data!);
            }
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: cancellationToken);
            return OperationResult<GetNextScheduledJobRunResponse>.Failure(error!);
        }

        public async Task<OperationResult<CreateScheduledJobDefinitionResponse>> CreateScheduledJobDefinitionAsync(
            CreateScheduledJobDefinitionRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _http.PostAsJsonAsync(
                request.GetApiRoute().ToString(),
                request,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<CreateScheduledJobDefinitionResponse>(cancellationToken: cancellationToken);
                return OperationResult<CreateScheduledJobDefinitionResponse>.Success(data!);
            }
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: cancellationToken);
            return OperationResult<CreateScheduledJobDefinitionResponse>.Failure(error!);
        }

        public async Task<OperationResult<UpdateScheduledJobDefinitionResponse>> UpdateScheduledJobDefinitionAsync(
            UpdateScheduledJobDefinitionRequest request,
            CancellationToken cancellationToken)
        {
            var message = new HttpRequestMessage(
                new HttpMethod("PATCH"),
                request.GetApiRoute().ToString())
            {
                Content = JsonContent.Create(request)
            };

            var response = await _http.SendAsync(message, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<UpdateScheduledJobDefinitionResponse>(cancellationToken: cancellationToken);
                return OperationResult<UpdateScheduledJobDefinitionResponse>.Success(data!);
            }
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: cancellationToken);
            return OperationResult<UpdateScheduledJobDefinitionResponse>.Failure(error!);
        }
    }