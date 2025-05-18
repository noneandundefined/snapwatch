using System;

namespace snapwatch.Core.Service
{
    public class Navigation
    {
        private readonly MainWindow _main;

        public Navigation(MainWindow main)
        {
            this._main = main;
        }

        public void SetFrameContent(string content_uri)
        {
            this._main.FrameContent.Navigate(new System.Uri(content_uri, UriKind.Absolute));
        }
    }
}
