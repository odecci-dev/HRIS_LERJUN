﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace API_HRIS.Models;

public partial class tbl_UsersRequiredDocuments
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string? FileType { get; set; }
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public bool? isDeleted { get; set; }
}