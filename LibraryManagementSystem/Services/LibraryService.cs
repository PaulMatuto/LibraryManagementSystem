using LibraryManagementSystem.Data;
using LibraryManagementSystem.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LibraryManagementSystem.Services
{
    public class LibraryService
    {
        private readonly AppDbContext _db;
        public LibraryService()
        {
            var factory = new AppDbContextFactory();
            _db = factory.CreateDbContext([]);
        }
        public void AddBook(String title, String author, String genre, int yearPublished, int quantity)
        {
            var book = new Book() 
            { 
                Title = title, 
                Author = author, 
                Genre = genre, 
                YearPublished = yearPublished, 
                Quantity = quantity 
            };

            _db.Books.Add(book);

            _db.SaveChanges();
        }
        public async Task<List<Book>> GetBooksAsync()
        {
            return await _db.Books.AsNoTracking().ToListAsync();
        }
        public void UpdateBook(int bookId, String newTitle, String newAuthor, String newGenre, int newYearPublished, int newQuantity)
        {
            var book = _db.Books.FirstOrDefault(x => x.BookId == bookId);
            if (book != null)
            {
                book.Title = newTitle;
                book.Author = newAuthor;
                book.Genre = newGenre;
                book.YearPublished = newYearPublished;
                book.Quantity = newQuantity;

                _db.SaveChanges();
            }
        }
        public void DeleteBook(int bookId)
        {
            var book = _db.Books.FirstOrDefault(x => x.BookId == bookId);
            if (book != null)
            {
                _db.Books.Remove(book);

                _db.SaveChanges();
            }
        }
    }
}
