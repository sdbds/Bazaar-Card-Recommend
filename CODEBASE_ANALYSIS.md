# BazaarPlusPlus 代码库分析文档

## 一、如何修改代码并编译成 DLL

### 问题：`.csproj` 是旧式格式，Mac 下路径全错

这个 `.csproj` 使用的是 VS2010 旧格式（`ToolsVersion="4.0"`），引用路径全部硬编码为 Windows 路径。**在 Mac 上无法直接 `dotnet build`**。

### 推荐方案：迁移到 SDK 风格的 `.csproj`

**步骤 1：确认游戏 DLL 的实际路径**

Mac 上 Steam 游戏通常在：
```
~/Library/Application Support/Steam/steamapps/common/The Bazaar/
```

**步骤 2：将 `.csproj` 改成 SDK 风格**

用以下内容替换 `BazaarPlusPlus.csproj`（将 `GamePath` 改成实际路径）：

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework>
    <AssemblyName>BazaarPlusPlus</AssemblyName>
    <RootNamespace>BazaarPlusPlus</RootNamespace>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <Nullable>enable</Nullable>
    <!-- 改成游戏实际安装路径 -->
    <GamePath>/Users/zhaozhiyan/Library/Application Support/Steam/steamapps/common/The Bazaar/TheBazaar_Data/Managed</GamePath>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="BazaarGameClient">
      <HintPath>$(GamePath)/BazaarGameClient.dll</HintPath>
    </Reference>
    <Reference Include="BazaarGameShared">
      <HintPath>$(GamePath)/BazaarGameShared.dll</HintPath>
    </Reference>
    <Reference Include="TheBazaarRuntime">
      <HintPath>$(GamePath)/TheBazaarRuntime.dll</HintPath>
    </Reference>
    <Reference Include="netstandard">
      <HintPath>$(GamePath)/netstandard.dll</HintPath>
    </Reference>
    <Reference Include="Newtonsoft.Json">
      <HintPath>$(GamePath)/Newtonsoft.Json.dll</HintPath>
    </Reference>
    <Reference Include="UnityEngine.CoreModule">
      <HintPath>$(GamePath)/UnityEngine.CoreModule.dll</HintPath>
    </Reference>
    <Reference Include="UnityEngine.UI">
      <HintPath>$(GamePath)/UnityEngine.UI.dll</HintPath>
    </Reference>
    <Reference Include="Unity.InputSystem">
      <HintPath>$(GamePath)/Unity.InputSystem.dll</HintPath>
    </Reference>
    <Reference Include="UnityEngine.PhysicsModule">
      <HintPath>$(GamePath)/UnityEngine.PhysicsModule.dll</HintPath>
    </Reference>
    <Reference Include="0Harmony">
      <HintPath>$(GamePath)/../../BepInEx/core/0Harmony.dll</HintPath>
    </Reference>
    <Reference Include="BepInEx">
      <HintPath>$(GamePath)/../../BepInEx/core/BepInEx.dll</HintPath>
    </Reference>
    <Reference Include="UnityEngine.IMGUIModule">
      <HintPath>$(GamePath)/UnityEngine.IMGUIModule.dll</HintPath>
    </Reference>
    <Reference Include="UnityEngine.TextRenderingModule">
      <HintPath>$(GamePath)/UnityEngine.TextRenderingModule.dll</HintPath>
    </Reference>
    <Reference Include="UnityEngine.UIModule">
      <HintPath>$(GamePath)/UnityEngine.UIModule.dll</HintPath>
    </Reference>
  </ItemGroup>

  <ItemGroup>
    <EmbeddedResource Include="Data/monsters_bazaardb.json" />
  </ItemGroup>
</Project>
```

**步骤 3：编译**

```bash
cd /Users/zhaozhiyan/windows-home/MAC/bazaar/BazaarPlusPlus
dotnet build -c Release
# 输出在 bin/Release/netstandard2.1/BazaarPlusPlus.dll
```

**步骤 4：部署**

将编译出的 DLL 复制到游戏的 BepInEx 插件目录：
```
The Bazaar/BepInEx/plugins/BazaarPlusPlus/BazaarPlusPlus.dll
```

---

## 二、工程整体架构

```
Plugin.cs（插件入口）
    ├── HarmonyPatch（方法补丁层）
    │   ├── CombatSim 相关（速度、帧数、模拟控制）
    │   └── Showcase/Tooltip 相关（交互拦截、预览注入）
    │
    ├── ModState（全局状态）
    │   ├── 运行周期监听（RunStarted/Ended/Interrupted）
    │   └── 遭遇战数据缓存
    │
    ├── CombatStatusBar（战斗 HUD）
    │   └── 战斗速度、时间、帧数显示 + 控制按钮
    │
    ├── MonsterPreviewController（怪物预览 UI）
    │   └── MonsterPreviewBoard（3D 棋盘渲染）
    │       └── MonsterPreviewOverlayCoordinator（定位协调）
    │
    ├── MonsterDatabase（数据层）
    │   └── monsters_bazaardb.json（嵌入 JSON）
    │
    ├── EncounterTracker（游戏事件监听）
    │   └── CardDealtSimEvent 监听 -> 更新遭遇战选项
    │
    ├── GameDataReader（游戏状态读取）
    │   └── 构建 RunInfo 快照
    │
    └── ItemEnchantPreviewService（附魔预览）
        └── 为物品 Tooltip 添加附魔后数值预览
```

核心价值链：
```
游戏事件触发
    → EncounterTracker 捕获遭遇战选项
    → MonsterDatabase 查询怪物数据
    → MonsterPreviewBoard 渲染3D棋盘
    → 玩家看到对手装备布局，辅助决策
```

---

## 三、各部分代码功能说明

### 3.1 插件入口层

| 文件 | 作用 |
|------|------|
| `Plugin.cs` | BepInEx 插件主类，`Awake()` 中初始化所有模块、注册所有 HarmonyPatch |
| `MyPluginInfo.cs` | 插件 GUID/名称/版本常量（当前版本 1.1.0） |
| `ModState.cs` | 全局静态状态（是否在游戏中、遭遇战数据、附魔列表等），监听游戏 Run 生命周期事件 |
| `BppLog.cs` | 日志封装，支持去重（连续重复日志合并显示） |
| `KeyBindings.cs` | 快捷键定义：F2=调试面板、F6=战斗状态栏，数字键1-4=调试页签 |

### 3.2 数据层

| 文件 | 作用 |
|------|------|
| `MonsterDatabase.cs` | 读取嵌入的 `monsters_bazaardb.json`，按 GUID/短ID 索引所有怪物 |
| `MonsterInfo.cs` | 怪物数据模型（ID、名称、等级、血量、奖励、板面卡牌） |
| `MonsterBoardCardInfo.cs` | 怪物棋盘上单张卡牌的信息 |
| `MonsterSkillInfo.cs` | 怪物技能信息 |
| `RunInfo.cs` | 当前局游戏状态快照（玩家/对手属性、卡牌、遭遇战选项） |
| `GameDataReader.cs` | 从游戏内部对象读取数据，构建 `RunInfo` 快照 |
| `Data/monsters_bazaardb.json` | 所有怪物的静态数据库（名称、血量、奖励、装备、技能），以嵌入资源形式打包进 DLL |

### 3.3 游戏状态追踪层

| 文件 | 作用 |
|------|------|
| `EncounterTracker.cs` | 监听 `CardDealtSimEvent` 事件，解析当前遭遇战选项（Encounter/Choice/Loot/Pedestal 四种类型） |
| `RunStateSyncController.cs` | 同步游戏 RunState 到本地状态 |
| `UpdatePlayerPatch.cs` | Harmony Patch，玩家状态变化时同步数据 |

### 3.4 怪物预览 UI 层

| 文件 | 作用 |
|------|------|
| `MonsterPreviewController.cs` | 控制器，每帧在 LateUpdate 中更新预览显示状态 |
| `MonsterPreviewBoard.cs` | 核心：用 Unity 3D 对象构建怪物装备棋盘，10个物品槽+3个技能槽 |
| `MonsterPreviewOverlayCoordinator.cs` | 协调多个预览板的位置，避免重叠 |
| `MonsterPreviewBoardRenderTarget.cs` | 渲染目标抽象 |
| `MonsterPreviewItemCardFactory.cs` | 创建预览用的物品卡对象 |
| `MonsterPreviewSkillCardFactory.cs` | 创建预览用的技能卡对象 |
| `MonsterPreviewDefaults.cs` | 预览棋盘的默认配置参数 |
| `MonsterPreviewWarmupController.cs` | 预热控制器，提前加载资源 |
| `MonsterPreviewDebugController.cs` | 调试面板中调整预览参数的控制器 |
| `MonsterPreviewDebugTuner.cs` | 调试参数调节器 |
| `MonsterLockShowcaseController.cs` | 锁定展示控制器（查看某张卡详细状态） |
| `MonsterLockShowcaseRuntime.cs` | 锁定展示运行时状态 |

### 3.5 预览棋盘数据模型

| 文件 | 作用 |
|------|------|
| `PreviewBoardPresentation.cs` | 棋盘外观参数（大小 8.25×2.75、间距、缩放比例） |
| `PreviewBoardModel.cs` | 棋盘数据模型 |
| `PreviewBoardRequest.cs` | 请求构建一个预览棋盘 |
| `PreviewBoardSession.cs` | 一次预览会话的生命周期管理 |
| `PreviewBoardSignature.cs` | 棋盘内容签名（判断是否需要重建） |
| `PreviewCardSpec.cs` | 单张预览卡的规格（模板ID、品阶、附魔、属性字典） |
| `PreviewCardKind.cs` | 卡牌类型枚举（物品/技能） |
| `PreviewCardLifecyclePolicy.cs` | 卡牌生命周期策略 |
| `PreviewCardSpecFilter.cs` | 过滤卡牌规格 |
| `BoardPose.cs` | 棋盘位置/旋转姿态 |
| `BoardRenderModel.cs` | 棋盘渲染模型 |

### 3.6 附魔预览系统（`Game/ItemEnchantPreview/`）

当玩家查看某张物品卡的 Tooltip 时，显示附魔后的数值变化。

| 文件 | 作用 |
|------|------|
| `ItemEnchantPreviewService.cs` | 入口：为卡片 Tooltip 构建附魔预览段落 |
| `ItemEnchantPreviewEligibility.cs` | 判断该卡是否可以显示附魔预览 |
| `ItemEnchantPreviewCandidateSelector.cs` | 从当前可用附魔中筛选候选项 |
| `ItemEnchantPreviewCache.cs` | 缓存已计算的预览（避免每帧重算） |
| `ItemEnchantPreviewFormatting.cs` | 格式化预览文本 |
| `ItemEnchantPreviewSnapshot.cs` | 不可变快照（作为缓存键） |
| `ItemEnchantPreviewSnapshotFactory.cs` | 创建快照 |
| `ItemEnchantPreviewCardCloneFactory.cs` | 克隆卡牌并模拟附魔效果（不影响实际游戏） |
| `ItemEnchantPreviewRenderer.cs` | 渲染模拟后的 Tooltip 内容 |

### 3.7 HarmonyPatch 补丁层

通过 Harmony 在不修改游戏原始代码的前提下拦截/替换方法：

| 补丁文件 | 拦截目标 | 功能 |
|----------|----------|------|
| `CombatSimPatch.cs` | 战斗模拟器 Simulate | 记录总战斗帧数、判断胜负结果 |
| `CombatFrameAdvancePatch.cs` | 帧推进逻辑 | 计数已处理战斗帧 |
| `CombatSpeedPatch.cs` | 战斗速度相关方法 | 注入自定义速度倍率（0.25x~3x） |
| `CardTooltipDataPassivePatch.cs` | Tooltip 数据填充 | 注入附魔预览信息到 Tooltip |
| `CardTooltipControllerLockTogglePatch.cs` | Tooltip 锁定切换 | 拦截锁切换操作 |
| `ShowcaseCardClickPatch.cs` | `CardController.ProceedClick` | 阻止对 ShowcaseCard 的点击传递 |
| `ShowcaseCardShowTooltipsPatch.cs` | ShowTooltips 方法 | 控制 Showcase 卡片的 Tooltip 行为 |
| `ShowcaseTooltipLockBypassPatch.cs` | Tooltip 锁定逻辑 | 绕过 Showcase 卡片的 Tooltip 锁定 |
| `ShowcaseDisableLockCanvasPatch.cs` | LockCanvas 方法 | Showcase 模式下禁用锁定画布 |
| `ShowcaseShowTooltipControllerPatch.cs` | TooltipController 显示 | 控制 Showcase 的 Tooltip 控制器 |
| `PreviewBoardSurfaceBlocksUnderlyingCardsPatch.cs` | 底层卡片遮挡 | 预览棋盘覆盖区域内屏蔽底层卡片点击 |
| `SetHeroNamePatch.cs` | 英雄名称设置 | Streamer 模式下匿名化名字 |
| `UpgradePreviewTooltipPatch.cs` | 升级预览 Tooltip | 注入升级效果预览信息 |

### 3.8 辅助工具类

| 文件 | 作用 |
|------|------|
| `CombatStatusBar.cs` | 战斗 HUD：显示帧数/时间，控制速度（0.25x~3x），暂停/继续战斗 |
| `DebugPanel.cs` | 调试面板（F2 开关），显示4个页签的调试信息 |
| `DebugPanelSection.cs` | 调试面板中的单个区块 |
| `DebugPanelState.cs` | 调试面板状态管理 |
| `CardJsonPathResolver.cs` | 解析游戏 `cards.json` 配置文件路径 |
| `LocalCardTemplateCatalog.cs` | 本地卡牌模板目录（从游戏 `cards.json` 加载） |
| `EncounterPreviewSpecConverter.cs` | 将遭遇战数据转换为预览卡规格 |
| `NameOverrideHelper.cs` | Streamer 模式名称替换工具 |
| `NextClickCloseFrameGate.cs` | 下一次点击关闭帧门控（防止误触） |
| `ItemAttr.cs` | 物品属性枚举/常量定义 |
| `InMemoryPreviewDataSource.cs` | 基于内存的预览数据源（测试/调试用） |
| `MonsterDatabasePreviewDataSource.cs` | 基于怪物数据库的预览数据源 |
| `FixedAnchorStrategy.cs` | 固定锚点策略（棋盘位置固定在某处） |
| `PreviewRenderGenerationGate.cs` | 控制预览渲染的生成速率（节流） |
| `ShowcaseCardMarker.cs` | 标记某张卡为 Showcase 状态 |
| `PreviewBoardSurfaceMarker.cs` | 标记预览棋盘表面对象 |
| `ShowcaseTooltipBypass.cs` | Showcase Tooltip 绕过逻辑 |

### 3.9 编译器辅助文件（通常不需要修改）

| 文件 | 作用 |
|------|------|
| `Microsoft/CodeAnalysis/EmbeddedAttribute.cs` | 编译器内嵌属性，反编译产物 |
| `System/Runtime/CompilerServices/NullableAttribute.cs` | 可空类型注解属性 |
| `System/Runtime/CompilerServices/NullableContextAttribute.cs` | 可空上下文属性 |
| `System/Runtime/CompilerServices/RefSafetyRulesAttribute.cs` | 引用安全规则属性 |
| `System/Runtime/CompilerServices/IgnoresAccessChecksToAttribute.cs` | 跨程序集访问私有成员的特性 |
| `Properties/AssemblyInfo.cs` | 程序集元数据 |

---

## 四、卡牌 Tooltip 注入机制深度分析

### 4.1 功能说明

右键（或悬停）物品卡时，Tooltip 底部会出现 `Bazaar++` 标题下的附魔预览文字，显示"如果附魔 X，数值会变成多少"。**这套机制没有自己的外部卡牌数据库，数据完全来自游戏内存。**

### 4.2 完整数据链路

```
玩家右键/悬停物品卡
    │
    ▼
CardTooltipDataPassivePatch.Postfix()
    │  Harmony 拦截 CardTooltipData.GetPassiveTooltipBlock()
    │  拿到 __instance.CardInstance（游戏实际 Card 对象）
    │  条件：非战斗中 && 非按住Shift（Shift是升级预览模式）
    ▼
ItemEnchantPreviewService.BuildPreviewSegments(card)
    │
    ├─ ItemEnchantPreviewEligibility.IsEligible()
    │   条件过滤：
    │   • 卡类型必须是 Item（不处理技能卡）
    │   • 在 Hand 或 Stash 区域（玩家自己的卡）
    │   • 或者是对手板面上的 Item（对手卡也可预览！）
    │   • 非战斗中
    │
    ├─ card.GetEnchantments()
    │   ← 游戏 API，返回这张卡支持的所有附魔类型及其定义
    │
    ├─ 与 ModState.AvailableEnchantments 取交集
    │   ← 当前局实际可用的附魔（从游戏 RunState 读取）
    │   优先只显示"当前局可用"的附魔预览
    │
    ├─ ItemEnchantPreviewCardCloneFactory.Create()
    │   克隆原卡，修改 Enchantment 字段为目标附魔
    │   Attributes 替换为预计算的附魔后数值
    │   （完全不影响游戏实际状态）
    │
    ├─ ItemEnchantPreviewCache.TryGet(snapshot)
    │   用快照（InstanceId+TemplateId+附魔类型+属性值）作缓存键
    │   命中直接返回，未命中则走渲染流程
    │
    └─ ItemEnchantPreviewRenderer.Render(克隆卡, 附魔定义)
        ├─ 从 enchantment.Localization.Tooltips 读取文本模板
        ├─ 用反射调用游戏私有方法 CardTooltipData.RenderTooltip()
        │   传入克隆卡（已附魔状态），让游戏自己渲染出数值文字
        └─ 降级方案：直接用 TooltipBuilder + ValueContext 渲染
```

### 4.3 关键数据来源对照表

| 数据 | 来源 | 说明 |
|------|------|------|
| 卡牌对象 | `CardTooltipData.CardInstance` | 游戏内存中的实际 Card |
| 卡牌唯一标识 | `card.TemplateId`（GUID） | 对应游戏 `cards.json` 里的模板 |
| 卡牌支持的附魔 | `card.GetEnchantments()` | 游戏 API，无需外部数据 |
| 当前可用附魔 | `ModState.AvailableEnchantments` | 从游戏 RunState 读取 |
| 附魔后数值渲染 | 游戏私有方法（反射调用） | 让游戏自己算，结果完全准确 |
| 外部数据库 | **无** | monsters_bazaardb.json 只服务于怪物预览 |

### 4.4 两种 Tooltip 触发模式

| 按键状态 | 触发内容 | 实现文件 |
|----------|----------|----------|
| 普通悬停/右键 | 附魔预览（Bazaar++ 标题） | `CardTooltipDataPassivePatch` + `ItemEnchantPreviewService` |
| 按住 Shift | 升级预览（卡牌升级后数值） | `UpgradePreviewTooltipPatch`，调用 `CardController.ShowTooltips` |

### 4.5 对扩展评级系统的参考价值

如需在 Tooltip 中注入卡牌评级信息，可完全复用此机制：

1. **入口点**：在 `CardTooltipDataPassivePatch.Postfix()` 中，已能拿到 `Card` 对象
2. **卡牌主键**：`card.TemplateId`（GUID）是唯一标识，可作为评级数据的 key
3. **注入方式**：向 `__result.Item1`（StringBuilder）追加文本即可
4. **数据格式示例**：
   ```
   // 维护一个本地字典
   Dictionary<Guid, string> cardRatings = { ... };
   // 在 Postfix 里查询并追加
   if (cardRatings.TryGetValue(card.TemplateId, out var rating))
       builder.Append($"评级: {rating}\n");
   ```

---

## 五、与 DESIGN.md 目标的对应关系

| DESIGN.md 模块 | 现有实现 | 还需开发 |
|---|---|---|
| 游戏状态获取 | EncounterTracker + GameDataReader 已实现 | 可扩展更多状态字段 |
| 知识库（卡牌评级） | monsters_bazaardb.json 仅有怪物数据；玩家卡牌无外部数据库 | 需添加 TemplateId→评级 的本地字典 |
| 数据采集（B站视频） | 未实现 | 需要独立 Python 服务 |
| 推荐引擎（3选1建议） | 未实现 | 核心待开发功能 |
| 用户界面（Tooltip注入） | CardTooltipDataPassivePatch 已提供完整注入机制 | 复用该机制追加评级文本即可 |
| 用户界面（棋盘展示） | MonsterPreviewBoard 3D棋盘已实现 | 可在此基础上叠加评级/推荐信息 |
