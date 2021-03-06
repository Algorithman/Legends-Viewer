﻿using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using LegendsViewer.Controls.HTML.Utilities;

namespace LegendsViewer.Controls.HTML
{
    public class ReadMeControl : PageControl
    {
        public WebBrowser HTMLBrowser;

        public ReadMeControl(DwarfTabControl tabControl)
        {
            Title = "README.md";
            TabControl = tabControl;
        }

        public override Control GetControl()
        {
            if (HTMLBrowser == null || HTMLBrowser.IsDisposed)
            {
                BrowserUtil.SetBrowserEmulationMode();

                var assembly = Assembly.GetExecutingAssembly();
                string markdown;
                var resourceName = "LegendsViewer.README.md";
                using (StreamReader reader = new StreamReader(assembly.GetManifestResourceStream(resourceName)))
                {
                    markdown = reader.ReadToEnd();
                }
                string readme = GetHtmlByMarkdown(markdown);

                var html = "<html>";
                html += "<head>";
                html += "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">";
                html += "<link rel=\"stylesheet\" href=\"" + LocalFileProvider.LocalPrefix + "WebContent/styles/github-markdown.css\">";
                html += "<style>.markdown-body { margin: 0 auto; padding: 20px; } </style>";
                html += "</head>";
                html += "<body><div class='markdown-body'>" + readme + "</div></body>";
                html += "</html>";

                HTMLBrowser = new WebBrowser
                {
                    Dock = DockStyle.Fill,
                    WebBrowserShortcutsEnabled = false,
                    DocumentText = html,
                    ScriptErrorsSuppressed = true
                };
                HTMLBrowser.Navigating += BrowserNavigating;

                return HTMLBrowser;
            }
            return HTMLBrowser;
        }

        private static string GetHtmlByMarkdown(string markdown)
        {
            string readme = CommonMark.CommonMarkConverter.Convert(markdown);
            readme = readme
                    .Replace(":high_brightness:",
                    "<img src=\"https://assets-cdn.github.com/images/icons/emoji/unicode/1f506.png\" alt=\":high_brightness:\" title=\":high_brightness:\" class=\"emoji\" height=\"20\" width=\"20\">")
                    .Replace(":blue_book:",
                    "<img src=\"https://assets-cdn.github.com/images/icons/emoji/unicode/1f4d8.png\" alt=\":blue_book:\" title=\":blue_book:\" class=\"emoji\" height=\"20\" width=\"20\">");
            return readme;
        }

        private void BrowserNavigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.ToString() != "about:blank")
            {
                string url = e.Url.ToString();
                Process.Start(url);
                e.Cancel = true; //Prevent the browser from actually navigating to a new page
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (HTMLBrowser != null)
                    {
                        HTMLBrowser.Dispose();
                        HTMLBrowser = null;
                    }
                }
                base.Dispose(disposing);
                this.disposed = true;
            }
        }

        public override void Refresh()
        {
        }
    }
}
