using BlogProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogProject.ViewModels.Blog
{
    public class BlogsViewModel
    {
        public List<Blogs> blogList { get; set; }
        public Blogs blog { get; set; }
        public SelectList categoriSelectList { get; set; }
        public Comments comment { get; set; }
        public Complaints Report { get; set; }
        public int? userID;
        public LikedBlogs likedBlogs { get; set; }
    }
}