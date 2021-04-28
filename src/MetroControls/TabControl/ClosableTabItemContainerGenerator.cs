using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.LogicalTree;
using Avalonia.Reactive;
using JetBrains.Annotations;

namespace Ruminoid.Common2.Metro.MetroControls
{
    public class ClosableTabItemContainerGenerator : ItemContainerGenerator<ClosableTabItem>
    {
        public ClosableTabItemContainerGenerator(ClosableTabControl owner)
            : base(owner, ContentControl.ContentProperty, ContentControl.ContentTemplateProperty)
        {
            Owner = owner;
        }

        [UsedImplicitly]
        public new ClosableTabControl Owner { get; }

        protected override IControl CreateContainer(object item)
        {
            var tabItem = (ClosableTabItem)base.CreateContainer(item);

            tabItem.Bind(ClosableTabItem.TabStripPlacementProperty, new OwnerBinding<Dock>(
                tabItem,
                ClosableTabControl.TabStripPlacementProperty));

            if (tabItem.HeaderTemplate == null)
                tabItem.Bind(HeaderedContentControl.HeaderTemplateProperty, new OwnerBinding<IDataTemplate>(
                    tabItem,
                    ItemsControl.ItemTemplateProperty));

            if (tabItem.Header == null)
                if (item is IHeadered headered)
                    tabItem.Header = headered.Header;
                else if (tabItem.DataContext is not IControl) tabItem.Header = tabItem.DataContext;

            if (tabItem.Content is not IControl)
                tabItem.Bind(ContentControl.ContentTemplateProperty, new OwnerBinding<IDataTemplate>(
                    tabItem,
                    ClosableTabControl.ContentTemplateProperty));

            return tabItem;
        }

        private class OwnerBinding<T> : SingleSubscriberObservableBase<T>
        {
            private readonly ClosableTabItem _item;
            private readonly StyledProperty<T> _ownerProperty;
            private IDisposable _ownerSubscription;
            private IDisposable _propertySubscription;

            public OwnerBinding(ClosableTabItem item, StyledProperty<T> ownerProperty)
            {
                _item = item;
                _ownerProperty = ownerProperty;
            }

            protected override void Subscribed()
            {
                _ownerSubscription = ControlLocator.Track(_item, 0, typeof(ClosableTabControl)).Subscribe(OwnerChanged);
            }

            protected override void Unsubscribed()
            {
                _ownerSubscription?.Dispose();
                _ownerSubscription = null;
            }

            private void OwnerChanged(ILogical c)
            {
                _propertySubscription?.Dispose();
                _propertySubscription = null;

                if (c is ClosableTabControl tabControl)
                    _propertySubscription = tabControl.GetObservable(_ownerProperty)
                        .Subscribe(PublishNext);
            }
        }
    }
}
