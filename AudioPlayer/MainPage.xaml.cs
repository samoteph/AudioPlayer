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

using Sound = SamuelBlanchard.Audio;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AudioPlayer
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

        enum AudioKeys
        {
            Loop,
            Switch
        }

        Sound.AudioPlayer<AudioKeys> audioPlayer = new Sound.AudioPlayer<AudioKeys>();

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await audioPlayer.InitializeAsync();

            await audioPlayer.AddSoundFromApplication(AudioKeys.Loop, "ms-appx:///Assets/Sounds/Loop.wav");
            await audioPlayer.AddSoundFromApplication(AudioKeys.Switch, "ms-appx:///Assets/Sounds/Switch.wav");
        }

        private void ButtonPlayLoop_Click(object sender, RoutedEventArgs e)
        {
            audioPlayer.PlaySound(AudioKeys.Loop, 1, true);    
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
            await audioPlayer.PlaySoundAsync(AudioKeys.Switch);
        }

        private void ButtonPlayFireAndForget_Click(object sender, RoutedEventArgs e)
        {
            audioPlayer.PlaySound(AudioKeys.Switch);
        }
    }
}
