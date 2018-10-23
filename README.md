# Anet

Anet 的目标是实现一个 .NET Core 通用库、通用框架和通用模板。它的定义是：

> A .NET Core Common Lib, Framework and Boilerplate.

它的取名正是来自于这句话的前面四个字母：ANET。Anet 的宗旨是打造一个简单易用的快速开发项目模板，适用于 Console 应用和 Web 应用。

这个项目还处于开发阶段，目前只封装了一些简单的功能，但已实现的部分已经可以投入使用了。

## 为什么选择 Anet?

目前很多 .NET 开源框架模板（比如 ABP）都比较重，学习成本高，使用起来很多条条框框，比较麻烦。在实际开发中，经常要做一些小服务或小工具，选这种重型的框架可能就不太合适了。而 Anet 就是为了解决这个问题，它追求的是简单易用，适合快速开发一个微服务或小工具。

## 一个简单任务轮循例子

下面演示如何通过 Anet 来实现一个简单任务轮循程序，模拟一个发送消息的任务调度服务。这个示例在 GitHub 有 Sample，我就不贴所有代码了，只选重点。

实际场景应该是这样的，每隔几秒钟从缓存队列中获取要发送的消息，调用其它接口执行发送，并且需要有日志。如果出现异常，服务不能终止或崩溃，即需要有容错机制（一般还会有重试机制等，就不在这例子中讲了）。Anet 封装了一个 JobScheduler，它可以满足大部分的这一类需求。

首先创建一个 Console（.NET Core）应用，安装 Anet 包：

```bash
dotnet add package Anet
```

然后添加一个 MessageJob 类，使它实现 IJob 接口，代码如下：

```csharp
public class MessageJob : IJob
{
    private readonly ILogger<MessageJob> _logger;
    public MessageJob(ILogger<MessageJob> logger)
    {
        _logger = logger;
    }

    public Task ExecuteAsync()
    {
        return Task.Run(() =>
        {
            // 模拟发送消息
            _logger.LogInformation("正在发送消息...");
            Thread.Sleep(3000);
            _logger.LogInformation("消息发送成功。");
        });
    }

    public Task OnExceptionAsync(Exception ex)
    {
        _logger.LogError(ex, "发送消息出错。");
        return Task.FromResult(0);
    }
}
```

每个 Console 程序都需要先调用 AnetGlobal 中的 InitConsoleApp 方法来初始化，通过这个方法内可以进行一些自定义配置。

在 Program.cs 中添编写如下代码：

```csharp
public class Program
{
    public static SettingsModel Settings { get; set; }

    static void Main(string[] args)
    {
        Console.Title = "Aet 示例 - 简单任务调度程序";

        // 初始化应用
        AnetGlobal.InitConsoleApp((config, services) =>
        {
            // 绑定配置
            Settings = new SettingsModel();
            config.Bind(Settings);

            // 注册服务
            services.AddTransient<MessageJob>();
        });

        // 1. 简单任务调度示例
        JobScheduler.StartNew<MessageJob>(Settings.JobIntervalSeconds);

        // ...（其它示例）

        // 等待 Ctrl+C 或关闭窗口
        JobScheduler.WaitForShutdown();
    }
}
```

一个简单的消息发送服务就做好了，每隔指定秒数就会执行发送任务。运行后在控制台看到的效果是：

![](https://i.imgur.com/plVdQD2.png)

这个示例包含了记录日志，控制台上的信息都是临时的，你也可以查看运行目录下的 logs 文件夹中的日志文件。完整代码请前往 GitHub 查看。

## Anet 目前状态

Anet 才刚起步，处在最小可用状态。它目前只是一个通用库，封装了一些常用的类（比如基于 Snowflake 算法的 Id 生成器、用户密码加密等），还算不上框架，还有很多事情要做。后面我也会写更多的文章介绍这个项目。

但一个人的力量终究是有限的，特别希望大家能加入到这个项目中和我一起开发。

![](https://static.xmt.cn/e416a5aea5504aa3a2abcf404e1a41b7.png)

## 贡献者

Thanks goes to these wonderful people:

| [<img src="https://avatars2.githubusercontent.com/u/5000396?v=4" width="100px;"/><br /><small>Liam Wang</small>](https://github.com/liamwang) |
| :-------------------------------------------------------------------------------------------------------------------------------------------: |

