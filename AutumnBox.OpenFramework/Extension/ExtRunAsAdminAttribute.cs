﻿/*************************************************
** auth： zsh2401@163.com
** date:  2018/8/6 2:23:33 (UTC +8:00)
** desc： ...
*************************************************/

namespace AutumnBox.OpenFramework.Extension
{
    /// <summary>
    /// 如果添加此标记,则秋之盒会保证该模块以管理员权限运行
    /// </summary>
    public class ExtRunAsAdminAttribute : ExtBeforeCreateAspectAttribute
    {
        /// <summary>
        /// 设定值
        /// </summary>
        /// <param name="value"></param>
        public ExtRunAsAdminAttribute(bool value) : base(value)
        {
        }
        /// <summary>
        /// 使用默认值true
        /// </summary>
        public ExtRunAsAdminAttribute() : base(true)
        {
        }
        /// <summary>
        /// Before
        /// </summary>
        /// <param name="args"></param>
        public override void Before(ExtBeforeCreateArgs args)
        {
            if ((bool)Value && !args.Context.App.IsRunAsAdmin)
            {
                args.Context.App.RunOnUIThread(() =>
                {
                    var choice = args.Context.Ux
                    .DoChoice("OpenFxNeedAdminPermission");
                    if (choice == Open.ChoiceResult.Accept)
                    {
                        args.Context.App.RestartAppAsAdmin();
                    }
                });
            }
        }
    }
}
