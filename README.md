# Build Ai with .Net Workshop

欢迎参加 Build Ai with .Net Workshop！这个仓库将引导您一步步学习如何使用 Microsoft.Extensions.AI 库构建一个聊天应用程序。

本项目由 Copilot 辅助完成，代码可能存在错误或不完整，请根据实际情况进行调整。

## 🎯 Workshop 目标

通过本 workshop，您将学习到：

1. **Microsoft.Extensions.AI 核心概念** - 理解统一的 AI 服务抽象
2. **IChatClient 接口使用** - 掌握聊天客户端的基本操作
3. **系统提示词设计** - 控制 AI 的行为和角色
4. **对话历史管理** - 实现多轮对话
5. **流式响应处理** - 实现实时聊天体验
6. **完整聊天应用** - 构建一个简洁而功能完整的聊天应用

## 🚀 快速开始

### 前置要求

- .NET 8.0 或更高版本
- Visual Studio 2022 或 Visual Studio Code
- Azure OpenAI 服务密钥 或 GitHub Models API 密钥

### 环境配置

1. 克隆或下载本项目
2. 配置用户机密（User Secrets）:

```bash
dotnet user-secrets init
dotnet user-secrets set AZURE_OPENAI_ENDPOINT <your-Azure-OpenAI-endpoint>
dotnet user-secrets set AZURE_OPENAI_GPT_NAME <your-Azure-OpenAI-model-name>
```

3. 运行初始示例：

```bash
dotnet run
```

## 📚 Workshop 结构

本 workshop 分为以下步骤，专为 1 小时实践设计：

### [步骤 1: 基础聊天客户端 (15分钟)](examples/step-01-basic-chat.md)
- 学习 IChatClient 基本用法
- 发送简单的聊天请求
- 处理响应结果

### [步骤 2: 系统提示词和角色设计 (15分钟)](examples/step-02-system-prompts.md)
- 设计有效的系统提示词
- 创建不同角色的 AI 助手
- 控制 AI 行为和输出风格

### [步骤 3: 对话历史管理 (15分钟)](examples/step-03-chat-history.md)
- 维护对话历史记录
- 实现多轮对话功能
- 理解消息角色系统

### [步骤 4: 流式响应 (15分钟)](examples/step-04-streaming-response.md)
- 实现实时流式响应
- 处理 IAsyncEnumerable
- 提升用户体验

### [完整项目: 聊天应用 (参考实现)](examples/complete-chat-app.md)
- 整合所有学习内容
- 构建完整的聊天应用
- 实现用户友好的交互

## 🔧 项目文件说明

- `Program.cs` - 保持最简示例代码，作为起点参考
- `examples/` - 包含每个步骤的详细指导
- `ChatApp.csproj` - 项目配置文件，已包含必要的 NuGet 包

## 📖 参考资源

- [Microsoft.Extensions.AI 官方文档](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai#request-a-streaming-chat-response??wt.mc_id=studentamb_255264)

## 📝 许可证

本项目采用 MIT 许可证。

---

**准备好开始了吗？从 [步骤 1](examples/step-01-basic-chat.md) 开始您的 1 小时 AI 开发之旅！** 🚀

## ⏱️ 时间安排建议

- **步骤 1 (15分钟)**: 基础聊天客户端
- **步骤 2 (15分钟)**: 系统提示词设计  
- **步骤 3 (15分钟)**: 对话历史管理
- **步骤 4 (15分钟)**: 流式响应实现

每个步骤都可以独立完成，如果时间紧张，可以专注于前 3 个步骤。