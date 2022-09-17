using Godch.Models;
using MyLib;

namespace Godch.ViewModels
{
    public class sWork : ViewBase
    {
        public long Id { get; set; }
        public string? WorkName { get; set; }
        public int AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public byte? PublishType { get; set; }
        public string? FileUrl { get; set; }
        public string? Description { get; set; }
        public sWork()
        {

        }
        public sWork(Work w)
        {
            Id = w.WorkId;
            WorkName = w.WorkName;
            AuthorId = w.AuthorId;
            PublishType = w.PublishType;            
            FileUrl = w.FileUrl;
            Description = w.Description;
        }
    }
    public class dWork : sWork
    {
        public TagSelection Tags = new();
        public string? UpLoadTime { get; set; }
        public List<Post>? Comments { get; set; }
        public dWork()
        {

        }
        public dWork(Work w) : base(w)
        {
            UpLoadTime = Time.ToString(w.UploadTime);
        }
    }
    public class uWork : dWork
    {
        public IFormFile? WorkFile { get; set; }
    }
}
