﻿using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.Customers.Contracts;

public sealed record GetCustomersRequest() : RequestBase(Route)
{
    public const string Route = "api/customers";
};