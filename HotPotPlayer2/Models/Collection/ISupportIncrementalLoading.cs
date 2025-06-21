using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HotPotPlayer2.Models.Collection
{
    public interface ISupportIncrementalLoading
    {
        public bool HasMoreItems { get; }
        public bool IsLoading { get; }
        public Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken token);
    }
}
