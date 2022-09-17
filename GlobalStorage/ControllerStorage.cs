using MyLib;
using MyLib.Mvc;
using String = MyLib.String;
namespace Godch.GlobalStorage
{
    public static class ControllerStorage
    {
        public static List<PageHelper> PageHelpers = new List<PageHelper>();
        public static List<UserVariable> UserVars = new List<UserVariable>();
    }
    public class UserVariable
    {
        public int Id { get; set; }
        public Number? ForumId { get; set; }
        public Number? PostId { get; set; }
        public Number? TempPostId { get; set; }
        public String? ForumName { get; set; }
        public String? PostName { get; set; }
        public dynamic Storage { get; set; }
        public UserVariable()
        {
            Storage = new System.Dynamic.ExpandoObject();
        }
        public UserVariable(int userId) : this()
        {
            Id = userId;
        }
    }
}
