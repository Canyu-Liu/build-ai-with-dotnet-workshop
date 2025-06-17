# 完整聊天应用 - 参考实现

这是一个集成了所有 workshop 内容的完整聊天应用示例，包含了所有学到的功能：系统提示词、对话历史、流式响应等。

## 🎯 功能特性

- ✅ 多种 AI 角色选择
- ✅ 智能对话历史管理
- ✅ 实时流式响应
- ✅ 丰富的用户交互功能
- ✅ 错误处理和恢复
- ✅ 美观的控制台界面

## 💻 完整代码

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

// AI 角色配置
var aiRoles = new Dictionary<string, (string Name, string Prompt, ConsoleColor Color)>
{
    ["1"] = ("专业助手", "你是一个专业、严谨的 AI 助手。回答问题时要准确、详细，使用正式的语言风格。", ConsoleColor.Blue),
    ["2"] = ("幽默朋友", "你是一个幽默风趣的朋友。回答问题时要轻松愉快，可以使用表情符号和俏皮的语言。", ConsoleColor.Green),
    ["3"] = ("代码导师", "你是一个经验丰富的编程导师。专注于提供清晰的代码解释和最佳实践建议。用代码示例说明概念。", ConsoleColor.Cyan),
    ["4"] = ("创意写手", "你是一个富有创意的写手。善于用生动的语言和比喻来表达想法，文风优美富有感染力。", ConsoleColor.Magenta),
    ["5"] = ("学习伙伴", "你是一个耐心的学习伙伴。擅长将复杂概念分解为简单易懂的部分，循序渐进地引导学习。", ConsoleColor.Yellow)
};

Console.Clear();
ShowWelcome();

// 角色选择
var selectedRole = SelectRole(aiRoles);
var (roleName, systemPrompt, roleColor) = aiRoles[selectedRole];

Console.Clear();
ShowChatInterface(roleName, roleColor);

// 初始化对话历史
var chatHistory = new List<ChatMessage>
{
    new(ChatRole.System, systemPrompt)
};

// 主对话循环
await RunChatLoop(chatClient, chatHistory, roleName, roleColor);

// 功能函数
static void ShowWelcome()
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("╔══════════════════════════════════════════════╗");
    Console.WriteLine("║        🤖 智能 AI 聊天助手 Workshop          ║");
    Console.WriteLine("║           Microsoft.Extensions.AI            ║");
    Console.WriteLine("╚══════════════════════════════════════════════╝");
    Console.WriteLine();
    Console.ResetColor();
}

static string SelectRole(Dictionary<string, (string Name, string Prompt, ConsoleColor Color)> roles)
{
    Console.WriteLine("请选择您的 AI 助手角色：");
    Console.WriteLine();
    
    foreach (var role in roles)
    {
        Console.ForegroundColor = role.Value.Color;
        Console.WriteLine($"  {role.Key}. {role.Value.Name}");
    }
    
    Console.ResetColor();
    Console.WriteLine();
    Console.Write("请输入选择 (1-5): ");
    
    string choice;
    do
    {
        choice = Console.ReadLine() ?? "";
    } while (!roles.ContainsKey(choice));
    
    return choice;
}

static void ShowChatInterface(string roleName, ConsoleColor roleColor)
{
    Console.ForegroundColor = roleColor;
    Console.WriteLine($"🤖 已选择角色: {roleName}");
    Console.ResetColor();
    Console.WriteLine();
    Console.WriteLine("💬 开始对话！支持的命令：");
    Console.WriteLine("   • 'help' - 显示帮助信息");
    Console.WriteLine("   • 'clear' - 清除对话历史");
    Console.WriteLine("   • 'stats' - 显示对话统计");
    Console.WriteLine("   • 'quit' - 退出程序");
    Console.WriteLine();
    Console.WriteLine("─".PadRight(50, '─'));
    Console.WriteLine();
}

static async Task RunChatLoop(IChatClient client, List<ChatMessage> chatHistory, string roleName, ConsoleColor roleColor)
{
    while (true)
    {
        // 用户输入
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("您: ");
        Console.ResetColor();
        
        var userInput = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(userInput))
            continue;
            
        // 处理命令
        switch (userInput.ToLower())
        {
            case "quit":
                ShowGoodbye();
                return;
                
            case "clear":
                ClearHistory(chatHistory);
                continue;
                
            case "help":
                ShowHelp();
                continue;
                
            case "stats":
                ShowStats(chatHistory);
                continue;
        }

        try
        {
            // 添加用户消息到历史
            chatHistory.Add(new(ChatRole.User, userInput));

            // 显示 AI 响应开始
            Console.ForegroundColor = roleColor;
            Console.Write($"🤖 {roleName}: ");
            Console.ResetColor();
              // 流式响应处理
            await ProcessStreamingResponse(chatClient, chatHistory);
            
            Console.WriteLine();
            
            // 历史管理（限制长度）
            ManageHistoryLength(chatHistory);
            
        }
        catch (Exception ex)
        {
            HandleError(ex, chatHistory);
        }
    }
}

static async Task ProcessStreamingResponse(IChatClient chatClient, List<ChatMessage> chatHistory)
{
    List<ChatResponseUpdate> updates = [];
    
    try
    {
        await foreach (ChatResponseUpdate update in chatClient.GetStreamingResponseAsync(chatHistory))
        {
            Console.Write(update);
            updates.Add(update);
            
            // 添加自然的打字延迟
            await Task.Delay(Random.Shared.Next(20, 50));
        }
        
        // 使用辅助方法将更新添加到对话历史
        chatHistory.AddMessages(updates);
    }
    catch (Exception)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(" [响应中断]");
        Console.ResetColor();
        throw;
    }
}

static void ClearHistory(List<ChatMessage> chatHistory)
{
    var systemMessage = chatHistory.FirstOrDefault(m => m.Role == ChatRole.System);
    chatHistory.Clear();
    
    if (systemMessage != null)
    {
        chatHistory.Add(systemMessage);
    }
    
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("✨ 对话历史已清除！");
    Console.ResetColor();
    Console.WriteLine();
}

static void ShowHelp()
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("\n📋 帮助信息：");
    Console.WriteLine("   • 直接输入文字与 AI 对话");
    Console.WriteLine("   • 'clear' - 清除对话历史重新开始");
    Console.WriteLine("   • 'stats' - 查看当前对话统计信息");
    Console.WriteLine("   • 'help' - 显示此帮助信息");
    Console.WriteLine("   • 'quit' - 退出程序");
    Console.ResetColor();
    Console.WriteLine();
}

static void ShowStats(List<ChatMessage> chatHistory)
{
    var userMessages = chatHistory.Count(m => m.Role == ChatRole.User);
    var aiMessages = chatHistory.Count(m => m.Role == ChatRole.Assistant);
    var totalChars = chatHistory.Where(m => m.Role != ChatRole.System)
                               .Sum(m => m.Text?.Length ?? 0);
    
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\n📊 对话统计：");
    Console.WriteLine($"   • 对话轮数: {userMessages}");
    Console.WriteLine($"   • AI 响应: {aiMessages}");
    Console.WriteLine($"   • 总字符数: {totalChars}");
    Console.WriteLine($"   • 历史消息数: {chatHistory.Count}");
    Console.ResetColor();
    Console.WriteLine();
}

static void ManageHistoryLength(List<ChatMessage> chatHistory)
{
    const int maxHistoryLength = 30; // 保留最近 30 条消息
    
    if (chatHistory.Count > maxHistoryLength)
    {
        var systemMessage = chatHistory.FirstOrDefault(m => m.Role == ChatRole.System);
        var recentMessages = chatHistory.Skip(chatHistory.Count - maxHistoryLength + 1).ToList();
        
        chatHistory.Clear();
        if (systemMessage != null)
        {
            chatHistory.Add(systemMessage);
        }
        chatHistory.AddRange(recentMessages);
    }
}

static void HandleError(Exception ex, List<ChatMessage> chatHistory)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\n❌ 发生错误: {ex.Message}");
    Console.ResetColor();
    
    // 如果出错，移除刚添加的用户消息
    if (chatHistory.LastOrDefault()?.Role == ChatRole.User)
    {
        chatHistory.RemoveAt(chatHistory.Count - 1);
    }
    
    Console.WriteLine();
}

static void ShowGoodbye()
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("╔══════════════════════════════════════════════╗");
    Console.WriteLine("║               谢谢使用！再见！ 👋             ║");
    Console.WriteLine("║         感谢体验 Microsoft.Extensions.AI     ║");
    Console.WriteLine("╚══════════════════════════════════════════════╝");
    Console.ResetColor();
}
```

## 🎨 界面效果预览

```
╔══════════════════════════════════════════════╗
║        🤖 智能 AI 聊天助手 Workshop          ║
║           Microsoft.Extensions.AI            ║
╚══════════════════════════════════════════════╝

请选择您的 AI 助手角色：

  1. 专业助手
  2. 幽默朋友
  3. 代码导师
  4. 创意写手
  5. 学习伙伴

请输入选择 (1-5): 3

🤖 已选择角色: 代码导师

💬 开始对话！支持的命令：
   • 'help' - 显示帮助信息
   • 'clear' - 清除对话历史
   • 'stats' - 显示对话统计
   • 'quit' - 退出程序

──────────────────────────────────────────────

您: 如何学习 C#？
🤖 代码导师: 学习 C# 是一个很棒的选择！让我为你制定一个循序渐进的学习路径...
```

## 🛠️ 主要特性说明

### 1. 多角色支持
- 5 种不同性格的 AI 角色
- 每个角色有独特的颜色主题
- 专门设计的系统提示词

### 2. 智能对话管理
- 自动历史长度限制
- 对话统计功能
- 历史清除功能

### 3. 增强的用户体验
- 彩色控制台输出
- 自然的打字效果
- 丰富的命令支持
- 美观的界面设计

### 4. 错误处理
- 优雅的错误恢复
- 网络中断处理
- 状态回滚机制

## 🚀 运行应用

1. 确保已配置 API 密钥：
```bash
dotnet user-secrets set AZURE_OPENAI_ENDPOINT <your-endpoint>
dotnet user-secrets set AZURE_OPENAI_GPT_NAME <your-model-name>
```

2. 运行应用：
```bash
dotnet run
```

3. 选择角色并开始聊天！

## 🏗️ 进一步扩展

这个完整示例为进一步扩展提供了良好的基础。您可以添加：

1. **配置文件支持**: 保存用户偏好设置
2. **对话导出**: 将对话保存为文件
3. **多语言支持**: 支持不同语言的交互
4. **插件系统**: 支持自定义功能插件
5. **Web 界面**: 构建 Web 版本的聊天应用

## 🎉 恭喜完成 Workshop！

您已经成功掌握了使用 Microsoft.Extensions.AI 构建智能聊天应用的核心技能：

- ✅ **IChatClient 基础使用** - 掌握了基本的 AI 客户端操作
- ✅ **系统提示词设计** - 学会了如何控制 AI 的行为和风格
- ✅ **对话历史管理** - 实现了智能的多轮对话功能
- ✅ **流式响应处理** - 提供了流畅的用户交互体验
- ✅ **完整应用集成** - 构建了一个功能完整的聊天应用

现在您可以将这些技能应用到自己的项目中，构建更加复杂和强大的 AI 应用！

---

**🌟 下一步建议**: 尝试将这个控制台应用改造为 Web 应用，或者集成到您现有的项目中！
