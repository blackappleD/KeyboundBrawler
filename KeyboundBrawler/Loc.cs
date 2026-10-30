using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.ManagedFontAtlas;

namespace KeyboundBrawler;

public enum Language { Auto, English, ChineseSimplified }

// English text is the key, so anything without a translation simply shows in English.
// Game names (rewards, casts, MGC) stay English: they match what the client shows and what we parse.
public static class Loc
{
    public static bool Chinese { get; private set; }

    public static void Apply(Language language) => Chinese = language switch
    {
        Language.English => false,
        Language.ChineseSimplified => true,
        _ => Plugin.PluginInterface.UiLanguage is "zh" or "tw",
    };

    public static string T(string en) => Chinese && Zh.TryGetValue(en, out var zh) ? zh : en;

    public static string T(string en, params object[] args) => string.Format(T(en), args);

    public static string Label(this Language language) => language switch
    {
        Language.English => "English",
        Language.ChineseSimplified => "简体中文",
        _ => T("Auto (Dalamud language)"),
    };

    // Dalamud only loads Chinese glyphs when its own UI is Chinese, so the window merges in the ones we use.
    public static ushort[] GlyphRanges() =>
        FontAtlasBuildToolkitUtilities.ToGlyphRange(string.Concat(Zh.Values.Append(Language.ChineseSimplified.Label())));

    private static readonly Dictionary<string, string> Zh = new(StringComparer.Ordinal)
    {
        // Header and typing card
        ["Typing for you"] = "正在替你打字",
        ["Ready, waiting for a match"] = "已就绪，等待对局开始",
        ["Auto-play is off"] = "自动打字已关闭",
        ["In a Keybound Brawler match"] = "正在 Keybound Brawler 对局中",
        ["At the Gold Saucer"] = "在金碟游乐场",
        ["On screen: "] = "屏幕上：",
        ["Press Start, or tick \"Start when a match begins\"."] = "点击开始，或勾选“对局开始时自动开始”。",
        ["Typing"] = "正在输入",
        ["Tick \"Start when a match begins\" and queue up."] = "勾选“对局开始时自动开始”后去排队。",
        ["custom"] = "自定义",
        ["Human"] = "常人",
        ["Monkey"] = "猴子",
        ["Custom"] = "自定义",
        ["Stop typing"] = "停止打字",
        ["Start typing"] = "开始打字",
        ["Start when a match begins"] = "对局开始时自动开始",
        ["Skip ? ! - '"] = "跳过 ? ! - '",

        // MGC progress
        ["{0} MGC. Tick Want on rewards to set a goal."] = "{0} MGC。在奖励里勾选“想要”来设定目标。",
        ["Next: {0}"] = "下一个：{0}",
        ["You can buy it now."] = "现在就能买了。",
        ["{0} MGC to go, about {1} runs."] = "还差 {0} MGC，大约 {1} 局。",
        ["{0} MGC to go."] = "还差 {0} MGC。",
        ["{0} Wish list: {1} rewards, {2} MGC."] = "{0} 愿望单：{1} 件奖励，共 {2} MGC。",

        // Sections
        ["Rewards"] = "奖励",
        ["Play rounds"] = "连续对局",
        ["Word log"] = "单词记录",
        ["Settings"] = "设置",

        // Play rounds
        ["Stop (round {0} of {1})"] = "停止（第 {0}/{1} 局）",
        ["rounds"] = "局",
        ["Play {0} rounds"] = "连续打 {0} 局",
        ["Needs the vnavmesh plugin to walk to the Keybound Registrar."] = "需要 vnavmesh 插件才能走到 Keybound Registrar 处。",
        ["Go to the Gold Saucer event area first."] = "请先前往金碟游乐场的活动区域。",
        ["Stopped after {0} of {1} rounds."] = "已停止，共完成 {0}/{1} 局。",
        ["Talking to the entry NPC"] = "正在与报名 NPC 对话",
        ["vnavmesh could not path to the entry NPC."] = "vnavmesh 无法寻路到报名 NPC 处。",
        ["Walking to the entry NPC"] = "正在走向报名 NPC",
        ["Took too long to reach the entry NPC."] = "走到报名 NPC 处耗时过长。",
        ["Could not find the entry NPC nearby."] = "附近找不到报名 NPC。",
        ["Opening the entry window"] = "正在打开报名窗口",
        ["Round {0} of {1}: playing"] = "第 {0}/{1} 局：对局中",
        ["Waiting for the match to start"] = "等待对局开始",
        ["The match did not start."] = "对局没有开始。",
        ["Match over, leaving"] = "对局结束，正在离开",
        ["Leaving the duty"] = "正在退出任务",
        ["Finished {0} rounds."] = "已完成 {0} 局。",
        ["Round {0} of {1} done, next one soon"] = "第 {0}/{1} 局已完成，即将开始下一局",
        ["Did not get back from the duty."] = "未能从任务中返回。",
        ["Stopped: {0}"] = "已停止：{0}",

        // Vendor and shop
        ["Vendor: {0}"] = "商人：{0}",
        ["Stop"] = "停止",
        ["Buy wanted rewards now"] = "立即购买想要的奖励",
        ["Walk to vendor and buy"] = "走到商人处并购买",
        ["Walk to vendor and open shop"] = "走到商人处并打开商店",
        ["Buy wanted rewards when the shop opens"] = "商店打开时自动购买想要的奖励",
        ["Needs the vnavmesh plugin to walk (talking works without it when you stand close)."] =
            "自动寻路需要 vnavmesh 插件（站在商人旁边时不装也能对话）。",
        ["Hide rewards I already have"] = "隐藏已拥有的奖励",
        ["Want"] = "想要",
        ["Got"] = "已有",
        ["Reward"] = "奖励",

        // Word log
        ["{0}% landed"] = "命中率 {0}%",
        ["nothing judged yet"] = "尚无判定",
        ["{0} hit · {1} missed · {2} dropped · {3} typos fixed · {4}"] = "命中 {0} · 未中 {1} · 丢弃 {2} · 修正错字 {3} · {4}",
        ["Clear"] = "清空",
        ["Hit: the enemy took it. Missed: still on screen after Enter. Dropped: gone before it was finished."] =
            "命中：敌人吃到了。未中：按回车后仍在屏幕上。丢弃：还没打完就消失了。",
        ["No words yet. Start auto-play in a match."] = "还没有单词。在对局中开启自动打字吧。",
        ["Time"] = "时间",
        ["Word"] = "单词",
        ["Result"] = "结果",
        ["hidden"] = "被遮挡",
        ["{0} typo fixed"] = "修正 {0} 处错字",
        ["{0} typos fixed"] = "修正 {0} 处错字",
        ["Hit"] = "命中",
        ["Missed"] = "未中",
        ["Dropped"] = "丢弃",

        // Settings
        ["held (updates from the shop)"] = "持有（打开商店时同步）",
        ["This session {0} runs / {1} MGC"] = "本次 {0} 局 / {1} MGC",
        ["All time {0} runs / {1} MGC, best run {2}"] = "累计 {0} 局 / {1} MGC，单局最高 {2}",
        ["Typing speed (moving these makes it custom)"] = "打字速度（拖动滑块会变为自定义）",
        ["Delay per letter (ms)"] = "每个字母间隔（毫秒）",
        ["Pause after Enter (ms)"] = "回车后停顿（毫秒）",
        ["Retry same word after (ms)"] = "同一单词重试间隔（毫秒）",
        ["Casts"] = "读条",
        ["Cursed Gaze / Sensory Jammer: keeps typing while the bar fills, then holds every key while it lands."] =
            "Cursed Gaze / Sensory Jammer：读条期间继续打字，生效时松开所有按键。",
        ["Also treat these casts as no-typing"] = "以下读条也视为禁止打字",
        ["Apply"] = "应用",
        ["Type enemies casting these first"] = "优先打正在读这些条的敌人",
        ["Other"] = "其他",
        ["MGC chat line (group 1 = amount)"] = "MGC 聊天消息正则（第 1 组 = 数量）",
        ["Save UI dumps during matches (for bug reports)"] = "对局中保存界面转储（用于反馈问题）",
        ["Open folder"] = "打开文件夹",
        ["Reset stats"] = "重置统计",
        ["Support on Ko-fi"] = "在 Ko-fi 上支持",
        ["Language"] = "语言",
        ["Auto (Dalamud language)"] = "自动（跟随 Dalamud）",

        // Auto-play status
        ["Off"] = "关闭",
        ["Waiting for the minigame"] = "等待小游戏开始",
        ["Paused: another text box has focus"] = "已暂停：其他输入框处于焦点",
        ["Paused: no-typing sign"] = "已暂停：禁止打字标志",
        ["Word gone, clearing what we typed"] = "单词已消失，正在清除已输入内容",
        ["No word to type"] = "没有可打的单词",
        ["Opening the input box"] = "正在打开输入框",
        ["Typing \"{0}\""] = "正在输入“{0}”",
        ["Sent \"{0}\""] = "已发送“{0}”",
        ["Clearing the input box"] = "正在清空输入框",
        ["Opening the input box (after a lost key)"] = "正在打开输入框（有按键丢失）",
        ["Auto-play on"] = "自动打字已开启",
        ["Auto-play off"] = "自动打字已关闭",
        ["Open the Keybound Brawler tracker. /kbb auto toggles auto-play."] = "打开 Keybound Brawler 助手。/kbb auto 切换自动打字。",

        // Vendor status
        ["Go to the Gold Saucer first."] = "请先前往金碟游乐场。",
        ["Talking to the vendor"] = "正在与商人对话",
        ["vnavmesh is not installed or not loaded."] = "vnavmesh 未安装或未加载。",
        ["vnavmesh is still building the map, try again in a moment."] = "vnavmesh 仍在构建地图，请稍后再试。",
        ["vnavmesh could not path to the vendor."] = "vnavmesh 无法寻路到商人处。",
        ["Walking to the vendor"] = "正在走向商人",
        ["Stopped."] = "已停止。",
        ["Took too long to reach the vendor."] = "走到商人处耗时过长。",
        ["Could not find the vendor nearby."] = "附近找不到商人。",
        ["Opening the shop"] = "正在打开商店",
        ["Shop open."] = "商店已打开。",
        ["The shop did not open."] = "商店没有打开。",

        // Shop status
        ["Nothing to buy: tick Want on some rewards."] = "没有要买的：请在奖励里勾选“想要”。",
        ["Open the MGC Trader's shop first."] = "请先打开 MGC Trader 的商店。",
        ["The shop closed."] = "商店已关闭。",
        ["Could not switch to the right tab."] = "无法切换到对应的分页。",
        ["Not enough MGC for {0} ({1}/{2})."] = "MGC 不足以购买 {0}（{1}/{2}）。",
        ["No confirmation showed up after picking the item."] = "选择物品后没有出现确认框。",
        ["More confirmations than expected for {0}."] = "购买 {0} 时出现了比预期更多的确认框。",
        ["{0} asks for an amount. Buy it by hand for now."] = "{0} 需要输入数量，请暂时手动购买。",
        ["MGC did not go down after buying {0}."] = "购买 {0} 后 MGC 没有减少。",
        ["All wanted rewards bought."] = "想要的奖励已全部购买。",
        ["Buying {0}"] = "正在购买 {0}",
        [" Saved a dump."] = " 已保存转储。",
    };
}
