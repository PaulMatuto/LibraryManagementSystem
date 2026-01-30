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
            string pipeName = GetPipeName();
            optionsBuilder.UseSqlServer(
                $@"Server={pipeName};Database=LibraryManagementSystemDb;Trusted_Connection=True;TrustServerCertificate=True;");
            
            return new AppDbContext(optionsBuilder.Options);
        }
        private string GetPipeName()
        {
            try
            {
                var startProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "sqllocaldb",
                        Arguments = "start mssqllocaldb",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                startProcess.Start();
                startProcess.WaitForExit();

                System.Threading.Thread.Sleep(1000);

                var infoProcess = new Process
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

                infoProcess.Start();
                string output = infoProcess.StandardOutput.ReadToEnd();
                infoProcess.WaitForExit();

                var match = Regex.Match(output, @"Instance pipe name:\s*(.+)");
                if (match.Success)
                {
                    return match.Groups[1].Value.Trim();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error starting LocalDB: {ex.Message}");
            }

            return @"np:\\.\pipe\LOCALDB#C2181B79\tsql\query";
        }
    }
}