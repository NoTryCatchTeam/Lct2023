using System;
using Android.Media;

namespace Lct2023.Android.Listeners;

public class MediaPlayerPreparedListener : Java.Lang.Object, MediaPlayer.IOnPreparedListener
{
    private readonly Action _onPrepared;

    public MediaPlayerPreparedListener(Action onPrepared)
    {
        _onPrepared = onPrepared;
    }

    public void OnPrepared(MediaPlayer mediaPlayer)
    {
        _onPrepared();
    }
}
