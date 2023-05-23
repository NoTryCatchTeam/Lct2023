using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Button;
using Lct2023.Android.Adapters;
using Lct2023.Android.Decorations;
using Lct2023.Android.Helpers;
using Lct2023.Converters;
using Lct2023.ViewModels.Tasks;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views.Fragments;

namespace Lct2023.Android.Fragments.Exercises;

[MvxFragmentPresentation]
public class TextExerciseFragment : MvxFragment
{
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        base.OnCreateView(inflater, container, savedInstanceState);

        var view = this.BindingInflate(Resource.Layout.TextExerciseFragment, container, false);

        var question = view.FindViewById<TextView>(Resource.Id.text_exercise_question);
        var answers = view.FindViewById<MvxRecyclerView>(Resource.Id.text_exercise_answers_list);
        var commentHeader = view.FindViewById<TextView>(Resource.Id.text_exercise_answers_comment_header);
        var comment = view.FindViewById<TextView>(Resource.Id.text_exercise_answers_comment_description);

        var answersAdapter = new TextExerciseAnswersAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.text_exercise_answer_item),
        };

        answers.SetLayoutManager(new MvxGuardedLinearLayoutManager(Activity) { Orientation = LinearLayoutManager.Vertical });
        answers.SetAdapter(answersAdapter);
        answers.AddItemDecoration(new ItemSeparateDecoration(DimensUtils.DpToPx(Activity, 8), LinearLayoutManager.Vertical));

        var set = this.CreateBindingSet<TextExerciseFragment, TextExerciseItem>();

        set.Bind(question)
            .For(x => x.Text)
            .To(vm => vm.Question);

        set.Bind(answersAdapter)
            .For(x => x.ItemsSource)
            .To(vm => vm.Answers);

        set.Bind(answersAdapter)
            .For(x => x.ItemClick)
            .To(vm => vm.AnswerTapCommand);

        set.Bind(commentHeader)
            .For(x => x.BindVisible())
            .To(vm => vm.IsCorrect)
            .WithConversion(new AnyExpressionConverter<bool, bool>(x => !x));

        set.Bind(comment)
            .For(x => x.Text)
            .To(vm => vm.DescriptionOfCorrectness);

        set.Bind(comment)
            .For(x => x.BindVisible())
            .To(vm => vm.IsCorrect)
            .WithConversion(new AnyExpressionConverter<bool, bool>(x => !x));

        set.Apply();

        return view;
    }
}
