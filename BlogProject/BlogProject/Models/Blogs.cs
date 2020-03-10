using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BlogProject.Models
{
    [Table("Blogs")]
    public class Blogs
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(250), Required]
        public string Title { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public string DefaultImage { get; set; }
        public int NumOfLikes { get; set; }
        public bool BannedStatus { get; set; }
        public bool BlogStatus { get; set; }        
        public DateTime? AddDate { get; set; }        
        public DateTime? UpdateDate { get; set; }
        public int? UsersID { get; set; }
        public int? CategoriesID { get; set; }

        //foreign
        public virtual Users Users { get; set; }
        public virtual Categories Categories { get; set; }
        public virtual List<LikedBlogs> LikedBlogs { get; set; }
        public virtual List<Complaints> Complaints { get; set; }
        //bir çok kez engellenebilir olabileceği için
        public virtual List<BannedBlogs> BannedBlogs { get; set; }
        public virtual List<BlogImages> BlogImages { get; set; }
        public virtual List<Comments> Comments { get; set; }
    }
}