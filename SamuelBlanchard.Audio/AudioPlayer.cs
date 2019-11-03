using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.Storage;

namespace SamuelBlanchard.Audio
{
    public class AudioPlayer<TKey>
    {
        public AudioPlayer()
        {
        }

        public AudioGraph AudioGraph
        {
            get
            {
                return _audioGraph;
            }
        }

        private AudioGraph _audioGraph;
        
        public AudioDeviceOutputNode AudioDeviceOuputNode
        {
            get
            {
                return _outputNode;
            }
        }
        
        private AudioDeviceOutputNode _outputNode;

        private Dictionary<TKey, AudioFileInputNode> soundLibrary = new Dictionary<TKey, AudioFileInputNode>();

        public bool IsMute
        {
            get
            {
                return _isMute;
            }

            set
            {
                if(this._isMute != value)
                {
                    this._isMute = value;
                    
                    if(this._audioGraph != null)
                    {
                        if(value == true)
                        {
                            volumeBeforeMute = Volume;
                            Volume = 0;
                        }
                        else
                        {
                            Volume = volumeBeforeMute;
                        }
                    }
                }
            }
        }

        private bool _isMute = false;

        /// <summary>
        /// Swith mute
        /// </summary>

        public void SwitchMute()
        {
            this.IsMute = !IsMute;
        }

        public double Volume
        {
            get
            {
                return _outputNode.OutgoingGain;
            }

            set
            {
                _outputNode.OutgoingGain = value;
            }
        }

        private double volumeBeforeMute = 1.0;

        public bool IsInitialized
        {
            get;
            private set;
        }

        public async Task<bool> InitializeAsync()
        {
            if(this.IsInitialized == true)
            {
                return true;
            }

            var result = await AudioGraph.CreateAsync(new AudioGraphSettings(AudioRenderCategory.Media));

            if (result.Status != AudioGraphCreationStatus.Success)
            {
                return false;
            }

            _audioGraph = result.Graph;
            var outputResult = await _audioGraph.CreateDeviceOutputNodeAsync();
            if (outputResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                return false;
            }

            _outputNode = outputResult.DeviceOutputNode;

            if (this.IsMute == false)
            {
                _audioGraph.Start();
            }

            this.IsInitialized = true;

            return true;
        }

        public async Task<bool> CopySound(TKey keySource, TKey keyDestination)
        {
            if (this.IsInitialized == false)
            {
                return false;
            }

            var inputNode = this.GetSound(keySource);

            var file = inputNode.SourceFile;

            return await this.AddSound(keyDestination, file);
        }

        public async Task<bool> AddSound(TKey key, StorageFile soundFile)
        {
            if(this.IsInitialized == false)
            {
                return false;
            }

            if(soundLibrary.ContainsKey(key) == false)
            {
                var fileInputNodeResult = await _audioGraph.CreateFileInputNodeAsync(soundFile);

                if (fileInputNodeResult.Status != AudioFileNodeCreationStatus.Success)
                {
                    return false;
                }

                var fileInputNode = fileInputNodeResult.FileInputNode;

                fileInputNode.Stop();

                fileInputNode.AddOutgoingConnection(_outputNode);

                this.soundLibrary.Add(key, fileInputNodeResult.FileInputNode);
            }
            else
            {
                throw new Exception("The sound '" + key + "' already exists in the sound library!");
            }

            return true;
        }


        public async Task<bool> AddSoundFromApplication(TKey key, string uriFile)
        {
            var soundFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uriFile));
            await AddSound(key, soundFile);

            return true;
        }

        public void Stop(TKey key)
        {
            if (this.IsInitialized == false)
            {
                return;
            }

            var fileInputNode = GetSound(key);
            fileInputNode.Stop();
        }

        /// <summary>
        /// Retirer le son de la librairie
        /// </summary>
        /// <param name="key"></param>
        public void RemoveSound(TKey key)
        {
            if (this.IsInitialized == false)
            {
                return;
            }

            var fileInputNode = GetSound(key);

            fileInputNode.Stop();
            fileInputNode.RemoveOutgoingConnection(_outputNode);
            fileInputNode.Dispose();               
        }

        /// <summary>
        /// Jouer et attendre
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>

        public async Task<bool> PlaySoundAsync(TKey key, double volume = 1)
        {
            if (this.IsInitialized == false)
            {
                return false;
            }
            
            var fileInputNode = GetSound(key);

            TypedEventHandler<AudioFileInputNode, object> completed = null;

            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

            completed = (audioFileInputNode, args) =>
            {
                fileInputNode.FileCompleted -= completed;
                taskCompletionSource.TrySetResult(null);
            };

            fileInputNode.FileCompleted += completed;

            fileInputNode.Seek(TimeSpan.Zero);

            this.SetVolume(fileInputNode, volume);

            fileInputNode.Start();

            await taskCompletionSource.Task;

            return true;
        }

        public void SetVolume(TKey key, double volume)
        {
            var fileInputNode = GetSound(key);
            SetVolume(fileInputNode, volume);
        }

        private void SetVolume(AudioFileInputNode fileInputNode, double volume)
        {
            fileInputNode.OutgoingGain = volume;
        }

        public bool PlaySound(TKey key, double volume = 1, bool isLoop = false)
        {
            if (this.IsInitialized == false)
            {
                return false;
            }

            var fileInputNode = GetSound(key);

            fileInputNode.Stop();

            if (isLoop)
            {
                fileInputNode.LoopCount = null;
            }
            else
            {
                fileInputNode.LoopCount = 0;
            }

            fileInputNode.Seek(TimeSpan.Zero);

            this.SetVolume(fileInputNode, volume);
            
            fileInputNode.Start();

            return true;
        }

        private AudioFileInputNode GetSound(TKey key)
        {
            if (soundLibrary.ContainsKey(key))
            {
                return this.soundLibrary[key];
            }

            throw new Exception("The sound '" + key + "' doesn't exist in the sound library!");
        }
    }
}
