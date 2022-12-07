using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KTB.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Nationality { get; set; }
        public DateTime DoB { get; set; }

        // Books
        public List<Book> Books { get; set; }
    }
}
