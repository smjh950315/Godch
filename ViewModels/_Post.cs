using Godch.Models;
using System.ComponentModel.DataAnnotations;
using MyLib;

namespace Godch.ViewModels
{
    public class sHeadPost : ViewBase
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public int? ForumId { get; set; }
        public int? AuthorId { get; set; }
        public string? LastReply { get; set; }
        public sHeadPost() { }
        public sHeadPost(Post p) : base(p.Title)
        {
            Id = p.PostId;
            Title = p.Title;
            AuthorId = p.AuthorId;
            ForumId = p.ForumId;
            LastReply = Time.ToString(p.LastReply);
        }
    }
    //public class dHeadPost : sHeadPost
    //{
    //    public dbQuery q = new();
    //    public List<Post>? ChildPosts { get; set; }
    //    public dHeadPost() { }
    //    public dHeadPost(Post p) : base(p) { }
    //}
    public class sPost : ViewBase
    {
        public int ForumId { get; set; }
        [Required]
        public string? Title { get; set; }
        public string? Data { get; set; }
        public sPost()
        {
        }
    }
    public class editPost : ViewBase
    {

        public string? ForumName { get; set; }
        public string? Title { get; set; }
        public string? Data { get; set; }
        public editPost() : base("edit:")
        {
        }
        public editPost(Post q) : base("edit:")
        {
            
        }
    }
}
