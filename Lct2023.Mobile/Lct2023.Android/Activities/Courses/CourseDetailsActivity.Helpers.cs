using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.Card;
using MvvmCross.DroidX.RecyclerView;

namespace Lct2023.Android.Activities.Courses;

public partial class CourseDetailsActivity
{
    private class Views
    {
        public Views(TeacherViews teacher, InfoViews info, ProgressViews progress, MvxRecyclerView sections, MaterialButton purchase)
        {
            Teacher = teacher;
            Info = info;
            Progress = progress;
            Sections = sections;
            Purchase = purchase;
        }

        public TeacherViews Teacher { get; }

        public InfoViews Info { get; }

        public ProgressViews Progress { get; }

        public MvxRecyclerView Sections { get; }

        public MaterialButton Purchase { get; }

        public class TeacherViews
        {
            public TeacherViews(ImageView image, TextView name, TextView description)
            {
                Image = image;
                Name = name;
                Description = description;
            }

            public ImageView Image { get; }

            public TextView Name { get; }

            public TextView Description { get; }
        }

        public class InfoViews
        {
            public InfoViews(MaterialCardView layout, ExtenderViews extender, DetailsViews details)
            {
                Layout = layout;
                Extender = extender;
                Details = details;
            }

            public MaterialCardView Layout { get; }

            public ExtenderViews Extender { get; }

            public DetailsViews Details { get; }

            public class ExtenderViews
            {
                public ExtenderViews(ConstraintLayout layout, ImageView chevron)
                {
                    Layout = layout;
                    Chevron = chevron;
                }

                public ConstraintLayout Layout { get; }

                public ImageView Chevron { get; }
            }

            public class DetailsViews
            {
                public DetailsViews(ConstraintLayout layout, MvxRecyclerView tags, MaterialButton showMap, TextView fullPrice, TextView creditPrice)
                {
                    Layout = layout;
                    Tags = tags;
                    ShowMap = showMap;
                    FullPrice = fullPrice;
                    CreditPrice = creditPrice;
                }

                public ConstraintLayout Layout { get; }

                public MvxRecyclerView Tags { get; }

                public MaterialButton ShowMap { get; }

                public TextView FullPrice { get; }

                public TextView CreditPrice { get; }
            }
        }

        public class ProgressViews
        {
            public ProgressViews(MaterialCardView layout, TextView value)
            {
                Layout = layout;
                Value = value;
            }

            public MaterialCardView Layout { get; }

            public TextView Value { get; }
        }
    }
}
