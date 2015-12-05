using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Norbert.Modules.Tumblr
{
    public interface ITumblrClient
    {
        Task<List<dynamic>> GetPhotoPostsAsync(string tag, DateTime before, int limit);
    }
}