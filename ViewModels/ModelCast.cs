using Godch.Models;
using dir = Godch.GodchDirectories;
namespace Godch.ViewModels
{
    public class ModelCast
    {
        public dbQuery q = new();
        //internal dynamic Cast<T>(T? t)
        //{
        //    throw new NotImplementedException();
        //}
        public sUser? Cast(User? user)
        {
            if (user == null) return null;
            sUser sUser = new(user);
            return sUser;
        }
        public dUser? dCast(User? user)
        {
            if (user == null) return null;
            dUser dUser = new(user);
            dUser.Contact = "";
            return dUser;
        }

        public sForum? Cast(Forum? f)
        {
            if (f == null) return null;
            sForum sforum = new(f);
            sforum.LastActivity = "";
            return sforum;
        }
        public dForum? dCast(Forum? f)
        {
            if (f == null) return null;
            dForum dforum = new(f);
            dforum.LastActivity = "";
            return dforum;
        }

        public sTag? Cast(Tag? t)
        {
            if (t == null) return null;
            sTag stag = new();
            stag.Id = t.TagId;
            stag.TagName = t.TagName;
            return stag;
        }

        public sWork? Cast(Work? w)
        {
            if (w == null) return null;
            sWork swork = new(w);
            dbQuery q = new();
            swork.AuthorName = q.UserName(w.AuthorId);
            return swork;
        }
        public dWork? dCast(Work? w)
        {
            if (w == null) return null;
            dWork dwork = new(w);
            dwork.AuthorName = q.UserName(w.AuthorId);
            return dwork;
        }
        public Work Cast(uWork w)
        {
            Work work = new();
            work.WorkName = w.WorkName ?? "Untitle";
            work.AuthorId = w.AuthorId;
            work.PublishType = w.PublishType;
            work.Description = w.Description;
            return work;
        }

        public sHeadPost? Cast(Post? post)
        {
            if (post == null) return null;
            var head = q.HeadPost(post.PostId);
            if (head == null)
            {
                return null;
            }
            sHeadPost sHP = new(head);
            return sHP;
        }
        //public dHeadPost? dCast(Post? post)
        //{
        //    if (post == null) return null;
        //    var head = q.HeadPost(post.PostId);
        //    if (head == null)
        //    {
        //        return null;
        //    }
        //    dHeadPost dHP = new(head);
        //    dHP.ChildPosts = q.PostFamily(head.PostId);
        //    return dHP;
        //}

        public Post Cast(sPost np)
        {
            Post p = new();
            p.ForumId = np.ForumId;
            p.Title = np.Title;
            return p;
        }

        
    }
}
