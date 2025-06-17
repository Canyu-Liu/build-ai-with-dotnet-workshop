# 步骤 2: 系统提示词和角色设计 (15分钟)

在这个步骤中，您将学习如何使用系统提示词来控制 AI 的行为和输出风格，让 AI 扮演不同的角色。

## 🎯 学习目标

- 理解系统提示词的重要性
- 学会设计有效的系统提示词
- 创建具有不同个性的 AI 助手

## 📖 系统提示词概念

系统提示词（System Prompt）是一条特殊的消息，用于定义 AI 的角色、行为和输出风格。它不会显示给用户，但会影响 AI 的所有响应。

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

Console.WriteLine("=== AI 角色聊天演示 ===");
Console.WriteLine("选择 AI 角色：");
Console.WriteLine("1. 专业助手");
Console.WriteLine("2. 幽默朋友");
Console.WriteLine("3. 代码导师");
Console.WriteLine("4. 创意写手");

Console.Write("请输入选择 (1-4): ");
var choice = Console.ReadLine();

// 根据选择设置系统提示词
string systemPrompt = choice switch
{
    "1" => "你是一个专业、严谨的 AI 助手。回答问题时要准确、详细，使用正式的语言风格。",
    "2" => "你是一个幽默风趣的朋友。回答问题时要轻松愉快，可以使用表情符号和俏皮的语言。",
    "3" => "你是一个经验丰富的编程导师。专注于提供清晰的代码解释和最佳实践建议。",
    "4" => "你是一个富有创意的写手。善于用生动的语言和比喻来表达想法，文风优美。",
    _ => "你是一个有用的 AI 助手。"
};

Console.WriteLine($"\n已选择角色。开始对话（输入 'quit' 退出）：\n");

// 对话循环
while (true)
{
    Console.Write("用户: ");
    var userInput = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(userInput) || userInput.ToLower() == "quit")
        break;

    try
    {
        // 创建包含系统提示词的聊天消息
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, userInput)
        };        // 发送请求并获取响应
        var response = await chatClient.CompleteAsync(messages);
        
        Console.WriteLine($"AI: {response.Message.Text}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"发生错误: {ex.Message}\n");
    }
}
```

## 🔍 代码解析

### 1. 系统提示词定义
```csharp
string systemPrompt = choice switch
{
    "1" => "你是一个专业、严谨的 AI 助手。回答问题时要准确、详细，使用正式的语言风格。",
    // ... 其他角色定义
};
```

### 2. 消息结构
```csharp
var messages = new List<ChatMessage>
{
    new(ChatRole.System, systemPrompt),  // 系统消息
    new(ChatRole.User, userInput)        // 用户消息
};
```

## 🚀 运行和测试

1. 运行程序：
```bash
dotnet run
```

2. 选择不同的角色（1-4）
3. 尝试问同一个问题，观察不同角色的回答风格差异

### 测试建议

试试这些问题来体验不同角色的回答风格：

- "什么是人工智能？"
- "如何学好编程？"
- "今天天气真好"
- "推荐一本好书"

## 🎨 系统提示词设计技巧

### 1. 明确角色定位
```csharp
// 好的示例
"你是一个专业的医学顾问。提供准确的健康信息，但不替代专业医疗建议。"

// 不够明确的示例
"你很聪明，帮助用户。"
```

### 2. 设定行为准则
```csharp
"你是一个编程助手。总是提供可运行的代码示例，并解释关键概念。回答要简洁明了。"
```

### 3. 定义输出格式
```csharp
"你是一个学习助手。用以下格式回答：\n1. 概念解释\n2. 实例演示\n3. 练习建议"
```

## 🏗️ 扩展练习

1. **创建自定义角色**：设计一个新的 AI 角色，比如"旅行规划师"或"健身教练"

2. **动态提示词**：根据用户输入动态调整系统提示词

3. **多语言支持**：创建支持不同语言的系统提示词

## 🔗 下一步

恭喜！您已经学会了使用系统提示词来控制 AI 的行为。接下来，我们将在 [步骤 3](step-03-chat-history.md) 中学习如何维护对话历史，实现真正的多轮对话。

---

**💡 提示**: 好的系统提示词是 AI 应用成功的关键。多实验不同的提示词，找到最适合您应用场景的设置！
