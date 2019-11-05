using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using SamuelBlanchard.Audio;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AudioPlayerSample
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        // key for the sound dictionnary of the AudioPlayer
        enum AudioKeys
        {
            Loop,
            Connected,
            Connected10Channels
        }

        AudioPlayer<AudioKeys> audioPlayer = new AudioPlayer<AudioKeys>();

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // initialize the audioPlayer
            await audioPlayer.InitializeAsync();

            // Add some sounds

            // Add 1 inputNode for sound loop
            await audioPlayer.AddSoundFromApplicationAsync(AudioKeys.Loop, "ms-appx:///Assets/Sounds/Loop.wav");
            // Add 1 inputNode
            await audioPlayer.AddSoundFromApplicationAsync(AudioKeys.Connected, "ms-appx:///Assets/Sounds/Connected.wav");
            // Add 10 inputNodes
            await audioPlayer.AddSoundFromApplicationAsync(AudioKeys.Connected10Channels, "ms-appx:///Assets/Sounds/Connected.wav", 10);
        }

        private void ButtonPlayLoop_Click(object sender, RoutedEventArgs e)
        {
            audioPlayer.PlayLoop(AudioKeys.Loop);    
        }

        private void ButtonStopLoop_Click(object sender, RoutedEventArgs e)
        {
            audioPlayer.Stop(AudioKeys.Loop);
        }

        private void CheckBoxMute_Click(object sender, RoutedEventArgs e)
        {
            audioPlayer.SwitchMute();
        }

        private async void ButtonPlayAsync_Click(object sender, RoutedEventArgs e)
        {
            await audioPlayer.PlaySoundAsync(AudioKeys.Connected);
        }

        private void ButtonPlayFireAndForget_Click(object sender, RoutedEventArgs e)
        {
            audioPlayer.PlaySound(AudioKeys.Connected);
        }

        private void ButtonPlayMultiChannels_Click(object sender, RoutedEventArgs e)
        {
            audioPlayer.PlaySound(AudioKeys.Connected10Channels);
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            audioPlayer.Stop();
        }
    }
}
