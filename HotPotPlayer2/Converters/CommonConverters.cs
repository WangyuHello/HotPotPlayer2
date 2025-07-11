﻿using Avalonia.Data.Converters;
using Avalonia.Media;
using Jellyfin.Sdk.Generated.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2.Converters
{
    public static class CommonConverters
    {
        public static FuncMultiValueConverter<string?, bool> NavButtonEnable =>
            new(names =>
            {
                var names2 = names.ToArray();
                var name = names2[0];
                var selectedName = names2[1];
                if (name == null || selectedName == null) return true;
                return !selectedName.StartsWith(name);
            });

        public static FuncMultiValueConverter<object, string> ListIndex => new(os =>
        {
            var os2 = os.ToArray();
            if (os2[0] == null) return string.Empty;
            var i = os2[0] as BaseItemDto;
            var list = os2[1] as List<BaseItemDto>;
            return (list!.IndexOf(i!) + 1).ToString();
        });

        public static FuncMultiValueConverter<object, SolidColorBrush> EvenColor => new(os =>
        {
            var os2 = os.ToArray();
            if (os2[0] == null) return new SolidColorBrush(Colors.Transparent);
            var i = os2[0] as BaseItemDto;
            var list = os2[1] as List<BaseItemDto>;
            var ind = list!.IndexOf(i!);

            return ((ind % 2) == 0) ? new SolidColorBrush(Color.FromArgb(0x40, 0xf4, 0xf7, 0xfe)) : new SolidColorBrush(Colors.Transparent);
        });

    }
}
