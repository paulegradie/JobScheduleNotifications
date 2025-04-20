namespace JobScheduleNotifications.Contracts.Customers;

public class CustomerListDto
{
    public IEnumerable<CustomerDto> Customers { get; set; } = new List<CustomerDto>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
} 