using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BlogProject.Models
{
    [Table("AdminList")]
    public class AdminList
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public int AuthDegree { get; set; }
        [Required]
        public string AuthName { get; set; }
        public int? UsersID { get; set; }

        //foreign
        public virtual Users Users { get; set; }
        public virtual List<BannedBlogs> BannedBlogs { get; set; }
    }
}