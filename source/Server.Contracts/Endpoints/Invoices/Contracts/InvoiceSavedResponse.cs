﻿using Server.Contracts.Dtos;

namespace Server.Contracts.Endpoints.Invoices.Contracts;

public sealed record InvoiceSavedResponse(InvoiceDto Invoice);
public sealed record InvoiceSavedLegacyResponse(InvoiceLegacyDto InvoiceDto);