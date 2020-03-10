using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BlogProject.Models
{
    [Table("BannedBlogs")]
    public class BannedBlogs
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [StringLength(500)]
        public string Reason { get; set; }
        public DateTime BannedDate { get; set; }
        public DateTime BannedLimit { get; set; }
        public int? BlogsID { get; set; }
        public int? ComplaintsID { get; set; }
        public int? AdminListID { get; set; }

        //foreign
        public virtual Blogs Blogs { get; set; }
        public virtual Complaints Complaints { get; set; }
        public virtual AdminList AdminList { get; set; }
    }
}