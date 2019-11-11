using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BikeService.Models;

namespace BikeService.Controllers
{
    public class BooksController : ApiController
    {
        private BikeServiceContext db = new BikeServiceContext();

        #region Non- DTO (Data Transfer Objects)
        //// GET: api/Books
        //public IQueryable<Book> GetBooks()
        //{
        //    return db.Books;
        //    //Eager loading
        //    //return db.books.Include(b => b.Author);

        //}

        //// GET: api/Books/5
        //[ResponseType(typeof(Book))]
        //public async Task<IHttpActionResult> GetBook(int id)
        //{
        //    Book book = await db.Books.FindAsync(id);
        //    if (book == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(book);
        //}

        #endregion


        /**
         * NOTE THIS IS MANUALLY USING DTO TO MAP OBJECTS.
         * You can use tools such as http://automapper.org/
         */

        #region Using DTO(Data Transfer Objects)    
        // GET api/Books
        public IQueryable<BookDto> GetBooks()
        {
            var books = from b in db.Books
                        select new BookDto()
                        {
                            Id = b.Id,
                            Title = b.Title,
                            AuthorName = b.Author.Name
                        };

            return books;
        }

        // GET api/Books/5
        [ResponseType(typeof(BookDetailDto))]
        public async Task<IHttpActionResult> GetBook(int id)
        {
            var book = await db.Books.Include(b => b.Author).Select(b =>
                new BookDetailDto()
                {
                    Id = b.Id,
                    Title = b.Title,
                    Year = b.Year,
                    Price = b.Price,
                    AuthorName = b.Author.Name,
                    Genre = b.Genre
                }).SingleOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        #endregion

        #region Non- DTO (Data Transfer Objects)

        // POST: api/Books
        //[ResponseType(typeof(Book))]
        //public async Task<IHttpActionResult> PostBook(Book book)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Books.Add(book);
        //    await db.SaveChangesAsync();

        //    return CreatedAtRoute("DefaultApi", new { id = book.Id }, book);
        //}

        #endregion

        #region DTO (Data Transfer Objects)
        // POST: api/Books
        [ResponseType(typeof(BookDto))]
        public async Task<IHttpActionResult> PostBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Books.Add(book);
            await db.SaveChangesAsync();

            // New code:
            // Load author name
            db.Entry(book).Reference(x => x.Author).Load();

            var dto = new BookDto()
            {
                Id = book.Id,
                Title = book.Title,
                AuthorName = book.Author.Name
            };

            return CreatedAtRoute("DefaultApi", new { id = book.Id }, dto);
        }
        #endregion

        // DELETE: api/Books/5
        [ResponseType(typeof(Book))]
        public async Task<IHttpActionResult> DeleteBook(int id)
        {
            Book book = await db.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            db.Books.Remove(book);
            await db.SaveChangesAsync();

            return Ok(book);
        }

        // PUT: api/Books/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBook(int id, Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != book.Id)
            {
                return BadRequest();
            }

            db.Entry(book).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BookExists(int id)
        {
            return db.Books.Count(e => e.Id == id) > 0;
        }
    }
}
