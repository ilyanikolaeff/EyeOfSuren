using System.Media;

namespace WinEvtLogWatcher
{
    class EventSound
    {
        public int EventLevel { get; private set; }
        public string SoundFile { get; private set; }
        
        private SoundPlayer _soundPLayer;

        public EventSound(int level, string soundFileName)
        {
            EventLevel = level;
            SoundFile = soundFileName;
            _soundPLayer = new SoundPlayer(soundFileName);
        }

        public void Play()
        {
            if (_soundPLayer != null)
                _soundPLayer.Play();
        }
    }
}
