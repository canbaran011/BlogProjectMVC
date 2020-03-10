using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BlogProject.Models
{
    [Table("Complaints")]
    public class Complaints
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [StringLength(50)]
        public string Subject { get; set; }
        [StringLength(500)]
        public string Text { get; set; }
        public int? UsersID { get; set; }
        public int? BlogsID { get; set; }

        //foreign
        public virtual Users Users { get; set; }
        public virtual Blogs Blogs { get; set; }
        public virtual List<BannedBlogs> BannedBlogs { get; set; }
    }
}