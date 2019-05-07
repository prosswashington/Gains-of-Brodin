using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using GainzTracker.Resources.Model;
using GainzTracker.Resources.DataHelper;
using GainzTracker.Fragments;
using Android.Support.V7.App;
using Newtonsoft.Json;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using System.Threading.Tasks;

namespace GainzTracker
{
   [Activity(Label = "@string/eToolbarName", Theme = "@style/AppTheme")]
    class ExerciseActivity : AppCompatActivity
    {
        int workout;
        private List<Exercise> mItems;
        private List<Exercise> mChecked;
        private ListView mListView;
        private SupportToolbar mToolbar;
        ArrayAdapter<Exercise> adapter;
        string exerciseList;
        DrawerLayout drawerLayout;

        public void addExercise(Exercise exercise)
        {
            exercise.inWorkout = workout;
            if (mListView != null)
            {
                adapter.Add(exercise);
                mItems.Add(exercise);
            }

            if(DataBase.insertExercise(exercise))
            {
                Toast.MakeText(this, $"{exercise.name} - saved.", ToastLength.Short).Show();
            }
        }

        public void refreshAdapter()
        {
            if (mListView != null)
            {
                adapter.Clear();
                mItems = DataBase.selectWorkoutExercise(workout);
                adapter.AddAll(mItems);
            }
        }

        public void finishActivity()
        {
            this.Finish();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.exercise_layout);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            bool parsed = Int32.TryParse(JsonConvert.DeserializeObject<string>(Intent.GetStringExtra("workout")), out workout);
            if (!parsed)
                Console.WriteLine("Problem with workout ID");

            mItems = new List<Exercise>();
            mChecked = new List<Exercise>();

            mItems = DataBase.selectWorkoutExercise(workout);
            mItems.Sort();
            
            mListView = FindViewById<ListView>(Resource.Id.ExerciseListView);

            adapter = new ArrayAdapter<Exercise>(this, Android.Resource.Layout.SimpleListItemChecked, mItems);
            mListView.Adapter = adapter;
            mListView.ChoiceMode = ChoiceMode.Multiple;

            mListView.ItemClick += MListView_ItemClick;

            mToolbar = FindViewById<SupportToolbar>(Resource.Id.app_bar);
            SetSupportActionBar(mToolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            mToolbar.Title = GetString(Resource.String.eToolbarName);

            // Attach item selected handler to navigation view
            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            // Create ActionBarDrawerToggle button and add it to the toolbar
            var drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, mToolbar, Resource.String.open_drawer, Resource.String.close_drawer);
            drawerLayout.AddDrawerListener(drawerToggle);
            drawerToggle.SyncState();
        }

        protected override void OnResume()
        {
            SupportActionBar.SetTitle(Resource.String.eToolbarName);
            base.OnResume();
        }

        async void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            // Close drawer
            drawerLayout.CloseDrawers();
            await Task.Delay(400);

            switch (e.MenuItem.ItemId)
            {
                case (Resource.Id.nav_workouts):
                    this.Finish();

                    Intent intent = new Intent(Application.Context, typeof(WorkoutListActivity));

                    List<Workout> workoutList = new List<Workout>();
                    workoutList = DataBase.selectTableWorkout();

                    intent.PutExtra("workoutList", JsonConvert.SerializeObject(workoutList));

                    StartActivity(intent);
                    break;
                case (Resource.Id.nav_gainz):
                    Toast.MakeText(this, Resource.String.BoBstring4, ToastLength.Long).Show();
                    Intent gIntent = new Intent(Application.Context, typeof(GainzActivity));
                    StartActivity(gIntent);
                    break;
                case (Resource.Id.nav_about):
                    var ft = SupportFragmentManager.BeginTransaction();
                    //Remove fragment since it is already added to backstack
                    var prev = SupportFragmentManager.FindFragmentByTag("about");

                    if (prev != null)
                    {
                        ft.Remove(prev);
                    }
                    ft.AddToBackStack(null);
                    // Create and show the dialog
                    AboutFragment newFragment = AboutFragment.NewInstance(null);
                    //Add fragment
                    newFragment.Show(ft, "about");
                    break;
            }
            
        }
        protected override void OnRestart()
        {
            base.OnRestart();

            mItems.Clear();
            mItems = DataBase.selectWorkoutExercise(workout);
            adapter.Clear();
            adapter = new ArrayAdapter<Exercise>(this, Android.Resource.Layout.SimpleListItemChecked, mItems);
            mListView.Adapter = adapter;

            for (int i = 0; i < mItems.Count; i++)
            {
                if (mChecked.Contains(mItems[i]))
                {
                        mListView.SetItemChecked(i, true);
                }
            }
        }

        private void MListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {

            var t = adapter.GetItem(e.Position);

            Intent intent = new Intent(Application.Context, typeof(ExerciseLogActivity));
            intent.PutExtra("exerciseLog", JsonConvert.SerializeObject(t.ToString()));
            StartActivity(intent);

            mListView.SetItemChecked(e.Position, true);
  
            mChecked.Add(t);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.workout_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                // New Option Button Clicked
                case Resource.Id.menu_New:

                    var ft = SupportFragmentManager.BeginTransaction();
                    //Remove fragment since it is already added to backstack
                    var prev = SupportFragmentManager.FindFragmentByTag("dialog");

                    if (prev != null)
                    {
                        ft.Remove(prev);
                    }

                    ft.AddToBackStack(null);

                    // Create and show the dialog
                    ExerciseNewFragment newFragment = ExerciseNewFragment.NewInstance(null);

                    //Add fragment
                    newFragment.Show(ft, "dialog");

                    return true;

                case Resource.Id.menu_Edit:
                    var editFT = SupportFragmentManager.BeginTransaction();
                    //Remove fragment since it is already added to backstack
                    var prevFT = SupportFragmentManager.FindFragmentByTag("dialog_edit");

                    if (prevFT != null)
                    {
                        editFT.Remove(prevFT);
                    }

                    editFT.AddToBackStack(null);
                    
                    exerciseList = JsonConvert.SerializeObject(mItems);
                    Bundle b = new Bundle();
                    b.PutString("exerciseActivity", exerciseList);

                    // Create and show the dialog
                    ExerciseEditFragment newEditFragment = ExerciseEditFragment.NewInstance(b);

                    //Add fragment
                    newEditFragment.Show(editFT, "dialog_edit");

                    return true;
                case Resource.Id.menu_Delete:
                    var deleteFT = SupportFragmentManager.BeginTransaction();
                    //Remove fragment since it is already added to backstack
                    var prev_FT = SupportFragmentManager.FindFragmentByTag("dialog_delete");

                    if (prev_FT != null)
                    {
                        deleteFT.Remove(prev_FT);
                    }

                    deleteFT.AddToBackStack(null);

                    exerciseList = JsonConvert.SerializeObject(mItems);
                    Bundle delete = new Bundle();
                    delete.PutString("exerciseActivityDelete", exerciseList);

                    // Create and show the dialog
                    ExerciseDeleteFragment newDeleteFragment = ExerciseDeleteFragment.NewInstance(delete);

                    //Add fragment
                    newDeleteFragment.Show(deleteFT, "dialog_delete");

                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}