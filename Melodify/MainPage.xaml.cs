using Melodify.Data;
using Melodify.Services;
using Melodify.ViewModels;
using System.Diagnostics.Metrics;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;

namespace Melodify
{
    public partial class MainPage : ContentPage
    {
        private readonly YoutubeClient _youtube = new YoutubeClient();
        private readonly AudioController _audioController;

        private readonly MainViewModel _mainViewModel;
        private CancellationTokenSource? _searchCancellationTokenSource;
        private bool _isLoading = false;

        public readonly static int LoadAmount = 10;

        public bool IsSearchEnabled { get; set; } = false;
        public MainPage()
        {
            InitializeComponent();
            _mainViewModel = new MainViewModel();
            BindingContext = _mainViewModel;
            _audioController = new AudioController(VideoWebView);
            _searchCancellationTokenSource = null;
        }

        private async Task<VideoItem> MakeItemFromVideoSearch(VideoSearchResult video, CancellationToken cancellationToken = default)
        {
            VideoItem videoItem = new VideoItem()
            {
                VideoId = video.Id,
                Channel = video.Author.ChannelTitle,
                Title = video.Title,
            };
            // Fetch the full video details using the video ID
            //ValueTask<Video> fullVideoTask = _youtube.Videos.GetAsync(video.Id, cancellationToken);

            // Thumbnail
            Thumbnail? thumbNail = video.Thumbnails.MinBy(x => x.Resolution.Width);
            if (thumbNail is not null)
            {
                videoItem.Thumbnail = thumbNail.Url;
            }

            // Duration
            if (video.Duration is not null)
            {
                var ts = video.Duration.Value;
                if (ts.Hours > 0)
                {
                    videoItem.Duration = $"{ts.Hours}:{ts.Minutes:D2}:{ts.Seconds:D2}";
                }
                else
                {
                    videoItem.Duration = $"{ts.Minutes}:{ts.Seconds:D2}";
                }
            }
            // Views
            /*Video? fullVideo = await fullVideoTask;
            if (fullVideo is not null)
            {
                long viewsCount = fullVideo.Engagement.ViewCount;
                string viewsStr = YoutubeHelper.FormatViewCount(viewsCount);
                videoItem.Views = viewsStr;
            }*/
            return videoItem;
        }

        private async Task LoadVideos(string query, int skipCount = 0, int maxResults = 5, CancellationToken cancellationToken = default)
        {
            int count = 0;
            int skipped = 0;
            await foreach (var result in _youtube.Search.GetResultsAsync(query, cancellationToken))
            {
                if(skipCount > 0 && skipped != skipCount)
                {
                    skipped++;
                    continue;
                }
                switch (result)
                {
                    case VideoSearchResult video:
                        {
                            var item = await MakeItemFromVideoSearch(video, cancellationToken);
                            _mainViewModel.SearchResults.Add(item);
                            count++;
                            break;
                        }
                    case PlaylistSearchResult playlist:
                        {

                            break;
                        }
                }
                if (count > 10)
                {
                    break;
                }
            }
        }

        private async void OnSearchClicked(object sender, EventArgs e)
        {
            try
            {
                if (_searchCancellationTokenSource is not null)
                {
                    _searchCancellationTokenSource.Cancel();
                }
            }
            catch (Exception ex)
            {

            }
            try
            {
                if (string.IsNullOrEmpty(VideoIdEntry.Text))
                {
                    throw new Exception("You must enter search query");
                }
                _searchCancellationTokenSource = new CancellationTokenSource();
                _mainViewModel.SearchResults.Clear();
                await LoadVideos(VideoIdEntry.Text, skipCount:0, maxResults: LoadAmount, cancellationToken: _searchCancellationTokenSource.Token);
            }
            catch (OperationCanceledException ex)
            {
                // Search canceled
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            //VideoListView.ItemsSource = videoList;
        }
        private async void OnPlayClicked(object sender, EventArgs e)
        {
            VideoSearchResult videoSearchResult;
            /* try
             {
                 var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(VideoIdEntry.Text);

                 // Get the audio-only stream with the highest bitrate
                 var audioStream = streamManifest.GetAudioOnlyStreams()
                                                 .OrderByDescending(s => s.Bitrate)
                                                 .FirstOrDefault();

                 if(audioStream != null)
                 {
                     VideoWebView.Source = audioStream.Url;
                 }
             }
             catch(Exception ex)
             {
                 await DisplayAlert("Error", ex.Message, "OK");
             }*/
        }

        private async void OnStopClicked(object sender, EventArgs e)
        {
            _audioController.Pause();
        }
        private void ContentPage_Unloaded(object? sender, EventArgs e)
        {
            if (_searchCancellationTokenSource is not null)
            {
                _searchCancellationTokenSource.Cancel();
            }
        }

        private async void VideoListView_RemainingItemsThresholdReached(object sender, EventArgs e)
        {
            if (_isLoading) return; // Prevent multiple requests
            _isLoading = true;

            try
            {
                await LoadVideos(VideoIdEntry.Text, skipCount: _mainViewModel.SearchResults.Count, maxResults: LoadAmount, _searchCancellationTokenSource.Token); // Load next batch
            }
            finally
            {
                _isLoading = false;
            }
        }
    }

}
