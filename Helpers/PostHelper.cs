using dir = Godch.GodchDirectories;
using cfg = Godch.Config;

namespace Godch
{
    public static class PostFileIO
    {
        public static string GetLast(long? pid)
        {
            if (pid == null) { return String.Empty; }
            string PostFile = "";
            for (int i = cfg.PostBackupVerMax; i > -1; i--)
            {
                string temp = pid + "_" + i + ".txt";
                if (dir.Posts.Exist(temp))
                {
                    PostFile = temp;
                    break;
                }
            }
            return PostFile;
        }
        public static void WritePostContent(long pid, string? content)
        {
            string PostFile = "";
            for (int i = 0; i < cfg.PostBackupVerMax; i++)
            {
                string temp = pid + "_" + i + ".txt";
                if (dir.Posts.Exist(temp))
                {
                    PostFile = temp;
                }
                else
                {
                    PostFile = temp;
                    break;
                }
            }
            dir.Posts.WriteText(PostFile, content);
        }
        public static string ReadPostContent(long? pid)
        {
            if(pid == null) { return String.Empty; }
            return dir.Posts.ReadText(GetLast(pid));
        }
        public static string LastModified(long pid)
        {
            var f = GetLast(pid);
            return dir.Posts.LastWriteTime(f);
        }
    }
}
