using System;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace snapwatch.Internal.Service
{
    public class VirtualizingWrapPanel : VirtualizingPanel, IScrollInfo
    {
        private Size _extent = new Size(0, 0);
        private Size _viewport = new Size(0, 0);
        private Point _offset;

        private ItemsControl _itemsControl;
        private IItemContainerGenerator _generator;

        public VirtualizingWrapPanel()
        {
            this.CanHorizontallyScroll = false;
            this.CanVerticallyScroll = true;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            _itemsControl = ItemsControl.GetItemsOwner(this);
            _generator = this.ItemContainerGenerator;
        }

        protected override void OnClearChildren()
        {
            base.OnClearChildren();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            System.Diagnostics.Debug.WriteLine("MeasureOverride called");

            _itemsControl ??= ItemsControl.GetItemsOwner(this);
            _generator ??= this.ItemContainerGenerator;

            if (_itemsControl == null || _generator == null)
            {
                return availableSize;
            }

            if (_itemsControl == null)
            {
                return availableSize;
            }

            int itemCount = _itemsControl.HasItems ? _itemsControl.Items.Count : 0;

            Size itemSize = new Size(ItemWidth, ItemHeight);
            int itemsPerRow = Math.Max(1, (int)(availableSize.Width / itemSize.Width));
            int rowCount = (int)Math.Ceiling((double)itemCount / itemsPerRow);

            _extent = new Size(itemsPerRow * itemSize.Width, rowCount * itemSize.Height);
            _viewport = availableSize;

            if (ScrollOwner != null)
            {
                ScrollOwner.InvalidateScrollInfo();
            }

            int firstVisibleIndex = (int)(_offset.Y / itemSize.Height) * itemsPerRow;
            int visibleRowCount = (int)Math.Ceiling(availableSize.Height / itemSize.Height);
            int lastVisibleIndex = Math.Min(itemCount, (firstVisibleIndex + (visibleRowCount + 1) * itemsPerRow));

            RemoveInternalChildRange(0, this.Children.Count);

            GeneratorPosition startPos = _generator.GeneratorPositionFromIndex(firstVisibleIndex);
            int childIndex = (startPos.Offset == 0) ? startPos.Index : startPos.Index + 1;

            using (_generator.StartAt(startPos, GeneratorDirection.Forward, true))
            {
                for (int i = firstVisibleIndex; i < lastVisibleIndex; i++, childIndex++)
                {
                    bool newlyRealized;

                    var child = _generator.GenerateNext(out newlyRealized) as UIElement;
                    if (newlyRealized)
                    {
                        if (childIndex >= this.Children.Count)
                            base.AddInternalChild(child);
                        else
                            base.InsertInternalChild(childIndex, child);

                        _generator.PrepareItemContainer(child);
                    }
                    else
                    {
                        // already realized
                    }

                    child.Measure(itemSize);
                }
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            System.Diagnostics.Debug.WriteLine("ArrangeOverride called");

            Size itemSize = new Size(ItemWidth, ItemHeight);
            int itemsPerRow = Math.Max(1, (int)(finalSize.Width / itemSize.Width));

            for (int i = 0; i < this.Children.Count; i++)
            {
                int itemIndex = i + (int)(_offset.Y / itemSize.Height) * itemsPerRow;

                int row = itemIndex / itemsPerRow;
                int column = itemIndex % itemsPerRow;

                Rect rect = new Rect(column * itemSize.Width, row * itemSize.Height, itemSize.Width, itemSize.Height);
                this.Children[i].Arrange(rect);
            }

            return finalSize;
        }

        #region IScrollInfo implementation

        public bool CanHorizontallyScroll { get; set; }
        public bool CanVerticallyScroll { get; set; }
        public ScrollViewer ScrollOwner { get; set; }

        public double ExtentHeight => _extent.Height;
        public double ExtentWidth => _extent.Width;

        public double ViewportHeight => _viewport.Height;
        public double ViewportWidth => _viewport.Width;

        public double HorizontalOffset => _offset.X;
        public double VerticalOffset => _offset.Y;

        public void LineDown() => SetVerticalOffset(this.VerticalOffset + 10);
        public void LineUp() => SetVerticalOffset(this.VerticalOffset - 10);
        public void LineLeft() => SetHorizontalOffset(this.HorizontalOffset - 10);
        public void LineRight() => SetHorizontalOffset(this.HorizontalOffset + 10);
        public void MouseWheelDown() => SetVerticalOffset(this.VerticalOffset + 20);
        public void MouseWheelUp() => SetVerticalOffset(this.VerticalOffset - 20);
        public void MouseWheelLeft() => SetHorizontalOffset(this.HorizontalOffset - 20);
        public void MouseWheelRight() => SetHorizontalOffset(this.HorizontalOffset + 20);
        public void PageDown() => SetVerticalOffset(this.VerticalOffset + this.ViewportHeight);
        public void PageUp() => SetVerticalOffset(this.VerticalOffset - this.ViewportHeight);
        public void PageLeft() => SetHorizontalOffset(this.HorizontalOffset - this.ViewportWidth);
        public void PageRight() => SetHorizontalOffset(this.HorizontalOffset + this.ViewportWidth);

        public void SetHorizontalOffset(double offset)
        {
            if (offset < 0 || this.ViewportWidth >= this.ExtentWidth)
            {
                offset = 0;
            }
            else
            {
                if (offset + this.ViewportWidth >= this.ExtentWidth)
                {
                    offset = this.ExtentWidth - this.ViewportWidth;
                }
            }

            _offset.X = offset;

            ScrollOwner?.InvalidateScrollInfo();
            InvalidateMeasure();
        }

        public void SetVerticalOffset(double offset)
        {
            if (offset < 0 || this.ViewportHeight >= this.ExtentHeight)
            {
                offset = 0;
            }
            else
            {
                if (offset + this.ViewportHeight >= this.ExtentHeight)
                {
                    offset = this.ExtentHeight - this.ViewportHeight;
                }
            }

            _offset.Y = offset;

            ScrollOwner?.InvalidateScrollInfo();
            InvalidateMeasure();
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return rectangle;
        }

        public bool IsEmpty => _viewport == default(Size);

        #endregion

        #region Customization

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(VirtualizingWrapPanel),
                new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(VirtualizingWrapPanel),
                new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion
    }
}
