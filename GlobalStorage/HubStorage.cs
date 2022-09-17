using Godch.Hubs.Models;

namespace Godch.GlobalStorage
{
    public static class HubStorage
    {
        public static List<Group> GroupList = new List<Group>();
        public static List<HubSlot> NotifySlot = new List<HubSlot>();
        public static List<HubConnection> Connections = new List<HubConnection>();
    }
}
