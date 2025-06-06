﻿using Server.Contracts.Dtos;

namespace Server.Contracts.Endpoints.Customers.Contracts;

public sealed record GetCustomersResponse(IEnumerable<CustomerDto> Customers)
{
    public bool AnyCustomers => Customers.Any();
};