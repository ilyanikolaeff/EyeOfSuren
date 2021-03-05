namespace WinEvtLogWatcher
{
    class User
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
