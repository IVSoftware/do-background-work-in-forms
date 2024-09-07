namespace do_background_work_in_forms
{
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
}
