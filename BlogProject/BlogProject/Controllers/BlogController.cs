using BlogProject.Models;
using BlogProject.Models.Manager;
using BlogProject.ViewModels.Blog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogProject.Controllers
{
    public class BlogController : Controller
    {
        DatabaseContext db = new DatabaseContext();
        // GET: Blog
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UserBlogs()
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("index", "Home");
            }
            int id = Convert.ToInt32(Session["ID"]);
            BlogsViewModel blogs = new BlogsViewModel();
            blogs.blogList = db.Blogs.Where(x => x.UsersID == id && x.BlogStatus == true).ToList();
            return View(blogs);
        }

        public ActionResult BlogCategories(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Categories categori = db.Categories.Where(x => x.ID == id).FirstOrDefault();
            BlogsViewModel model = new BlogsViewModel();
            model.blogList = db.Blogs.Where(x => x.CategoriesID == categori.ID && x.BlogStatus == true).ToList();//categori ye ait bloglar


            ViewBag.CategoriName = categori.CategoriName;

            return View(model);
        }
        public ActionResult AddBlog()
        {
            BlogsViewModel blogModel = new BlogsViewModel();
            var list = db.Categories.ToList();
            blogModel.categoriSelectList = new SelectList(list, "ID", "CategoriName");
            return View(blogModel);
        }
        [HttpPost]
        public ActionResult AddBlog(BlogsViewModel model, HttpPostedFileBase file)
        {
            //var fileName = Path.GetFileName(file.FileName);
            //var path = Path.Combine(Server.MapPath("~/Images"), fileName);
            //file.SaveAs(path);
            int id = Convert.ToInt32(Session["ID"]);
            Blogs blog = new Blogs();
            if (file != null && blog != null)
            {
                string path = Server.MapPath("~/Images/");
                string fileName = Guid.NewGuid().ToString().Replace("-", "") + ".jpg";
                file.SaveAs(path + fileName);
                blog.Title = model.blog.Title;
                blog.Text = model.blog.Text;
                blog.CategoriesID = model.blog.Categories.ID;
                blog.DefaultImage = fileName;
                blog.UsersID = id;
                blog.AddDate = DateTime.Now;
                blog.BlogStatus = true;
                db.Blogs.Add(blog);
                db.SaveChanges();
                return RedirectToAction("UserBlogs", "Blog");
            }
            else
            {
                ViewBag.Warning = "blog ekleme başarısız.";
                ViewBag.Status = "danger";
            }
            var list = db.Categories.ToList();
            model.categoriSelectList = new SelectList(list, "ID", "CategoriName");

            return View(model);
        }
        [HttpGet]
        public ActionResult ReadBlog(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            int userID = Convert.ToInt32(Session["ID"]);
            BlogsViewModel model = new BlogsViewModel();
            model.blog = db.Blogs.Where(x => x.ID == id).FirstOrDefault();
            if (model != null)
            {
                //model.blog.Text = model.blog.Text.Replace(Environment.NewLine, @"<br />");
                model.likedBlogs = db.LikedBlogs.Where(x => x.BlogsID == id && x.UsersID == userID).FirstOrDefault();
                if (model.likedBlogs == null) //fav null ise blog kişinin favorilerinde değildir.
                {
                    model.likedBlogs = new LikedBlogs();
                    model.likedBlogs.Favourite = false;
                    model.likedBlogs.Liked = false;
                }
            }
            else
            {
                return RedirectToAction("BlogCategories", "Blog");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult AddComment(int? blogID, string blogText)
        {
            if (Session["ID"] == null)
            {
                return Json(new { success = false, text = "To add comment please log in / or register" }, JsonRequestBehavior.AllowGet);
            }
            int? userID = Convert.ToInt32(Session["ID"]);
            Comments comment = new Comments();
            if (!string.IsNullOrEmpty(blogText))
            {
                comment.BlogsID = blogID;
                comment.UsersID = userID;
                comment.Text = blogText;
                comment.CommentDate = DateTime.Now;
                db.Comments.Add(comment);
                db.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, text = "Please add a comment before send!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteComment(int? id)
        {
            Comments comment = db.Comments.Where(x => x.ID == id).FirstOrDefault();
            Blogs blog = db.Blogs.Where(x => x.ID == comment.BlogsID).FirstOrDefault();//yonleneceği yer için
            bool IsDeleted;
            try
            {
                db.Entry(comment).State = EntityState.Deleted;
                db.SaveChanges();
                IsDeleted = true;
            }
            catch (Exception)
            {
                IsDeleted = false;
            }
            return Json(IsDeleted, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddFavourite(int? blogID)
        {
            if (Session["ID"] == null)
            {
                return Json(new { success = false, text = "To add Favourite please log in / or register" }, JsonRequestBehavior.AllowGet);
            }
            if (blogID != null)
            {
                int userID = Convert.ToInt32(Session["ID"]);
                LikedBlogs like = new LikedBlogs();
                like = db.LikedBlogs.Where(x => x.BlogsID == blogID && x.UsersID == userID).FirstOrDefault();
                if (like == null)//blogid ve userid aynı anda ekli değilse ekler
                {
                    like = new LikedBlogs();
                    like.UsersID = userID;
                    like.BlogsID = blogID;
                    like.Favourite = true;
                    like.Liked = false;
                    db.LikedBlogs.Add(like);
                    db.SaveChanges();
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (like.Favourite == false)
                    {
                        like.Favourite = true;
                        db.SaveChanges();
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, text = "You already add the blog to favourite." }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                return View("BlogCategories", "Blog");//herhangi bir kullanıcı bloga bakarken blog silindiyse 
            }
        }

        public ActionResult RemoveFavourite(int? blogID)
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("Login", "Blog");
            }
            if (blogID != null)
            {
                int userID = Convert.ToInt32(Session["ID"]);
                LikedBlogs like = new LikedBlogs();
                like = db.LikedBlogs.Where(x => x.BlogsID == blogID && x.UsersID == userID).FirstOrDefault();
                if (like != null)//sorgu sonucu bi data geldiyse
                {
                    if (like.Favourite == true && like.Liked == true)//burada fav true ise false yapacağız.
                    {//liked true olma durumunu kontrol etme sebebi eğer false gelirse, 2 durumda false olacağı için kaydı kaldıracağız. veritabanında yer kaplamaması için
                        like.Favourite = false;
                        db.SaveChanges();
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        db.Entry(like).State = EntityState.Deleted;
                        db.SaveChanges();
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, text = "You already remove the blog from your favourite list." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return View("BlogCategories", "Blog");//herhangi bir kullanıcı bloga bakarken blog silindiyse 
            }
        }

        public ActionResult IncreaseLike(int? blogID)
        {
            if (Session["ID"] == null)
            {
                return Json(new { success = false, text = "To Like a blog please log in / or register" }, JsonRequestBehavior.AllowGet);
            }
            if (blogID != null)
            {
                int userID = Convert.ToInt32(Session["ID"]);
                LikedBlogs like = new LikedBlogs();
                like = db.LikedBlogs.Where(x => x.BlogsID == blogID && x.UsersID == userID).FirstOrDefault();
                if (like == null)//blogid ve userid aynı anda ekli değilse ekler
                {//kullanıcı yok ise
                    like = new LikedBlogs();
                    like.UsersID = userID;
                    like.BlogsID = blogID;
                    like.Favourite = false;
                    like.Liked = true;
                    db.LikedBlogs.Add(like);

                    Blogs blog = db.Blogs.Where(x=> x.ID==blogID).FirstOrDefault();
                    blog.NumOfLikes++;
                    db.SaveChanges();
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else//zaten kullanıcı varsa ve daha önce favourite eklemiş fakat beğenmemişse bu alan çalışacak
                {
                    if (like.Liked == false)
                    {
                        like.Liked = true;
                        Blogs blog = db.Blogs.Where(x => x.ID == blogID).FirstOrDefault();
                        blog.NumOfLikes++;
                        db.SaveChanges();
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, text = "You already Like the blog." }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                return View("BlogCategories", "Blog");//herhangi bir kullanıcı bloga bakarken blog silindiyse 
            }
        }

        public ActionResult DecreaseLike(int? blogID)
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("Login", "Blog");
            }
            if (blogID != null)
            {
                int userID = Convert.ToInt32(Session["ID"]);
                LikedBlogs like = new LikedBlogs();
                like = db.LikedBlogs.Where(x => x.BlogsID == blogID && x.UsersID == userID).FirstOrDefault();
                if (like != null)//sorgu sonucu bi data geldiyse
                {
                    //like.Liked sadece True iken burası çalışacağı için Liked'ın false olma durumunu kontrol etmedim.Buradaki kontrolün asıl amacı eğer favourite de false ise veritabanında yer kaplamasını engellemek.yani eğer fav false gelirse, 2 durumda false olacağı için kaydı kaldıracağız. 
                    if (like.Liked == true && like.Favourite == true)//burada liked veritabanında true ise false yapacağız.
                    {
                        like.Liked = false;
                        Blogs blog = db.Blogs.Where(x => x.ID == blogID).FirstOrDefault();
                        blog.NumOfLikes--;
                        db.SaveChanges();
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        db.Entry(like).State = EntityState.Deleted;
                        Blogs blog = db.Blogs.Where(x => x.ID == blogID).FirstOrDefault();
                        blog.NumOfLikes--;
                        db.SaveChanges();
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, text = "You already remove your Like from the Blog." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return View("BlogCategories", "Blog");//herhangi bir kullanıcı bloga bakarken blog silindiyse 
            }
        }

        public ActionResult FavouriteBlogs()
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int id = Convert.ToInt32(Session["ID"]);
            List<Blogs> blogList = new List<Blogs>();
            List<LikedBlogs> liked = db.LikedBlogs.Where(x => x.UsersID == id).ToList();
            foreach (LikedBlogs item in liked)
            {
                Blogs b = new Blogs();
                b = db.Blogs.Where(x => x.ID == item.BlogsID).FirstOrDefault();
                blogList.Add(b);
            }
            BlogsViewModel model = new BlogsViewModel();
            model.blogList = blogList;
            return View(model);
        }
        [HttpGet]
        public ActionResult ReportBlog(int? blogID)
        {
            int? id = Convert.ToInt32(Session["ID"]);
            if (id == null || id == 0)
            {
                return RedirectToAction("Login", "Account");
            }
            BlogsViewModel model = new BlogsViewModel();
            model.blog = db.Blogs.Where(x => x.ID == blogID).FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        public ActionResult ReportBlog(BlogsViewModel model)
        {
            int? id = Convert.ToInt32(Session["ID"]);
            if (id == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Complaints report = new Complaints();
                report.Subject = model.Report.Subject;
                report.Text = model.Report.Text;
                report.UsersID = id;
                report.BlogsID = model.blog.ID;
                db.Complaints.Add(report);
                db.SaveChanges();
                ViewBag.Status = "success";
                ViewBag.Warning = "Your report sent successfully.We will respond as soon as possible.";
            }



            return View(model);
        }

        public ActionResult DeleteBlog(int? blogID)
        {
            Blogs blog = new Blogs();
            blog = db.Blogs.Where(x => x.ID == blogID).FirstOrDefault();
            blog.BlogStatus = false;
            db.SaveChanges();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("UserBlogs", "Blog");
        }

    }
}