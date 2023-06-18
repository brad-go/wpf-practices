# WPF Practices

WPF를 학습하며 만든 프로젝트 및 개념들을 정리하기 위한 저장소입니다.

## 목차

- [MVVM](#MVVM)

## MVVM

- [View와 ViewModel의 결합도 낮추기](#view와-viewmodel의-결합도-낮추기)
- [TextBox 바인딩 시에 데이터가 바로 업데이트되지 않는 문제 해결하기](#textbox-바인딩-시에-데이터가-바로-업데이트되지-않는-문제-해결하기)

### View와 ViewModel의 결합도 낮추기

WPF 프로젝트를 생성하면 기본적으로 `App.xaml` 파일과 `App.xaml.cs` 파일을 볼 수 있다.

```xml
<!-- App.xaml -->

<Application 
    x:Class="LSF_CAMS.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="clr-namespace:LSF_CAMS"
    StartupUri="MainWindow.xaml">
    <Application.Resources>
         
    </Application.Resources>
</Application>
```

```cs
// App.xaml.cs

public partial class App : Application
{
}
```

`App.xaml` 파일의 `StartupUri`에서 프로젝트를 시작할 MainWindow를 결정하게 되는데, 이는 다음과 같이 바꿔줄 수 있다.

```xml
<!-- App.xaml -->

<Application 
    x:Class="LSF_CAMS.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="clr-namespace:LSF_CAMS">
    <Application.Resources>
         
    </Application.Resources>
</Application>
```

`App.xaml` 파일에서 `StartupUri`를 제거한다.

```cs
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        MainWindow = new MainWindow()
        {
            DataContext = new MainViewModel()
        };
        MainWindow.Show();

        base.OnStartup(e);
    }
}
```

`App.xaml.cs` 파일에 위와 같이 OnStartup 메서드를 생성한다. 

이렇게 하면 View와 ViewModel을 연결하기 위해, 각 View의 xaml.cs파일에서 ViewModel을 생성해 DataContext를 할당하는 **View에서 ViewModel 레이어를 의존해 결합도가 높아지는 문제를 해결**할 수 있다. 

[⬆️ Back to top](#목차)

### TextBox 바인딩 시에 데이터가 바로 업데이트되지 않는 문제 해결하기

TextBox를 생성하고 ViewModel의 데이터와 바인딩했는데 입력 후 다른 곳을 클릭하기 전까지 데이터가 업데이트되지 않는 문제가 있다. 아래의 코드를 보자.

```xml
<!-- MainWindow.xaml -->

<Window  
    x:Class="Reservoom.MainWindow"
    // ...>
    <Grid MaxWidth="600" Margin="20 10">
        <views:MakeReservationView DataContext="{Binding CurrentViewModel}"/>
    </Grid>
</Window>
```

```cs
public class MainViewModel : ViewModelBase
{
    public ViewModelBase CurrentViewModel { get; }

    public MainViewModel()
    {
        CurrentViewModel = new MakeReservationViewModel();
    }
}
```

```xml
<!-- MakeReservaitionView -->

<UserControl 
    x:Class="Reservoom.Views.MakeReservationView"
    // ...>
    <Grid>
        <TextBox 
            Grid.Row="0" 
            Grid.Column="0"
            Margin="0 5 0 0" 
            Text="{Binding Username}" />
    </Grid>
</UserControl>
```

위와 같이 MakeRservationViewModel에 `Username`이란 프로퍼티가 있고, 이를 MakeReservationView의 한 TextBox에 바인딩했다고 생각해보자. 지금 프로젝트를 실행시켜서 확인해보면 TextBox에 입력한 값은 다른 곳을 클릭하기 전까지 데이터가 업데이트되지 않는다. 이러한 문제를 해결하기 위해서는 `UpdateSourceTrigger`라는 Tirgger를 추가해줘야 한다.

```xml
<!-- MakeReservaitionView -->

<UserControl 
    x:Class="Reservoom.Views.MakeReservationView"
    // ...>
    <Grid>
        <TextBox 
            Grid.Row="0" 
            Grid.Column="0"
            Margin="0 5 0 0" 
            Text="{Binding Username, UpdateSourceTrigger=PropertyChanged}" />
    </Grid>
</UserControl>
```

[⬆️ Back to top](#목차)
