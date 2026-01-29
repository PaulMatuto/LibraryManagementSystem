using LibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace LibraryManagementSystem.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            string pipeName = GetLocalDbPipeName();
            optionsBuilder.UseSqlServer(
                $@"Server={pipeName};Database=LibraryManagementSystemDb;Trusted_Connection=True;TrustServerCertificate=True;");
            
            return new AppDbContext(optionsBuilder.Options);
        }
        private string GetLocalDbPipeName()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "sqllocaldb",
                        Arguments = "info mssqllocaldb",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                var match = Regex.Match(output, @"Instance pipe name:\s*(.+)");
                if (match.Success)
                {
                    return match.Groups[1].Value.Trim();
                }
            }
            catch
            {

            }
            return @"np:\\.\pipe\LOCALDB#C2181B79\tsql\query";
        }
    }
}