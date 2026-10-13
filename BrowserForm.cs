using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Drawing;

namespace RedXBrowser;

public sealed class BrowserForm : Form
{
    private readonly WebView2 web = new();
    private readonly TextBox address = new();
    private readonly Label status = new();
    private readonly Button back = MakeButton("←", "Back");
    private readonly Button forward = MakeButton("→", "Forward");
    private readonly Button reload = MakeButton("⟳", "Reload");
    private readonly Button home = MakeButton("⌂", "Home");
    private readonly Button go = MakeButton("Go", "Go to address");
    private const string HomeUrl = "https://www.bing.com";

    public BrowserForm()
    {
        Text = "RedXBrowser";
        MinimumSize = new Size(800, 520);
        Size = new Size(1200, 800);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(24, 24, 28);
        ForeColor = Color.White;
        BuildLayout();
        Shown += async (_, _) => await InitializeBrowserAsync();
    }

    private void BuildLayout()
    {
        var top = new TableLayoutPanel
        {
            Dock = DockStyle.Top, Height = 58, Padding = new Padding(10, 9, 10, 9),
            ColumnCount = 6, RowCount = 1, BackColor = Color.FromArgb(30, 30, 36)
        };
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 42));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 42));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 42));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 42));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 58));

        foreach (var b in new[] { back, forward, reload, home, go })
        {
            b.Dock = DockStyle.Fill;
            b.Margin = new Padding(2);
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 82);
            b.BackColor = Color.FromArgb(42, 42, 50);
            b.ForeColor = Color.White;
            b.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        }

        address.Dock = DockStyle.Fill;
        address.Margin = new Padding(6, 4, 6, 4);
        address.Font = new Font("Segoe UI", 11);
        address.BorderStyle = BorderStyle.FixedSingle;
        address.BackColor = Color.FromArgb(245, 245, 248);
        address.ForeColor = Color.FromArgb(25, 25, 30);
        address.PlaceholderText = "Search the web or enter an address";

        top.Controls.Add(back, 0, 0);
        top.Controls.Add(forward, 1, 0);
        top.Controls.Add(reload, 2, 0);
        top.Controls.Add(home, 3, 0);
        top.Controls.Add(address, 4, 0);
        top.Controls.Add(go, 5, 0);

        status.Dock = DockStyle.Bottom;
        status.Height = 24;
        status.Padding = new Padding(10, 3, 0, 0);
        status.BackColor = Color.FromArgb(30, 30, 36);
        status.ForeColor = Color.Gainsboro;
        status.Text = "RedXBrowser • Ready";
        web.Dock = DockStyle.Fill;
        Controls.Add(web);
        Controls.Add(status);
        Controls.Add(top);

        back.Click += (_, _) => { if (web.CoreWebView2?.CanGoBack == true) web.CoreWebView2.GoBack(); };
        forward.Click += (_, _) => { if (web.CoreWebView2?.CanGoForward == true) web.CoreWebView2.GoForward(); };
        reload.Click += (_, _) => web.CoreWebView2?.Reload();
        home.Click += (_, _) => Navigate(HomeUrl);
        go.Click += (_, _) => NavigateFromAddress();
        address.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter) { NavigateFromAddress(); e.SuppressKeyPress = true; }
        };
    }

    private static Button MakeButton(string text, string accessibleName) =>
        new() { Text = text, AccessibleName = accessibleName, TabStop = true };

    private async Task InitializeBrowserAsync()
    {
        try
        {
            status.Text = "RedXBrowser • Starting browser engine…";
            await web.EnsureCoreWebView2Async();
            web.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
            web.CoreWebView2.Settings.IsStatusBarEnabled = true;
            web.CoreWebView2.Settings.AreDevToolsEnabled = true;
            web.CoreWebView2.NavigationStarting += (_, _) => status.Text = "RedXBrowser • Loading…";
            web.CoreWebView2.NavigationCompleted += (_, e) =>
            {
                address.Text = web.Source?.ToString() ?? "";
                status.Text = e.IsSuccess ? $"RedXBrowser • {web.CoreWebView2.DocumentTitle}" : $"RedXBrowser • Navigation failed ({e.WebErrorStatus})";
                back.Enabled = web.CoreWebView2.CanGoBack;
                forward.Enabled = web.CoreWebView2.CanGoForward;
            };
            web.CoreWebView2.DocumentTitleChanged += (_, _) =>
                Text = string.IsNullOrWhiteSpace(web.CoreWebView2.DocumentTitle) ? "RedXBrowser" : $"{web.CoreWebView2.DocumentTitle} — RedXBrowser";
            Navigate(HomeUrl);
        }
        catch (Exception ex)
        {
            MessageBox.Show("RedXBrowser couldn't start its browser engine.\n\nInstall the Microsoft Edge WebView2 Runtime, then try again.\n\nDetails: " + ex.Message,
                "RedXBrowser startup error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            status.Text = "RedXBrowser • Browser engine unavailable";
        }
    }

    private void NavigateFromAddress()
    {
        var input = address.Text.Trim();
        if (input.Length == 0) return;
        string target;
        if (Uri.TryCreate(input, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            target = uri.ToString();
        else if (input.Contains('.') && !input.Contains(' ') && Uri.TryCreate("https://" + input, UriKind.Absolute, out var domainUri))
            target = domainUri.ToString();
        else
            target = "https://www.bing.com/search?q=" + Uri.EscapeDataString(input);
        Navigate(target);
    }

    private void Navigate(string url)
    {
        if (web.CoreWebView2 is not null) web.CoreWebView2.Navigate(url);
        else web.Source = new Uri(url);
    }
}
