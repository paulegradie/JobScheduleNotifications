﻿using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.Auth.Contracts;

public record SignOutRequest(string Email) : RequestBase(Route)
{
    public const string Route = "api/auth/sign-out";
};