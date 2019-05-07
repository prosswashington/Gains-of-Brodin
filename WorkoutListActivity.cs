using System.Collections.Generic;
using Android.Support.V7.App;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using GainzTracker.Resources.Model;
using GainzTracker.Resources.DataHelper;
using GainzTracker.Fragments;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using System.Threading.Tasks;

namespace GainzTracker
{
   [Activity(Label = "@string/wToolbarName", Theme= "@style/AppTheme")]
    public class WorkoutListActivity : AppCompatActivity
    {
        private List<Workout> mItems;
        private ListView mListView;
        private SupportToolbar mToolbar;
        ArrayAdapter<Workout> adapter;
        DrawerLayout drawerLayout;

        public void addWorkout(Workout workout)
        {   
            if (mListView != null)
            {
                adapter.Add(workout);
                mItems.Add(workout);

            }

            if (DataBase.insertWorkout(workout))
            {
                Toast.MakeText(this, $"{workout.name} - saved.", ToastLength.Short).Show();
            }
        }

        public void refreshAdapter()
        {
            if (mListView != null)
            {
                adapter.Clear();
                mItems = DataBase.selectTableWorkout();
                adapter.AddAll(mItems);
            }
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.workout_layout);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            mItems = new List<Workout>();
            mItems = JsonConvert.DeserializeObject<List<Workout>>(Intent.GetStringExtra("workoutList"));
            mItems.Sort();
           
            mListView = FindViewById<ListView>(Resource.Id.WorkoutListView);
           
            adapter = new ArrayAdapter<Workout>(this, Android.Resource.Layout.SimpleListItem1, mItems);
            mListView.Adapter = adapter;

            mListView.ItemClick += MListView_ItemClick;

            mToolbar = FindViewById<SupportToolbar>(Resource.Id.app_bar);
            SetSupportActionBar(mToolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

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
            SupportActionBar.SetTitle(Resource.String.wToolbarName);
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
                    Toast.MakeText(this, "Workouts selected!", ToastLength.Short).Show();
                    break;
                case (Resource.Id.nav_gainz):
                    Toast.MakeText(this, Resource.String.BoBstring4, ToastLength.Long).Show();
                    Intent intent = new Intent(Application.Context, typeof(GainzActivity));
                    StartActivity(intent);
                    
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

        private void MListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var t = adapter.GetItem(e.Position);
            string workout = (t.id).ToString();

            Intent intent = new Intent(Application.Context, typeof(ExerciseActivity));
            intent.PutExtra("workout", JsonConvert.SerializeObject(workout));
            StartActivity(intent);
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
                case Android.Resource.Id.Home:
                   
                    return true;
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
                    WorkoutNew newFragment = WorkoutNew.NewInstance(null);
                    

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

                    // Create and show the dialog
                    WorkoutEditFragment newEditFragment = WorkoutEditFragment.NewInstance(null);
                    

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

                    // Create and show the dialog
                    WorkoutDeleteFragment newDeleteFragment = WorkoutDeleteFragment.NewInstance(null);


                    //Add fragment
                    newDeleteFragment.Show(deleteFT, "dialog_delete");

                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}