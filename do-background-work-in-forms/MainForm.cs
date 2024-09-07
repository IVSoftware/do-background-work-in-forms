using System.Diagnostics;

namespace do_background_work_in_forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            buttonStartWorkload.Click += async (sender, e) =>
            {
                buttonStartWorkload.Enabled = false;
#if false
                await NotifyWhenComplete(
                    work: () => 
                    Task
                    .Delay(TimeSpan.FromSeconds(5))
                    .Wait(),
                    true
                 );
#elif false
                await RunClockWithReminders();
#else

                await RunBackgroundWorkInStages();
#endif
                buttonStartWorkload.Enabled = true;
            };
        }

        private async Task NotifyWhenComplete(Action work, bool notify)
        {
            await Task.Run(work);
            using (var notifier = new Notification())
            {
                notifier.ShowDialog(this, "Backgound work is complete.");
            }
        }
        private async Task RunClockWithReminders()
        {
            using (var notification = new Notification())
            {
                await Task.Run(async () =>
                {
                    var stopwatch = Stopwatch.StartNew();
                    while (notification.DialogResult == DialogResult.None)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (notification.DialogResult == DialogResult.Cancel) return;
                            await Task.Delay(1000);
                            if (notification.DialogResult == DialogResult.Cancel) return;

                            labelClock.BeginInvoke(() => labelClock.Text = $@"{DateTime.Now:hh\:mm\:ss}");
                        }
                        // Discard/Ignore the return task in this case
                        _ = notification.ThreadsafeShowWithMessageAsync(
                            this,
                            $"Performed {stopwatch.Elapsed.Seconds} seconds total work.",
                            ok: "Snooze",
                            cancel: "Dismiss");
                    }
                });
            }
        }
        enum Stage { Idle, Stage1, Stage2, Stage3 }
        private async Task RunBackgroundWorkInStages()
        {
            using (var notification = new Notification())
            {
                labelClock.BeginInvoke(() => labelClock.Text = $@"{Stage.Idle}");
                var stopwatchTotal = Stopwatch.StartNew();
                foreach (Stage stage in Enum.GetValues<Stage>().Skip(1))
                {
                    var stopwatchTask = Stopwatch.StartNew();
                    labelClock.BeginInvoke(() => labelClock.Text = $@"{stage}");
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    stopwatchTask.Stop();
                    await notification.ThreadsafeShowWithMessageAsync(
                                this,
                                $"Performed {stage} in {stopwatchTask.Elapsed.Seconds} seconds.{Environment.NewLine}" +
                                $"Total time is {stopwatchTotal.Elapsed.Seconds} seconds",
                                ok: "Continue",
                                cancel: "Cancel");
                    if (notification.DialogResult == DialogResult.Cancel) break;
                }
            }
            labelClock.BeginInvoke(() => labelClock.Text = $@"{Stage.Idle}");
        }
    }
}
