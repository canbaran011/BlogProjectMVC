using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BlogProject.Models
{
    [Table("BlogImages")]
    public class BlogImages
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [StringLength(250), Required]
        public string ImagesPath { get; set; }
        public int? BlogsID { get; set; }

        //foreign
        public virtual Blogs Blogs { get; set; }
    }
}