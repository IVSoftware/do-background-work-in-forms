using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

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
#elif true
                await RunClockWithReminders();
#else
#endif
                buttonStartWorkload.Enabled = true;
            };
        }
        async Task NotifyWhenComplete(Action work, bool notify)
        {
            await Task.Run(work);
            using (var notifier = new Notification())
            {
                notifier.ShowDialog(this, "Backgound work is complete.");
            }
        }
        async Task RunClockWithReminders()
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
                    //breakFromInner:
                    //await notification.ThreadsafeShowWithMessageAsync(
                    //    this,
                    //    $"Completed long-running task",
                    //    cancel: "Done");
                    //await Task.Delay(250); // Cosmetic
                });
            }
        }
    }
}
