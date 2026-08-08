using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using 社員_求人管理アプリ.Models;

namespace 社員_求人管理アプリ.Data;

public class AppDbContext : IdentityDbContext

{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<FavoriteProject> FavoriteProjects { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<ChatRoom> ChatRooms { get; set; }
    public DbSet<ChatMemo> ChatMemos { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    public DbSet<AnnouncementRead> AnnouncementReads { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<EmployeeSkill> EmployeeSkills { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ChatMessage>()
            .HasOne(m => m.Room)
            .WithMany(r => r.Messages)
            .HasForeignKey(m => m.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProjectMember>()
            .HasOne(pm => pm.Project)
            .WithMany(p => p.Members)
            .HasForeignKey(pm => pm.ProjectId);

        modelBuilder.Entity<ProjectMember>()
            .HasOne(pm => pm.Employee)
            .WithMany(e => e.ProjectMembers)
            .HasForeignKey(pm => pm.EmployeeId);

        modelBuilder.Entity<AnnouncementRead>()
            .HasIndex(ar => new
            {
                ar.AnnouncementId,
                ar.EmployeeId
            })
            .IsUnique();

        modelBuilder.Entity<AnnouncementRead>()
            .HasOne(ar => ar.Announcement)
            .WithMany(a => a.AnnouncementReads)
            .HasForeignKey(ar => ar.AnnouncementId);

        modelBuilder.Entity<AnnouncementRead>()
            .HasOne(ar => ar.Employee)
            .WithMany(e => e.AnnouncementReads)
            .HasForeignKey(ar => ar.EmployeeId);

        modelBuilder.Entity<EmployeeSkill>()
            .HasKey(es => es.Id);

        modelBuilder.Entity<EmployeeSkill>()
            .HasIndex(es => new
            {
                es.EmployeeId,
                es.SkillId
            })
            .IsUnique();

        modelBuilder.Entity<EmployeeSkill>()
            .HasOne(es => es.Employee)
            .WithMany(e => e.EmployeeSkills)
            .HasForeignKey(es => es.EmployeeId);

        modelBuilder.Entity<EmployeeSkill>()
            .HasOne(es => es.Skill)
            .WithMany(s => s.EmployeeSkills)
            .HasForeignKey(es => es.SkillId);
    }
}