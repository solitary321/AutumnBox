﻿/*************************************************
** auth： zsh2401@163.com
** date:  2018/8/13 4:10:56 (UTC +8:00)
** desc： ...
*************************************************/
using AutumnBox.Basic.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutumnBox.OpenFramework.Extension
{
    /// <summary>
    /// 明确标记标明该拓展需要设备ROOT权限
    /// </summary>
    public class ExtRequireRootAttribute : ExtBeforeCreateAspectAttribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public ExtRequireRootAttribute(bool value) : base(value)
        {
        }
        /// <summary>
        /// 构造
        /// </summary>
        public ExtRequireRootAttribute() : base(true) { }

        public override void Before(ExtBeforeCreateArgs args)
        {
            if ((!(bool)Value) ||  args.TargetDevice == null)
            {
                return;
            }
            if (!DeviceHaveRoot(args.TargetDevice) && (bool)Value)
            {
                args.Context.App.RunOnUIThread(() =>
                {
                    args.Context.Ux.Warn("OpenFxNoRoot!");
                });
                args.Prevent = true;
            }
        }
        private static bool DeviceHaveRoot(IDevice device)
        {
            return device.HaveSU();
        }
    }
}
