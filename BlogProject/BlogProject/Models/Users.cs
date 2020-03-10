using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BlogProject.Models
{
    [Table("Users")]
    public class Users
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(50), Required]
        public string Name { get; set; }

        [StringLength(50), Required]
        public string Surname { get; set; }
        //, Index("IX_EmailAndUserName", 1, IsUnique = true)
        [StringLength(50), Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [StringLength(150)]
        public string Adress { get; set; }
        [StringLength(12)]
        public string Phone { get; set; }
        //, Index("IX_EmailAndUserName", 2, IsUnique = true)
        [StringLength(50), Required]
        public string UserName { get; set; }
        public bool BannedStatus { get; set; }
        public bool AccountStatus { get; set; }


        public virtual List<Blogs> Blogs { get; set; }
        public virtual List<Comments> Comments { get; set; }
        public virtual List<LikedBlogs> LikedBlogs { get; set; }
        public virtual List<Complaints> Complaints { get; set; }
        public virtual List<AdminList> AdminList { get; set; }

    }
}