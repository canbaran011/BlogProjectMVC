using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BlogProject.Models
{
    [Table("LikedBlogs")]
    public class LikedBlogs
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int? UsersID { get; set; }
        public int? BlogsID { get; set; }
        public bool Liked { get; set; }
        public bool Favourite { get; set; }

        //foreign
        public virtual Users Users { get; set; }
        public virtual Blogs Blogs { get; set; }
    }
}