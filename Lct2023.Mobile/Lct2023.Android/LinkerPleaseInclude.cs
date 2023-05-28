using System.Collections.Specialized;
using System.Windows.Input;
using Android.App;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;

namespace ArtecMobile.Android
{
    public class LinkerPleaseInclude
    {
        public void Include(Button button)
        {
            button.Click += (_, _) => button.Text = button.Text + "";
        }

        public void Include(CheckBox checkBox)
        {
            checkBox.CheckedChange += (_, _) => checkBox.Checked = !checkBox.Checked;
        }

        public void Include(Switch @switch)
        {
            @switch.CheckedChange += (_, _) => @switch.Checked = !@switch.Checked;
        }

        public void Include(View view)
        {
            view.Click += (_, _) => view.ContentDescription = view.ContentDescription + "";
        }

        public void Include(TextView text)
        {
            text.AfterTextChanged += (_, _) => text.Text = "" + text.Text;
            text.Hint = "" + text.Hint;
        }

        public void Include(CheckedTextView text)
        {
            text.AfterTextChanged += (_, _) => text.Text = "" + text.Text;
            text.Hint = "" + text.Hint;
        }

        public void Include(CompoundButton cb)
        {
            cb.CheckedChange += (_, _) => cb.Checked = !cb.Checked;
        }

        public void Include(SeekBar sb)
        {
            sb.ProgressChanged += (_, _) => sb.Progress = sb.Progress + 1;
        }

        public void Include(RadioGroup radioGroup)
        {
            radioGroup.CheckedChange += (_, args) => radioGroup.Check(args.CheckedId);
        }

        public void Include(RadioButton radioButton)
        {
            radioButton.CheckedChange += (_, args) => radioButton.Checked = args.IsChecked;
        }

        public void Include(RatingBar ratingBar)
        {
            ratingBar.RatingBarChange += (_, _) => ratingBar.Rating = 0 + ratingBar.Rating;
        }

        public void Include(Activity act)
        {
            act.Title = act.Title + "";
        }

        public void Include(INotifyCollectionChanged changed)
        {
            changed.CollectionChanged += (_, e) =>
            {
                var test = $"{e.Action}{e.NewItems}{e.NewStartingIndex}{e.OldItems}{e.OldStartingIndex}";
            };
        }

        public void Include(ICommand command)
        {
            command.CanExecuteChanged += (_, _) =>
            {
                if (command.CanExecute(null)) command.Execute(null);
            };
        }

        public void Include(MvvmCross.IoC.MvxPropertyInjector injector)
        {
            injector = new MvvmCross.IoC.MvxPropertyInjector();
        }

        public void Include(System.ComponentModel.INotifyPropertyChanged changed)
        {
            changed.PropertyChanged += (_, e) =>
            {
                var test = e.PropertyName;
            };
        }

        public void Include(MvxTaskBasedBindingContext context)
        {
            context.Dispose();
            var context2 = new MvxTaskBasedBindingContext();
            context2.Dispose();
        }

        public void Include(RecyclerView.ViewHolder vh, MvxRecyclerView list)
        {
            vh.ItemView.Click += (_, _) =>
            {
            };

            vh.ItemView.LongClick += (_, _) =>
            {
            };

            list.ItemsSource = null;
            list.Click += (_, _) =>
            {
            };
        }
    }
}
