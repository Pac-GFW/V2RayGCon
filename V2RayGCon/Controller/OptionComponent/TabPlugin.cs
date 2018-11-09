﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Forms;

namespace V2RayGCon.Controller.OptionComponent
{
    class TabPlugin : OptionComponentController
    {
        FlowLayoutPanel flyPanel;
        Button btnUpdate;

        Service.Setting setting;
        Service.PluginsServer pluginServ;

        string oldOptions;
        List<Model.Data.PluginInfoItem> curPluginInfos;

        public TabPlugin(
            FlowLayoutPanel flyPanel,
            Button btnUpdate)
        {
            setting = Service.Setting.Instance;
            pluginServ = Service.PluginsServer.Instance;

            this.flyPanel = flyPanel;
            this.btnUpdate = btnUpdate;

            curPluginInfos = setting.GetPluginInfoItems();
            MarkdownCurOption();

            InitPanel();
            BindEvent();
        }

        #region public method
        public override bool SaveOptions()
        {
            if (!IsOptionsChanged())
            {
                return false;
            }

            curPluginInfos = CollectPluginInfoItems();
            MarkdownCurOption();
            setting.SavePluginInfoItems(curPluginInfos);
            pluginServ.Restart();
            return true;
        }

        public override bool IsOptionsChanged()
        {
            return GetCurOptions() != oldOptions;
        }
        #endregion

        #region private method
        string GetCurOptions()
        {
            return JsonConvert.SerializeObject(CollectPluginInfoItems());
        }

        List<Model.Data.PluginInfoItem> CollectPluginInfoItems()
        {
            var itemList = new List<Model.Data.PluginInfoItem>();
            foreach (Views.UserControls.PluginInfoUI item in this.flyPanel.Controls)
            {
                itemList.Add(item.GetValue());
            }
            return itemList;
        }

        void MarkdownCurOption()
        {
            this.oldOptions = Lib.Utils.SerializeObject(curPluginInfos);
        }

        void RemoveAllControls()
        {
            var controls = flyPanel.Controls;
            flyPanel.Controls.Clear();
            foreach (Views.UserControls.PluginInfoUI control in controls)
            {
                control.Dispose();
            }
        }

        void InitPanel()
        {
            RemoveAllControls();
            foreach (var item in curPluginInfos)
            {
                this.flyPanel.Controls.Add(
                    new Views.UserControls.PluginInfoUI(item));
            }
        }

        void BindEvent()
        {
            this.btnUpdate.Click += (s, a) =>
            {
                curPluginInfos = pluginServ.GetterPluginDirInfo();
                InitPanel();
            };
        }
        #endregion
    }
}
