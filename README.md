## 什么是 Anet

Anet 是一个 .NET Core 通用框架，特点是简单易用。它的定义是：

> A .NET Core Common Lib, Framework and Boilerplate.

它的取名正是来自于这个定义的前面四个字母：ANET。Anet 的宗旨是使 .NET 项目开发变得简单和快速。它适用于面向现代化微服务开发 WebAPI、服务程序和网站。

## 为什么选择 Anet

很多传统的 .NET 开源框架模板（比如 ABP）都比较重，学习成本高，使用起来条条框框，比较麻烦。而 Anet 就简单易用得多，尤其适合面向微服务快速开发。

和其它模板框架一样，Anet 封装了一些实用工具类，集成了轻量 ORM 框架 Dapper。但 Anet 对 Dapper 做了一些改进，使得事务可以放在业务层独立处理，数据访问层则不需要关心事务。

## Anet 的使用

下面贴一些 Anet 的使用示例，这些示例代码都可以在本仓库中找到。

使用前先安装 Nuget 包：

```bash
Install-Package Anet
# 或者
dotnet add package Anet
```

### 1. 查询操作

```csharp
public class UserService : ServiceBase
{
    public UserService(Db db) : base(db)
    {
    }

    public async Task<PagedResult<UserDto>> GetAsync(int page, int size, string keyword = null)
    {
        var param = new SqlParams();
        var sql = Sql.Select().From("AnetUser").Where();

        if (!string.IsNullOrEmpty(keyword))
        {
            sql.AndLike("Name");
            param.Add("Name", $"'%{keyword}%'");
        }

        sql.OrderBy("Id").Page(page, size);

        var test = sql.Count();

        var result = new PagedResult<UserDto>(page, size);
        result.Items = await Db.QueryAsync<UserDto>(sql, param);
        result.Total = await Db.QuerySingleAsync<int>(sql.Count(), param);

        return result;
    }
    
    public Task<UserDto> GetByIdAsync(long id)
    {
        var param = new { Id = id };
        var sql = Sql.Select("AnetUser", param);
        return Db.QueryFirstOrDefaultAsync<UserDto>(sql, param);
    }
}
```

### 2. 新增操作

```csharp
public class UserService : ServiceBase
{
    public UserService(Db db) : base(db)
    {
    }

    public async Task CreateAsync(UserEditDto dto)
    {
        var newUser = new AnetUser { UserName = dto.UserName };

        using var tran = Db.BeginTransaction();

        await Db.InsertAsync(newUser);

        // Other business logic code.

        tran.Commit();
    }
}
```

### 3. 更新操作

```csharp
public class UserService : ServiceBase
{
    public UserService(Db db) : base(db)
    {
    }

    public async Task UpdateAsync(long userId, UserEditDto dto)
    {
        var user = await Db.FindAsync<AnetUser>(new { Id = userId });
        if (user == null)
            throw new NotFoundException();

        using var tran = Db.BeginTransaction();

        user.UserName = dto.UserName;

        await Db.UpdateAsync(user);

        // Other business logic code.

        tran.Commit();
    }
}

```

### 4. 删除操作

```csharp
public class UserService : ServiceBase
{
    public UserService(Db db) : base(db)
    {
    }

    public async Task DeleteAsync(long id)
    {
        var rows = await Db.DeleteAsync("AnetUser", new { Id = id });
        if (rows == 0)
            throw new NotFoundException();
    }
}
```

### 5. 定时任务

Anet 封装了一个 Scheduler，它可以满足大部分任务调度的需求。下面演示如何通过 Anet 来实现一个简单任务轮循程序，模拟一个发送消息的任务调度服务。这个示例也可以在 GitHub 仓库中找到源代码。

首先创建一个 Console（.NET Core）应用，需要先安装 Anet 的两个包：

```bash
Install-Package Anet
Install-Package Anet.Job
```

要添加一个定时任务就添加一个 IJob 接口的实现。这里添加一个 MessageJob 类，使它实现 IJob 接口，代码如下：

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
        // 模拟异步发送消息
        return Task.Run(() =>
        {
            _logger.LogInformation("正在发送消息...");
            Thread.Sleep(3000);
            _logger.LogInformation("消息发送成功。");
        });
    }

    public Task OnExceptionAsync(Exception ex)
    {
        _logger.LogError(ex,"发送消息出错。");
        return Task.FromResult(0);
    }
}
```

你要关心的就是 `ExecuteAsync` 方法，把你的执行代码放在此方法中。

然后只需在 Program.cs 的入口 Main 方法中进行初始化和配置即可，例如：

```csharp
// 初始化应用
App.Init((config, services) =>
{
    // 绑定配置
    Settings = new SettingsModel();
    config.Bind(Settings);

    // 注册服务
    services.AddTransient<MessageJob>();
});

var logger = App.ServiceProvider.GetRequiredService<ILogger<Program>>();

logger.LogInformation("程序已启动。");

// 启动定时轮循任务
Scheduler.StartNew<MessageJob>(Settings.JobIntervalSeconds);

logger.LogInformation("已启动消息发送任务处理程序。");

// 等待程序关闭
Scheduler.WaitForShutdown();
```

一个简单的消息发送服务就做好了，每隔指定秒数就会执行发送任务。此外，你还可以用 Scheduler.StartNewAt 方法进一步实现定时需求：

```csharp
// 等同于 Scheduler.StartNew<MessageJob>(Settings.JobIntervalSeconds);
Scheduler.StartNewAt<MessageJob>(DateTime.Now, TimeSpan.FromSeconds(Settings.JobIntervalSeconds), true);

// 10秒钟后开启任务, 且只执行一次
Scheduler.StartNewAt<MessageJob>(DateTime.Now.AddSeconds(10), TimeSpan.Zero);

// 每天1点执行任务
// 第三个参数默认为 true, 指定当到达开始时间时, 是否立即执行一次任务
Scheduler.StartNewAt<MessageJob>(DateTime.Today.AddHours(1), TimeSpan.FromDays(1), false);

// 此任务无法执行, 因为10秒钟后不立即执行且不继续进行, 会抛出 ArgumentException 异常
// Scheduler.StartNewAt<MessageJob>(DateTime.Now.AddSeconds(10), TimeSpan.Zero, false);
```

运行后在控制台看到的效果是：

![](https://i.imgur.com/plVdQD2.png)

这个示例包含了记录日志，控制台上的信息都是临时的，你也可以查看运行目录下的 logs 文件夹中的日志文件。完整代码请在本仓库查看。

## Anet 的目前状态

Anet 才刚起步，处在最小可用状态。它目前只是一个通用库，封装了一些常用的类（比如基于 Snowflake 算法的 Id 生成器、用户密码加密等），还算不上框架，还有很多事情要做。后面我也会写一些文章介绍这个项目。

但一个人的力量终究是有限的，特别希望大家能加入到这个项目中和我一起开发。

## 贡献者

Thanks goes to these wonderful people:

| [<img src="https://avatars2.githubusercontent.com/u/5000396?v=4" width="100px;"/><br/>Liam Wang](https://github.com/liamwang) | [<img src="https://avatars2.githubusercontent.com/u/12966814?v=4" width="100px;"/><br/>Zae](https://github.com/Zaeworks) |
| :---------------------------------------------------------------------------------------------------------------------------: | :----------------------------------------------------------------------------------------------------------------------: |


