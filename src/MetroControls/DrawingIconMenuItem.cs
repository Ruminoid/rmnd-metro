using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Ruminoid.Common2.Metro.MetroControls
{
    public class DrawingIconMenuItem : MenuItem
    {
        #region Icon

        public new static readonly StyledProperty<Drawing> IconProperty =
            AvaloniaProperty.Register<DrawingIconButton, Drawing>(nameof(Icon));

        public new Drawing Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        #endregion
    }
}
