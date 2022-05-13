namespace FairPlayTube.Client.Shared
{
    public partial class CssGridLayout
    {
        private bool ShowCultureSelector { get; set; }        
        private void OnShowCultureSelectorClicked()
        {
            this.ShowCultureSelector = true;
        }
        private void HideCultureSelector()
        {
            this.ShowCultureSelector = false;
        }
    }
}
