
#nullable disable
using System;
using System.Collections.Generic;

namespace API_HRIS.Models;

public class StatusReturns
{
    public string? Status { get; set; }
    public string? Message { get; set; }
    public string? JwtToken { get; set; }
    public string? UserType { get; set; }
}