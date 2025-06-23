using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using HotPotPlayer2.Base;
using HotPotPlayer2.Models;
using System;
using System.Windows.Input;

namespace HotPotPlayer2.Controls;

public partial class AddJellyfinServerPopup : UserControl
{
    public AddJellyfinServerPopup()
    {
        InitializeComponent();
    }

    public event EventHandler? OnShow;
    public event EventHandler? OnHide;
    public void Show() { OnShow?.Invoke(this, new EventArgs()); }
    public void Hide() { OnHide?.Invoke(this, new EventArgs()); }

    public ICommand? OnLoginSucceededCommand
    {
        get { return (ICommand?)GetValue(OnLoginSucceededCommandProperty); }
        set { SetValue(OnLoginSucceededCommandProperty, value); }
    }

    public static readonly AvaloniaProperty<ICommand?> OnLoginSucceededCommandProperty =
        AvaloniaProperty.Register<AddJellyfinServerPopup,ICommand?>("OnLoginSucceededCommand");

    public string? QuickCode
    {
        get { return (string?)GetValue(QuickCodeProperty); }
        set { SetValue(QuickCodeProperty, value); }
    }

    public static readonly AvaloniaProperty<string?> QuickCodeProperty =
        AvaloniaProperty.Register<AddJellyfinServerPopup,string?>("QuickCode");

    private async void OnLogin(object sender, RoutedEventArgs e)
    {
        e.Handled = true;

        if (Application.Current is not AppBase app) return;

        if (string.IsNullOrEmpty(JellyfinUrl.Text) ||
            string.IsNullOrEmpty(JellyfinPassword.Text) ||
            string.IsNullOrEmpty(JellyfinUserName.Text))
        {
            app.ShowToast(new ToastInfo { Text = "不能为空" });
            return;
        }

        var prefix = UrlPrefix.SelectedIndex == 0 ? "https://" : "http://";
        var url = prefix + JellyfinUrl.Text;

        var (info, msg) = await app.JellyfinMusicService.TryGetSystemInfoPublicAsync(url);
        if (info == null)
        {
            app.ShowToast(new ToastInfo { Text = msg });
            return;
        }

        var (loginResult, message) = await app.JellyfinMusicService.TryLoginAsync(url, JellyfinUserName.Text, JellyfinPassword.Text);

        if (!loginResult)
        {
            app.ShowToast(new ToastInfo { Text = message });
            return;
        }

        app.ShowToast(new ToastInfo { Text = "登录成功" });

        app.Config.SetConfig("JellyfinUrl", url);
        app.Config.SetConfig("JellyfinUserName", JellyfinUserName.Text);
        app.Config.SetConfig("JellyfinPassword", JellyfinPassword.Text);
        app.Config.SaveSettings();

        app.JellyfinMusicService.Reset();

        if (OnLoginSucceededCommand != null && OnLoginSucceededCommand.CanExecute(null)) {
            OnLoginSucceededCommand.Execute(null);
        }
    }
}
