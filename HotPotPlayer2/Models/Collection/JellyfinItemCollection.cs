using Jellyfin.Sdk.Generated.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HotPotPlayer2.Models.Collection
{
    public class JellyfinItemCollection(Func<BaseItemDto> library, Func<Func<BaseItemDto>, int, int, CancellationToken, Task<List<BaseItemDto>?>> func) : ObservableCollection<BaseItemDto>, ISupportIncrementalLoading
    {
        readonly Func<BaseItemDto> _library = library;
        readonly Func<Func<BaseItemDto>, int, int, CancellationToken, Task<List<BaseItemDto>?>> _func = func;

        int _pageNum;
        const int _perPageItem = 30;
        bool _hasMore = true;
        bool _isLoading = false;
        public bool HasMoreItems => _hasMore;
        public bool IsLoading => _isLoading;

        public async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken token)
        {
            if (_isLoading || !_hasMore) return new LoadMoreItemsResult() { Count = 0 };
            _isLoading = true;
            var list = await _func(_library, _pageNum * _perPageItem, _perPageItem, token);
            _isLoading = false;
            _pageNum++;
            if (list == null)
            {
                _hasMore = false;
                return new LoadMoreItemsResult() { Count = 0 };
            }
            else if (list.Count < _perPageItem)
            {
                _hasMore = false;
                foreach (var item in list)
                {
                    Add(item);
                }
                return new LoadMoreItemsResult() { Count = list.Count };
            }
            else
            {
                _hasMore = true;
                foreach (var item in list)
                {
                    Add(item);
                }
                return new LoadMoreItemsResult() { Count = list.Count };
            }
        }
    }
}
