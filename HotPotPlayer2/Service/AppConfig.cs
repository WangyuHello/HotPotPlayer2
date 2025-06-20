﻿using HotPotPlayer2.Base;
using HotPotPlayer2.Models.Jellyfin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2.Service
{
    public class AppConfig : ConfigBase
    {
        public override string CacheFolder => Path.Combine(LocalFolder, "Cache");

        private string? localFolder;
        public override string LocalFolder => localFolder ??= 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HotPotPlayer2");

        private List<JellyfinServerItem>? _jellyfinServers;
        public override List<JellyfinServerItem> JellyfinServers
        {
            set
            {
                _jellyfinServers = value;
                // Current only support one server
                SetConfig("JellyfinUrl", value[0].Url);
                SetConfig("JellyfinUserName", value[0].UserName);
                SetConfig("JellyfinPassword", value[0].PassWord);
            }
            get
            {
                if (_jellyfinServers == null)
                {
                    _jellyfinServers = [];
                    var url = GetConfig<string>("JellyfinUrl");
                    var username = GetConfig<string>("JellyfinUserName");
                    var password = GetConfig<string>("JellyfinPassword");
                    if (url != null)
                    {
                        _jellyfinServers.Add(new()
                        {
                            Url = url,
                            UserName = username,
                            PassWord = password,
                        });
                    }
                }
                return _jellyfinServers;
            }
        }
    }
}
