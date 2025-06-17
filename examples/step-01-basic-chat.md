# 步骤 1: 基础聊天客户端 (15分钟)

在这个步骤中，您将学习如何使用 Microsoft.Extensions.AI 创建一个基础的聊天客户端，发送消息并接收 AI 的响应。

## 🎯 学习目标

- 理解 Microsoft.Extensions.AI 的核心接口 `IChatClient`
- 学会创建和配置 Azure OpenAI 聊天客户端
- 发送基本的聊天请求并处理响应
- 实现简单的对话循环

## 📖 核心概念

Microsoft.Extensions.AI 提供了统一的 `IChatClient` 接口，让您可以轻松地与不同的 AI 提供商进行交互，而无需学习每个提供商的特定 API。

## 💻 代码实现

将您的 `Program.cs` 更新为以下内容：

```csharp
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Azure.AI.OpenAI;
using Azure.Identity;

// 读取配置
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

Console.WriteLine("=== 基础聊天示例 ===");
Console.WriteLine("输入消息开始对话，输入 'quit' 退出程序\n");

// 简单的对话循环
while (true)
{
    Console.Write("您: ");
    var userInput = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(userInput) || userInput.ToLower() == "quit")
        break;

    try
    {
        // 发送消息并获取响应
        var response = await chatClient.CompleteAsync(userInput);
        Console.WriteLine($"AI: {response.Message.Text}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"发生错误: {ex.Message}\n");
    }
}

Console.WriteLine("再见！");
```

## 🔍 代码解析

### 1. 配置读取
```csharp
IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();
```
从用户机密中读取 Azure OpenAI 的配置信息。

### 2. 客户端创建
```csharp
IChatClient chatClient =
    new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential())
    .GetChatClient(deployment)
    .AsIChatClient();
```
- 使用 Azure OpenAI 终结点和身份验证
- 获取指定部署的聊天客户端
- 转换为统一的 `IChatClient` 接口

### 3. 发送请求
```csharp
var response = await chatClient.CompleteAsync(userInput);
Console.WriteLine($"AI: {response.Message.Text}");
```
使用 `CompleteAsync` 方法发送用户输入并获取 AI 响应。

## 🚀 运行和测试

1. 确保已配置用户机密：
```bash
dotnet user-secrets set AZURE_OPENAI_ENDPOINT <your-endpoint>
dotnet user-secrets set AZURE_OPENAI_GPT_NAME <your-model-name>
```

2. 运行程序：
```bash
dotnet run
```

3. 尝试发送不同类型的消息：
   - "你好！"
   - "什么是人工智能？"
   - "用 C# 写一个 Hello World 程序"

## 🔗 下一步

恭喜！您已经成功创建了第一个 AI 聊天客户端。接下来，我们将在 [步骤 2](step-02-system-prompts.md) 中学习如何使用系统提示词来控制 AI 的行为和角色。

---

**💡 提示**: `IChatClient` 是 Microsoft.Extensions.AI 的核心接口，掌握它的使用是构建 AI 应用的基础！
```

## ✅ 检查点

完成此步骤后，您应该能够：

- [ ] 创建和配置 `IChatClient` 实例
- [ ] 使用 `GetResponseAsync` 发送聊天请求
- [ ] 理解 `ChatResponse` 对象的结构
- [ ] 实现基本的错误处理

## ➡️ 下一步

继续学习 [步骤 2: 系统提示词设计](step-02-system-prompts.md)，了解如何控制 AI 的行为和角色。
