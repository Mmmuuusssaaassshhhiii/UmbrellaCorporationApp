﻿using Microsoft.EntityFrameworkCore;
using UmbrellaCorp.Models;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorp.Data
{
    public class UmbrellaDbContext : DbContext
    {
        public UmbrellaDbContext(DbContextOptions<UmbrellaDbContext> options)
            : base(options)
        {
        }

        // ===== DbSets =====
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Virus> Viruses { get; set; }
        public DbSet<TestSubject> TestSubjects { get; set; }
        public DbSet<Sample> Samples { get; set; }
        public DbSet<LabReport> LabReports { get; set; }

        public DbSet<Archive> Archives { get; set; }
        public DbSet<ClassifiedFile> ClassifiedFiles { get; set; }
        public DbSet<Development> Developments { get; set; }
        public DbSet<EmergencyMessage> EmergencyMessages { get; set; }
        public DbSet<EmergencyProtocol> EmergencyProtocols { get; set; }
        public DbSet<IncidentLog> IncidentLogs { get; set; }
        public DbSet<Loss> Losses { get; set; }
        public DbSet<Mutation> Mutations { get; set; }
        public DbSet<Statistic> Statistics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // ENUM CONFIGURATION
            // =========================

            modelBuilder.Entity<Employee>()
                .Property(e => e.Status)
                .HasConversion<string>();

            modelBuilder.Entity<TestSubject>()
                .Property(t => t.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Virus>()
                .Property(v => v.DangerLevel)
                .HasConversion<string>();

            modelBuilder.Entity<LabReport>()
                .Property(l => l.ConfidentialityLevel)
                .HasConversion<string>();

            modelBuilder.Entity<IncidentLog>()
                .Property(i => i.Severity)
                .HasConversion<string>();

            modelBuilder.Entity<Development>()
                .Property(d => d.Status)
                .HasConversion<string>();

            modelBuilder.Entity<ClassifiedFile>()
                .Property(c => c.Level)
                .HasConversion<string>();


            // =========================
            // RELATIONSHIPS
            // =========================

            // Employee → LabReports
            modelBuilder.Entity<LabReport>()
                .HasOne(l => l.Author)
                .WithMany(e => e.LabReports)
                .HasForeignKey(l => l.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee → Sample
            modelBuilder.Entity<Sample>()
                .HasOne(s => s.ResponsibleScientist)
                .WithMany(e => e.Samples)
                .HasForeignKey(s => s.ResponsibleScientistId)
                .OnDelete(DeleteBehavior.SetNull);

            // Employee → Development
            modelBuilder.Entity<Development>()
                .HasOne(d => d.LeadScientist)
                .WithMany(e => e.Developments)
                .HasForeignKey(d => d.LeadScientistId)
                .OnDelete(DeleteBehavior.SetNull);

            // Employee → IncidentLog
            modelBuilder.Entity<IncidentLog>()
                .HasOne(i => i.ReportedBy)
                .WithMany(e => e.IncidentLogs)
                .HasForeignKey(i => i.ReportedById)
                .OnDelete(DeleteBehavior.SetNull);

            // Virus → Sample
            modelBuilder.Entity<Sample>()
                .HasOne(s => s.Virus)
                .WithMany(v => v.Samples)
                .HasForeignKey(s => s.VirusId)
                .OnDelete(DeleteBehavior.Cascade);

            // Virus → TestSubject
            modelBuilder.Entity<TestSubject>()
                .HasOne(t => t.Virus)
                .WithMany(v => v.TestSubjects)
                .HasForeignKey(t => t.VirusId)
                .OnDelete(DeleteBehavior.SetNull);

            // LabReport → Sample
            modelBuilder.Entity<LabReport>()
                .HasOne(l => l.Sample)
                .WithMany()
                .HasForeignKey(l => l.SampleId)
                .OnDelete(DeleteBehavior.SetNull);

            // LabReport → TestSubject
            modelBuilder.Entity<LabReport>()
                .HasOne(l => l.TestSubject)
                .WithMany(t => t.LabReports)
                .HasForeignKey(l => l.TestSubjectId)
                .OnDelete(DeleteBehavior.SetNull);

            // Mutation → TestSubject
            modelBuilder.Entity<Mutation>()
                .HasOne(m => m.TestSubject)
                .WithMany()
                .HasForeignKey(m => m.TestSubjectId)
                .OnDelete(DeleteBehavior.SetNull);

            // Mutation → Virus
            modelBuilder.Entity<Mutation>()
                .HasOne(m => m.Virus)
                .WithMany()
                .HasForeignKey(m => m.VirusId)
                .OnDelete(DeleteBehavior.SetNull);

            // Loss → связи
            modelBuilder.Entity<Loss>()
                .HasOne(l => l.Sample)
                .WithMany()
                .HasForeignKey(l => l.SampleId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Loss>()
                .HasOne(l => l.TestSubject)
                .WithMany()
                .HasForeignKey(l => l.TestSubjectId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Loss>()
                .HasOne(l => l.Employee)
                .WithMany()
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Statistic → Virus
            modelBuilder.Entity<Statistic>()
                .HasOne(s => s.Virus)
                .WithMany()
                .HasForeignKey(s => s.VirusId)
                .OnDelete(DeleteBehavior.SetNull);

            // ClassifiedFile → Employee
            modelBuilder.Entity<ClassifiedFile>()
                .HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // EmergencyMessage → Employee
            modelBuilder.Entity<EmergencyMessage>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmergencyMessage>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmergencyMessage>()
                .Property(m => m.Text)
                .HasColumnType("longtext");


            // =========================
            // SPECIAL CONFIG
            // =========================

            modelBuilder.Entity<Archive>()
                .Property(a => a.DataSnapshot)
                .HasColumnType("longtext");
        }
    }
}