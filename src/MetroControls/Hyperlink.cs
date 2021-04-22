// https://github.com/AvaloniaUtils/MessageBox.Avalonia/blob/master/src/MessageBox.Avalonia/Controls/Hyperlink.cs

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using JetBrains.Annotations;
using Ruminoid.Common2.Utils.Net;

namespace Ruminoid.Common2.Metro.MetroControls
{
    public class Hyperlink : TextBlock
    {
        private string _url;

        [UsedImplicitly]
        public static readonly DirectProperty<Hyperlink, string> UrlProperty
            = AvaloniaProperty.RegisterDirect<Hyperlink, string>(nameof(Url), o => o.Url, (o, v) => o.Url = v);

        public string Url
        {
            get => _url;
            set => SetAndRaise(UrlProperty, ref _url, value);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            NetUtils.OpenExternalLink(Url);
        }

    }
}
