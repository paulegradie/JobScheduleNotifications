namespace Api.Infrastructure.DbTables.OrganizationModels;

public enum OrganizationRole
{
    Owner, // full control—only Owners can promote other users
    Admin, // can CRUD customers & schedules
    Employee // can CRUD a subset of customers & schedules
}