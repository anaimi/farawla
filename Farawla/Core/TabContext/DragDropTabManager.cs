using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Farawla.Core.TabContext
{
	public class DragDropTabManager
	{
		private static readonly DependencyProperty ManagerProperty =
			DependencyProperty.Register(typeof(DragDropTabManager).ToString(), typeof(DragDropTabManager),
										typeof(DragDropTabManager));

		public static readonly DependencyProperty EnabledProperty =
			DependencyProperty.RegisterAttached("Enabled", typeof(bool),
												typeof(DragDropTabManager),
												new PropertyMetadata(false, DDTM_EnabledChanged));

		private bool isMoving;
		private TabItem movingTabItem;
		private TabItem lastTab;
		private Point ptStart;
		
		private static void DDTM_EnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var tc = d as TabControl;
			
			if (tc != null)
			{
				var oldValue = (bool)e.OldValue;
				var newValue = (bool)e.NewValue;

				if (oldValue && !newValue)
				{
					var ddtm = tc.GetValue(ManagerProperty) as DragDropTabManager;
					
					if (ddtm != null)
					{
						tc.PreviewMouseDown -= ddtm.TabItem_PreviewMouseDown;
						tc.SetValue(ManagerProperty, null);
					}
				}
				else if (!oldValue && newValue)
				{
					var ddtm = new DragDropTabManager();
					
					tc.SetValue(ManagerProperty, ddtm);
					tc.PreviewMouseDown += ddtm.TabItem_PreviewMouseDown;
				}
			}
		}

		public static bool GetEnabled(DependencyObject obj)
		{
			return (bool)obj.GetValue(EnabledProperty);
		}
		
		public static int GetHighestZIndex()
		{
			return Controller.Current.CurrentTabs.Count + 1;
		}

		public static void SetEnabled(DependencyObject obj, bool value)
		{
			obj.SetValue(EnabledProperty, value);
		}

		private static TabItem FindTabItem(UIElement parent, Point pt)
		{
			var fe = parent.InputHitTest(pt) as FrameworkElement;

			while (fe != null && fe.GetType() != typeof(TabItem))
				fe = VisualTreeHelper.GetParent(fe) as FrameworkElement;

			return fe as TabItem;
		}

		private void TabItem_PreviewMouseDown(object sender, MouseEventArgs e)
		{
			TabItem ti;
			
			if (e.Source is TabItem)
				ti = e.Source as TabItem;
			else if (e.Source is TextBlock)
				ti = (e.Source as TextBlock).Parent as TabItem;
			else
				return;
			
			if (ti != null && e.LeftButton == MouseButtonState.Pressed)
			{
				var tc = ti.Parent as TabControl;
				
				if (tc != null)
				{
					tc.MouseMove += tc_MouseMove;
					tc.MouseLeftButtonUp += tc_MouseLeftButtonUp;

					ptStart = e.GetPosition(tc);
					movingTabItem = ti;
				}
			}
		}

		private void tc_MouseMove(object sender, MouseEventArgs e)
		{
			var tc = sender as TabControl;
			
			if (tc == null)
				return;

			var pt = e.GetPosition(tc);

			if (movingTabItem == null || !movingTabItem.IsVisible)
			{
				tc_MouseLeftButtonUp(sender, null);
				return;
			}

			if (!isMoving)
			{
				if (Math.Abs(pt.X - ptStart.X) > 10)
				{
					movingTabItem.IsHitTestVisible = false;
					movingTabItem.RenderTransformOrigin = new Point(.5, .5);
					movingTabItem.RenderTransform = new TranslateTransform(0, 0);
					
					tc.Cursor = Cursors.Hand;					
					tc.CaptureMouse();

					Panel.SetZIndex(movingTabItem, GetHighestZIndex());
					isMoving = true;
				}
				
				return;
			}

			var newPos = FindTabItem(tc, pt);
			
			if (newPos != null)
			{
				lastTab = newPos;
				
				var xform = movingTabItem.RenderTransform as TranslateTransform;
				
				if (xform != null)
					xform.X = pt.X - ptStart.X;
				
				tc.Cursor = Cursors.Hand;
			}
		}

		private void tc_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			var tc = sender as TabControl;
			
			Debug.Assert(tc != null);

			tc.ReleaseMouseCapture();
			tc.MouseMove -= tc_MouseMove;
			tc.MouseLeftButtonUp -= tc_MouseLeftButtonUp;

			if (isMoving)
			{
				isMoving = false;
				tc.Cursor = Cursors.Arrow;
				
				movingTabItem.RenderTransform = null;
				movingTabItem.IsHitTestVisible = true;

				if (lastTab != null)
				{
					if (lastTab != null && movingTabItem != lastTab)
					{
						var targetIndex = tc.Items.IndexOf(lastTab);
						
						tc.Items.Remove(movingTabItem);
						tc.Items.Insert(targetIndex, movingTabItem);

						movingTabItem.Focus();
					}
				}
			}
			
			movingTabItem = lastTab = null;
		}
	}
}
