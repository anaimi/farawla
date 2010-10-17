﻿#pragma checksum "..\..\..\..\Features\Search\Widget.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "53D58CB8BF720032198F6720D865EB73"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3615
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Farawla.Utilities;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Farawla.Features.Search {
    
    
    /// <summary>
    /// Widget
    /// </summary>
    public partial class Widget : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 24 "..\..\..\..\Features\Search\Widget.xaml"
        internal System.Windows.Controls.TextBox Query;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\Features\Search\Widget.xaml"
        internal System.Windows.Controls.TextBox ReplaceWith;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\..\Features\Search\Widget.xaml"
        internal System.Windows.Controls.ComboBox SearchArea;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\..\Features\Search\Widget.xaml"
        internal System.Windows.Controls.ComboBoxItem SearchAreaSelectedText;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\Features\Search\Widget.xaml"
        internal System.Windows.Controls.ComboBoxItem SearchAreaCurrentDocument;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\..\Features\Search\Widget.xaml"
        internal System.Windows.Controls.ComboBoxItem SearchAreaOpenDocuments;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Farawla;component/features/search/widget.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Features\Search\Widget.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.Query = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.ReplaceWith = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.SearchArea = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 4:
            this.SearchAreaSelectedText = ((System.Windows.Controls.ComboBoxItem)(target));
            return;
            case 5:
            this.SearchAreaCurrentDocument = ((System.Windows.Controls.ComboBoxItem)(target));
            return;
            case 6:
            this.SearchAreaOpenDocuments = ((System.Windows.Controls.ComboBoxItem)(target));
            return;
            case 7:
            
            #line 43 "..\..\..\..\Features\Search\Widget.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.FindClicked);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 44 "..\..\..\..\Features\Search\Widget.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ReplaceClicked);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
