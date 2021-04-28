using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using JetBrains.Annotations;

namespace Ruminoid.Common2.Metro.MetroControls
{
    [UsedImplicitly]
    public class ClosableTabControl : SelectingItemsControl, IContentPresenterHost
    {
        /// <summary>
        /// Defines the <see cref="TabStripPlacement"/> property.
        /// </summary>
        public static readonly StyledProperty<Dock> TabStripPlacementProperty =
            AvaloniaProperty.Register<ClosableTabControl, Dock>(nameof(TabStripPlacement), Dock.Top);

        /// <summary>
        /// Defines the <see cref="HorizontalContentAlignment"/> property.
        /// </summary>
        [UsedImplicitly]
        public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
            ContentControl.HorizontalContentAlignmentProperty.AddOwner<ClosableTabControl>();

        /// <summary>
        /// Defines the <see cref="VerticalContentAlignment"/> property.
        /// </summary>
        [UsedImplicitly]
        public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty =
            ContentControl.VerticalContentAlignmentProperty.AddOwner<ClosableTabControl>();

        /// <summary>
        /// Defines the <see cref="ContentTemplate"/> property.
        /// </summary>
        [UsedImplicitly]
        public static readonly StyledProperty<IDataTemplate> ContentTemplateProperty =
            ContentControl.ContentTemplateProperty.AddOwner<ClosableTabControl>();

        /// <summary>
        /// The selected content property
        /// </summary>
        [UsedImplicitly]
        public static readonly StyledProperty<object> SelectedContentProperty =
            AvaloniaProperty.Register<ClosableTabControl, object>(nameof(SelectedContent));

        /// <summary>
        /// The selected content template property
        /// </summary>
        [UsedImplicitly]
        public static readonly StyledProperty<IDataTemplate> SelectedContentTemplateProperty =
            AvaloniaProperty.Register<ClosableTabControl, IDataTemplate>(nameof(SelectedContentTemplate));

        /// <summary>
        /// The default value for the <see cref="ItemsControl.ItemsPanel"/> property.
        /// </summary>
        private static readonly FuncTemplate<IPanel> DefaultPanel =
            new(() => new WrapPanel());

        /// <summary>
        /// Initializes static members of the <see cref="ClosableTabControl"/> class.
        /// </summary>
        static ClosableTabControl()
        {
            SelectionModeProperty.OverrideDefaultValue<ClosableTabControl>(SelectionMode.AlwaysSelected);
            ItemsPanelProperty.OverrideDefaultValue<ClosableTabControl>(DefaultPanel);
            AffectsMeasure<ClosableTabControl>(TabStripPlacementProperty);
            SelectedItemProperty.Changed.AddClassHandler<ClosableTabControl>((x, _) => x.UpdateSelectedContent());
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the content within the control.
        /// </summary>
        public HorizontalAlignment HorizontalContentAlignment
        {
            get => GetValue(HorizontalContentAlignmentProperty);
            set => SetValue(HorizontalContentAlignmentProperty, value);
        }

        /// <summary>
        /// Gets or sets the vertical alignment of the content within the control.
        /// </summary>
        public VerticalAlignment VerticalContentAlignment
        {
            get => GetValue(VerticalContentAlignmentProperty);
            set => SetValue(VerticalContentAlignmentProperty, value);
        }

        /// <summary>
        /// Gets or sets the tabstrip placement of the TabControl.
        /// </summary>
        public Dock TabStripPlacement
        {
            get => GetValue(TabStripPlacementProperty);
            set => SetValue(TabStripPlacementProperty, value);
        }

        /// <summary>
        /// Gets or sets the default data template used to display the content of the selected tab.
        /// </summary>
        public IDataTemplate ContentTemplate
        {
            get => GetValue(ContentTemplateProperty);
            set => SetValue(ContentTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the content of the selected tab.
        /// </summary>
        /// <value>
        /// The content of the selected tab.
        /// </value>
        public object SelectedContent
        {
            get => GetValue(SelectedContentProperty);
            internal set => SetValue(SelectedContentProperty, value);
        }

        /// <summary>
        /// Gets or sets the content template for the selected tab.
        /// </summary>
        /// <value>
        /// The content template of the selected tab.
        /// </value>
        public IDataTemplate SelectedContentTemplate
        {
            get => GetValue(SelectedContentTemplateProperty);
            internal set => SetValue(SelectedContentTemplateProperty, value);
        }

        [UsedImplicitly]
        internal ItemsPresenter ItemsPresenterPart { get; private set; }

        [UsedImplicitly]
        internal IContentPresenter ContentPart { get; private set; }

        /// <inheritdoc/>
        IAvaloniaList<ILogical> IContentPresenterHost.LogicalChildren => LogicalChildren;

        /// <inheritdoc/>
        bool IContentPresenterHost.RegisterContentPresenter(IContentPresenter presenter)
        {
            return RegisterContentPresenter(presenter);
        }

        protected override void OnContainersMaterialized(ItemContainerEventArgs e)
        {
            base.OnContainersMaterialized(e);
            UpdateSelectedContent();
        }

        protected override void OnContainersRecycled(ItemContainerEventArgs e)
        {
            base.OnContainersRecycled(e);
            UpdateSelectedContent();
        }

        private void UpdateSelectedContent()
        {
            if (SelectedIndex == -1)
                SelectedContent = SelectedContentTemplate = null;
            else
            {
                var container = SelectedItem as IContentControl ??
                    ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as IContentControl;
                SelectedContentTemplate = container?.ContentTemplate;
                SelectedContent = container?.Content;
            }
        }

        /// <summary>
        /// Called when an <see cref="IContentPresenter"/> is registered with the control.
        /// </summary>
        /// <param name="presenter">The presenter.</param>
        protected virtual bool RegisterContentPresenter(IContentPresenter presenter)
        {
            if (presenter.Name != "PART_SelectedContentHost") return false;

            ContentPart = presenter;
            return true;
        }

        protected override IItemContainerGenerator CreateItemContainerGenerator()
        {
            return new ClosableTabItemContainerGenerator(this);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            ItemsPresenterPart = e.NameScope.Get<ItemsPresenter>("PART_ItemsPresenter");
        }

        /// <inheritdoc/>
        protected override void OnGotFocus(GotFocusEventArgs e)
        {
            base.OnGotFocus(e);

            if (e.NavigationMethod == NavigationMethod.Directional)
                e.Handled = UpdateSelectionFromEventSource(e.Source);
        }

        /// <inheritdoc/>
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && e.Pointer.Type == PointerType.Mouse)
                e.Handled = UpdateSelectionFromEventSource(e.Source);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            if (e.InitialPressMouseButton != MouseButton.Left || e.Pointer.Type == PointerType.Mouse) return;

            var container = GetContainerFromEventSource(e.Source);
            if (container != null
                && container.GetVisualsAt(e.GetPosition(container))
                    .Any(c => container == c || container.IsVisualAncestorOf(c)))
                e.Handled = UpdateSelectionFromEventSource(e.Source);
        }
    }
}
