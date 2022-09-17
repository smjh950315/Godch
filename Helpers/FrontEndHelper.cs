using Godch.ViewModels;

namespace Godch
{
    public static class FrontEndHelper
    {
        public static sUser? GetUser(int? uid) 
        {
            dbQuery q = new();
            var user=q.User(uid);
            var suser = new ModelCast().Cast(user);
            return suser; 
        }
        public static bool IsMember(int? uid)
        {
            if(uid == null) { return false; }
            dbQuery q = new();
            return q.User(uid)!=null;
        }
        public static string UserName(int? uid)
        {
            var u = GetUser(uid);
            return u?.Name ?? "Guest";
        }
    }
}
