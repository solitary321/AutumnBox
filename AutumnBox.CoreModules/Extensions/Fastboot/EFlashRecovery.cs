﻿/*************************************************
** auth： zsh2401@163.com
** date:  2018/8/29 1:45:39 (UTC +8:00)
** desc： ...
*************************************************/
using AutumnBox.Basic.Device;
using AutumnBox.OpenFramework.Extension;
using Microsoft.Win32;

namespace AutumnBox.CoreModules.Extensions.Fastboot
{
    [ExtName("刷入REC")]
    [ExtName("Flash recovery.img", Lang = "en-US")]
    [ExtRequiredDeviceStates(DeviceState.Fastboot)]
    [ExtIcon("Icons.cd.png")]
    internal class EFlashRecovery : StoppableOfficialExtension
    {
        protected override int VisualMain()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Reset();
            fileDialog.Title = Res("EFlashRecoverySelectingTitle");
            fileDialog.Filter = Res("EFlashRecoverySelectingFilter");
            fileDialog.Multiselect = false;
            if (fileDialog.ShowDialog() != true) return ERR_CANCELED_BY_USER;
            fileDialog = null;

            var result = CmdStation
                .GetFastbootCommand(TargetDevice,
                $"flash recovery \"{fileDialog.FileName}\"")
                .To(OutputPrinter)
                .Execute();

            if (result.ExitCode == 0)
            {
                CmdStation
               .GetFastbootCommand(TargetDevice,
               $"boot \"{fileDialog.FileName}\"")
               .To(OutputPrinter)
               .Execute();
            }

            WriteExitCode(result.ExitCode);
            return result.ExitCode;
        }
    }
}
