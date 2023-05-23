using System;
using Android.Media;

namespace Lct2023.Android.Listeners;

public class MediaPlayerCompletionListener : Java.Lang.Object, MediaPlayer.IOnCompletionListener
{
    private readonly Action _onCompletion;

    public MediaPlayerCompletionListener(Action onCompletion)
    {
        _onCompletion = onCompletion;
    }

    public void OnCompletion(MediaPlayer mediaPlayer) =>
        _onCompletion();
}
