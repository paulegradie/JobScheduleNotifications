using System.ComponentModel.DataAnnotations;
using Api.Infrastructure.EntityFramework;

namespace Api.Infrastructure.DbTables;

[DatabaseModel]
public class UserOrganizationRecord
{
#pragma warning disable CS8618
    public UserOrganizationRecord()
#pragma warning restore CS8618
    {
    }

    public UserOrganizationRecord(ApplicationUserRecord userRecord, OrganizationRecord organizationRecord)
    {
        ApplicationUserRecord = userRecord;
        OrganizationRecord = organizationRecord;
    }

    [Key]
    public string ApplicationUserId { get; set; }
    public ApplicationUserRecord ApplicationUserRecord { get; set; }
    public int OrganizationId { get; set; }
    public OrganizationRecord OrganizationRecord { get; set; }
}