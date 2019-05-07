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
    [Activity(Label = "@string/ghToolbarName", Theme = "@style/AppTheme")]
    class GainzHistoryActivity : AppCompatActivity
    {
        private SupportToolbar mToolbar;
        DrawerLayout drawerLayout;
        ExpandableListViewAdapter mAdapter;
        ExpandableListView expandableListView;
        List<string> group = new List<string>();
        Dictionary<string, List<string>> dicMyMap = new Dictionary<string, List<string>>();

        static string height;
        static string weight;
        static string bodyFat;
        static string neck;
        static string chest;
        static string waist;
        static string leftBicep;
        static string rightBicep;
        static string leftThigh;
        static string rightThigh;
        static string leftCalf;
        static string rightCalf;

        List <Gainz> heightList;
        List<Gainz> weightList;
        List<Gainz> bodyFatList;
        List<Gainz> neckList;
        List<Gainz> chestList;
        List<Gainz> waistList;
        List<Gainz> leftBicepList;
        List<Gainz> rightBicepList;
        List<Gainz> leftThighList;
        List<Gainz> rightThighList;
        List<Gainz> leftCalfList;
        List<Gainz> rightCalfList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.history_main_layout);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            height = GetString(Resource.String.height);
            weight = GetString(Resource.String.weight);
            bodyFat = GetString(Resource.String.bodyFat);
            neck = GetString(Resource.String.neck);
            chest = GetString(Resource.String.chest);
            waist = GetString(Resource.String.waist);
            leftBicep = GetString(Resource.String.leftBicep);
            rightBicep = GetString(Resource.String.rightBicep);
            leftThigh = GetString(Resource.String.leftThigh);
            rightThigh = GetString(Resource.String.rightThigh);
            leftCalf = GetString(Resource.String.leftCalf);
            rightCalf = GetString(Resource.String.rightCalf);

            expandableListView = FindViewById<ExpandableListView>(Resource.Id.expandableListView);

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

            heightList = JsonConvert.DeserializeObject<List<Gainz>>(Intent.GetStringExtra(height));
            weightList = JsonConvert.DeserializeObject<List<Gainz>>(Intent.GetStringExtra(weight));
            bodyFatList = JsonConvert.DeserializeObject<List<Gainz>>(Intent.GetStringExtra(bodyFat));
            neckList = JsonConvert.DeserializeObject<List<Gainz>>(Intent.GetStringExtra(neck));
            chestList = JsonConvert.DeserializeObject<List<Gainz>>(Intent.GetStringExtra(chest));
            waistList = JsonConvert.DeserializeObject<List<Gainz>>(Intent.GetStringExtra(waist));
            leftBicepList = JsonConvert.DeserializeObject<List<Gainz>>(Intent.GetStringExtra(leftBicep));
            rightBicepList = JsonConvert.DeserializeObject<List<Gainz>>(Intent.GetStringExtra(rightBicep));
            leftThighList = JsonConvert.DeserializeObject<List<Gainz>>(Intent.GetStringExtra(leftThigh));
            rightThighList = JsonConvert.DeserializeObject<List<Gainz>>(Intent.GetStringExtra(rightThigh));
            leftCalfList = JsonConvert.DeserializeObject<List<Gainz>>(Intent.GetStringExtra(leftCalf));
            rightCalfList = JsonConvert.DeserializeObject<List<Gainz>>(Intent.GetStringExtra(rightCalf));

            // Set Data
            SetData(out mAdapter);
            expandableListView.SetAdapter(mAdapter);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.gainz_history_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_history_clear:
                    if (DataBase.deleteHistory())
                    {
                        heightList.Clear();
                        weightList.Clear();
                        bodyFatList.Clear();
                        neckList.Clear();
                        chestList.Clear();
                        waistList.Clear();
                        leftBicepList.Clear();
                        rightBicepList.Clear();
                        leftThighList.Clear();
                        rightThighList.Clear();
                        leftCalfList.Clear();
                        rightCalfList.Clear();

                        dicMyMap.Clear();

                        this.Finish();
                        Intent intentGainz = new Intent(Application.Context, typeof(GainzActivity));
                        StartActivity(intentGainz);

                        Toast.MakeText(this, "History cleared!", ToastLength.Short).Show();
                    }
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }

        }

        private void SetData(out ExpandableListViewAdapter mAdapter)
        {
            List<string> groupHeight = new List<string>();

            foreach (Gainz g in heightList)
            {
                groupHeight.Add(g.ToString());
            }

            List<string> groupWeight = new List<string>();

            foreach (Gainz g in weightList)
            {
                groupWeight.Add(g.ToString());
            }

            List<string> groupBodyFat = new List<string>();

            foreach (Gainz g in bodyFatList)
            {
                groupBodyFat.Add(g.ToString());
            }

            List<string> groupNeck = new List<string>();

            foreach (Gainz g in neckList)
            {
                groupNeck.Add(g.ToString());
            }

            List<string> groupChest = new List<string>();

            foreach (Gainz g in chestList)
            {
                groupChest.Add(g.ToString());
            }

            List<string> groupWaist = new List<string>();

            foreach (Gainz g in waistList)
            {
                groupWaist.Add(g.ToString());
            }

            List<string> groupLeftBicep = new List<string>();

            foreach (Gainz g in leftBicepList)
            {
                groupLeftBicep.Add(g.ToString());
            }

            List<string> groupRightBicep = new List<string>();

            foreach (Gainz g in rightBicepList)
            {
                groupRightBicep.Add(g.ToString());
            }

            List<string> groupLeftThigh = new List<string>();

            foreach (Gainz g in leftThighList)
            {
                groupLeftThigh.Add(g.ToString());
            }

            List<string> groupRightThigh = new List<string>();

            foreach (Gainz g in rightThighList)
            {
                groupRightThigh.Add(g.ToString());
            }

            List<string> groupLeftCalf = new List<string>();

            foreach (Gainz g in leftCalfList)
            {
                groupLeftCalf.Add(g.ToString());
            }

            List<string> groupRightCalf = new List<string>();

            foreach (Gainz g in rightCalfList)
            {
                groupRightCalf.Add(g.ToString());
            }

            group.Add(height);
            group.Add(weight);
            group.Add(bodyFat);
            group.Add(neck);
            group.Add(chest);
            group.Add(waist);
            group.Add(leftBicep);
            group.Add(rightBicep);
            group.Add(leftThigh);
            group.Add(rightThigh);
            group.Add(leftCalf);
            group.Add(rightCalf);

            dicMyMap.Add(group[0], groupHeight);
            dicMyMap.Add(group[1], groupWeight);
            dicMyMap.Add(group[2], groupBodyFat);
            dicMyMap.Add(group[3], groupNeck);
            dicMyMap.Add(group[4], groupChest);
            dicMyMap.Add(group[5], groupWaist);
            dicMyMap.Add(group[6], groupLeftBicep);
            dicMyMap.Add(group[7], groupRightBicep);
            dicMyMap.Add(group[8], groupLeftThigh);
            dicMyMap.Add(group[9], groupRightThigh);
            dicMyMap.Add(group[10], groupLeftCalf);
            dicMyMap.Add(group[11], groupRightCalf);

            mAdapter = new ExpandableListViewAdapter(this, group, dicMyMap);
        }

        protected override void OnResume()
        {
            SupportActionBar.SetTitle(Resource.String.ghToolbarName);
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
                    this.Finish();
                    Intent intentGainz = new Intent(Application.Context, typeof(GainzActivity));

                    StartActivity(intentGainz);
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

    }
}