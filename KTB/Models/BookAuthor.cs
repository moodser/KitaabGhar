using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KTB.Models
{
    public class BookAuthor
    {
        [Key]
        public int Id { get; set; }


        [ForeignKey("Books")]
        public int BookId { get; set; }
        public Book Books { get; set; }

        [ForeignKey("Authors")]
        public int AuthorId { get; set; }
        public Author Authors { get; set; }
    }
}
