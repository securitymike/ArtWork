﻿using System.Diagnostics;

using ArtWork.Database;
using ArtWork.Database.Tables;

using CommunityToolkit.WinUI.UI;

using Vanara.Windows.Shell;

namespace ArtWork.ViewModels;
public partial class ArtWorkDetailViewModel : ObservableRecipient, INavigationAware
{
    [ObservableProperty]
    private ObservableCollection<Art> arts;

    [ObservableProperty]
    private AdvancedCollectionView artsACV;

    [ObservableProperty]
    private object selectedItem;

    [ObservableProperty]
    private object selectedTime;

    [ObservableProperty]
    private object selectedWallpaperFit;

    [ObservableProperty]
    private int selectedInterval = 5;

    [ObservableProperty]
    private bool shuffleSlideShow;

    public ContentDialog SlideShowDialog { get; set; }

    public void OnNavigatedFrom()
    {
    }

    public void OnNavigatedTo(object parameter)
    {
        var simplifiedSig = parameter as string;
        using var db = new ArtWorkDbContext();
        var items = db.Arts.Where(x => x.SimplifiedSig.Equals(simplifiedSig));
        Arts = new(items);
        ArtsACV = new AdvancedCollectionView(Arts, true);
    }

    [RelayCommand]
    private void OnSetWallpaper(object sender)
    {
        var item = SelectedItem as Art;
        if (item != null)
        {
            var filePath = Path.Combine(Settings.ArtWorkDirectory, item.FileFolderPath);

            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                var wallpaperFit = ApplicationHelper.GetEnum<WallpaperFit>(button.Tag.ToString());
                WallpaperManager.WallpaperFit = wallpaperFit;
            }
            else
            {
                WallpaperManager.SetPicture(filePath, WallpaperFit.Fit);
            }
        }
    }

    [RelayCommand]
    private async Task OnSetSlideShow()
    {
        var item = SelectedItem as Art;
        if (item != null)
        {
            SlideShowDialog.PrimaryButtonClick += (s, e) =>
            {
                WallpaperFit wallpaperFit = WallpaperFit.Fit;
                TimeSpan timeSpan = TimeSpan.FromMinutes(SelectedInterval);
                if (SelectedWallpaperFit != null)
                {
                    var cmbItem = SelectedWallpaperFit as ComboBoxItem;
                    wallpaperFit = ApplicationHelper.GetEnum<WallpaperFit>(cmbItem.Content.ToString());
                }

                if (SelectedTime != null)
                {
                    var rbItem = SelectedTime as RadioButton;
                    var timeMode = rbItem.Content as string;
                    if (timeMode.Equals("Second"))
                    {
                        timeSpan = TimeSpan.FromSeconds(SelectedInterval);
                    }
                    else if (timeMode.Equals("Minute"))
                    {
                        timeSpan = TimeSpan.FromMinutes(SelectedInterval);
                    }
                    else
                    {
                        timeSpan = TimeSpan.FromHours(SelectedInterval);
                    }
                }

                var folderPath = Path.Combine(Settings.ArtWorkDirectory, item.FolderName);
                WallpaperManager.SetSlideshow(folderPath, wallpaperFit, timeSpan, ShuffleSlideShow);
            };

            await SlideShowDialog.ShowAsync();
        }
    }

    [RelayCommand]
    private async Task OnNavigateToDirectory()
    {
        var item = SelectedItem as Art;
        if (item != null)
        {
            var folderPath = Path.Combine(Settings.ArtWorkDirectory, item.FolderName);
            await Launcher.LaunchUriAsync(new Uri(folderPath));
        }
    }

    [RelayCommand]
    private void OnNavigateToFile()
    {
        var item = SelectedItem as Art;
        if (item != null)
        {
            var filePath = Path.Combine(Settings.ArtWorkDirectory, item.FileFolderPath);
            Process.Start("explorer.exe", $"/select,\"{filePath}\"");
        }
    }

    [RelayCommand]
    private async Task OnOpenImage()
    {
        var item = SelectedItem as Art;
        if (item != null)
        {
            var filePath = Path.Combine(Settings.ArtWorkDirectory, item.FileFolderPath);
            await Launcher.LaunchUriAsync(new Uri(filePath));
        }
    }
}
