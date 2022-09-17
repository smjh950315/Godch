using Godch.Models;
using Godch;
namespace Godch.ViewModels
{
    public class sForum : ViewBase
    {
        public int Id { get; set; }
        public string? ForumName { get; set; }
        public string? Description { get; set; }
        public string? LastActivity { get; set; }
        public sForum() { }
        public sForum(Forum forum) : base(forum.ForumName)
        {
            Id = forum.ForumId;
            ForumName = forum.ForumName;
            Description = forum.Description;
        }
    }
    public class dForum : sForum
    {
        public dbQuery q = new();
        public List<Post>? HeadPosts { get; set; }
        public List<dynamic>? HeadPost { get; set; }
        public dForum() { } 
        public dForum(Forum forum) : base(forum)
        {
            HeadPosts = q.HeadPosts(forum.ForumId);
        }
        public dForum(Forum forum, string? option) : base(forum)
        {
            if (option == "NoList")
            {
                HeadPost = new List<dynamic>();
            }
            else
            {
                HeadPosts = q.HeadPosts(forum.ForumId);
            }
        }
    }

}
