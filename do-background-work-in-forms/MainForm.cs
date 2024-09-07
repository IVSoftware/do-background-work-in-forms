using System.Diagnostics;

namespace do_background_work_in_forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
#if false
            buttonStartWorkload.Click += async (sender, e) =>
                // Simulate workload with delay
                await NotifyWhenComplete(
                    work: () => 
                    Task
                    .Delay(TimeSpan.FromSeconds(10))
                    .Wait(),
                    true
                 );
#elif false
            buttonStartWorkload.Click += async (sender, e) =>
                await RunClockWithReminders();
#else                
            buttonStartWorkload.Click += async (sender, e) =>
                await RunBackgroundWorkInStages();
#endif
        }
        private void UpdateMainForm(bool enableButton, string? label = null, string? title = "")
        {
            BeginInvoke(() =>
            {
                buttonStartWorkload.Enabled = enableButton;
                if (label is string) this.label.Text = label;
                if (title is string)
                {
                    // Specifically, on empty, copy label to Title
                    if (title == string.Empty) Text = this.label.Text;
                    else Text = title;
                }
            });
        }
        private async Task NotifyWhenComplete(Action work, bool notify)
        {
            try
            {
                UpdateMainForm(enableButton: false, label: $@"{DateTime.Now:hh\:mm\:ss}");
                await Task.Run(work);
                if (notify) using (var notifier = new Notification())
                {
                    notifier.ShowDialog(this, "Backgound work is complete.", cancel: "Close");
                }
            }
            finally
            {
                UpdateMainForm(enableButton: true, label: $@"{DateTime.Now:hh\:mm\:ss}");
            }
        }
        private async Task RunClockWithReminders()
        {
            try
            {
                UpdateMainForm(enableButton: false, label: $@"{DateTime.Now:hh\:mm\:ss}");
                using (var notification = new Notification())
                {
                    await Task.Run(async () =>
                    {
                        var stopwatch = Stopwatch.StartNew();
                        while (notification.DialogResult == DialogResult.None)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                if (notification.DialogResult == DialogResult.Cancel) return;
                                await Task.Delay(TimeSpan.FromSeconds(1));
                                if (notification.DialogResult == DialogResult.Cancel) return;
                                UpdateMainForm(enableButton: false, label: $@"{DateTime.Now:hh\:mm\:ss}");
                            }
                            // Discard/Ignore the return task in this case
                            _ = notification.ShowAsync(
                                this,
                                $"Performed {(int)stopwatch.Elapsed.TotalSeconds} seconds total work.",
                                ok: "Snooze",
                                cancel: "Dismiss");
                        }
                    });
                }
            }
            finally
            {
                UpdateMainForm(enableButton: true);
            }
        }
        enum Stage { Idle, Stage1, Stage2, Stage3 }
        private async Task RunBackgroundWorkInStages()
        {
            try
            {
                UpdateMainForm(enableButton: true, label: $@"{Stage.Idle}");
                using (var notification = new Notification())
                {
                    var stopwatchTotal = Stopwatch.StartNew();
                    foreach (Stage stage in Enum.GetValues<Stage>().Skip(1))
                    {
                        var stopwatchTask = Stopwatch.StartNew();
                        UpdateMainForm(enableButton: false, label: $@"{stage}", title: $"{DateTime.Now:hh\\:mm\\:ss} {stage}");
                        await Task.Delay(TimeSpan.FromSeconds(10));
                        UpdateMainForm(enableButton: false, label: $@"{stage}", title: $"{DateTime.Now:hh\\:mm\\:ss} {stage}");
                        stopwatchTask.Stop();
                        await notification.ShowAsync(
                            this,
                            $"Performed {stage} in {(int)stopwatchTask.Elapsed.TotalSeconds} seconds.{Environment.NewLine}" +
                            $"Total time is {(int)stopwatchTotal.Elapsed.TotalSeconds} seconds",
                            ok: "Continue",
                            cancel: "Cancel");
                        if (notification.DialogResult == DialogResult.Cancel) break;
                    }
                }
            }
            finally
            {
                UpdateMainForm(enableButton: true, label: $@"{Stage.Idle}");
            }
        }
    }
}
