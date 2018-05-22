using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TimerApp
{
    public partial class MainWindow : Window
    {
        private Timer timer;
        private DateTime timerTime;

        private int currentLap;
        private ObservableCollection<Lap> laps;

        public MainWindow()
        {
            InitializeComponent();

            timer = new Timer(10);
            timer.Elapsed += new ElapsedEventHandler(TimerTick);
            timerTime = new DateTime(1, 1, 1, 0, 0, 0, 0);

            currentLap = 1;
            laps = new ObservableCollection<Lap>();
            laps.Reverse();

            TimeList.ItemsSource = laps;

            StopBtn.Visibility = Visibility.Hidden;
            LapBtn.Visibility = Visibility.Hidden;
        }

        // Start Click
        private async void StartBtnClick(object sender, RoutedEventArgs e)
        {
            StartBtn.Visibility = Visibility.Hidden;
            StopBtn.Visibility = Visibility.Visible;
            LapBtn.Visibility = Visibility.Visible;

            await Task.Run(() => { timer.Start(); });
        }

        public void TimerTick(object sender, ElapsedEventArgs args)
        {
            timerTime = timerTime.AddMilliseconds(18);

            Dispatcher.Invoke(() =>
            {
                TimerText.Text = timerTime.ToString("mm:ss:ff");
            });
        }

        // Lap Click
        private async void LapBtnClick(object sender, RoutedEventArgs e)
        {
            laps.Insert(0, await GetLap());
        }

        public Task<Lap> GetLap()
        {
            return Task.Run(() =>
            {
                return new Lap
                {
                    LapIndex = currentLap++,
                    Time = timerTime.ToString("mm:ss:ff")
                };
            });
        }

        // Stop Click
        private async void StopBtnClick(object sender, RoutedEventArgs e)
        {
            timerTime = new DateTime(1, 1, 1, 0, 0, 0, 0);
            TimerText.Text = "00:00:00";

            StartBtn.Visibility = Visibility.Visible;
            StopBtn.Visibility = Visibility.Hidden;
            LapBtn.Visibility = Visibility.Hidden;

            await Task.Run(() =>
            {
                timer.Stop();

                laps = new ObservableCollection<Lap>();
                currentLap = 1;
            });

            TimeList.ItemsSource = laps;
        }
    }
}
