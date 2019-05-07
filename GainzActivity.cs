using System;
using System.Collections.Generic;
using System.Linq;
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
    [Activity(Label = "@string/gToolbarName", Theme = "@style/AppTheme")]
    class GainzActivity : AppCompatActivity
    {
        private SupportToolbar mToolbar;
        DrawerLayout drawerLayout;
        TextView txtHeight;
        TextView txtWeight;
        TextView txtBodyFat;
        TextView txtNeck;
        TextView txtChest;
        TextView txtWaist;
        TextView txtLeftBicep;
        TextView txtRightBicep;
        TextView txtLeftThigh;
        TextView txtRightThigh;
        TextView txtLeftCalf;
        TextView txtRightCalf;
        Button btnUpdateHeight;
        Button btnUpdateWeight;
        Button btnUpdateBodyFat;
        Button btnUpdateNeck;
        Button btnUpdateChest;
        Button btnUpdateWaist;
        Button btnUpdateLeftB;
        Button btnUpdateRightB;
        Button btnUpdateLeftT;
        Button btnUpdateRightT;
        Button btnUpdateLeftC;
        Button btnUpdateRightC;

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

        List<Gainz> heightList;
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
            SetContentView(Resource.Layout.gainz_layout);

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

            txtHeight = FindViewById<TextView>(Resource.Id.txtHeight);
            txtWeight = FindViewById<TextView>(Resource.Id.txtWeight);
            txtBodyFat = FindViewById<TextView>(Resource.Id.txtBodyFat);
            txtNeck = FindViewById<TextView>(Resource.Id.txtNeck);
            txtChest = FindViewById<TextView>(Resource.Id.txtChest);
            txtWaist = FindViewById<TextView>(Resource.Id.txtWaist);
            txtLeftBicep = FindViewById<TextView>(Resource.Id.txtLeftBicep);
            txtRightBicep = FindViewById<TextView>(Resource.Id.txtRightBicep);
            txtLeftThigh = FindViewById<TextView>(Resource.Id.txtLeftThigh);
            txtRightThigh = FindViewById<TextView>(Resource.Id.txtRightThigh);
            txtLeftCalf = FindViewById<TextView>(Resource.Id.txtLeftCalf);
            txtRightCalf = FindViewById<TextView>(Resource.Id.txtRightCalf);

            btnUpdateHeight = FindViewById<Button>(Resource.Id.btnUpdateHeight);
            btnUpdateWeight = FindViewById<Button>(Resource.Id.btnUpdateWeight);
            btnUpdateBodyFat = FindViewById<Button>(Resource.Id.btnUpdateBodyFat);
            btnUpdateNeck = FindViewById<Button>(Resource.Id.btnUpdateNeck);
            btnUpdateChest = FindViewById<Button>(Resource.Id.btnUpdateChest);
            btnUpdateWaist = FindViewById<Button>(Resource.Id.btnUpdateWaist);
            btnUpdateLeftB = FindViewById<Button>(Resource.Id.btnUpdateLeftB);
            btnUpdateRightB = FindViewById<Button>(Resource.Id.btnUpdateRightB);
            btnUpdateLeftT = FindViewById<Button>(Resource.Id.btnUpdateLeftT);
            btnUpdateRightT = FindViewById<Button>(Resource.Id.btnUpdateRightT);
            btnUpdateLeftC = FindViewById<Button>(Resource.Id.btnUpdateLeftC);
            btnUpdateRightC = FindViewById<Button>(Resource.Id.btnUpdateRightC);

            txtHeight.AfterTextChanged += EnableHeightButton;
            txtWeight.AfterTextChanged += EnableWeightButton;
            txtBodyFat.AfterTextChanged += EnableBodyFatButton;
            txtNeck.AfterTextChanged += EnableNeckButton;
            txtChest.AfterTextChanged += EnableChestButton;
            txtWaist.AfterTextChanged += EnableWaistButton;
            txtLeftBicep.AfterTextChanged += EnableLeftBButton;
            txtRightBicep.AfterTextChanged += EnableRightBButton;
            txtLeftThigh.AfterTextChanged += EnableLeftTButton;
            txtRightThigh.AfterTextChanged += EnableRightTButton;
            txtLeftCalf.AfterTextChanged += EnableLeftCButton;
            txtRightCalf.AfterTextChanged += EnableRightCButton;

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

            //Height Measurement
            heightList = DataBase.selectLastMeasurment(height);

            if (heightList.Count > 0) {
                Gainz hint = heightList.First<Gainz>();
                txtHeight.Hint = hint.measurement.ToString();
            }

            btnUpdateHeight.Click += delegate
            {
                if (!String.IsNullOrEmpty(txtHeight.Text))
                {
                    Gainz gainz = new Gainz();

                    float measurement;
                    if (float.TryParse(txtHeight.Text, out measurement))
                    {
                        gainz.measurement = measurement;
                    }
                    else
                    {
                        Toast.MakeText(this, "Problem with height measurement!", ToastLength.Short).Show();
                    }

                    gainz.item = height;
                    gainz.date = DateTime.Now;

                    DataBase.insertMeasurement(gainz);
                    heightList.Add(gainz);

                    txtHeight.Text = "";
                    txtHeight.Hint = measurement.ToString();
                    btnUpdateHeight.Enabled = false;
                }
            };

            // Weight Measurement
            weightList = DataBase.selectLastMeasurment(weight);

            if (weightList.Count > 0)
            {
                Gainz hint = weightList.First<Gainz>();
                txtWeight.Hint = hint.measurement.ToString();
            }

            btnUpdateWeight.Click += delegate
            {
                if (!String.IsNullOrEmpty(txtWeight.Text))
                {
                    Gainz gainz = new Gainz();

                    float measurement;
                    if (float.TryParse(txtWeight.Text, out measurement))
                    {
                        gainz.measurement = measurement;
                    }
                    else
                    {
                        Toast.MakeText(this, "Problem with weight measurement!", ToastLength.Short).Show();
                    }

                    gainz.item = weight;
                    gainz.date = DateTime.Now;

                    DataBase.insertMeasurement(gainz);
                    weightList.Add(gainz);

                    txtWeight.Text = "";
                    txtWeight.Hint = measurement.ToString();
                    btnUpdateWeight.Enabled = false;
                }
            };

            // Body Fat % Measurement
            bodyFatList = DataBase.selectLastMeasurment(bodyFat);

            if (bodyFatList.Count > 0)
            {
               Gainz hint = bodyFatList.First<Gainz>();
               txtBodyFat.Hint = hint.measurement.ToString();
            }

            btnUpdateBodyFat.Click += delegate
            {
                if (!String.IsNullOrEmpty(txtBodyFat.Text))
                {
                    Gainz gainz = new Gainz();

                    float measurement;
                    if (float.TryParse(txtBodyFat.Text, out measurement))
                    {
                        gainz.measurement = measurement;
                    }
                    else
                    {
                        Toast.MakeText(this, "Problem with body fat % measurement!", ToastLength.Short).Show();
                    }

                    gainz.item = bodyFat;
                    gainz.date = DateTime.Now;

                    DataBase.insertMeasurement(gainz);
                    bodyFatList.Add(gainz);

                    txtBodyFat.Text = "";
                    txtBodyFat.Hint = measurement.ToString();
                    btnUpdateBodyFat.Enabled = false;
                }
            };

            // Neck Measurement
            neckList = DataBase.selectLastMeasurment(neck);
        
            if (neckList.Count > 0)
            {
                Gainz hint = neckList.First<Gainz>();
                txtNeck.Hint = hint.measurement.ToString();
            }

            btnUpdateNeck.Click += delegate
            {
                if (!String.IsNullOrEmpty(txtNeck.Text))
                {
                    Gainz gainz = new Gainz();

                    float measurement;
                    if (float.TryParse(txtNeck.Text, out measurement))
                    {
                        gainz.measurement = measurement;
                    }
                    else
                    {
                        Toast.MakeText(this, "Problem with neck measurement!", ToastLength.Short).Show();
                    }

                    gainz.item = neck;
                    gainz.date = DateTime.Now;

                    DataBase.insertMeasurement(gainz);
                    neckList.Add(gainz);

                    txtNeck.Text = "";
                    txtNeck.Hint = measurement.ToString();
                    btnUpdateNeck.Enabled = false;
                }
            };

            // Chest Measurement
            chestList = DataBase.selectLastMeasurment(chest);

            if (chestList.Count > 0)
            {
                Gainz hint = chestList.First<Gainz>();
                txtChest.Hint = hint.measurement.ToString();
            }

            btnUpdateChest.Click += delegate
            {
                if (!String.IsNullOrEmpty(txtChest.Text))
                {
                    Gainz gainz = new Gainz();

                    float measurement;
                    if (float.TryParse(txtChest.Text, out measurement))
                    {
                        gainz.measurement = measurement;
                    }
                    else
                    {
                        Toast.MakeText(this, "Problem with chest measurement!", ToastLength.Short).Show();
                    }

                    gainz.item = chest;
                    gainz.date = DateTime.Now;

                    DataBase.insertMeasurement(gainz);
                    chestList.Add(gainz);

                    txtChest.Text = "";
                    txtChest.Hint = measurement.ToString();
                    btnUpdateChest.Enabled = false;
                }
            };

            // Waist Measurement
            waistList = DataBase.selectLastMeasurment(waist);

            if (waistList.Count > 0)
            {
                Gainz hint = waistList.First<Gainz>();
                txtWaist.Hint = hint.measurement.ToString();
            }

            btnUpdateWaist.Click += delegate
            {
                if (!String.IsNullOrEmpty(txtWaist.Text))
                {
                    Gainz gainz = new Gainz();

                    float measurement;
                    if (float.TryParse(txtWaist.Text, out measurement))
                    {
                        gainz.measurement = measurement;
                    }
                    else
                    {
                        Toast.MakeText(this, "Problem with waist measurement!", ToastLength.Short).Show();
                    }

                    gainz.item = waist;
                    gainz.date = DateTime.Now;

                    DataBase.insertMeasurement(gainz);
                    waistList.Add(gainz);

                    txtWaist.Text = "";
                    txtWaist.Hint = measurement.ToString();
                    btnUpdateWaist.Enabled = false;
                }
            };

            // Left Bicep Measurement
            leftBicepList = DataBase.selectLastMeasurment(leftBicep);

            if (leftBicepList.Count > 0)
            {
                Gainz hint = leftBicepList.First<Gainz>();
                txtLeftBicep.Hint = hint.measurement.ToString();
            }

            btnUpdateLeftB.Click += delegate
            {
                if (!String.IsNullOrEmpty(txtLeftBicep.Text))
                {
                    Gainz gainz = new Gainz();

                    float measurement;
                    if (float.TryParse(txtLeftBicep.Text, out measurement))
                    {
                        gainz.measurement = measurement;
                    }
                    else
                    {
                        Toast.MakeText(this, "Problem with left bicep measurement!", ToastLength.Short).Show();
                    }

                    gainz.item = leftBicep;
                    gainz.date = DateTime.Now;

                    DataBase.insertMeasurement(gainz);
                    leftBicepList.Add(gainz);

                    txtLeftBicep.Text = "";
                    txtLeftBicep.Hint = measurement.ToString();
                    btnUpdateLeftB.Enabled = false;
                }
            };

            // Right Bicep Measurement
            rightBicepList = DataBase.selectLastMeasurment(rightBicep);

            if (rightBicepList.Count > 0)
            {
                Gainz hint = rightBicepList.First<Gainz>();
                txtRightBicep.Hint = hint.measurement.ToString();
            }

            btnUpdateRightB.Click += delegate
            {
                if (!String.IsNullOrEmpty(txtRightBicep.Text))
                {
                    Gainz gainz = new Gainz();

                    float measurement;
                    if (float.TryParse(txtRightBicep.Text, out measurement))
                    {
                        gainz.measurement = measurement;
                    }
                    else
                    {
                        Toast.MakeText(this, "Problem with right bicep measurement!", ToastLength.Short).Show();
                    }

                    gainz.item = rightBicep;
                    gainz.date = DateTime.Now;

                    DataBase.insertMeasurement(gainz);
                    rightBicepList.Add(gainz);

                    txtRightBicep.Text = "";
                    txtRightBicep.Hint = measurement.ToString();
                    btnUpdateRightB.Enabled = false;
                }
            };

            // Left Thigh Measurement
            leftThighList = DataBase.selectLastMeasurment(leftThigh);

            if (leftThighList.Count > 0)
            {
                Gainz hint = leftThighList.First<Gainz>();
                txtLeftThigh.Hint = hint.measurement.ToString();
            }

            btnUpdateLeftT.Click += delegate
            {
                if (!String.IsNullOrEmpty(txtLeftThigh.Text))
                {
                    Gainz gainz = new Gainz();

                    float measurement;
                    if (float.TryParse(txtLeftThigh.Text, out measurement))
                    {
                        gainz.measurement = measurement;
                    }
                    else
                    {
                        Toast.MakeText(this, "Problem with left tricep measurement!", ToastLength.Short).Show();
                    }

                    gainz.item = leftThigh;
                    gainz.date = DateTime.Now;

                    DataBase.insertMeasurement(gainz);
                    leftThighList.Add(gainz);

                    txtLeftThigh.Text = "";
                    txtLeftThigh.Hint = measurement.ToString();
                    btnUpdateLeftT.Enabled = false;
                }
            };

            // Right Thigh Measurement
            rightThighList = DataBase.selectLastMeasurment(rightThigh);

            if (rightThighList.Count > 0)
            {
                Gainz hint = rightThighList.First<Gainz>();
                txtRightThigh.Hint = hint.measurement.ToString();
            }

            btnUpdateRightT.Click += delegate
            {
                if (!String.IsNullOrEmpty(txtRightThigh.Text))
                {
                    Gainz gainz = new Gainz();

                    float measurement;
                    if (float.TryParse(txtRightThigh.Text, out measurement))
                    {
                        gainz.measurement = measurement;
                    }
                    else
                    {
                        Toast.MakeText(this, "Problem with right tricep measurement!", ToastLength.Short).Show();
                    }

                    gainz.item = rightThigh;
                    gainz.date = DateTime.Now;

                    DataBase.insertMeasurement(gainz);
                    rightThighList.Add(gainz);

                    txtRightThigh.Text = "";
                    txtRightThigh.Hint = measurement.ToString();
                    btnUpdateRightT.Enabled = false;
                }
            };

            // Left Calf Measurement
            leftCalfList = DataBase.selectLastMeasurment(leftCalf);

            if (leftCalfList.Count > 0)
            {
                Gainz hint = leftCalfList.First<Gainz>();
                txtLeftCalf.Hint = hint.measurement.ToString();
            }

            btnUpdateLeftC.Click += delegate
            {
                if (!String.IsNullOrEmpty(txtLeftCalf.Text))
                {
                    Gainz gainz = new Gainz();

                    float measurement;
                    if (float.TryParse(txtLeftCalf.Text, out measurement))
                    {
                        gainz.measurement = measurement;
                    }
                    else
                    {
                        Toast.MakeText(this, "Problem with left calf measurement!", ToastLength.Short).Show();
                    }

                    gainz.item = leftCalf;
                    gainz.date = DateTime.Now;

                    DataBase.insertMeasurement(gainz);
                    leftCalfList.Add(gainz);

                    txtLeftCalf.Text = "";
                    txtLeftCalf.Hint = measurement.ToString();
                    btnUpdateLeftC.Enabled = false;
                }
            };

            // Right Calf Measurement
            rightCalfList = DataBase.selectLastMeasurment(rightCalf);

           if (rightCalfList.Count > 0)
            {
                Gainz hint = rightCalfList.First<Gainz>();
                txtRightCalf.Hint = hint.measurement.ToString();
            }

            btnUpdateRightC.Click += delegate
            {
                if (!String.IsNullOrEmpty(txtRightCalf.Text))
                {
                    Gainz gainz = new Gainz();

                    float measurement;
                    if (float.TryParse(txtRightCalf.Text, out measurement))
                    {
                        gainz.measurement = measurement;
                    }
                    else
                    {
                        Toast.MakeText(this, "Problem with right calf measurement!", ToastLength.Short).Show();
                    }

                    gainz.item = rightCalf;
                    gainz.date = DateTime.Now;

                    DataBase.insertMeasurement(gainz);
                    rightCalfList.Add(gainz);

                    txtRightCalf.Text = "";
                    txtRightCalf.Hint = measurement.ToString();
                    btnUpdateRightC.Enabled = false;
                }
            };
        }

       
        protected override void OnResume()
        {
            SupportActionBar.SetTitle(Resource.String.gToolbarName);
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.gainz_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_History:
                    Intent intent = new Intent(Application.Context, typeof(GainzHistoryActivity));

                    intent.PutExtra(height, JsonConvert.SerializeObject(heightList));
                    intent.PutExtra(weight, JsonConvert.SerializeObject(weightList));
                    intent.PutExtra(bodyFat, JsonConvert.SerializeObject(bodyFatList));
                    intent.PutExtra(neck, JsonConvert.SerializeObject(neckList));
                    intent.PutExtra(chest, JsonConvert.SerializeObject(chestList));
                    intent.PutExtra(waist, JsonConvert.SerializeObject(waistList));
                    intent.PutExtra(leftBicep, JsonConvert.SerializeObject(leftBicepList));
                    intent.PutExtra(rightBicep, JsonConvert.SerializeObject(rightBicepList));
                    intent.PutExtra(leftThigh, JsonConvert.SerializeObject(leftThighList));
                    intent.PutExtra(rightThigh, JsonConvert.SerializeObject(rightThighList));
                    intent.PutExtra(leftCalf, JsonConvert.SerializeObject(leftCalfList));
                    intent.PutExtra(rightCalf, JsonConvert.SerializeObject(rightCalfList));

                    StartActivity(intent);
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public void EnableHeightButton(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            btnUpdateHeight.Enabled = true;
        }

        public void EnableWeightButton(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            btnUpdateWeight.Enabled = true;
        }
        public void EnableBodyFatButton(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            btnUpdateBodyFat.Enabled = true;
        }
        public void EnableNeckButton(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            btnUpdateNeck.Enabled = true;
        }
        public void EnableChestButton(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            btnUpdateChest.Enabled = true;
        }
        public void EnableWaistButton(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            btnUpdateWaist.Enabled = true;
        }
        public void EnableLeftBButton(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            btnUpdateLeftB.Enabled = true;
        }
        public void EnableRightBButton(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            btnUpdateRightB.Enabled = true;
        }
        public void EnableLeftTButton(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            btnUpdateLeftT.Enabled = true;
        }
        public void EnableRightTButton(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            btnUpdateRightT.Enabled = true;
        }
        public void EnableLeftCButton(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            btnUpdateLeftC.Enabled = true;
        }
        public void EnableRightCButton(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            btnUpdateRightC.Enabled = true;
        }
    }
}