﻿#pragma checksum "..\..\..\..\Features\Stats\StatsWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "5FB2D09362EC40165A03C96E98D50662"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Farawla.Core;
using ICSharpCode.AvalonEdit.CodeCompletion;
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


namespace Farawla.Features.Stats {
    
    
    /// <summary>
    /// StatsWindow
    /// </summary>
    public partial class StatsWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 39 "..\..\..\..\Features\Stats\StatsWindow.xaml"
        internal System.Windows.Controls.TextBlock Position;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\..\Features\Stats\StatsWindow.xaml"
        internal System.Windows.Controls.TextBlock Chars;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\..\Features\Stats\StatsWindow.xaml"
        internal System.Windows.Controls.TextBlock Words;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\..\Features\Stats\StatsWindow.xaml"
        internal System.Windows.Controls.TextBlock LastModified;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\..\..\Features\Stats\StatsWindow.xaml"
        internal System.Windows.Controls.TextBlock FileSize;
        
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
            System.Uri resourceLocater = new System.Uri("/Farawla;component/features/stats/statswindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Features\Stats\StatsWindow.xaml"
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
            this.Position = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.Chars = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.Words = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.LastModified = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.FileSize = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
