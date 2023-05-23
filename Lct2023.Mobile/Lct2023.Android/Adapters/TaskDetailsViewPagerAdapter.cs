using System;
using System.Collections.Generic;
using System.Linq;
using AndroidX.Fragment.App;
using AndroidX.ViewPager2.Adapter;
using Lct2023.Android.Fragments.Exercises;
using Lct2023.ViewModels.Tasks;

namespace Lct2023.Android.Adapters;

public class TaskDetailsViewPagerAdapter : FragmentStateAdapter
{
    private readonly Fragment[] _fragmentsArray;

    public TaskDetailsViewPagerAdapter(FragmentActivity activity, IEnumerable<BaseExerciseItem> exercises)
        : base(activity)
    {
        var exercisesColl = exercises.ToList();

        ItemCount = exercisesColl.Count;

        _fragmentsArray = exercisesColl.Select<BaseExerciseItem, Fragment>(exercise =>
                exercise switch
                {
                    TextExerciseItem textExerciseItem => new TextExerciseFragment
                    {
                        DataContext = textExerciseItem,
                    },
                    VideoToAudioExerciseItem videoToAudioExerciseItem => new VideoToAudioExerciseFragment
                    {
                        DataContext = videoToAudioExerciseItem,
                    },
                    AudioToPictureExerciseItem audioToPictureExerciseItem => new AudioToPictureExerciseFragment
                    {
                        DataContext = audioToPictureExerciseItem,
                    },
                    _ => throw new NotImplementedException(),
                })
            .ToArray();
    }

    public override int ItemCount { get; }

    public override Fragment CreateFragment(int position) => _fragmentsArray[position];
}
