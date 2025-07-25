using DiamondAssessmentSystem.Infrastructure.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace DiamondAssessmentSystem.Infrastructure.Models;

public partial class DiamondAssessmentDbContext : IdentityDbContext<User>
{
    public DiamondAssessmentDbContext()
    {
    }

    public DiamondAssessmentDbContext(DbContextOptions<DiamondAssessmentDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<Certificate> Certificates { get; set; }

    public MessageType MessageType { get; set; }

    public virtual DbSet<ChatLog> ChatLogs { get; set; }
    public virtual DbSet<Conversation> Conversations { get; set; }

    public virtual DbSet<CommitmentRecord> CommitmentRecords { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Result> Results { get; set; }

    public virtual DbSet<SealingRecord> SealingRecords { get; set; }

    public virtual DbSet<ServicePrice> ServicePrices { get; set; }

    public virtual DbSet<ServicePriceAudit> ServicePriceAudits { get; set; }

    private string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
             .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
        var strConn = config["ConnectionStrings:DefaultConnection"];

        return strConn;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(GetConnectionString());
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__Blogs__2975AA28461437B8");

            entity.Property(e => e.BlogId).HasColumnName("blog_id");
            entity.Property(e => e.BlogType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("blog_type");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.Employee).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Blogs__employee___6EF57B66");
        });

        modelBuilder.Entity<Certificate>(entity =>
        {
            entity.HasKey(e => e.CertificateId).HasName("PK__Certific__E2256D3153E19E06");

            entity.Property(e => e.CertificateId).HasColumnName("certificate_id");
            entity.Property(e => e.CertificateNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("certificate_number");
            entity.Property(e => e.IssueDate).HasColumnName("issue_date");
            entity.Property(e => e.ResultId).HasColumnName("result_id");

            entity.HasOne(d => d.Result).WithMany(p => p.Certificates)
                .HasForeignKey(d => d.ResultId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Certifica__resul__5812160E");
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.ConversationId);

            entity.Property(e => e.ConversationId)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.Status)
                  .HasMaxLength(20)
                  .HasDefaultValue("open");

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.Conversations)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Employee)
                  .WithMany(e => e.Conversations)
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<ChatLog>(entity =>
        {
            entity.HasKey(e => e.ChatId);

            entity.Property(e => e.ChatId)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.MessageType)
                  .HasConversion<string>()
                  .HasMaxLength(20);

            entity.Property(e => e.SenderRole)
                  .HasConversion<string>()
                  .HasMaxLength(20);

            entity.Property(e => e.SentAt)
                  .HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Conversation)
                  .WithMany(c => c.ChatLogs)
                  .HasForeignKey(e => e.ConversationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CommitmentRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("PK__Commitme__BFCFB4DDB991787C");

            entity.ToTable("Commitment_records");

            entity.Property(e => e.RecordId).HasColumnName("record_id");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
            entity.Property(e => e.ApprovedDate)
                .HasColumnType("datetime")
                .HasColumnName("approved_date");
            entity.Property(e => e.CommitDate).HasColumnName("commit_date");
            entity.Property(e => e.CommitmentReason)
                .HasMaxLength(255)
                .HasColumnName("commitment_reason");
            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.CommitmentRecords)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__Commitmen__appro__5BE2A6F2");

            entity.HasOne(d => d.Request).WithMany(p => p.CommitmentRecords)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Commitmen__reque__5AEE82B9");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__CD65CB85428BDAEC");

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.Idcard)
                .HasColumnType("numeric(20, 0)")
                .HasColumnName("IDcard");
            entity.Property(e => e.TaxCode)
                .HasMaxLength(20)
                .HasColumnName("tax_code");
            entity.Property(e => e.UnitName)
                .HasMaxLength(50)
                .HasColumnName("unit_name");
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(450)
                .HasColumnName("userId");

            entity.HasOne(e => e.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Customer_User");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__C52E0BA8D6606125");

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Salary).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(450)
                .HasColumnName("userId");

            entity.HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Employee_User");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__46596229F2D54AB0");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.OrderDate).HasColumnName("order_date");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_price");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__customer__6754599E");

            entity.HasOne(d => d.Service).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__service___68487DD7");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__ED1FC9EAE8D291C7");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.Method)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("method");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PaymentDate).HasColumnName("payment_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__order___6B24EA82");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Requests__18D3B90F425A5378");

            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.RequestDate).HasColumnName("request_date");
            entity.Property(e => e.RequestType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("request_type");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Customer).WithMany(p => p.Requests)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Requests__custom__5070F446");

            entity.HasOne(d => d.Employee).WithMany(p => p.Requests)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Requests__employ__5165187F");

            entity.HasOne(d => d.Service).WithMany(p => p.Requests)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Requests__servic__52593CB8");
        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__Results__AFB3C316E0E04CA2");

            entity.Property(e => e.ResultId).HasColumnName("result_id");
            entity.Property(e => e.CaratWeight)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("carat_weight");
            entity.Property(e => e.Clarity)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("clarity");
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("color");
            entity.Property(e => e.Cut)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cut");
            entity.Property(e => e.DiamondId).HasColumnName("diamond_id");
            entity.Property(e => e.DiamondOrigin)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("diamond_origin");
            entity.Property(e => e.Fluorescence)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("fluorescence");
            entity.Property(e => e.Measurements)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("measurements");
            entity.Property(e => e.Polish)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("polish");
            entity.Property(e => e.Proportions)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("proportions");
            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.Shape)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("shape");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.Symmetry)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("symmetry");

            entity.HasOne(d => d.Request).WithMany(p => p.Results)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Results__request__5535A963");
        });

        modelBuilder.Entity<SealingRecord>(entity =>
        {
            entity.HasKey(e => e.SealingId).HasName("PK__Sealing___B2156BB833332E8B");

            entity.ToTable("Sealing_records");

            entity.Property(e => e.SealingId).HasColumnName("sealing_id");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
            entity.Property(e => e.ApprovedDate)
                .HasColumnType("datetime")
                .HasColumnName("approved_date");
            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.SealDate).HasColumnName("seal_date");
            entity.Property(e => e.SealingReason)
                .HasMaxLength(255)
                .HasColumnName("sealing_reason");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.SealingRecords)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__Sealing_r__appro__5FB337D6");

            entity.HasOne(d => d.Request).WithMany(p => p.SealingRecords)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sealing_r__reque__5EBF139D");
        });

        modelBuilder.Entity<ServicePrice>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Service___3E0DB8AFC9383DFD");

            entity.ToTable("Service_prices");

            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ServiceType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("service_type");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Employee).WithMany(p => p.ServicePrices)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Service_p__emplo__4D94879B");
        });

        modelBuilder.Entity<ServicePriceAudit>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("PK__Service___5AF33E33214B811D");

            entity.ToTable("Service_price_audit");

            entity.Property(e => e.AuditId).HasColumnName("audit_id");
            entity.Property(e => e.ActionType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("action_type");
            entity.Property(e => e.ChangeDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("change_date");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.NewDuration).HasColumnName("new_duration");
            entity.Property(e => e.NewPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("new_price");
            entity.Property(e => e.OldDuration).HasColumnName("old_duration");
            entity.Property(e => e.OldPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("old_price");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.ServiceType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("service_type");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Employee).WithMany(p => p.ServicePriceAudits)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Service_p__emplo__6383C8BA");

            entity.HasOne(d => d.Service).WithMany(p => p.ServicePriceAudits)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__Service_p__servi__6477ECF3");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}