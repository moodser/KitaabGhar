using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTB.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdated { get; set; }
        // Book
        [ForeignKey("Books")]
        public int BookId { get; set; }
        public virtual Book Books { get; set; }
        // User
        [ForeignKey("Users")]
        public string UserId { get; set; }
        public virtual IdentityUser Users { get; set; }
    }
}
