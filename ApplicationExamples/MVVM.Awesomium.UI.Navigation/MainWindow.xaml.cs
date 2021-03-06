﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Awesomium.Core;
using MVVMAwesomium;


namespace MVVMAwesomium.UI.Navigation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private void SetUpRoute(INavigationBuilder iNavigationBuilder)
        {
            iNavigationBuilder.Register<MVVMAwesomium.ViewModel.Example.ForNavigation.Person>("HTMLUI\\index_one.html");
            iNavigationBuilder.Register<MVVMAwesomium.ViewModel.Example.ForNavigation.Couple>("HTMLUI\\index_couple.html");
        }


        public MainWindow()
        {
            WebConfig webC = new WebConfig();
            webC.RemoteDebuggingPort = 8001;
            webC.RemoteDebuggingHost = "127.0.0.1";
            WebCore.Initialize(webC);

            InitializeComponent();

            HTMLWindow.UseINavigable = true;
            SetUpRoute(HTMLWindow.NavigationBuilder);

            var datacontext = new MVVMAwesomium.ViewModel.Example.ForNavigation.Couple();
            var my = new MVVMAwesomium.ViewModel.Example.ForNavigation.Person()
            {
                Name = "O Monstro",
                LastName = "Desmaisons",
                Local = new MVVMAwesomium.ViewModel.Example.Local() { City = "Florianopolis", Region = "SC" }
            };
            my.Couple = datacontext;
            datacontext.One = my;

            HTMLWindow.NavigateAsync(datacontext);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.HTMLWindow.Dispose();
        } 
    }
}
