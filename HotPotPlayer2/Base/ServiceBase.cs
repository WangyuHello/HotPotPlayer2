using CommunityToolkit.Mvvm.ComponentModel;

namespace HotPotPlayer2.Base
{
    public class ServiceBase(ConfigBase config, AppBase app): ObservableObject
    {
        protected ConfigBase Config { get; init; } = config;
        protected AppBase App { get; init; } = app;
    }
}
