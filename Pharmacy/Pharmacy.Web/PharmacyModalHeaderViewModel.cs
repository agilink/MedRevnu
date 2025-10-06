namespace ATI.Pharmacy.Web
{
    public class PharmacyModalHeaderViewModel
    {
        public string Title { get; set; }
        public string Header { get; set; }

        public PharmacyModalHeaderViewModel(string title, string header = null)
        {
            Title = title;
            Header = header;
        }
    }
}