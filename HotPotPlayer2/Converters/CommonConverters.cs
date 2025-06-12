using Avalonia.Data.Converters;
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
    }
}
