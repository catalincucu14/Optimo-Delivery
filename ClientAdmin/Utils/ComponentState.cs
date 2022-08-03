namespace ClientAdmin.Utils
{
    public class ComponentState
    {
        public List<string> Errors { get; set; } = new();

        public bool Processing { get; set; } = false;

        public bool Edit { get; set; } = false;

        public bool Disable { get; set; } = true;

        public bool Ready { get; set; } = false;

        public bool Loaded { get; set; } = false;

        public void NotEdit()
        {
            Edit = !Edit;
            Disable = !Edit;
        }
    }
}
