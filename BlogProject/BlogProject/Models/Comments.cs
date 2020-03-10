using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BlogProject.Models
{
    [Table("Comments")]
    public class Comments
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [StringLength(1000)]
        public string Text { get; set; }
        public DateTime? CommentDate { get; set; }
        public int? UsersID { get; set; }
        public int? BlogsID { get; set; }


        //foreign
        public virtual Users Users { get; set; }
        public virtual Blogs Blogs { get; set; }
    }
}