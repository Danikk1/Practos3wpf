using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;

namespace AudioPlayer
{
    public partial class MainWindow : Window
    {
        private List<string> audioFiles = new List<string>();
        private int currentIndex = 0;
        private bool isPlaying = false;
        private bool isRepeating = false;
        private bool isShuffled = false;
        private DispatcherTimer timer;
        private List<string> history = new List<string>(); 

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFileDialog();
            folderDialog.Filter = "Audio files (*.mp3, *.wav, *.m4a)|*.mp3;*.wav;*.m4a";
            if (folderDialog.ShowDialog() == true)
            {
                string directory = Path.GetDirectoryName(folderDialog.FileName);
                string[] files = Directory.GetFiles(directory);
                audioFiles = files.Where(file => file.EndsWith(".mp3") || file.EndsWith(".wav") || file.EndsWith(".m4a")).ToList();
                currentIndex = 0;
                PlayCurrentAudio();
            }
        }

        private async void PlayCurrentAudio()
        {
            if (audioFiles.Count > 0)
            {
                mediaElement.Source = new Uri(audioFiles[currentIndex]);
                await Task.Delay(100); 
                mediaElement.Play();
                isPlaying = true;
                timer.Start();
                history.Add(audioFiles[currentIndex]); 
            }
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            NextAudio();
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                mediaElement.Pause();
                isPlaying = false;
            }
            else
            {
                mediaElement.Play();
                isPlaying = true;
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            PreviousAudio();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            NextAudio();
        }

        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            isRepeating = !isRepeating;
            if (isRepeating)
            {
                mediaElement.Position = TimeSpan.Zero; 
                mediaElement.Play(); 
            }
        }

        private void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            isShuffled = !isShuffled;
            if (isShuffled)
            {
                Random rng = new Random();
                audioFiles = audioFiles.OrderBy(a => rng.Next()).ToList();
            }
            else
            {
                audioFiles = audioFiles.OrderBy(a => a).ToList();
            }
            currentIndex = 0;
            PlayCurrentAudio();
        }

        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan)
            {
                mediaElement.Position = TimeSpan.FromSeconds(positionSlider.Value * mediaElement.NaturalDuration.TimeSpan.TotalSeconds / 100);
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement.Volume = volumeSlider.Value;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan)
            {
                positionSlider.Value = mediaElement.Position.TotalSeconds / mediaElement.NaturalDuration.TimeSpan.TotalSeconds * 100;
                TimeSpan remainingTime = mediaElement.NaturalDuration.TimeSpan - mediaElement.Position;
                durationTextBlock.Text = $"Длительность: {mediaElement.NaturalDuration.TimeSpan:mm\\:ss}";
                currentTimeTextBlock.Text = $"Текущее время: {mediaElement.Position:mm\\:ss}";
            }
        }

        private void PreviousAudio()
        {
            if (audioFiles.Count > 0)
            {
                currentIndex = (currentIndex == 0) ? audioFiles.Count - 1 : currentIndex - 1;
                PlayCurrentAudio();
            }
        }

        private void NextAudio()
        {
            if (audioFiles.Count > 0)
            {
                currentIndex = (currentIndex == audioFiles.Count - 1) ? 0 : currentIndex + 1;
                PlayCurrentAudio();
            }
        }

        private void ShowHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            // Создание и отображение окна истории прослушивания
            Window2 historyWindow = new Window2(history);
            historyWindow.Show();
        }
    }
}
