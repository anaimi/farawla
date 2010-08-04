using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Farawla.Features.FileExplorer
{
	public partial class FileList : UserControl
	{
		public ItemCollection Items
		{
			get
			{
				return list.Items;
			}
		}
		
		public event Action<ListBoxItem> OnItemOpen;
		
		private Dictionary<ListBoxItem, List<ListBoxItem>> nodes;
		
		public FileList()
		{
			nodes = new Dictionary<ListBoxItem, List<ListBoxItem>>();
			
			InitializeComponent();
		}

		public void AddItem(ListBoxItem parent, ListBoxItem newItem)
		{
			var index = parent == null ? 0 : list.Items.IndexOf(parent) + 1;
			AddItem(index, parent, newItem);
		}
		
		public void AddItem(int index, ListBoxItem parent, ListBoxItem newItem)
		{
			// assign events
			newItem.MouseDoubleClick += ListItemDoubleClick;

			// assign parent
			if (parent == null)
			{
				nodes.Add(newItem, new List<ListBoxItem>());
			}
			else
			{
				if (!nodes.ContainsKey(parent))
					nodes.Add(parent, new List<ListBoxItem>());

				nodes[parent].Add(newItem);
			}

			// insert!
			list.Items.Insert(index, newItem);
		}

		private void ListItemDoubleClick(object s, EventArgs e)
		{
			OnItemOpen(s as ListBoxItem);
		}
		
		public int GetIndex(ListBoxItem item)
		{
			return list.Items.IndexOf(item);
		}
		
		public void Collapse(ListBoxItem node)
		{
			if (!nodes.ContainsKey(node))
				return;
			
			var items = nodes[node];
			
			foreach(var item in items)
			{
				list.Items.Remove(item);
				
				if (nodes.ContainsKey(item))
					Collapse(item);
			}
		}
	}
}
