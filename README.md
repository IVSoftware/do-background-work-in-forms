Your question is about showing a notification form on to _during_ backgound work (emphasis mine on _during_). That is, if you only want to do background work and pop up a modal or non-modal notification when it's done, just await the `Task` and show the notification when you get execution back. I'd like to attempt to answer the question as worded, and there's a subtle distinction to be made here. It seems to me there are at least two variations: a notification that allows the background work to keep running (Snooze/Dismiss Clock Runner example), and one that blocks the _background_ work, but not the UI (Continue/Cancel Work in Stages Example). So, one might consider implementing the notification `Form` in a manner that can optionally `await` when shown as a non-modal, and also stays on top of the parent form as per the spec. By checking the `DialogResult` after awaiting `ShowAsync()` the background task can either continue or cancel in response.

_The general advice being offered about marshaling onto the UI thread with `Invoke` or preferably `BeginInvoke` before interacting with the UI controls from the background thread still stands._

#### Custom notification form
```
public partial class Notification : Form
{
    public Notification()
    {
        InitializeComponent();
        ControlBox = false;
        StartPosition = FormStartPosition.CenterParent;
        buttonCancel.Click += (sender, e) =>
        {
            DialogResult = DialogResult.Cancel;
            localSafeSignalSemaphore();
        };
        buttonOK.Click += (sender, e) =>
        {
            localSafeSignalSemaphore();
        };
        void localSafeSignalSemaphore()
        {
            Owner?.BeginInvoke(() =>
            {
                Hide();
                Owner?.BringToFront();
                // Ensure that there's always "something to release".
                // We constrained maxCount on the semaphore to hold
                // ourselves accountable for managing this correctly.
                _busy.Wait(0);   // If count is 1, decrement it...
                _busy.Release(); // ... even if we're just going to increment it again.
            });
        }
    }

    SemaphoreSlim _busy = new SemaphoreSlim(1, maxCount: 1);

    /// <summary>
    /// Alternative to `Show` that is awaitable.
    /// </summary>
    public async Task ShowAsync(
            Control owner, 
            string message,
            string? ok = null,
            string? cancel = null
        )
    {
        // Block the awaiter unconditionally, but allow reentry.
        _busy.Wait(0);
        DialogResult = DialogResult.None;
        if(owner is null) throw new ArgumentNullException(nameof(owner));
        owner.BeginInvoke(() =>
        {
            Configure(message, ok, cancel);
            if (!Visible) base.Show(owner);
            CenterToParent();
        });
        await _busy.WaitAsync();
    }
    public DialogResult ShowDialog(
            Control owner,
            string message,
            string? ok = null,
            string? cancel = null)
    {
        Configure(message, ok, cancel);
        return base.ShowDialog();
    }
    private void Configure(
            string message,
            string? ok = null,
            string? cancel = null)
    {

        buttonOK.Visible = !string.IsNullOrWhiteSpace(ok);
        buttonOK.Text = ok ?? "OK";
        buttonCancel.Text = cancel ?? "Cancel";
        textBoxMessage.Text = message ?? string.Empty;
    }
    [Obsolete("Use ShowAsync with this class")]
    public new void Show() => throw new NotImplementedException();
    
    [Obsolete("Use ShowAsync with this class")]
    public new void Show(IWin32Window owner) => throw new NotImplementedException();

    [Obsolete("Use ShowAsync with this class")]
    public new void ShowDialog() => throw new NotImplementedException();

    [Obsolete("Use ShowAsync with this class")]
    public new void ShowDialog(IWin32Window owner) => throw new NotImplementedException();
}
```

___

##### Notify on, but do not suspend, background (Clock Runner Example)

Here the goal is to remind user every so often that time has elapsed, keep the clock running in the meantime, keep the main UI responsive, and if the user hasn't dismissed the notification when the next notification happens, update the visible notification rather than showing a new one.

[![clock runner][1]][1]

```
public MainForm()
{
    InitializeComponent();
    buttonStartWorkload.Click += async (sender, e) =>
        await RunClockWithReminders();
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
                        $"Performed {stopwatch.Elapsed.Seconds} seconds total work.",
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
private void UpdateMainForm(bool enableButton, string? label = null, bool copyToTitle = true)
{
    BeginInvoke(() =>
    {
        buttonStartWorkload.Enabled = enableButton;
        if (label is string)
        {
            this.label.Text = label;
            if (copyToTitle) Text = label;
        }
    });
}
```

___


##### Notify on background progress, but block background work pending Continue/Cancel (Work in Stages Example)

Here the goal is to inform user that a portion of the work has completed before continuing to do more background work. The total elapsed time of the background task can be >> the total of the task stages in this case. There may be a temptation to `ShowDialog()` to keep the background from doing any more work until user confirms it, but don't do that.

[![background work in stages][2]][2]

```
public MainForm()
{
    InitializeComponent();
    buttonStartWorkload.Click += async (sender, e) =>
        await RunBackgroundWorkInStages();
}
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
                    $"Performed {stage} in {stopwatchTask.Elapsed.Seconds} seconds.{Environment.NewLine}" +
                    $"Total time is {stopwatchTotal.Elapsed.Seconds} seconds",
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
```
___

##### ShowDialog Notification _after_ work is complete ('not' the question as worded)

```
enum Stage { Idle, Stage1, Stage2, Stage3 }
private async Task RunBackgroundWorkInStages()
{
    using (var notification = new Notification())
    {
        label.BeginInvoke(() => label.Text = $@"{Stage.Idle}");
        var stopwatchTotal = Stopwatch.StartNew();
        foreach (Stage stage in Enum.GetValues<Stage>().Skip(1))
        {
            var stopwatchTask = Stopwatch.StartNew();
            label.BeginInvoke(() => label.Text = $@"{stage}");
            await Task.Delay(TimeSpan.FromSeconds(10));
            stopwatchTask.Stop();
            await notification.ShowAsync(
                        this,
                        $"Performed {stage} in {stopwatchTask.Elapsed.Seconds} seconds.{Environment.NewLine}" +
                        $"Total time is {stopwatchTotal.Elapsed.Seconds} seconds",
                        ok: "Continue",
                        cancel: "Cancel");
            if (notification.DialogResult == DialogResult.Cancel) break;
        }
    }
    label.BeginInvoke(() => label.Text = $@"{Stage.Idle}");
}
```


  [1]: https://i.sstatic.net/lGUMIcu9.png
  [2]: https://i.sstatic.net/YF7s8Xsx.png