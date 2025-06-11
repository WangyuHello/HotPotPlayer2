using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;
using HotPotPlayer2.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2.ViewModels
{
    public class MainWindowPageTemplate : IDataTemplate
    {
        public Control? Build(object? data)
        {
            if (data is PageViewModelBase pv)
            {
                return pv.Page;
            }
            else
            {
                return new TextBlock { Text = "Error" };
            }
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}
