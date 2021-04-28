using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using JetBrains.Annotations;

namespace Ruminoid.Common2.Metro.MetroControls
{
    [PseudoClasses(":pressed", ":selected")]
    public class ClosableTabItem : HeaderedContentControl, ISelectable
    {
        /// <summary>
        /// Defines the <see cref="TabStripPlacement"/> property.
        /// </summary>
        [UsedImplicitly]
        public static readonly StyledProperty<Dock> TabStripPlacementProperty =
            ClosableTabControl.TabStripPlacementProperty.AddOwner<ClosableTabItem>();

        /// <summary>
        /// Defines the <see cref="IsSelected"/> property.
        /// </summary>
        [UsedImplicitly]
        public static readonly StyledProperty<bool> IsSelectedProperty =
            ListBoxItem.IsSelectedProperty.AddOwner<ClosableTabItem>();

        /// <summary>
        /// Initializes static members of the <see cref="ClosableTabItem"/> class.
        /// </summary>
        static ClosableTabItem()
        {
            SelectableMixin.Attach<ClosableTabItem>(IsSelectedProperty);
            PressedMixin.Attach<ClosableTabItem>();
            FocusableProperty.OverrideDefaultValue(typeof(ClosableTabItem), true);
            DataContextProperty.Changed.AddClassHandler<ClosableTabItem>((x, e) => x.UpdateHeader(e));
        }

        /// <summary>
        /// Gets the tab strip placement.
        /// </summary>
        /// <value>
        /// The tab strip placement.
        /// </value>
        public Dock TabStripPlacement => GetValue(TabStripPlacementProperty);

        /// <summary>
        /// Gets or sets the selection state of the item.
        /// </summary>
        public bool IsSelected
        {
            get => GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        private void UpdateHeader(AvaloniaPropertyChangedEventArgs obj)
        {
            if (Header == null)
                if (obj.NewValue is IHeadered headered)
                {
                    if (Header != headered.Header) Header = headered.Header;
                }
                else
                {
                    if (obj.NewValue is not IControl) Header = obj.NewValue;
                }
            else if (Header == obj.OldValue) Header = obj.NewValue;
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            Button tabCloseButton = e.NameScope.Get<Button>("PART_TabCloseButton");
            tabCloseButton.Click += (_, _) => OnTabClosing();
        }

        #region Closable

        [UsedImplicitly]
        public static readonly StyledProperty<bool> IsClosableProperty =
            AvaloniaProperty.Register<ClosableTabItem, bool>(nameof(IsClosable));

        public bool IsClosable
        {
            get => GetValue(IsClosableProperty);
            set => SetValue(IsClosableProperty, value);
        }

        public static readonly RoutedEvent<RoutedEventArgs> TabClosingEvent =
            RoutedEvent.Register<ClosableTabItem, RoutedEventArgs>("", RoutingStrategies.Bubble);

        public event EventHandler<RoutedEventArgs> TabClosing
        {
            add => AddHandler(TabClosingEvent, value);
            remove => RemoveHandler(TabClosingEvent, value);
        }

        protected virtual void OnTabClosing()
        {
            RoutedEventArgs e = new(TabClosingEvent);

            RaiseEvent(e);
        }

        #endregion
    }
}
