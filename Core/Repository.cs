using System.Linq;

namespace Core
{
    public class Repository
    {
        public int Add(string subject)
        {
            var post = new BlogPost
            {
                Subject = subject,
                Body = "Body one",
                Attribute1 = "Attribute1",
                Attribute2 = ""
            };

            using (var db = new ApplicationDbContext())
            {
                db.BlogPosts.Add(post);

                db.SaveChanges();
            }

            return post.Id;
        }

        public void Edit(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var post = db.BlogPosts.FirstOrDefault(e=>e.Id == id);

                post.Attribute1 = "";

                db.SaveChanges();
            }
        }
    }
}
