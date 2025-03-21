using Melodify.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode.Search;

namespace Melodify.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isSearchEnabled = true; // Default value

        public bool IsSearchEnabled
        {
            get => _isSearchEnabled;
            set
            {
                if (_isSearchEnabled != value)
                {
                    _isSearchEnabled = value;
                    OnPropertyChanged(); // Notify the UI of the change
                }
            }
        }

        public ObservableCollection<VideoItem> SearchResults { get; } = new();

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
