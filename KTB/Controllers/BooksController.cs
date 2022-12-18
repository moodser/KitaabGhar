using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KTB.Data;
using KTB.Models;
using Microsoft.AspNetCore.Authorization;

namespace KTB.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            ViewData["BookAuthor"] = await _context.BookAuthor.ToListAsync();
            ViewData["Authors"] = await _context.Author.ToListAsync();
            var applicationDbContext = _context.Book.Include(b => b.Publishers);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> BookDetails(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            ViewData["BookAuthor"] = await _context.BookAuthor.ToListAsync();
            ViewData["Authors"] = await _context.Author.ToListAsync();
            var applicationDbContext = _context.Book.Include(b => b.Publishers).Where(b=>b.Id==Id);
            return View("Index", await applicationDbContext.ToListAsync());
        }


        // GET: Books/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["PublisherId"] = new SelectList(_context.Publisher, "Id", "Name");
            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "Name");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Title,URL,Price,Edition,PublisherId")] Book book, List<int> Authors)
        {
            
            if (ModelState.IsValid)
            {
                _context.Book.Add(book);
                await _context.SaveChangesAsync(); 
                List<BookAuthor> bookAuthtor = new List<BookAuthor>();
                foreach (int author in Authors)
                {
                    bookAuthtor.Add(new BookAuthor { AuthorId = author, BookId = book.Id});
                }
                _context.BookAuthor.AddRange(bookAuthtor);
                _context.SaveChanges();
                
                return RedirectToAction(nameof(Index));
            }
            ViewData["PublisherId"] = new SelectList(_context.Publisher, "Id", "Id", book.PublisherId);
            return View(book);
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            IList<BookAuthor> bookAuthors = await _context.BookAuthor.Where<BookAuthor>(a=>a.BookId == book.Id).ToListAsync();
            IList<int> listAuthors = new List<int>();
            foreach(BookAuthor bookAuthor in bookAuthors)
            {
                listAuthors.Add(bookAuthor.AuthorId);
            }
            
            ViewData["PublisherId"] = new SelectList(_context.Publisher, "Id", "Name", book.PublisherId);
            ViewData["AuthorId"] = new MultiSelectList(_context.Author, "Id", "Name", listAuthors.ToArray());
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,URL,Price,Edition,PublisherId")] Book book, List<int> Authors)
        {
            var transaction = _context.Database.BeginTransaction();
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    _context.SaveChanges();
                    List<BookAuthor> bookAuthor = new List<BookAuthor>();
                    foreach (int author in Authors)
                    {
                        bookAuthor.Add(new BookAuthor { AuthorId = author, BookId = book.Id });
                    }
                    List<BookAuthor> deleteBookAuthors = await _context.BookAuthor.Where<BookAuthor>(a => a.BookId == book.Id).ToListAsync();
                    _context.BookAuthor.RemoveRange(deleteBookAuthors);
                    _context.SaveChanges();
                    
                    _context.BookAuthor.UpdateRange(bookAuthor);
                    _context.SaveChanges();

                    await transaction.CommitAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            
            ViewData["PublisherId"] = new SelectList(_context.Publisher, "Id", "Id", book.PublisherId);
            return View(book);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Publishers)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Book.FindAsync(id);
            _context.Book.Remove(book);
            _context.SaveChanges();

            List<BookAuthor> deleteBookAuthors = await _context.BookAuthor.Where<BookAuthor>(a => a.BookId == book.Id).ToListAsync();
            _context.BookAuthor.RemoveRange(deleteBookAuthors);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
    }
}
