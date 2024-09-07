Your question is about showing a notification form on to _during_ backgound work, emphasis mine on _during_. That is, if you want to do background work and pop up a modal or non-modal notification when you're done, just await the Task and pop it up when you get execution back.

##### Notification _after_ work is complete.

```
public MainForm()
{
    InitializeComponent();
    buttonStartWorkload.Click += async (sender, e) =>
    {
        buttonStartWorkload.Enabled = false;
        await DoWorkWithOptionalNotify(
            work: () => Task.Delay(TimeSpan.FromSeconds(5)).Wait(),
            true
            );
        buttonStartWorkload.Enabled = true;
    };
}
async Task DoWorkWithOptionalNotify(Action work, bool notify)
{
    await Task.Run(work);
    using (var notifier = new Notification())
    {
        notifier.ShowDialog(this);
    }
}
```

##### Notification _during_ work in progress 

There are at least two variations of this.

###### Notify on, but do not suspend, background (Clock Runner Example)

Here the goal is to remind user every so often that time has elapsed, keep the clock running in the meantime, keep the main UI responsive, and if the user hasn't dismissed the notification when the next notification happens, update the visible notification rather than showing a new one.





