using Godch.Models;
using Godch.ViewModels;
using MyLib;
using System.Data.Entity;
using _uR = Godch.Models.UserRelationShip;
using lz = MyLib.LazyConvert;

namespace Godch
{
    public class dbAction : Microsoft.AspNetCore.Mvc.Controller
    {
        public GODCHContext db = new GODCHContext();
        public void Create<T>(T item)
        {
            if (ModelState.IsValid && item != null)
            {
                db.Add(item);
                db.SaveChanges();
            }
        }
        public void Delete<T>(T item)
        {
            if (ModelState.IsValid && item != null)
            {
                db.Remove(item);
                db.SaveChanges();
            }
        }
        public void Update<T>(T item)
        {
            if (ModelState.IsValid && item != null)
            {
                db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                db.SaveChanges();
            }
        }
        public bool DBValid(DbSet dbSet, int? id)
        {
            return id == null ? false : DBValid(dbSet, (long)id);
        }
        public bool DBValid(DbSet dbSet, long? id)
        {
            bool rq = true;
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    rq = false;
                }
                if (dbSet.Find(id) == null)
                {
                    rq = false;
                }
            }
            else
            {
                rq = false;
            }
            return rq;
        }
        public bool DbIsON() { return ModelState.IsValid; }
    }
    public class dbQuery
    {
        public dbAction act = new();
        public GODCHContext db = new();
        public User? User(int? uid)
        {
            return db.Users.Find(uid);
        }
        public string UserName(int? uid)
        {
            if(uid == null) { return "Guest"; }
            var uName = db.Users.Find(uid)?.UserName;
            uName = uName ?? uid.ToString();
            return uName ?? $"user{uid}";
        }
        public bool UidValid(int? uid) 
        { 
            return uid == null ? false : User(uid) != null; 
        }

        public _uR? UserRelation(int? uid1, int? uid2)
        { 
            return db.UserRelationShips.Where(ur => ur.UserId == uid1 && ur.UserId2 == uid2).FirstOrDefault();
        }
        public _uR? UserRelation(int? uid1, User? user)
        {
            return UserRelation(uid1, user?.UserId);
        }
        public _uR? UserRelation(User? user, int? uid2)
        {
            return UserRelation(user?.UserId, uid2);
        }

        public bool[] UserRelations(int? uid1, int uid2)
        {
            var rA = UserRelation(uid1, uid2);
            var rB = UserRelation(uid2, uid1);
            return new bool[6]
            {
                    lz.TorF(rA?.Block),
                    lz.TorF(rB?.Block),
                    lz.TorF(rA?.Following),
                    lz.TorF(rB?.Following),
                    lz.TorF(rA?.FriendShip),
                    lz.TorF(rB?.FriendShip)
            };
        }
        public List<User> FriendsOfId(int uid)
        {
            List<_uR> UR = db.UserRelationShips.Where(ur => ur.UserId2 == uid && ur.Block == false && ur.FriendShip == true).ToList();
            List<User> Friends = new List<User>();
            foreach (_uR ur in UR)
            {
                var r = UserRelations(ur.UserId, uid);
                if (r[4] && r[5])
                {
                    var u = User(ur.UserId);
                    if (u != null)
                    {
                        Friends.Add(u);
                    }
                }
            }
            return Friends;
        }
        public List<User> FriendsWaiting(int uid)
        {
            List<_uR> UR = db.UserRelationShips.Where(ur => ur.UserId == uid && ur.Block == false && ur.FriendShip == true).ToList();
            List<User> Friends = new List<User>();
            foreach (_uR ur in UR)
            {
                var r = UserRelations(uid, ur.UserId2);
                if (r[4] && !r[5])
                {
                    var u = User(ur.UserId2);
                    if (u != null)
                    {
                        Friends.Add(u);
                    }
                }
            }
            return Friends;
        }
        public List<User> FriendsUnrequest(int uid)
        {
            List<_uR> UR = db.UserRelationShips.Where(ur => ur.UserId2 == uid && ur.Block == false && ur.FriendShip == true).ToList();
            List<User> Friends = new List<User>();
            foreach (_uR ur in UR)
            {
                var r = UserRelations(uid, ur.UserId);
                if (!r[4] && r[5])
                {
                    var u = User(ur.UserId);
                    if (u != null)
                    {
                        Friends.Add(u);
                    }
                }
            }
            return Friends;
        }
        public List<User> FollowersOfId(int uid)
        {
            List<_uR> UR = db.UserRelationShips.Where(ur => ur.UserId2 == uid && ur.Block == false && ur.Following == true).ToList();
            List<User> Followers = new List<User>();
            foreach (_uR ur in UR)
            {
                var u = User(ur.UserId);
                if (u != null)
                {
                    Followers.Add(u);
                }                
            }
            return Followers;
        }
        public List<User> FollowingOfId(int? uid)
        {
            if (uid == null) { return new List<User>(); }
            List<_uR> UR = db.UserRelationShips.Where(ur => ur.UserId == uid && ur.Block == false && ur.Following == true).ToList();
            List<User> Following = new List<User>();
            foreach (_uR ur in UR)
            {
                var u = User(ur.UserId2);
                if (u != null)
                {
                    Following.Add(u);
                }
            }
            return Following;
        }
        public Forum? Forum(int? fid)
        {
            if(fid == null)
            {
                return null;
            }
            return db.Forums.Find(fid.Value);
        }
        public Forum? Forum(ForumMember? fm)
        {
            return fm == null ? null : db.Forums.Where(f=>f.ForumId == fm.ForumId).FirstOrDefault();
        }
        public List<Forum> Forums()
        {
            return db.Forums.ToList();
        }
        public string? ForumName(int? fid)
        {
            return Forum(fid)?.ForumName;
        }

        public Post? Post(long? pid)
        {
            return db.Posts.Find(pid);
        }
        public Post? HeadPost(long? pid)
        {
            var p = db.Posts.Find(pid);
            if(p == null) { return null; }
            if (p != null && p.HeadPostId == null)
            {
                return p;
            }
            else
            {
                return Post(p.HeadPostId);
            }
        }        
        /// <summary>
        /// Return a Series of HeadPost in a forum
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public List<Post>? HeadPosts(int? fid)
        {
            var hps = db.Posts.Where(p => p.ForumId == fid && p.Floor == 0).ToList();
            return hps.OrderByDescending(p => p.LastReply).ToList();
        }
        /// <summary>
        /// Return a Series of Post with a headPost
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public List<Post>? PostFamily(long? pid)
        {
            if(pid == null) { return null; }
            var hp = HeadPost(pid);
            if (hp == null) { return null; }
            List<Post> result = new();
            result.Add(hp);
            long hpid = hp.PostId;
            var ps = db.Posts.Where(p => p.HeadPostId == hpid).ToList();
            result.AddRange(ps);
            return result;
        }

        public Tag? Tag(int? tid)
        {
            return db.Tags.Find(tid);
        }
        public TagSelection TagsOf(Work? work)
        {
            TagSelection tagSelection = new();
            if (work == null) { return tagSelection; }
            ViewList vlist = new();
            List<Tag> stag = new();
            var trs = db.TagRelations.Where(tr => tr.WorkId == work.WorkId).ToList();
            var alltags = db.Tags.ToList();            
            foreach (var tr in trs)
            {
                var t = db.Tags.Find(tr.TagId);
                if(t != null)
                {
                    stag.Add(t);
                }                
            }            
            foreach (var tag in stag)
            {
                alltags.Remove(tag);
            }            
            tagSelection.SelectedTags = vlist.CastToList(stag);
            tagSelection.AvailableTags = vlist.CastToList(alltags);
            return tagSelection;
        }
        public TagSelection TagsOf(long? workId)
        {
            return TagsOf(Work(workId));
        }
        public List<dynamic> SelectedTagsOf(long? workId)
        {
            return TagsOf(Work(workId)).SelectedTags;
        }
        public Work? Work(long? wid)
        {
            return db.Works.Find(wid);
        }
        public List<Work>? Works()
        {
            return db.Works.ToList();
        }
        public List<Work>? WorksOfTag(int? tid)
        {
            var trs = db.TagRelations.Where(tr=>tr.TagId==tid);
            if(trs == null) { return null; }
            List<Work> works = new List<Work>();
            foreach(var tr in trs)
            {
                var w = db.Works.Find(tr.WorkId);
                if(w!= null)
                {
                    works.Add(w);
                }
            }
            return works;
        }
        public List<Work> WorksOfUserTag(int? uid)
        {
            if (uid == null || uid == 0) { return new List<Work>(); }
            var utag=AccountHelper.ReadUserTag(uid);
            if(utag == null || utag.TagList.Count == 0) { return new List<Work>(); }
            List<Work> works = new List<Work>();
            List<long> wids = new List<long>();
            List<TagRelation> relations = new List<TagRelation>();
            foreach (var t in utag.TagList)
            {
                var related = db.TagRelations.Where(r => r.TagId == t.TagId).ToList();
                foreach(var r in related)
                {
                    if (!wids.Contains(r.WorkId)) { wids.Add(r.WorkId); }
                }                
            }
            foreach(var wid in wids)
            {
                var w = db.Works.Find(wid);
                if(w!= null)
                {
                    works.Add(w);
                }
            }    
            return works;
        }
        public List<Work> WorksFromFollowingUser(int?uid)
        {
            var following = FollowingOfId(uid);
            List<Work> works = new List<Work>();
            foreach(var f in following)
            {
                var work = db.Works.Where(w => w.AuthorId == f.UserId).ToList();
                foreach(Work w in work)
                {
                    if(w!=null&& !works.Contains(w))
                    {
                        works.Add(w);
                    }
                }
            }
            return works;
        }
        public ChatGroup? Group(long? gid)
        {
            if (gid == null) { return null; }           
            List<ChatGroup> chatGroups = new List<ChatGroup>();
            var group = db.ChatGroups.Find(gid);
            return group;
        }
        public List<long> CommonGroup(params int[] userIds)
        {
            List<long> groupIds = new List<long>();
            var chatGroups = new List<ChatGroup>();
            foreach (var uid in userIds)
            {
                var userGroupList = ChatGroupOfUser(uid);
                if (userGroupList == null)
                {
                    break;
                }
                foreach (var group in userGroupList)
                {
                    long id = group.ChatId;
                    if (groupIds.IndexOf(id) < 0)
                    {
                        groupIds.Add(id);
                    }
                }
            }
            return groupIds;
        }
        public List<ChatGroup>? ChatGroupOfUser(int? uid)
        {
            if(uid == null) { return null; }
            if (User(uid) == null) { return null; }
            List<ChatGroup> chatGroups = new List<ChatGroup>();
            var groupRelations = db.ChatGroupRelations.Where(gr => gr.UserId == uid.Value).ToList();
            foreach (var relation in groupRelations)
            {
                var group = db.ChatGroups.Find(relation.ChatId);
                if(group != null)
                {
                    chatGroups.Add(group);
                }                
            }
            return chatGroups;
        }
        public int MemberCountOfGroup(ChatGroup group)
        {
            List<ChatGroupRelation> relations = db.ChatGroupRelations.Where(g=>g.ChatId == group.ChatId).ToList();
            return relations.Count;
        }

        public List<ChatGroup>? PrivateGroupOfUser(int? uid)
        {
            var groups = ChatGroupOfUser(uid);
            if (groups == null) { return null; }
            return groups.FindAll(g => g.IsPrivate == true);
        }
        public long? GetPrivateGroup(int? uid1, int? uid2)
        {
            if (uid1 == null || uid2 == null) { return null; }
            if(User(uid1) == null || User(uid2) == null) { return null; }
            var pGroup = PrivateGroupOfUser(uid1);
            if(pGroup == null)
            {
                return NewPrivateGroup(uid1.Value, uid2.Value);
            }
            foreach (var pg in pGroup)
            {
                var privateGroupWithUser2 = db.ChatGroupRelations.Where(cgr => cgr.ChatId == pg.ChatId && cgr.UserId == uid2.Value);
                if (privateGroupWithUser2.Count() > 0)
                {
                    return privateGroupWithUser2.First().ChatId;
                }
            }
            return NewPrivateGroup(uid1.Value, uid2.Value);
        }
        public long NewPrivateGroup(int uid1,int uid2)
        {
            ChatGroup chatGroup = new ChatGroup();
            chatGroup.ChatTitle = $"_default_";
            chatGroup.IsPrivate = true;
            chatGroup.LastActivity = Time.TimeNowInt();
            act.Create(chatGroup);
            ChatGroupRelation groupRelation1 = new ChatGroupRelation
            {
                ChatId = chatGroup.ChatId,
                UserId = uid1,
                LastConnected = Time.TimeNowInt()
            };
            ChatGroupRelation groupRelation2 = new ChatGroupRelation
            {
                ChatId = chatGroup.ChatId,
                UserId = uid2,
                LastConnected = Time.TimeNowInt()
            };
            act.Create(groupRelation1);
            act.Create(groupRelation2);
            return chatGroup.ChatId;
        }
    }
    public class dbItemRelatedQuery
    {
        protected dbQuery q = new dbQuery();
        public List<Post> RelatedPost(long? pid)
        {
            List<Post> posts = new List<Post>();    
            return q.PostFamily(pid) ?? posts;
        }
        public List<User>? RelatedUser(int? pid)
        {
            return null;
        }
    }
}
