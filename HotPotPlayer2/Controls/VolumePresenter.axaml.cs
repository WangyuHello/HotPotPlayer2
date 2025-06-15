using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace HotPotPlayer2.Controls;

public partial class VolumePresenter : UserControl
{
    public VolumePresenter()
    {
        InitializeComponent();
    }

    public int Volume
    {
        get { return (int)GetValue(VolumeProperty)!; }
        set { SetValue(VolumeProperty, value); }
    }

    public static readonly AvaloniaProperty<int> VolumeProperty =
        AvaloniaProperty.Register<VolumePresenter, int>("Volume");
}

public static class VolumePresenterConverters
{
    public static FuncValueConverter<int, string> GetVolumeText = new(i =>
    {
        return i + "%";
    });

    /// <summary>
    /// 静音，0格，1格，2格，3格
    /// </summary>
    readonly static string[] volIcons = [ "\uE198", "\uE992", "\uE993", "\uE994", "\uE995" ];

    public static FuncValueConverter<int, string> GetVolumeIcon = new(v =>
    {
        var v2 = v;
        return v2 switch
        {
            0 => volIcons[0],
            (> 0) and (<= 25) => volIcons[1],
            (> 25) and (<= 50) => volIcons[2],
            (> 50) and (<= 75) => volIcons[3],
            _ => volIcons[4],
        };
    });

    public static FuncValueConverter<int, double> GetVolumeTranslation = new(v =>
    {
        var x = -72 * (100 - (float)v) / 100 - 8;
        return x;
    });
}