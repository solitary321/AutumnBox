﻿/*************************************************
** auth： zsh2401@163.com
** date:  2018/8/13 4:15:02 (UTC +8:00)
** desc： ...
*************************************************/
using AutumnBox.Basic.Device;
using AutumnBox.Basic.Device.Management.AppFx;
using AutumnBox.OpenFramework.Open;

namespace AutumnBox.OpenFramework.Extension
{
    /// <summary>
    /// 声明需要依赖的APP,秋之盒将在运行前进行检查
    /// </summary>
    public sealed class ExtAppPropertyAttribute : ExtBeforeCreateAspectAttribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public ExtAppPropertyAttribute(string value) : base(value)
        {
        }
        /// <summary>
        /// 在那之前...
        /// </summary>
        /// <param name="args"></param>
        public override void Before(ExtBeforeCreateArgs args)
        {
            if (!InstallApplication(args.TargetDevice, Value as string))
            {
                bool ignore = false;
                args.Context.App.RunOnUIThread(() =>
                 {
                     var choice  = args.Context.Ux.DoChoice("OpenFxInstallAppFirst", "OpenFxInstallBtnIgnore", "OpenFxInstallBtnOk");
                     ignore = choice == ChoiceResult.Deny;
                 });
                args.Prevent = !ignore;
            }
        }
        private static bool InstallApplication(IDevice device, string pkgName)
        {
            return new PackageManager(device).IsInstall(pkgName) == true;
        }
    }
}
