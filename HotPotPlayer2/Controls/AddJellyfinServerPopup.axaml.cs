using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using DebounceThrottle;
using HotPotPlayer2.Base;
using HotPotPlayer2.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Input;

namespace HotPotPlayer2.Controls;

public partial class AddJellyfinServerPopup : UserControl
{
    public AddJellyfinServerPopup()
    {
        InitializeComponent(); 
    }

    private DebounceDispatcher? _debounceDispatcher;
    private DebounceDispatcher DebounceDispatcher => _debounceDispatcher ??= new DebounceDispatcher(
            interval: TimeSpan.FromMilliseconds(2000),
            maxDelay: TimeSpan.FromSeconds(5));

    private HttpClient? _http;
    private HttpClient Http
    {
        get
        {
            if(_http == null)
            {
                _http = new HttpClient();
                _http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("HotPotPlayer", (Application.Current as AppBase)!.ApplicationVersion));
                _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1.0));
                _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8));
            }
            return _http;
        }
    }

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

    private async void UrlChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(JellyfinUrl.Text))
        {
            return;
        }
        var prefix = UrlPrefix.SelectedIndex == 0 ? "https://" : "http://";
        var url = prefix + JellyfinUrl.Text;
        await DebounceDispatcher.DebounceAsync(async () =>
        {
            if (Application.Current is not AppBase app) return;
            try
            {
                var code = await app.JellyfinMusicService.QuickConnectInitiate(Http, url);
                Dispatcher.UIThread.Post(() => { QuickCode = code; });
            }
            catch (Exception)
            {

            }
        });
    }
}
