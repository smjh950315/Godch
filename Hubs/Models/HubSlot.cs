using Godch.Event;
using String = MyLib.String;
namespace Godch.Hubs.Models
{
    public class HubSlot
    {
        public bool IsUsing { get; private set; }
        public List<EventData> EventList { get; set; }
        public string SlotId { get; set; }
        public int StartNotify_Index { get; set; }
        public long? NotifyRead { get; set; }
        public void AddNotify(EventData eventData)
        {
            EventList.Add(eventData);
        }
        public void LoadNotify(List<EventData> eventData)
        {
            foreach(var evtData in eventData)
            {
                EventList.Add(evtData);
            }
        }
        public void Clear()
        {
            IsUsing = false;
            EventList.Clear();
        }
        public HubSlot()
        {
            IsUsing = true;
            SlotId = "";
            EventList = new List<EventData>();
        }
        public HubSlot(String userId) :this()
        {
            SlotId = "notify" + userId;            
        }
    }
}
