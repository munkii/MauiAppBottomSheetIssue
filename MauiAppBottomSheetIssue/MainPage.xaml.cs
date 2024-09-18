namespace MauiAppBottomSheetIssue
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await this.Sheet.CloseSheet();
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            await this.Sheet.OpenSheet();
        }
    }

}
