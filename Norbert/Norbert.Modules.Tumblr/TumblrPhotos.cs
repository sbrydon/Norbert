using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;
using Norbert.Modules.Common;
using Norbert.Modules.Common.Events;
using Norbert.Modules.Common.Exceptions;

namespace Norbert.Modules.Tumblr
{
    public class TumblrPhotos
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TumblrPhotos));

        private static readonly Regex Regex =
            new Regex(@"tumblr\s*(?:of\s*)?(?<tag>.*)", RegexOptions.IgnoreCase);

        private static readonly DateTime MinBefore = new DateTime(2010, 1, 1);

        private readonly IChatClient _chatClient;
        private readonly ITumblrClient _tumblrClient;
        private readonly IRandomiser _randomiser;

        public TumblrPhotos(IChatClient chatClient, ITumblrClient tumblrClient, IRandomiser randomiser)
        {
            _chatClient = chatClient;
            _tumblrClient = tumblrClient;
            _randomiser = randomiser;

            _chatClient.CommandReceived += OnCommandReceived;
        }

        private async void OnCommandReceived(object sender, CommandEventArgs cmd)
        {
            var match = Regex.Match(cmd.Message);
            if (!match.Success)
            {
                Log.Debug($"Message ignored: '{cmd.Message}' doesn't match '{Regex}'");
                return;
            }

            var tag = match.Groups["tag"].Value.TrimEnd();
            if (tag == string.Empty)
            {
                Log.Debug($"Message ignored: matches '{Regex}' but <tag> is empty");
                return;
            }

            Log.Debug($"Replying: '{cmd.Message}' matches '{Regex}', <tag> = '{tag}'");

            try
            {
                var post = await GetRandomPost(tag);
                var tumblrMsg = post == null
                    ? $"{cmd.Nick}: Whoops, no tumblrs found"
                    : $"{cmd.Nick}: {post.image_permalink}";

                _chatClient.SendMessage(tumblrMsg, cmd.Source);
            }
            catch (HttpClientException)
            {
                _chatClient.SendMessage($"{cmd.Nick}: Whoops, something went wrong", cmd.Source);
            }
        }

        private async Task<dynamic> GetRandomPost(string tag)
        {
            var allPosts = new List<dynamic>();

            for (var i = 0; i < 3; i++)
            {
                var before = _randomiser.NextDateTime(MinBefore);
                var posts = await _tumblrClient.GetPhotoPostsAsync(tag, before, 7);
                allPosts.AddRange(posts);
            }

            if (!allPosts.Any())
            {
                Log.Debug("No posts found!");
                return null;
            }

            var index = _randomiser.NextInt(allPosts.Count);
            Log.Debug($"{allPosts.Count} posts found, choosing post {index + 1}");

            return allPosts.ElementAtOrDefault(index);
        }
    }
}