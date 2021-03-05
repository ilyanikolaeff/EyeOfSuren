using System.Collections.Generic;
using System.Linq;

namespace WinEvtLogWatcher
{
    class SoundQueue
    {
        private List<EventSound> _eventSoundPlayers = new List<EventSound>();
        private Queue<int> _soundQueue;

        private delegate void OnItemAddedHandler();
        private event OnItemAddedHandler OnItemAddedEvent;

        private object _locker = new object();

        private bool _playingSound;
        public SoundQueue()
        {
            _soundQueue = new Queue<int>();
            OnItemAddedEvent += OnItemAdded;
            _eventSoundPlayers = Settings.GetInstance().Sounds;
        }

        public void Add(int eventLevel)
        {
            _soundQueue.Enqueue(eventLevel);
            OnItemAddedEvent?.Invoke();
        }

        private void OnItemAdded()
        {
            // берем из очереди и проигрываем
            if (!_playingSound)
                Play(_soundQueue.Dequeue());
        }

        private void Play(int eventLevel)
        {
            // take event sound player
            //var sp = _eventSoundPlayers.FirstOrDefault(p => p.EventLevel == eventLevel);
            //if (sp != null)
            //{
            //    _playingSound = true;
            //    sp.Play();
            //    _playingSound = false;
            //}
        }
    }
}
