# 步骤 4: 流式响应 (15分钟)

在这个步骤中，您将学习如何实现流式响应，让 AI 的回答像真人一样逐字显示，提供更流畅的用户体验。

## 🎯 学习目标

- 理解流式响应的优势
- 学会使用 `CompleteStreamingAsync` 方法
- 实现实时的文字显示效果
- 提升用户交互体验

## 📖 流式响应概念

流式响应（Streaming Response）让 AI 的回答逐步显示，而不是等待完整回答生成后一次性显示。这种方式：
- 减少用户等待时间
- 提供更自然的对话体验
- 让用户感觉 AI 在"思考"和"打字"

Microsoft.Extensions.AI 使用 `ChatResponseUpdate` 对象来表示响应的各个部分，这些更新共同形成完整的响应。`ChatResponseUpdate` 可以直接输出到控制台，因为它实现了适当的字符串转换。

### 简单示例

最简单的流式响应用法：

```csharp
await foreach (ChatResponseUpdate update in chatClient.GetStreamingResponseAsync("什么是 AI？"))
{
    Console.Write(update);
}
```

## 💻 代码实现

将您的 `Program.cs` 更新为以下内容：

```csharp
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Azure.AI.OpenAI;
using Azure.Identity;

IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string endpoint = config["AZURE_OPENAI_ENDPOINT"];
string deployment = config["AZURE_OPENAI_GPT_NAME"];

// 创建聊天客户端
IChatClient chatClient =
    new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential())
    .GetChatClient(deployment)
    .AsIChatClient();

Console.WriteLine("=== 流式对话助手 ===");
Console.WriteLine("AI 回答将实时逐字显示，体验更自然的对话");
Console.WriteLine("输入 'quit' 退出\n");

// 初始化对话历史
var chatHistory = new List<ChatMessage>
{
    new(ChatRole.System, "你是一个友善、乐于助人的 AI 助手。请用自然、对话式的语调回答问题。")
};

// 对话循环
while (true)
{
    Console.Write("用户: ");
    var userInput = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(userInput))
        continue;
        
    if (userInput.ToLower() == "quit")
        break;    try
    {
        // 添加用户消息到历史
        chatHistory.Add(new(ChatRole.User, userInput));

        Console.Write("AI: ");
        
        // 使用流式响应，收集所有更新
        List<ChatResponseUpdate> updates = [];
        await foreach (ChatResponseUpdate update in chatClient.GetStreamingResponseAsync(chatHistory))
        {
            Console.Write(update);
            updates.Add(update);
            
            // 添加短暂延迟以模拟更自然的打字效果
            await Task.Delay(10);
        }
        
        Console.WriteLine("\n");
        
        // 使用辅助方法将更新添加到对话历史
        chatHistory.AddMessages(updates);
        
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n发生错误: {ex.Message}\n");
        // 如果出错，移除刚添加的用户消息
        if (chatHistory.LastOrDefault()?.Role == ChatRole.User)
        {
            chatHistory.RemoveAt(chatHistory.Count - 1);
        }
    }
}

Console.WriteLine("\n对话结束。再见！");
```

## 🔍 代码解析

### 1. 流式 API 调用
```csharp
await foreach (ChatResponseUpdate update in chatClient.GetStreamingResponseAsync(chatHistory))
```

### 2. 收集并显示更新
```csharp
List<ChatResponseUpdate> updates = [];
await foreach (ChatResponseUpdate update in chatClient.GetStreamingResponseAsync(chatHistory))
{
    Console.Write(update);  // 直接输出 ChatResponseUpdate
    updates.Add(update);    // 收集所有更新
}
```

### 3. 添加到对话历史
```csharp
chatHistory.AddMessages(updates); // 使用辅助方法添加消息
```

`AddMessages` 是 Microsoft.Extensions.AI 提供的辅助方法，它会：
- 将多个 `ChatResponseUpdate` 合并为完整的 `ChatResponse`
- 从响应中提取消息并添加到对话历史中
- 自动处理消息角色和内容的组合

## 🚀 运行和测试

1. 运行程序：
```bash
dotnet run
```

2. 输入问题，观察 AI 回答的流式显示效果

### 测试建议

尝试这些类型的问题来观察流式效果：

- **长回答**: "请详细解释什么是人工智能"
- **代码示例**: "写一个 C# 的 Hello World 程序"
- **故事类**: "给我讲一个关于编程的小故事"

## 🛠️ 高级功能

### 1. 带进度指示的流式响应

```csharp
Console.Write("AI: ");
var dots = 0;
var fullResponse = "";

await foreach (var update in streamingResponse)
{
    if (!string.IsNullOrEmpty(update.Text))
    {
        // 清除进度点
        if (dots > 0)
        {
            Console.Write(new string('\b', dots));
            Console.Write(new string(' ', dots));
            Console.Write(new string('\b', dots));
            dots = 0;
        }
        
        Console.Write(update.Text);
        fullResponse += update.Text;
    }
    else
    {
        // 显示进度点
        Console.Write(".");
        dots++;
        if (dots > 3)
        {
            Console.Write(new string('\b', 3));
            Console.Write("   ");
            Console.Write(new string('\b', 3));
            dots = 0;
        }
    }
    
    await Task.Delay(10);
}
```

### 2. 可取消的流式响应

```csharp
var cancellationTokenSource = new CancellationTokenSource();

// 在另一个任务中监听 ESC 键
_ = Task.Run(() =>
{
    while (!cancellationTokenSource.Token.IsCancellationRequested)
    {
        if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
        {
            cancellationTokenSource.Cancel();
            Console.WriteLine("\n[响应已取消]");
            break;
        }
    }
});

try
{
    await foreach (var update in streamingResponse.WithCancellation(cancellationTokenSource.Token))
    {
        // 处理流式更新
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("响应被用户取消。");
}
```

### 3. 响应统计信息

```csharp
var startTime = DateTime.Now;
var characterCount = 0;

await foreach (var update in streamingResponse)
{
    if (!string.IsNullOrEmpty(update.Text))
    {
        Console.Write(update.Text);
        characterCount += update.Text.Length;
        fullResponse += update.Text;
    }
}

var duration = DateTime.Now - startTime;
Console.WriteLine($"\n[响应完成: {characterCount} 字符，耗时 {duration.TotalSeconds:F1} 秒]");
```

## 🎨 用户体验优化

### 1. 响应状态指示
```csharp
Console.Write("AI 正在思考");
// 显示流式内容
Console.WriteLine(" ✓");
```

### 2. 错误处理优化
```csharp
try
{
    await foreach (var update in streamingResponse)
    {
        // 处理更新
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("\n[响应被取消]");
}
catch (Exception ex)
{
    Console.WriteLine($"\n[流式响应错误: {ex.Message}]");
}
```

### 3. 打字机效果调优
```csharp
// 根据内容类型调整延迟
var delay = update.Text.Contains('\n') ? 50 : 10; // 换行处稍微停顿
await Task.Delay(delay);
```

## 📊 流式响应 vs 普通响应

| 特性 | 普通响应 | 流式响应 |
|------|----------|----------|
| 显示方式 | 一次性显示完整回答 | 逐字实时显示 |
| 等待时间 | 长（需等待完整响应） | 短（立即开始显示） |
| 用户体验 | 可能感觉卡顿 | 更自然流畅 |
| 取消能力 | 难以中途取消 | 可以随时取消 |
| 网络要求 | 需要完整下载 | 边接收边显示 |

## 🏗️ 扩展练习

1. **打字音效**: 添加打字声音效果
2. **多色显示**: 为不同类型的内容使用不同颜色
3. **响应保存**: 将流式响应保存到文件
4. **响应回放**: 重放之前的流式响应

## 🔗 完成 Workshop

恭喜！您已经完成了所有核心步骤，学会了：
- ✅ 基础聊天功能
- ✅ 系统提示词设计
- ✅ 对话历史管理
- ✅ 流式响应实现

现在可以查看 [完整聊天应用](complete-chat-app.md) 来了解如何将所有功能整合到一个完整的应用中。

---

**🎉 恭喜！您已经掌握了使用 Microsoft.Extensions.AI 构建现代聊天应用的核心技能！**

**💡 提示**: 流式响应是现代 AI 应用的标配功能，它能显著提升用户体验。在实际项目中，建议默认使用流式响应。
