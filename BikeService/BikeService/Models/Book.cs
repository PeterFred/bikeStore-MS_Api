using System.ComponentModel.DataAnnotations;

namespace BikeService.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string Genre { get; set; }

        //Foreign Key
        public int AuthorId { get; set; }

        //Navigation property
        //Virtual allows lazy loading -should diable if object will be serialized
        public virtual Author Author { get; set; }
    }
}