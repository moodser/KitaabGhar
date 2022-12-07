using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KTB.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public int Price { get; set; }
        public string Edition { get; set; }
        
        // Publisher
        [ForeignKey("Publishers")]
        public int PublisherId { get; set; }
        public virtual Publisher Publishers { get; set; }

        // Author
        public List<Author> Authors { get; set; }
    }
}