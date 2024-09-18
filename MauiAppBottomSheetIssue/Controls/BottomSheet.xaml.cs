using Microsoft.Maui.Controls;

namespace MauiAppBottomSheetIssue.Controls;

public partial class BottomSheet : ContentView
{
    /// <summary>
    /// The height of the BottomSheet when open.
    /// </summary>
    public static readonly BindableProperty SheetHeightProperty = BindableProperty.Create(
       nameof(SheetHeight),
       typeof(double),
       typeof(BottomSheet),
       defaultValue: default(double),
       defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Property that will be the content of the BottomSheet control.
    /// </summary>
    public static readonly BindableProperty SheetContentProperty = BindableProperty.Create(
            nameof(SheetContent),
            typeof(View),
            typeof(BottomSheet),
            defaultValue: default(View),
            defaultBindingMode: BindingMode.TwoWay);

    private const uint SheetAnimationDuration = 2000;

    private bool isPanInXPlane;
    private bool isPanInYPlane;


    public BottomSheet()
	{
		InitializeComponent();
	}

    public double SheetHeight
    {
        get
        {
            return (double)this.GetValue(SheetHeightProperty);
        }

        set
        {
            this.SetValue(SheetHeightProperty, value);
            this.OnPropertyChanged();
        }
    }

    public View SheetContent
    {
        get
        {
            return (View)this.GetValue(SheetContentProperty);
        }

        set
        {
            this.SetValue(SheetContentProperty, value);
            this.OnPropertyChanged();
        }
    }

    public async void SheetContainer_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        var x = e.TotalX; // TotalX Left/Right
        var y = e.TotalY; // TotalY Up/Down

        if (e.StatusType == GestureStatus.Running)
        {
            if ((x >= 5 || x <= -5) && !this.isPanInXPlane && !this.isPanInYPlane)
            {
                this.isPanInXPlane = true;
            }

            if ((y >= 5 || y <= -5) && !this.isPanInYPlane && !this.isPanInXPlane)
            {
                this.isPanInYPlane = true;
            }
        }
        else if (e.StatusType == GestureStatus.Completed)
        {
            if (this.isPanInYPlane && !this.isPanInXPlane)
            {
                if (y <= 0)
                {
                    await this.OpenSheet(0);
                }

                if (y >= 0)
                {
                    await this.CloseSheet();
                }
            }
        }
    }

    public async Task OpenSheet(double? position = null)
    {
        System.Diagnostics.Debug.WriteLine("BottomSheetControl.OpenSheet " + position);

        if (position.HasValue == false)
        {
            position = 0;
            System.Diagnostics.Debug.WriteLine("BottomSheetControl.OpenSheet Default Set " + position);
        }

        ////await Task.WhenAll(
        ////    this.Backdrop.FadeTo(0.4, length: BottomSheetControl.SheetAnimationDuration),
        ////    this.Sheet.TranslateTo(0, position.Value, length: BottomSheetControl.SheetAnimationDuration, easing: Easing.SinIn));

        this.Backdrop.Opacity = 0.4;
        this.Sheet.TranslationY = position.Value;

        this.BottomSheetContentView.InputTransparent = this.Backdrop.InputTransparent = false;
    }

    public async Task CloseSheet()
    {
        await Task.WhenAll(
            this.Backdrop.FadeTo(0, length: BottomSheet.SheetAnimationDuration),
            this.SheetContainer.Content.TranslateTo(x: 0, y: this.SheetHeight + 60, length: BottomSheet.SheetAnimationDuration, easing: Easing.SinIn));

        this.BottomSheetContentView.InputTransparent = this.Backdrop.InputTransparent = true;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        System.Diagnostics.Debug.WriteLine("**** OnBindingContextChanged SheetContent TranslationY:" + this.SheetContainer.Content.TranslationY + " Moving to: " + (this.SheetHeight + 60));
        this.SheetContainer.Content.TranslationY = this.SheetHeight + 60;
    }

    private async void IOSSheetCloseButton_Clicked(object sender, EventArgs e)
    {
        await this.CloseSheet();
    }

    private async void Backdrop_Tapped(object sender, System.EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("Backdrop_Tapped. Now calling CloseSheet");
        await this.CloseSheet();
    }
}