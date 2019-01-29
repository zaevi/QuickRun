# QuickRun
绝赞WIP

和所有快捷启动工具一样, 你可以将程序/文件/网页添加进QuickRun中, 配置好启动Uri和各项参数, 在需要的时候点一下就能启动. 另外, 它还有以下特性:
* 支持拖放数据启动
* 可自定制界面样式
* 允许开发插件, 实现你自己想要的功能

目前版本: 0.7.4.11 - [更新日志](ChangeLog.md)

## 程序清单
- QuickRun.exe - 启动器
- QuickRun.Plugin.dll - 插件接口
- QuickRun.Extension.dll - 启动器相关的扩展插件, 包含模板导出等功能
- QuickRun.Setting.exe - 编辑器, 用于可视化编辑配置

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
<Item>
  <Item Name="QuickRun选项" Plugin="QuickRun.Extension">
    <Item Name="打开编辑器" Uri="QuickRun.Setting.exe"/>
    <Item Name="打开AppData目录" Uri="%APPDATA%\QuickRun\" />
    <Item Name="Github页面" Uri="https://github.com/Zaeworks/QuickRun" />
    <Item Name="导出样式模板" Key="$ExportStyle"/>
    <Item Name="导出配置模板" Key="$ExportDesign"/>
    <Item Name="设置开机启动" Key="$SetStartup"/>
    <Item Name="Back" Type="BackButton" />
  </Item>
</Item>
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
        <Setter Property="Padding" Value="5,8"/>
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

### 插件扩展
插件的接口引用和使用参见QuickRun.Extension例子, 只要实现了IPlugin接口, 都可以作为扩展插件执行;
加载插件需要在配置文件里设置参数Plugin, 值为插件的程序集名称, 只要在任意位置设置一次即可;
为了方便配置Item, 可以使用PluginAttribute设置插件Key.

程序扩展插件的例子:
```csharp
// ...
using QuickRun.Plugin; // 引用QuickRun.Extension程序集

namespace QuickRun.Extension
{
    // 设置插件Key和描述, 用于方便Item配置参数
    [Plugin("$SetStartup", "设置开机启动")]
    public class SetStartupPlugin : IPlugin // 实现IPlugin接口
    {
        // ...

        public void Execute() // 实现Execute()方法
        {
            // ...
        }
    }
}
```

在配置文件中配置插件:
```xml
<Item>
  <!-- 设置Plugin参数加载插件 -->
  <Item Name="QuickRun选项" Plugin="QuickRun.Extension">
    <!-- 使用刚才设置的Key配置插件功能 -->
    <Item Name="设置开机启动" Key="$SetStartup"/>
    <Item Name="导出样式模板" Key="$ExportStyle"/>
    <Item Name="导出配置模板" Key="$ExportDesign"/>
  </Item>
</Item>
```

### 非标准插件
对于仅调用或仅传递文件拖拽数据的插件实现, 可以不依赖QuickRun.Plugin接口, 而是用特定命名来实现插件对接:
```csharp
// ...
namespace MyNameSpace
{
    [DisplayName("$MyKey")] // 使用DisplayName标记, Key="$MyKey"
    public class MyTestQuickRun // 也可以命名为MyTestQuickRunPlugin
    {
        public static void Execute() // 实现Execute()方法
        {
            // ...
        }

        // 其它四种实现 filenames:拖拽文件数据, arguments:Item的Arguments参数
        // public static void Execute(string[] filenames);
        // public static void Execute(string arguments);
        // public static void Execute(string arguments, string[] filenames);
        // public static void Execute(string[] filenames, string arguments);
    }

    // 不使用DisplayName标记, Key="$MyNameSpace.MySecond"
    // 即此类型的FullName去掉最后的QuickRun(Plugin)
    public class MySecondQuickRunPlugin
    {
        // ...
    }
}
```

## Todo
- 写Wiki
- 悬浮窗支持(针对拖拽)
- 高级启动参数
- 编辑器插件化
- 样式进一步支持
