# 步骤 3: 对话历史管理 (15分钟)

在这个步骤中，您将学习如何维护对话历史记录，实现真正的多轮对话功能，让 AI 能够记住之前的对话内容。

## 🎯 学习目标

- 理解对话历史的重要性
- 学会维护和管理聊天消息历史
- 实现连贯的多轮对话体验

## 📖 对话历史概念

对话历史是聊天应用的核心功能之一。通过维护消息历史记录，AI 可以：
- 记住之前的对话内容
- 提供上下文相关的回答
- 实现更自然的对话体验

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

Console.WriteLine("=== 智能对话助手 ===");
Console.WriteLine("支持多轮对话，AI 会记住我们的对话历史");
Console.WriteLine("输入 'clear' 清除历史，输入 'quit' 退出\n");

// 初始化对话历史
var chatHistory = new List<ChatMessage>
{
    new(ChatRole.System, "你是一个友善、乐于助人的 AI 助手。你会记住我们对话的内容，并根据上下文提供相关回答。")
};

// 对话循环
while (true)
{
    Console.Write("用户: ");
    var userInput = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(userInput))
        continue;
        
    if (userInput.ToLower() == "quit")
        break;
        
    if (userInput.ToLower() == "clear")
    {
        // 清除历史，但保留系统提示词
        chatHistory.Clear();
        chatHistory.Add(new(ChatRole.System, "你是一个友善、乐于助人的 AI 助手。你会记住我们对话的内容，并根据上下文提供相关回答。"));
        Console.WriteLine("对话历史已清除。\n");
        continue;
    }

    try
    {
        // 添加用户消息到历史
        chatHistory.Add(new(ChatRole.User, userInput));        // 发送完整的对话历史
        var response = await chatClient.CompleteAsync(chatHistory);
        
        // 添加 AI 响应到历史
        chatHistory.Add(response.Message);
        
        Console.WriteLine($"AI: {response.Message.Text}\n");
        
        // 显示当前对话轮数
        var userMessages = chatHistory.Count(m => m.Role == ChatRole.User);
        Console.WriteLine($"[对话轮数: {userMessages}]\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"发生错误: {ex.Message}\n");
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

### 1. 对话历史初始化
```csharp
var chatHistory = new List<ChatMessage>
{
    new(ChatRole.System, "系统提示词...")
};
```

### 2. 添加消息到历史
```csharp
// 添加用户消息
chatHistory.Add(new(ChatRole.User, userInput));

// 添加 AI 响应
chatHistory.Add(response.Message);
```

### 3. 清除历史功能
```csharp
if (userInput.ToLower() == "clear")
{
    chatHistory.Clear();
    chatHistory.Add(new(ChatRole.System, "系统提示词..."));
}
```

## 🚀 运行和测试

1. 运行程序：
```bash
dotnet run
```

2. 尝试多轮对话来测试历史记忆功能

### 测试对话示例

```
用户: 我的名字是小明
AI: 你好小明！很高兴认识你...

用户: 我喜欢编程
AI: 那很棒，小明！编程是一个很有趣的领域...

用户: 你还记得我的名字吗？
AI: 当然记得！你是小明，而且你喜欢编程...
```

## 🛠️ 高级功能

### 1. 历史长度限制

为了避免消息过多影响性能，可以限制历史长度：

```csharp
// 在添加新消息后检查历史长度
const int maxHistoryLength = 20; // 保留最近 20 条消息

if (chatHistory.Count > maxHistoryLength)
{
    // 保留系统消息和最近的消息
    var systemMessage = chatHistory.First();
    var recentMessages = chatHistory.Skip(chatHistory.Count - maxHistoryLength + 1).ToList();
    
    chatHistory.Clear();
    chatHistory.Add(systemMessage);
    chatHistory.AddRange(recentMessages);
}
```

### 2. 对话摘要

对于长对话，可以使用 AI 生成摘要：

```csharp
private async Task<string> SummarizeHistory(List<ChatMessage> history)
{
    var summaryPrompt = "请简要总结以下对话的要点：\n" + 
                       string.Join("\n", history.Select(m => $"{m.Role}: {m.Text}"));
    
    var summaryResponse = await client.CompleteAsync(summaryPrompt);
    return summaryResponse.Message.Text;
}
```

## 📊 消息角色说明

Microsoft.Extensions.AI 支持以下消息角色：

- **System**: 系统提示词，定义 AI 行为
- **User**: 用户输入的消息
- **Assistant**: AI 的响应消息
- **Tool**: 工具调用相关消息（高级功能）

## 🎨 最佳实践

### 1. 合理的系统提示词
```csharp
var systemPrompt = "你是一个专业的编程助手。记住用户的偏好和之前讨论的内容，提供个性化的帮助。";
```

### 2. 错误处理
```csharp
try
{
    // API 调用
}
catch (Exception ex)
{
    // 出错时回滚历史状态
    if (chatHistory.LastOrDefault()?.Role == ChatRole.User)
    {
        chatHistory.RemoveAt(chatHistory.Count - 1);
    }
}
```

### 3. 用户体验优化
- 显示对话轮数
- 提供清除历史的选项
- 适当的提示信息

## 🏗️ 扩展练习

1. **对话保存**: 将对话历史保存到文件
2. **对话加载**: 启动时加载之前的对话
3. **多用户支持**: 为不同用户维护独立的对话历史
4. **历史搜索**: 在对话历史中搜索特定内容

## 🔗 下一步

太棒了！您已经掌握了对话历史管理。现在 AI 可以记住您的对话内容了。接下来，我们将在 [步骤 4](step-04-streaming-response.md) 中学习如何实现流式响应，让对话更加流畅自然。

---

**💡 提示**: 对话历史是构建智能聊天应用的基础。合理管理历史记录可以显著提升用户体验！
