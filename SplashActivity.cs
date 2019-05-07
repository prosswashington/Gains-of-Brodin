using Android.App;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using Android.Support.V7.App;
using Android.Content;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using GainzTracker.Resources.Model;
using GainzTracker.Resources.DataHelper;

namespace GainzTracker
{
    [Activity(Theme = "@style/AppTheme.Splash", MainLauncher = true, Icon = "@drawable/icon", NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Random random = new Random();
            
            String[] welcomeText = new string[]
            {
                GetString(Resource.String.SAstring1),
                GetString(Resource.String.SAstring2),
                GetString(Resource.String.SAstring3),
                GetString(Resource.String.SAstring4),
                GetString(Resource.String.SAstring5),
                GetString(Resource.String.SAstring6),
                GetString(Resource.String.BoBstring1),
                GetString(Resource.String.BoBstring2),
                GetString(Resource.String.BoBstring3),
                GetString(Resource.String.BoBstring5),
                GetString(Resource.String.BoBstring6),
                GetString(Resource.String.BoBstring7),
                GetString(Resource.String.BoBstring8),
                GetString(Resource.String.BoBstring9),
                GetString(Resource.String.BoBstring10),
                GetString(Resource.String.BoBstring11)
            };

            int randomNum = random.Next(0, welcomeText.Length);

            Toast.MakeText(this, welcomeText[randomNum], ToastLength.Long).Show();
        }

        protected override void OnResume()
        {
            base.OnResume();

            Intent intent = new Intent(Application.Context, typeof(WorkoutListActivity)); 

            Task startupWork = new Task(() =>
                                         {
                                             string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                                             List<Workout> workoutList = new List<Workout>(); 

                                             if (File.Exists(Path.Combine(path, "GainzTracker.db")))
                                             {
                                                 //Get list of workouts
                                                 workoutList = DataBase.selectTableWorkout();
                                             }
                                             else
                                             {
                                                 DataBase.createDataBase();

                                                 //Create welcome messages
                                                 DataBase.insertWorkout(GetString(Resource.String.welcomeWorkout), 1);

                                                 Exercise e = new Exercise();
                                                 e.inWorkout = 1;
                                                 e.name = GetString(Resource.String.welcomeExercise);
                                                 DataBase.insertExercise(e);

                                                 workoutList = DataBase.selectTableWorkout();
                                             }
                                            
                                             intent.PutExtra("workoutList", JsonConvert.SerializeObject(workoutList));
                                             
                                         });
            startupWork.ContinueWith(t =>
                                    {
                                        intent.SetFlags(ActivityFlags.SingleTop);
                                       StartActivity(intent); 
                                    }, TaskScheduler.FromCurrentSynchronizationContext());

            startupWork.Start();
        }
    }
}

