using LibraryManagementSystem.Data;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;


namespace LibraryManagementSystem.Services
{
    public class LibraryService
    {
        private readonly AppDbContext _db;
        public LibraryService(AppDbContext db)
        {
            var factory = new AppDbContextFactory();
            _db = factory.CreateDbContext([]);
        }


    }
}
