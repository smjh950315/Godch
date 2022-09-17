namespace Godch.ViewModels
{
    public class ViewList
    {
        public List<dynamic> Values = new();
        public dbQuery q = new();
        public ModelCast mCast = new();
        public ViewList() { }
        public ViewList(dynamic values)
        {
            CastList(values);
        }
        public void CastList(dynamic values)
        {
            foreach (dynamic value in values)
            {
                Values.Add(mCast.Cast(value));
            }
        }
        public List<dynamic> CastToList(dynamic values)
        {
            List<dynamic> list = new List<dynamic>();
            foreach (dynamic value in values)
            {
                list.Add(mCast.Cast(value));
            }
            return list;
        }
    }
    public class ViewBase
    {
        protected dbQuery q = new();
        public string ViewTitle { get; set; }
        public ViewBase()
        {
            ViewTitle = "untitled";
        }
        public ViewBase(string title)
        {
            ViewTitle = title;
        }
    }
}
