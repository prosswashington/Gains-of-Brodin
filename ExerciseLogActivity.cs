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
using Android.Views.InputMethods;

namespace GainzTracker
{
    [Activity(Label = "@string/lToolbarName", Theme = "@style/AppTheme")]
    class ExerciseLogActivity : AppCompatActivity
    {
        string exercise;
        private List<ExerciseLog> mItems;
        private SupportToolbar mToolbar;
        TextView txtReps;
        TextView txtWeight;
        TextView txtDate;
        TextView txtNote;
        TextView txtExercise;
        Button button_save;
        string logList;
        DrawerLayout drawerLayout;
        
        ExpandableListViewAdapter mAdapter;
        ExpandableListView expandableLogListView;
        List<string> group = new List<string>();
        Dictionary<string, List<string>> dicMyMap = new Dictionary<string, List<string>>();

        static string historyText;
        static string currentText;
        List<string> groupCurrentLogs;
        List<string> groupHistory;

        public void addExercise(ExerciseLog exLog)
        {
            DataBase.insertExerciseLog(exLog);

            groupCurrentLogs.Add(exLog.ToString());

            expandableLogListView.CollapseGroup(0);
            expandableLogListView.ExpandGroup(0);
        }

        public void refreshListView()
        {
            expandableLogListView.CollapseGroup(0);
            expandableLogListView.CollapseGroup(1);

            groupHistory.Clear();
            groupCurrentLogs.Clear();

            this.fillLists();
            expandableLogListView.ExpandGroup(0);
        }

        public void fillLists()
        {
            mItems = new List<ExerciseLog>();
            mItems = DataBase.selectTableExerciseLog(exercise);
            foreach (ExerciseLog eL in mItems)
            {
                if (eL.date > Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yy")))
                {
                    groupCurrentLogs.Add(eL.ToString());
                }
                else
                {
                    groupHistory.Add(eL.ToString());
                }
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.exercise_log);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            exercise = JsonConvert.DeserializeObject<string>(Intent.GetStringExtra("exerciseLog"));

            txtReps = FindViewById<TextView>(Resource.Id.txtReps);
            txtWeight = FindViewById<TextView>(Resource.Id.txtWeight);
            txtDate = FindViewById<TextView>(Resource.Id.txtDate);
            txtDate.Click += DateSelect_OnClick;
            txtNote = FindViewById<TextView>(Resource.Id.txtNote);
            txtExercise = FindViewById<TextView>(Resource.Id.txtExercise);

            historyText = GetString(Resource.String.historyText);
            currentText = GetString(Resource.String.currentText);

            // Expandable Log Lists
            expandableLogListView = FindViewById<ExpandableListView>(Resource.Id.expandableLogListView);

            // Set Log History Data
            SetData(out mAdapter);
            expandableLogListView.SetAdapter(mAdapter);
            expandableLogListView.ExpandGroup(0);

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

            txtDate.Text = DateTime.Now.ToString("MM/dd/yy");
            txtExercise.Text = exercise;

            // Button save
            button_save = FindViewById<Button>(Resource.Id.btnSave);
            button_save.Enabled = false;

            txtReps.AfterTextChanged += EnableSaveButton;

            button_save.Click += delegate
            {
                if (!String.IsNullOrEmpty(txtWeight.Text) && !String.IsNullOrEmpty(txtReps.Text))
                {
                    ExerciseLog exerciseLog = new ExerciseLog();

                    int reps;
                    if (Int32.TryParse(txtReps.Text, out reps))
                        exerciseLog.repetitions = reps;

                    float weight;
                    if (float.TryParse(txtWeight.Text, out weight))
                        exerciseLog.weight = weight;

                    string logDate = txtDate.Text + " " + DateTime.Now.ToString("H:mm:ss");
                    exerciseLog.date = Convert.ToDateTime(logDate);

                    if (exerciseLog.date > DateTime.Now)
                    {
                        exerciseLog.date = DateTime.Now;
                    }

                    exerciseLog.exercise = exercise;
                    exerciseLog.note = txtNote.Text;

                    this.addExercise(exerciseLog);

                    txtReps.Text = "";
                    txtWeight.Text = "";
                    txtNote.Text = "";
                    button_save.Enabled = false;

                    //Close Soft Keyboard
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.HideSoftInputFromWindow(button_save.WindowToken, 0);
                }
            };
        }

        private void SetData(out ExpandableListViewAdapter mAdapter)
        {
            // Group for previous logs
            groupHistory = new List<string>();
            //Group for current logs
            groupCurrentLogs = new List<string>();

            this.fillLists();

            group.Add(currentText);
            group.Add(historyText);

            dicMyMap.Add(group[0], groupCurrentLogs);
            dicMyMap.Add(group[1], groupHistory);

            mAdapter = new ExpandableListViewAdapter(this, group, dicMyMap);  
        }

        protected override void OnResume()
        {
            SupportActionBar.SetTitle(Resource.String.lToolbarName);
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

        void DateSelect_OnClick(object sender, EventArgs eventArgs)
        {
            Fragments.DatePickerFragment frag = Fragments.DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    txtDate.Text = time.ToString("MM/dd/yy");
                });
                frag.Show(base.FragmentManager, Fragments.DatePickerFragment.TAG);
        }

        public void EnableSaveButton(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            button_save.Enabled = true;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.log_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_Edit:
                    var editFT = FragmentManager.BeginTransaction();
                    //Remove fragment since it is already added to backstack
                    var prevFT = FragmentManager.FindFragmentByTag("log_edit");

                    if (prevFT != null)
                    {
                        editFT.Remove(prevFT);
                    }

                    editFT.AddToBackStack(null);

                    mItems = DataBase.selectTableExerciseLog(exercise);

                    logList = JsonConvert.SerializeObject(mItems);
                    Bundle b = new Bundle();
                    b.PutString("LogEdit", logList);

                    // Create and show the dialog
                    LogEditFragment newEditFragment = LogEditFragment.NewInstance(b);

                    //Add fragment
                    newEditFragment.Show(editFT, "log_edit");

                    return true;
                case Resource.Id.menu_Delete:
                    var deleteFT = SupportFragmentManager.BeginTransaction();
                    //Remove fragment since it is already added to backstack
                    var prev_FT = SupportFragmentManager.FindFragmentByTag("log_delete");

                    if (prev_FT != null)
                    {
                        deleteFT.Remove(prev_FT);
                    }

                    deleteFT.AddToBackStack(null);

                    mItems = DataBase.selectTableExerciseLog(exercise);

                    logList = JsonConvert.SerializeObject(mItems);
                    Bundle delete = new Bundle();
                    delete.PutString("LogDelete", logList);

                    // Create and show the dialog
                    LogDeleteFragment newDeleteFragment = LogDeleteFragment.NewInstance(delete);

                    //Add fragment
                    newDeleteFragment.Show(deleteFT, "log_delete");

                    return true;
                case Resource.Id.menu_Delete_All:
                    if (DataBase.deleteExerciseLogAll(exercise))
                    {
                        Toast.MakeText(this, exercise + " logs deleted.", ToastLength.Short).Show();
                        expandableLogListView.CollapseGroup(0);
                        expandableLogListView.CollapseGroup(1);
                        groupHistory.Clear();
                        groupCurrentLogs.Clear();
                    }
                        
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}