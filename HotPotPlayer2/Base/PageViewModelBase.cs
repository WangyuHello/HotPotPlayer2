﻿using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2.Base
{
    public class PageViewModelBase : ViewModelBase
    {
        public UserControl? Page { get; set; }
        public virtual string? Name { get; }

        public virtual void OnNavigatedTo(object? args) { }
    }
}
