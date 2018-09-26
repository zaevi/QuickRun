# QuickRun
绝赞WIP

这是一个追求简洁的快捷启动器, 你可以将文件/应用程序/网页URL等添加至程序中; 它们实际上会变成一个个按钮, 这之后你可以对按钮的行为进行自定义, 同时也能对样式进行有限的修改.

目前版本: 0.6.2 - [更新日志](ChangeLog.md)

## 程序清单
- QuickRun.exe - 启动器
- QuickRun.Setting.exe - 编辑器, 用于可视化编辑配置
- design.xml - 启动项配置文件*
- styles.xaml - 样式文件*

## 使用
- [下载程序](https://github.com/Zaeworks/QuickRun/releases)
- 编辑配置文件
> 可以用编辑器新建配置, 也可以直接新建design.xml并手动编辑, 见下文
- 启动QuickRun

## 自定义
### 自定义配置
程序内置的模板配置(design.xml)如下:
```xml
<?xml version="1.0" encoding="utf-8"?>
<QuickRunSetting>
  <Item Name="QuickRun选项">
    <Item Name="打开编辑器" Uri="QuickRun.Setting.exe"/>
    <Item Name="打开配置目录" Uri="%APPDATA%\QuickRun\" />
    <Item Name="打开Github" Uri="https://github.com/Zaeworks/QuickRun" />
    <Item Name="Back" Type="BackButton" />
  </Item>
</QuickRunSetting>
```
你也可以直接在程序目录下新建design.xml并写入上面的XML配置.

> 程序加载配置文件(design.xml)时, 会先从本地(程序所在目录下)寻找, 然后从%AppData%\QuickRun目录下寻找, 最后加载内置的模板.

### 自定义样式
外置样式的寻找方式同上, 有所不同的是程序会先加载内置样式, 然后合并外置样式.

样式文件实际上是[ResourceDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.windows.resourcedictionary)的Xaml格式文件,
程序加载样式后会合并进主窗体的Resource中.

内置样式如下, 有按钮默认的Default样式:
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Style TargetType="Button" x:Key="Default" >
        <Setter Property="Padding" Value="5"/>
        <Setter Property="Background" Value="White"/>
        <Setter Property="BorderThickness" Value="0"/>
    </Style>
</ResourceDictionary>
```

你可以继承此样式做些修改, 或者添加新样式:
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <!-- 合并后直接覆盖默认样式 -->
    <Style TargetType="Button" x:Key="Default" >
        <Setter Property="Padding" Value="5,8"/>
        <Setter Property="Background" Value="White"/>
        <Setter Property="BorderThickness" Value="0"/>
    </Style>
    <!-- 基于默认样式修改 -->
    <Style TargetType="Button" x:Key="BlueText" BasedOn="{StaticResource Default}">
        <Setter Property="Foreground" Value="Blue"/>
    </Style>
    <!-- 新样式 -->
    <Style TargetType="Button" x:Key="Gray">
        <Setter Property="Background" Value="Gray"/>
    </Style>
</ResourceDictionary>
```
设置Item的Style属性为BlueText或Gray, 就会呈现出不同样式

## Todo
- 键盘操作
- 悬浮窗支持(针对拖拽)
- 内置命令(及插件?)
- 高级启动参数
- 样式进一步支持
- 写Wiki
