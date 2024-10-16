using System;
using AdminBot.Net.NetWork;

namespace AdminBot.Net.Command
{
    internal abstract class HelpCommand
    {
        private static string CommandPrefix = Program.GetConfigManager().GetCommandPrefix();

        private static Dictionary<string, string> HelpTextReference = new()
        {
            { "help" , string.Format("---------------help---------------\n指令列表：\n权限等级0：\n    {0}help       - 查看帮助\n    {0}symmet - 图片、表情对称\n    {0}titleself  - 设置自己的群头衔\n    {0}permission - 查询自己的权限等级\n    {0}listop     - 查看群管列表\n权限等级1：\n    {0}ban        - 禁言\n    {0}kick       - 踢出群\n    {0}settitle   - 设置成员的群头衔\n    {0}recall     - 撤回消息\n权限等级2：\n    {0}op         - 设置群管\n    {0}deop       - 取消群管\n权限等级3：\n    {0}setadmin   - 设置管理员\n    {0}deadmin    - 取消管理员\n    {0}enable     - 启用功能\n    {0}disable    - 禁用功能\n权限等级：\n    群员0，群管1，管理员2，后台3\n    仅可以使用权限等级不大于自身权限等级的指令\n    使用{0}help+具体指令查看使用方法\n    e.g. {0}help titleself",CommandPrefix) },
            { "symmet" , string.Format("---------------help---------------\n权限等级0\n{0}symmet - 图片、表情对称\n使用方法：{0}symmet <对称方法> [表情/图片] 或用 {0}symmet <对称方法> 回复[表情/图片]，支持上下、下上、左右、右左\ne.g. {0}symmet 上下 [图片]",CommandPrefix) },
            { "titleself" , string.Format("---------------help---------------\n权限等级0\n{0}titleself - 设置自己的群头衔\n使用方法：{0}titleself <群头衔>\ne.g. {0}titleself 菜就多练\n注意，群头衔最多6个汉字或18个英文字符，可以混排，但是容易产生乱码",CommandPrefix) },
            { "permission" , string.Format("---------------help---------------\n权限等级0\n{0}permission - 查询自己的权限等级\n使用方法：{0}permission\ne.g. {0}permission",CommandPrefix) },
            { "ban" , string.Format("---------------help---------------\n权限等级1\n{0}ban - 禁言\n使用方法：{0}ban <@成员|对应成员的QQ号> <时间(单位秒)>\ne.g. {0}ban @xxx 3600 或 {0}ban 123456789 3600\n注意，时长为0的禁言为解除禁言，仅可以禁言权限等级低于自己的成员",CommandPrefix) },
            { "kick" , string.Format("---------------help---------------\n权限等级1\n{0}kick - 踢出群\n使用方法：{0}kick <@成员|对应成员的QQ号>\ne.g. {0}kick @xxx 或 {0}kick 123456789\n注意，仅可以踢出权限等级低于自己的成员",CommandPrefix) },
            { "settitle" , string.Format("---------------help---------------\n权限等级1\n{0}settitle - 设置成员的群头衔\n使用方法：{0}settitle <@成员|对应成员的QQ号> <群头衔>\ne.g. {0}settitle @xxx 菜就多练 或 {0}settitle 123456789 菜就多练\n注意，群头衔最多6个汉字或18个英文字符，可以混排，但是容易产生乱码",CommandPrefix) },
            { "recall" , string.Format("---------------help---------------\n权限等级1\n{0}recall - 撤回消息\n使用方法：用 {0}recall 回复想撤回的消息(需要删除回复自带的at)\n注意：仅可以撤回权限等级低于自己的成员的消息",CommandPrefix) },
            { "op" , string.Format("---------------help---------------\n 权限等级2\n{0}op - 设置群管\n使用方法：{0}op <@成员|对应成员的QQ号>\ne.g. {0}op @xxx 或 {0}op 123456789",CommandPrefix) },
            { "deop" , string.Format("---------------help---------------\n权限等级2\n{0}deop - 取消群管\n使用方法：{0}deop <@成员|对应成员的QQ号>\ne.g. {0}deop @xxx 或 {0}deop 123456789",CommandPrefix) },
            { "admin" , string.Format("---------------help---------------\n权限等级3\n{0}admin - 设置管理员\n使用方法：{0}admin <@成员|对应成员的QQ号>\ne.g. {0}admin @xxx 或 {0}admin 123456789",CommandPrefix) },
            { "deadmin" , string.Format("---------------help---------------\n权限等级3\n{0}deadmin - 取消管理员\n使用方法：{0}deadmin <@成员|对应成员的QQ号>\ne.g. {0}deadmin @xxx 或 {0}deadmin 123456789",CommandPrefix) },
            { "enable" , string.Format("---------------help---------------\n权限等级3\n{0}enable - 启用指令\n使用方法：{0}enable 指令名(无前缀)\ne.g. {0}enable ban",CommandPrefix) },
            { "disable" , string.Format("---------------help---------------\n权限等级3\n{0}disable - 禁用指令\n使用方法：{0}disable 指令名(无前缀)\ne.g. {0}disable ban",CommandPrefix) },
            //{ "" , string.Format("",CommandPrefix) },
        };
        public static bool PrintHelpText(long GroupId,string Command)
        {
            if (HelpTextReference.TryGetValue(Command,out var txt))
            {
                HttpApi.SendPlainMsg(GroupId, txt);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
