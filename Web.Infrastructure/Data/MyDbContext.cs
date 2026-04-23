using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Domain.Entities.RBAC;

namespace Web.Infrastructure.Data
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public partial class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            // ── UserRole 外键 & 唯一索引 ──────────────────────
            mb.Entity<UserRole>()
                .HasOne<User>().WithMany().HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            mb.Entity<UserRole>()
                .HasOne<Role>().WithMany().HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            mb.Entity<UserRole>()
                .HasIndex(ur => new { ur.UserId, ur.RoleId, ur.DeleteId }).IsUnique();

            // ── RolePermission 外键 & 唯一索引 ────────────────
            mb.Entity<RolePermission>()
                .HasOne<Role>().WithMany().HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            mb.Entity<RolePermission>()
                .HasOne<Permission>().WithMany().HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
            mb.Entity<RolePermission>()
                .HasIndex(rp => new { rp.RoleId, rp.PermissionId, rp.DeleteId }).IsUnique();

            // ── 主表唯一索引 ──────────────────────────────────
            mb.Entity<User>().HasIndex(u => new { u.Username, u.DeleteId }).IsUnique();
            mb.Entity<User>().HasIndex(u => new { u.Email, u.DeleteId }).IsUnique();
            mb.Entity<Role>().HasIndex(r => new { r.Name, r.DeleteId }).IsUnique();
            mb.Entity<Permission>().HasIndex(p => new { p.Name, p.DeleteId }).IsUnique();
        }
    }
}
