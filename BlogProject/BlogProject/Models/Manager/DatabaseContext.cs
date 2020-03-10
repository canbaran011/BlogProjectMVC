using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace BlogProject.Models.Manager
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Blogs> Blogs { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<BlogImages> BlogImages { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<AdminList> AdminList { get; set; }
        public DbSet<Complaints> Complaints { get; set; }
        public DbSet<LikedBlogs> LikedBlogs { get; set; }
        public DbSet<BannedBlogs> BannedBlogs { get; set; }
        public DatabaseContext()
        {
            Database.SetInitializer(new VeritabaniOlusturucu());
        }
    }
    public class VeritabaniOlusturucu : CreateDatabaseIfNotExists<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            for (int i = 0; i < 10; i++)
            {
                Users user = new Users();
                user.Name = FakeData.NameData.GetFirstName();
                user.Surname = FakeData.NameData.GetSurname();
                user.Email = FakeData.NetworkData.GetEmail();
                user.Adress = FakeData.PlaceData.GetAddress();
                user.Phone = "553 510 1122";
                user.Password = "AEsnGm/7BfPmlrcMzygzzxTWLlM1Faikyhj3Z8mTaytVPV6giPYWJiISEGAE0FpAfw==";//123
                user.UserName = FakeData.NameData.GetCompanyName();
                user.BannedStatus = FakeData.BooleanData.GetBoolean();
                user.AccountStatus = FakeData.BooleanData.GetBoolean();
                context.Users.Add(user);
            }
            context.SaveChanges();
            for (int i = 0; i < 10; i++)
            {
                Categories categories = new Categories();
                categories.CategoriName = FakeData.PlaceData.GetCountry();
                context.Categories.Add(categories);
            }
            context.SaveChanges();
            List<Users> users = context.Users.ToList();
            List<Categories> kategori = context.Categories.ToList();

            for (int i = 0; i < 3; i++)
            {
                AdminList admin = new AdminList();
                admin.Users = users[i];
                admin.AuthDegree = 2;
                admin.AuthName = "Admin";
                context.AdminList.Add(admin);
            }
            foreach (Users user in users)
            {
                for (int i = 0; i < FakeData.NumberData.GetNumber(1, 5); i++)
                {
                    Blogs blog = new Blogs();
                    try
                    {
                        blog.Users = user;
                        blog.Categories = kategori[i];
                        blog.Title = FakeData.TextData.GetSentence();
                        blog.Text = FakeData.TextData.GetSentences(2);
                        blog.DefaultImage = "bc06537e49814c258bd0b6d738a1e792.jpg";
                        blog.AddDate = FakeData.DateTimeData.GetDatetime();
                        blog.UpdateDate = FakeData.DateTimeData.GetDatetime();
                        blog.NumOfLikes = FakeData.NumberData.GetNumber(1, 10000);
                        blog.BannedStatus = FakeData.BooleanData.GetBoolean();
                        blog.BlogStatus = FakeData.BooleanData.GetBoolean();
                        context.Blogs.Add(blog);
                        context.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)//faydalı bir kod 1 saat  sonunda sorunu bulmamı sağladı :D
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                    for (int t = 0; t < 5; t++)
                    {
                        BlogImages blogImages = new BlogImages();
                        blogImages.Blogs = blog;
                        blogImages.ImagesPath = "bc06537e49814c258bd0b6d738a1e792.jpg";
                        context.BlogImages.Add(blogImages);
                        Comments comments = new Comments();
                        comments.Blogs = blog;
                        comments.Users = user;
                        comments.CommentDate = DateTime.Now;
                        comments.Text = FakeData.TextData.GetSentence();
                        context.Comments.Add(comments);
                    }
                }
            }

            context.SaveChanges();


        }
    }
}