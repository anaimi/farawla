﻿#pragma checksum "..\..\..\..\Features\ColorPicker\AlphaSlider.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "9E905735CDEE1D8A9828286BE00CBDF9"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace Farawla.Features.ColorPicker {
    
    
    /// <summary>
    /// AlphaSlider
    /// </summary>
    public partial class AlphaSlider : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 4 "..\..\..\..\Features\ColorPicker\AlphaSlider.xaml"
        internal Farawla.Features.ColorPicker.AlphaSlider AlphaSliderControl;
        
        #line default
        #line hidden
        
        
        #line 7 "..\..\..\..\Features\ColorPicker\AlphaSlider.xaml"
        internal System.Windows.Controls.Canvas AlphaGradiant;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\..\Features\ColorPicker\AlphaSlider.xaml"
        internal System.Windows.Controls.Grid Selector;
        
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
            System.Uri resourceLocater = new System.Uri("/Farawla;component/features/colorpicker/alphaslider.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Features\ColorPicker\AlphaSlider.xaml"
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
            this.AlphaSliderControl = ((Farawla.Features.ColorPicker.AlphaSlider)(target));
            return;
            case 2:
            this.AlphaGradiant = ((System.Windows.Controls.Canvas)(target));
            
            #line 7 "..\..\..\..\Features\ColorPicker\AlphaSlider.xaml"
            this.AlphaGradiant.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.SelectorMouseDown);
            
            #line default
            #line hidden
            
            #line 7 "..\..\..\..\Features\ColorPicker\AlphaSlider.xaml"
            this.AlphaGradiant.MouseMove += new System.Windows.Input.MouseEventHandler(this.SelectorMouseMove);
            
            #line default
            #line hidden
            return;
            case 3:
            this.Selector = ((System.Windows.Controls.Grid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
