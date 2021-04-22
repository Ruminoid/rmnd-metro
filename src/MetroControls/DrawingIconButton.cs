using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Ruminoid.Common2.Metro.MetroControls
{
    public class DrawingIconButton : Button
    {
        #region Icon

        public static readonly StyledProperty<Drawing> IconProperty =
            AvaloniaProperty.Register<DrawingIconButton, Drawing>(nameof(Icon));

        public Drawing Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        #endregion
    }
}
