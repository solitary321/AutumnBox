﻿//#define SHOW_OUTPUT
//#define SHOW_COMMAND
namespace AutumnBox.Basic.Executer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using AutumnBox.Basic.Devices;
    using AutumnBox.Basic.Util;
    public enum ExeType
    {
        Null = -1,
        Adb,
        Fastboot
    }
    public sealed partial class CommandExecuter : BaseObject, IDisposable, IDevicesGetter
    {
        private static readonly string ADB_FILENAME = Paths.ADB_FILENAME;
        private static readonly string FB_FILENAME = Paths.FASTBOOT_FILENAME;
        /// <summary>
        /// 执行器主进程开始时发生,可通过该事件获取进程PID
        /// </summary>
        public event ProcessStartEventHandler ProcessStarted;
        /// <summary>
        /// 接收到重定向输出时发生
        /// </summary>
        public event DataReceivedEventHandler OutputDataReceived
        {
            add { MainProcess.OutputDataReceived += value; }
            remove { MainProcess.OutputDataReceived -= value; }
        }
        /// <summary>
        /// 接收到重定向错误时发生
        /// </summary>
        public event DataReceivedEventHandler ErrorDataReceived
        {
            add { MainProcess.ErrorDataReceived += value; }
            remove { MainProcess.ErrorDataReceived -= value; }
        }
        /// <summary>
        /// 当前的执行类型
        /// </summary>
        private ExeType NowExeType
        {
            get {
                return exeType;
            }
            set
            {
                MainProcess.StartInfo.FileName = (value == ExeType.Adb) ? ADB_FILENAME : FB_FILENAME;
                exeType = value;
            }
        }
        private ExeType exeType;
        /// <summary>
        /// 执行命令时输出的缓存
        /// </summary>
        private OutputData tempOut;
        /// <summary>
        /// 执行器的底层进程
        /// </summary>
        private Process MainProcess = new Process();
        /// <summary>
        /// 构造!
        /// </summary>
        public CommandExecuter()
        {
            //初始化Cmd
            MainProcess.StartInfo.FileName = "xxx";
            MainProcess.StartInfo.CreateNoWindow = true;         // 不创建新窗口    
            MainProcess.StartInfo.UseShellExecute = false;       //不启用shell启动进程  
            MainProcess.StartInfo.RedirectStandardInput = true;  // 重定向输入    
            MainProcess.StartInfo.RedirectStandardOutput = true; // 重定向标准输出    
            MainProcess.StartInfo.RedirectStandardError = true;  // 重定向错误输出  
            tempOut = new OutputData();
            MainProcess.OutputDataReceived += (s, e) =>
            {
#if SHOW_OUTPUT
                LogD("Out: " + e.Data);
#endif
                tempOut.OutAdd(e.Data);
            };
            MainProcess.ErrorDataReceived += (s, e) =>
            {
#if SHOW_OUTPUT
                LogD("Error: " + e.Data);
#endif
                tempOut.ErrorAdd(e.Data);
            };
        }
        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="devList"></param>
        public void GetDevices(out DevicesList devList)
        {
            if (Process.GetProcessesByName("adb").Length == 0) AdbExecute("start-server");
            devList = new DevicesList();
            //Adb devices
            List<string> l;
            var o = AdbExecute("devices");
            l = o.LineOut;
            for (int i = 1; i < l.Count - 2; i++)
            {
                devList.Add(
                    new DeviceSimpleInfo
                    {
                        Id = l[i].Split('\t')[0],
                        Status = DevicesHelper.StringStatusToEnumStatus(l[i].Split('\t')[1])
                    });
            }
            //Fastboot devices
           var ofb =  FastbootExecute("devices");
            l = ofb.LineOut;
            for (int i = 0; i < l.Count - 1; i++)
            {
                try
                {
                    devList.Add(
                    new DeviceSimpleInfo
                    {
                        Id = l[i].Split('\t')[0],
                        Status = DevicesHelper.StringStatusToEnumStatus(l[i].Split('\t')[1])
                    });
                }
                catch { }
            }
        }
        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="devList">设备信息列表</param>
        public DevicesList GetDevices() {
            GetDevices(out DevicesList devList);
            return devList;
        }

        /// <summary>
        /// 启动adb服务
        /// </summary>
        public static void Start()
        {
            new CommandExecuter().AdbExecute("start-server");
        }
        /// <summary>
        /// 关闭adb服务
        /// </summary>
        public static void Kill()
        {
            new CommandExecuter().AdbExecute("kill-server");
        }
        /// <summary>
        /// 重启adb服务
        /// </summary>
        public static void Restart()
        {
            Kill();
            Start();
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            MainProcess.Dispose();
        }
    }
}