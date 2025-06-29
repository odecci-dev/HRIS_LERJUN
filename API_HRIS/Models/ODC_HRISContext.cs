﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API_HRIS.Models;

public partial class ODC_HRISContext : DbContext
{
    public ODC_HRISContext(DbContextOptions<ODC_HRISContext> options)
        : base(options)
    {
    }
    public virtual DbSet<TblHolidayModel> TblHolidayModel { get; set; }
    public virtual DbSet<TblEmergencyContactsModel> TblEmergencyContactsModel { get; set; }
    public virtual DbSet<TblAddressInfo> TblAddressInfos { get; set; }

    public virtual DbSet<TblApiTokenModel> TblApiTokenModels { get; set; }

    public virtual DbSet<TblAudittrail> TblAudittrails { get; set; }

    public virtual DbSet<TblDeparmentModel> TblDeparmentModels { get; set; }

    public virtual DbSet<TblModulesModel> TblModulesModels { get; set; }

    public virtual DbSet<TblPayrollType> TblPayrollTypes { get; set; }

    public virtual DbSet<TblPositionModel> TblPositionModels { get; set; }
    public virtual DbSet<TblPositionLevelModel> TblPositionLevelModels { get; set; }

    public virtual DbSet<TblSalaryType> TblSalaryTypes { get; set; }

    public virtual DbSet<TblStatusModel> TblStatusModels { get; set; }

    public virtual DbSet<TblTaskModel> TblTaskModels { get; set; }

    public virtual DbSet<TblTimeLog> TblTimeLogs { get; set; }
    public virtual DbSet<TblNotification> TblTimeLogNotifications { get; set; }
    public virtual DbSet<TblTimeLogStatus> TblTimeLogsStatus { get; set; }
    public virtual DbSet<TblUserType> TblUserTypes { get; set; }

    public virtual DbSet<TblUsersModel> TblUsersModels { get; set; }
    public virtual DbSet<GetAllUserDetailsResult> AllUserDetails { get; set; }
    public virtual DbSet<TblModulesModel> TblModulesModel { get; set; }
    public virtual DbSet<TblScheduleModel> TblScheduleModels { get; set; }
    public virtual DbSet<TblScheduleDayModel> TblScheduleDayModels { get; set; }
    public virtual DbSet<TblEmploymentStatusModel> TblEmploymentStatusModels { get; set; }
    public DbSet<TblCalendarModel> TimeSchedules { get; set; }
    public virtual DbSet<TblEmployeeTypeModel> TblEmployeeTypes { get; set; }
    public virtual DbSet<TblOvertimeModel> TblOvertimeModel { get; set; }
    public virtual DbSet<TblLeaveLedgerModel> TblLeaveLedgerModel { get; set; }
    public virtual DbSet<TblLeaveTypeModel> TblLeaveTypeModel { get; set; }
    public virtual DbSet<TblPayrollPeriodModel> TblPayrollPeriodModel { get; set; }
    public virtual DbSet<TblPayrollModel> TblPayrollModel { get; set; }
    public virtual DbSet<TblDeductionModel> TblDeductionModel { get; set; }
    public virtual DbSet<TblTaxTypeModel> TblTaxTypeModel { get; set; }
    public virtual DbSet<TblTaxModel> TblTaxModel { get; set; }
    public virtual DbSet<TblSSSModel> TblSSSModel { get; set; }
    public virtual DbSet<TblPhilHealthModel> TblPhilHealthModel { get; set; }
    public virtual DbSet<TblPagIbigModel> TblPagIbigModel { get; set; }
    public virtual DbSet<TblPayslipModel> TblPayslipModel { get; set; }
    public virtual DbSet<TblLeaveRequestModel> TblLeaveRequestModel { get; set; }
    public virtual DbSet<tbl_UsersRequiredDocuments> tbl_UsersRequiredDocuments { get; set; }
    public virtual DbSet<TblPayslipGeneratedDate> TblPayslipGeneratedDate { get; set; }
    public List<GetAllUserDetailsResult> GetEmployees()
    {
        return AllUserDetails.FromSqlRaw("EXEC GetAllUserDetails").ToList();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblLeaveRequestModel>()
        .Property(r => r.LeaveRequestNo)
        .HasComputedColumnSql(
            "('LR' + CONVERT(nvarchar(10), [Id]) + FORMAT([Date], 'yyyyMMdd'))",
            stored: false // set to true if you want it stored in the table
        );
        modelBuilder.Entity<TblTimeLog>(entity =>
        {
            entity.ToTable("tbl_TimeLogs");

            entity.Property(e => e.Date).HasColumnType("date");
            entity.Property(e => e.RenderedHours).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TimeIn)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TimeOut)
                .HasMaxLength(50)
                .IsUnicode(false);
        });
        modelBuilder.Entity<TblTimeLogStatus>(entity =>
        {
            entity.ToTable("tbl_TimeLogStatus");
        });
        modelBuilder.Entity<TblEmployeeTypeModel>(entity =>
        {
            entity.ToTable("tbl_EmployeeType");

        });
        modelBuilder.Entity<TblCalendarModel>(entity =>
        {
            entity.ToTable("TblCalendarModel");

        });
        modelBuilder.Entity<TblScheduleModel>(entity =>
        {
            entity.ToTable("tbl_ScheduleModel");

        });
        modelBuilder.Entity<TblEmploymentStatusModel>(entity =>
        {
            entity.ToTable("tbl_EmploymentStatusModel");

        });
        modelBuilder.Entity<TblAddressInfo>(entity =>
        {
            entity.ToTable("tbl_AddressInfo");

            entity.Property(e => e.Barangay)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.CountryOfBirth)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.DateCreated).HasColumnType("date");
            entity.Property(e => e.Municipality)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Province)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Region)
                .HasMaxLength(500)
                .IsUnicode(false);
        });
        modelBuilder.Entity<TblApiTokenModel>(entity =>
        {
            entity.ToTable("tbl_ApiTokenModel");

            entity.Property(e => e.ApiToken).IsUnicode(false);
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.Role).IsUnicode(false);
        });
        modelBuilder.Entity<TblAudittrail>(entity =>
        {
            entity.ToTable("tbl_audittrail");

            entity.Property(e => e.Actions)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.Module)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId)
                .HasMaxLength(250)
                .IsUnicode(false);
        });
        modelBuilder.Entity<TblDeparmentModel>(entity =>
        {
            entity.ToTable("tbl_DeparmentModel");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.DateCreated).HasColumnType("date");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .IsUnicode(false);
        });
        modelBuilder.Entity<TblModulesModel>(entity =>
        {
            entity.ToTable("TblModulesModel");

            entity.Property(e => e.Class).IsUnicode(false);
            entity.Property(e => e.Img).IsUnicode(false);
            entity.Property(e => e.Link).IsUnicode(false);
            entity.Property(e => e.Title).IsUnicode(false);
        });
        modelBuilder.Entity<TblPayrollType>(entity =>
        {
            entity.ToTable("tbl_PayrollType");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DateCreated).HasColumnType("date");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PayrollType)
                .HasMaxLength(50)
                .IsUnicode(false);
        });
        modelBuilder.Entity<TblPositionModel>(entity =>
        {
            entity.ToTable("tbl_PositionModel");

            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.PositionId)
                .HasMaxLength(35)
                .IsUnicode(false)
                .HasComputedColumnSql("((('POS'+'-')+'0')+CONVERT([varchar],[Id],(0)))", false)
                .HasColumnName("PositionID");
        });
        modelBuilder.Entity<TblPositionLevelModel>(entity =>
        {
            entity.ToTable("tbl_PositionLevel");

        });
        modelBuilder.Entity<TblSalaryType>(entity =>
        {
            entity.ToTable("tbl_SalaryType");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DateCreated).HasColumnType("date");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Rate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SalaryType)
                .HasMaxLength(250)
                .IsUnicode(false);
        });
        modelBuilder.Entity<TblStatusModel>(entity =>
        {
            entity.ToTable("tbl_StatusModel");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
        });
        modelBuilder.Entity<TblTaskModel>(entity =>
        {
            entity.ToTable("tbl_TaskModel");

            entity.Property(e => e.DateCreated).HasColumnType("date");
            entity.Property(e => e.DateUpdated).HasColumnType("date");
            entity.Property(e => e.TaskDescription)
                .IsUnicode(false)
                .HasColumnName("Task Description");
            entity.Property(e => e.Title)
                .HasMaxLength(550)
                .IsUnicode(false);
        });
        modelBuilder.Entity<TblNotification>(entity =>
        {
            entity.ToTable("tbl_TimeLogNotification");
            entity.Property(e => e.Date).HasColumnType("date");
            entity.Property(e => e.Notification)
                .IsUnicode(false)
                .HasColumnName("Notification");
            entity.Property(e => e.UserId)
                .IsUnicode(false)
                .HasColumnName("UserId");
            entity.Property(e => e.StatusId)
                .IsUnicode(false)
                .HasColumnName("StatusId");
        });
        modelBuilder.Entity<TblEmergencyContactsModel>(entity =>
        {
            entity.ToTable("tbl_EmergencyContactsModel");


            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Relationship)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
        });
        modelBuilder.Entity<TblUserType>(entity =>
        {
            entity.ToTable("tbl_UserType");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(550)
                .IsUnicode(false);
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateDeleted).HasColumnType("datetime");
            entity.Property(e => e.DateRestored).HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy)
                .HasMaxLength(550)
                .IsUnicode(false);
            entity.Property(e => e.RestoredBy)
                .HasMaxLength(550)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(550)
                .IsUnicode(false);
            entity.Property(e => e.UserType)
                .HasMaxLength(550)
                .IsUnicode(false);
        });
        modelBuilder.Entity<TblUsersModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UsersModel");

            entity.ToTable("tbl_UsersModel");

            entity.Property(e => e.Address).IsUnicode(false);
            entity.Property(e => e.Cno).IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .IsUnicode(false)
                .HasColumnName("Created_By");
            entity.Property(e => e.DateCreated)
                .HasColumnType("date")
                .HasColumnName("Date_Created");
            entity.Property(e => e.DateDeleted)
                .HasColumnType("date")
                .HasColumnName("Date_Deleted");
            entity.Property(e => e.DateRestored)
                .HasColumnType("date")
                .HasColumnName("Date_Restored");
            entity.Property(e => e.DateStarted).HasColumnType("date");
            entity.Property(e => e.DateUpdated)
                .HasColumnType("date")
                .HasColumnName("Date_Updated");
            entity.Property(e => e.DeleteFlag).HasColumnName("Delete_Flag");
            entity.Property(e => e.DeletedBy)
                .IsUnicode(false)
                .HasColumnName("Deleted_By");
            entity.Property(e => e.Email).IsUnicode(false);
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(4000)
                .HasComputedColumnSql("(('ODC-'+CONVERT([varchar],[id]))+format(getdate(),'yyyyMMdd'))", false)
                .HasColumnName("EmployeeID");
            entity.Property(e => e.FilePath).IsUnicode(false);
            entity.Property(e => e.Fname).IsUnicode(false);
            entity.Property(e => e.Fullname).IsUnicode(false);
            entity.Property(e => e.Gender).IsUnicode(false);
            entity.Property(e => e.Jwtoken)
                .IsUnicode(false)
                .HasColumnName("JWToken");
            entity.Property(e => e.Lname).IsUnicode(false);
            entity.Property(e => e.Mname).IsUnicode(false);
            entity.Property(e => e.Password).IsUnicode(false);
            entity.Property(e => e.RememberToken).IsUnicode(false);
            entity.Property(e => e.RestoredBy)
                .IsUnicode(false)
                .HasColumnName("Restored_By");
            entity.Property(e => e.Suffix)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy)
                .IsUnicode(false)
                .HasColumnName("Updated_By");
            entity.Property(e => e.Username).IsUnicode(false);
        });
        OnModelCreatingGeneratedProcedures(modelBuilder);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}