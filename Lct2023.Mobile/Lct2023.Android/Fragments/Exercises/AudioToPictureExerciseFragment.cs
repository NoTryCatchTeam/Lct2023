using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.Card;
using Google.Android.Material.ProgressIndicator;
using Lct2023.Android.Adapters;
using Lct2023.Android.Decorations;
using Lct2023.Android.Helpers;
using Lct2023.ViewModels.Tasks;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views.Fragments;

namespace Lct2023.Android.Fragments.Exercises;

[MvxFragmentPresentation]
public class AudioToPictureExerciseFragment : MvxFragment
{
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        base.OnCreateView(inflater, container, savedInstanceState);

        var view = this.BindingInflate(Resource.Layout.AudioToPictureExerciseFragment, container, false);

        var question = view.FindViewById<TextView>(Resource.Id.audio_to_picture_exercise_question);
        var playContainer = view.FindViewById<MaterialCardView>(Resource.Id.audio_to_picture_exercise_question_layout);
        var playProgress = view.FindViewById<LinearProgressIndicator>(Resource.Id.audio_to_picture_exercise_question_progress);
        var play = view.FindViewById<MaterialButton>(Resource.Id.audio_to_picture_exercise_question_button);
        var answers = view.FindViewById<MvxRecyclerView>(Resource.Id.audio_to_picture_exercise_answers_list);

        var answersAdapter = new AudioToPictureExerciseAnswersAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.audio_to_picture_exercise_answer_item),
        };

        answers.SetLayoutManager(new MvxGuardedGridLayoutManager(Activity, 2) { Orientation = LinearLayoutManager.Vertical });
        answers.SetAdapter(answersAdapter);
        answers.AddItemDecoration(new GridItemDecoration(2, DimensUtils.DpToPx(Activity, 16)));

        var set = this.CreateBindingSet<AudioToPictureExerciseFragment, AudioToPictureExerciseItem>();

        set.Bind(question)
            .For(x => x.Text)
            .To(vm => vm.Question);

        set.Bind(answersAdapter)
            .For(x => x.ItemsSource)
            .To(vm => vm.Answers);

        set.Bind(answersAdapter)
            .For(x => x.ItemClick)
            .To(vm => vm.AnswerTapCommand);

        set.Apply();

        return view;
    }
}
