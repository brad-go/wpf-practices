# WPF Practices

WPF를 학습하며 만든 프로젝트 및 개념들을 정리하기 위한 저장소입니다.

## 목차

- [MVVM](#MVVM)

## MVVM

- [View와 ViewModel의 결합도 낮추기](#view와-viewmodel의-결합도-낮추기)
- [TextBox 바인딩 시에 데이터가 바로 업데이트되지 않는 문제 해결하기](#textbox-바인딩-시에-데이터가-바로-업데이트되지-않는-문제-해결하기)
- [Navigation 기능 구현하기](#navigation-기능-구현하기)

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

### Navigation 기능 구현하기

#### Store 생성

ViewModel을 변경할 수 있도록 현재 ViewModel의 상태를 가지고 있고, ViewModel을 변경할 수 있는 클래스를 생성한다. 

```csharp
// Stores/NavigationStore.cs

public class NavigationStore
{
    private ViewModelBase _currentViewModel;

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            _currentViewModel = value;
            OnCurrentViewModelChanged();
        }
    }

    public event Action CurrentViewModelChanged;

    // 위에서 정의한 Action에 등록된 이벤트 핸들러가 실행될 수 있도록 구현
    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}
```

#### Service 생성

NavigationService를 생성해 NavigationStore의 현재 뷰모델을 입력받은 함수의 반환값인 ViewModel로 정의할 수 있는 기능을 구현한다.

```csharp
public class NavigationService
{
    private readonly NavigationStore _navigationStore;
    private readonly Func<ViewModelBase> _createViewModel;

    public NavigationService(NavigationStore navigationStore, Func<ViewModelBase> createViewModel)
    {
        _navigationStore = navigationStore;
        _createViewModel = createViewModel;
    }

    public void Navigate()
    {
        _navigationStore.CurrentViewModel = _createViewModel();
    }
}
```

#### 최상단에서 Store 및 Service 정의하기

NavigationStore를 최상단인 App.xaml.cs에서 정의하고 MainViewModel에 전달한다.

```csharp
// App.xaml.cs

public partial class App : Application
{
    private readonly Hotel _hotel;
    private readonly NavigationStore _navigationStore;

    public App()
    {
        _hotel = new Hotel("Brad Suites");
        _navigationStore = new NavigationStore();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        // 초기 CurrentViewModel을 ReservationListingViewModel로 지정
        _navigationStore.CurrentViewModel = CreateReservationListingViewModel();

        MainWindow = new MainWindow()
        {
            // MainViewModel에게 현재 ViewModel의 상태를 가진 navigationStore를 전달
            DataContext = new MainViewModel(_navigationStore)
        };

        MainWindow.Show();

        base.OnStartup(e);
    }

    // 각각 뷰모델들에게 store, hotel, service 등을 전달하려면 점점 인수가 많아지게 된다.
    // 아래와 같이 앱의 최상단으로 끌어올린 후 함수로 만들어 전달할 인수를 줄여주고, 
    // 동일한 상태값을 사용할 수 있도록 해준다. 
    private MakeReservationViewModel CreateMakeReservationViewModel()
    {
        return new MakeReservationViewModel(_hotel, new NavigationService(_navigationStore, CreateReservationListingViewModel));
    }

    private ReservationListingViewModel CreateReservationListingViewModel()
    {
        return new ReservationListingViewModel(_hotel, new NavigationService(_navigationStore, CreateMakeReservationViewModel));
    }
}
```

#### View 및 ViewModel에 연결

MainViewModel에서 MainWindow에서 Binding해서 사용할 CurrentViewModel 프로퍼티를 생성해주고, NavigationStore에서 정의한 액션에 View가 업데이트 됨을 알리는 이벤트 핸들러를 등록해준다. 

```csharp
// ViewModels/MainViewModel.cs

public class MainViewModel : ViewModelBase
{
    private readonly NavigationStore _navigationStore;

    public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;

    public MainViewModel(NavigationStore navigationStore)
    {
        _navigationStore = navigationStore;
        // navigationStore에서 정의한 Action에 이벤트를 부여한다.
        _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
    }

    // OnPropertyChanged를 통해 View에게 ViewModel이 변경되었음을 알려준다. 
    private void OnCurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentViewModel));
    }
}
```

CurrentViewModel이 변경될 때, 다른 View를 렌더링하도록 지정해준다. 

```xml
<!-- Views/MainWindow.xaml -->

<Window ...>
    <Grid MaxWidth="600" Margin="20 10">
        <Grid.Resources>
            <!-- 각 ViewModel이 Content가 될 떄, 각 템플릿에 맞는 뷰를 렌더 -->
            <DataTemplate DataType="{x:Type viewmodels:MakeReservationViewModel}">
                <views:MakeReservationView />
            </DataTemplate>
            <DataTemplate DataType="{x:Type viewmodels:ReservationListingViewModel}">
                <views:ReservationListingView />
            </DataTemplate>
        </Grid.Resources>
        <ContentControl Content="{Binding CurrentViewModel}" />
    </Grid>
</Window>
```

Command들을 생성한다.

```csharp
// Commands/NavigateCommand.cs

public class NavigateCommand : CommandBase
{
    private readonly NavigationService _navigationService;

    public NavigateCommand(NavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public override void Execute(object? parameter)
    {
        // 현재 NavigationService가 가진 NavigationStore의 CurrentViewModel에 맞는 View를 렌더
        _navigationService.Navigate();
    }
}
```

```csharp
public class MakeReservationCommand : CommandBase
{
    // ...
    private readonly NavigationService _reservationViewNavigationService;

    public MakeReservationCommand(MakeReservationViewModel makeReservationViewModel,
        Hotel hotel,
        NavigationService reservationViewNavigationService)
    {
        _makeReservationViewModel = makeReservationViewModel;
        _hotel = hotel;
        _reservationViewNavigationService = reservationViewNavigationService;
    }

    public override void Execute(object? parameter)
    {
        // ...

        try
        {
            // ...

            // 현재 NavigationService가 가진 NavigationStore의 CurrentViewModel에 맞는 View를 렌더링
            _reservationViewNavigationService.Navigate();
        }
            catch (ReservationConflictException ex)
        {
            // ...
        }
    }
}
```

각 하위 ViewModel에게 Command들을 할당해준다. 

```csharp
// ViewModels/MakeReservationViewModel.cs

public class MakeReservationViewModel : ViewModelBase
{
    // ...
		
    // Submit 버튼과 Cancel 버튼에 연결된 Command
    public ICommand SubmitCommand { get; }
    public ICommand CancelCommand { get; }
		
    public MakeReservationViewModel(Hotel hotel, NavigationService reservationViewNavigationService)
    {
        // 각 Command에 할당
        SubmitCommand = new MakeReservationCommand(this, hotel, reservationViewNavigationService);
        CancelCommand = new NavigateCommand(reservationViewNavigationService);
    }
}
```

```csharp
// ViewModels/ReservationListingViewModel.cs

public class ReservationListingViewModel : ViewModelBase
{
    // ...

    // MakeReservation 버튼에 연결된 Command
    public ICommand MakeReservationCommand { get; }

    public ReservationListingViewModel(Hotel hotel, NavigationService makeReservationNavigationService)
    {
        // ...

        // 해당 버튼의 Command에 NavigateCommand를 연결해 makeReservation 페이지로 넘어갈 수 있도록 할당
        MakeReservationCommand = new NavigateCommand(makeReservationNavigationService);

        // ...
    }

    // ...
}
```

[⬆️ Back to top](#목차)
