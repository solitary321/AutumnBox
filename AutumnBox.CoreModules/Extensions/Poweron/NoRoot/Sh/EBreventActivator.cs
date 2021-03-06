﻿/*************************************************
** auth： zsh2401@163.com
** date:  2018/8/13 9:01:58 (UTC +8:00)
** desc： ...
*************************************************/
using AutumnBox.Basic.Calling;
using AutumnBox.Basic.Calling.Adb;
using AutumnBox.Basic.Device;
using AutumnBox.Basic.Device.Management.AppFx;
using AutumnBox.Basic.Util;
using AutumnBox.OpenFramework.Extension;
using System.Threading;

namespace AutumnBox.CoreModules.Extensions.Poweron.NoRoot.Sh
{
    [ExtName("黑阀一键激活")]
    [ExtName("Activate brevent by one key", Lang = "en-us")]
    [ExtDesc("一键激活黑阀,但值得注意的是,这样的激活方式,在重启后将失效")]
    [ExtAppProperty("me.piebridge.brevent")]
    [ExtRequiredDeviceStates(DeviceState.Poweron)]
    [ExtIcon("Icons.brevent.png")]
    internal class EBreventActivator : StoppableOfficialExtension
    {
        private const string SH_PATH = "/data/data/me.piebridge.brevent/brevent.sh";
        private const int stateCheck = 0;
        private const int stateExecutingShell = 1;
        private int state = 0;
        protected override int VisualMain()
        {
            WriteInitInfo();
            new ActivityManager(TargetDevice).StartActivity("me.piebridge.brevent", "ui.BreventActivity");
            var catCommand = CmdStation.GetShellCommand(TargetDevice, $"cat {SH_PATH}");
            state = stateCheck;
            while (catCommand.Execute().ExitCode != (int)LinuxReturnCode.None && !Canceled)
            {
                Ux.Message(Res("EBreventActivatorFirstMsg"));
                Thread.Sleep(2000);
            }
            ThrowIfCanceled();

            state = stateExecutingShell;
            var result = CmdStation
                .GetShellCommand(TargetDevice, $"sh {SH_PATH}")
                .To(OutputPrinter)
                .Execute();

            ThrowIfCanceled();
            WriteExitCode(result.ExitCode);
            return result.ExitCode;
        }
    }
}
