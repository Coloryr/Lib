﻿using System.Windows;

namespace ColoryrBuild.Windows;

/// <summary>
/// Input1Window.xaml 的交互逻辑
/// </summary>
public partial class Input1Window : Window
{
    private bool Res;
    public Input1Window(string title, string lable1, string lable2,
        string input1 = null, string input2 = null)
    {
        InitializeComponent();
        Title = title;
        Lable1.Content = lable1;
        Lable2.Content = lable2;
        Input1.Text = input1;
        Input2.Text = input2;
    }

    public bool Set(out string input1, out string input2)
    {
        ShowDialog();
        input1 = Input1.Text;
        input2 = Input2.Text;
        return Res;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Res = true;
        Close();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        Res = false;
        Close();
    }
}
