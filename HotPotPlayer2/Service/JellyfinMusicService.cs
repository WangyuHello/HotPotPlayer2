﻿using CommunityToolkit.Mvvm.ComponentModel;
using HotPotPlayer2.Base;
using Jellyfin.Sdk;
using Jellyfin.Sdk.Generated.Artists;
using Jellyfin.Sdk.Generated.Items;
using Jellyfin.Sdk.Generated.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HotPotPlayer2.Service
{
    public partial class JellyfinMusicService(ConfigBase config, AppBase app) : ServiceBase(config, app)
    {
        [ObservableProperty]
        public partial PublicSystemInfo SystemInfo { get; set; }

        private string? devideId;
        public string DevideId
        {
            get
            {
                if (string.IsNullOrEmpty(devideId))
                {
                    var fromConf = Config.GetConfig("JellyfinDeviceId", $"{Guid.NewGuid():N}");
                    Config.SetConfig("JellyfinDeviceId", fromConf);
                    devideId = fromConf;
                }
                return devideId!;
            }
        }

        private JellyfinSdkSettings? sdkClientSettings;

        public JellyfinSdkSettings SdkClientSettings
        {
            get
            {
                if (sdkClientSettings == null)
                {
                    sdkClientSettings = new JellyfinSdkSettings();
                    sdkClientSettings.Initialize(
                        "HotPotPlayer2",
                        App.ApplicationVersion,
                        Environment.MachineName,
                        DevideId);
                    sdkClientSettings.SetServerUrl(Config.GetConfig<string>("JellyfinUrl"));
                }
                return sdkClientSettings;
            }
        }

        private HttpClient? httpClient;

        private HttpClient HttpClient
        {
            get
            {
                if (httpClient == null)
                {
                    httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("HotPotPlayer", App.ApplicationVersion));
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1.0));
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8));
                }
                return httpClient;
            }
        }

        private JellyfinApiClient? jellyfinApiClient;

        private JellyfinApiClient JellyfinApiClient
        {
            get => jellyfinApiClient ??= new JellyfinApiClient(JellyfinRequestAdapter);
        }

        private JellyfinAuthenticationProvider? jellyfinAuthenticationProvider;
        private JellyfinAuthenticationProvider JellyfinAuthenticationProvider
        {
            get => jellyfinAuthenticationProvider ??= new JellyfinAuthenticationProvider(SdkClientSettings);
        }

        private JellyfinRequestAdapter? jellyfinRequestAdapter;
        private JellyfinRequestAdapter JellyfinRequestAdapter
        {
            get => jellyfinRequestAdapter ??= new JellyfinRequestAdapter(JellyfinAuthenticationProvider, SdkClientSettings, HttpClient);
        }

        [ObservableProperty]
        public partial UserDto UserDto { get; set; }

        [ObservableProperty]
        public partial BaseItemDto SelectedMusicLibraryDto { get; set; }

        [ObservableProperty]
        public partial List<BaseItemDto> MusicLibraryDto { get; set; }

        [ObservableProperty]
        public partial List<BaseItemDto> VideoLibraryDto { get; set; }

        [ObservableProperty]
        public partial SessionInfoDto Session { get; set; }

        private bool IsLogin = false;
        public bool IsMusicPageFirstNavigate { get; set; } = true;
        public bool IsVideoPageFirstNavigate { get; set; } = true;

        public bool IsJellfinAvailable()
        {
            var avail = Config.HasConfig("JellyfinPassword") &&
                        Config.HasConfig("JellyfinUrl");
            return avail;
        }

        private List<BaseItemDto>? jellyfinPlaylistList;
        public async Task<List<BaseItemDto>?> GetJellyfinPlayListList()
        {
            return jellyfinPlaylistList ??= await GetJellyfinPlayListsAsync(() => SelectedMusicLibraryDto, default);
        }
        public async Task<PublicSystemInfo?> GetSystemInfoPublicAsync()
        {
            var result = await JellyfinApiClient.System.Info.Public.GetAsync().ConfigureAwait(false);
            return result;
        }

        private async Task GetViews()
        {
            var views = await JellyfinApiClient.UserViews.GetAsync().ConfigureAwait(false);

            MusicLibraryDto = [.. views!.Items!.Where(v => v.CollectionType == BaseItemDto_CollectionType.Music)];

            var selectedMusicLibraryName = Config.GetConfig<string>("JellyfinMusicLibraryName");
            if (string.IsNullOrEmpty(selectedMusicLibraryName))
            {
                SelectedMusicLibraryDto = MusicLibraryDto.FirstOrDefault()!;
                Config.SetConfig("JellyfinMusicLibraryName", SelectedMusicLibraryDto.Name);
            }
            else
            {
                SelectedMusicLibraryDto = MusicLibraryDto!.FirstOrDefault(m => m.Name == selectedMusicLibraryName)!;
            }

            VideoLibraryDto = [.. views.Items!.Where(v => v.CollectionType == null ||
                                                v.CollectionType == BaseItemDto_CollectionType.Movies ||
                                                v.CollectionType == BaseItemDto_CollectionType.Tvshows)];
        }

        public async Task<List<BaseItemDto>?> GetVideoViews()
        {
            if (!IsLogin)
            {
                var pw = Config.GetConfig<string>("JellyfinPassword");
                if (string.IsNullOrEmpty(pw))
                {
                    return null;
                }
                await JellyfinLoginAsync();
            }

            return VideoLibraryDto;
        }

        public async Task<List<BaseItemDto>?> GetJellyfinAlbumListAsync(Func<BaseItemDto> library, int startIndex = 0, int limit = 50, CancellationToken token = default)
        {
            if (!IsLogin)
            {
                var pw = Config.GetConfig<string>("JellyfinPassword");
                if (string.IsNullOrEmpty(pw))
                {
                    return null;
                }
                await JellyfinLoginAsync();
            }

            var result = await JellyfinApiClient.Items.GetAsync(param =>
            {
                param.QueryParameters = new ItemsRequestBuilder.ItemsRequestBuilderGetQueryParameters
                {
                    UserId = UserDto.Id,
                    ParentId = library().Id,
                    SortBy = [ItemSortBy.ProductionYear, ItemSortBy.PremiereDate, ItemSortBy.SortName],
                    SortOrder = [SortOrder.Descending],
                    IncludeItemTypes = [BaseItemKind.MusicAlbum],
                    Recursive = true,
                    Fields = [ItemFields.PrimaryImageAspectRatio, ItemFields.SortName],
                    ImageTypeLimit = 1,
                    EnableImageTypes = [ImageType.Primary, ImageType.Backdrop, ImageType.Banner, ImageType.Thumb],
                    StartIndex = startIndex,
                    Limit = limit,
                };
            }, token).ConfigureAwait(false);
            return result?.Items;
        }

        public async Task<List<BaseItemDto>?> GetJellyfinPlayListsAsync(Func<BaseItemDto> library, int startIndex = 0, int limit = 50, CancellationToken token = default)
        {
            var result = await JellyfinApiClient.Items.GetAsync(param =>
            {
                param.QueryParameters = new ItemsRequestBuilder.ItemsRequestBuilderGetQueryParameters
                {
                    UserId = UserDto.Id,
                    SortBy = [ItemSortBy.SortName],
                    SortOrder = [SortOrder.Ascending],
                    IncludeItemTypes = [BaseItemKind.Playlist],
                    Recursive = true,
                    Fields = [ItemFields.PrimaryImageAspectRatio, ItemFields.SortName, ItemFields.CanDelete],
                    StartIndex = startIndex,
                    Limit = limit,
                };
            }, token).ConfigureAwait(false);
            return result?.Items;
        }

        public async Task<List<BaseItemDto>?> GetJellyfinArtistListAsync(Func<BaseItemDto> library, CancellationToken token, int startIndex = 0, int limit = 50)
        {
            if (!IsLogin)
            {
                var pw = Config.GetConfig<string>("JellyfinPassword");
                if (string.IsNullOrEmpty(pw))
                {
                    return null;
                }
                await JellyfinLoginAsync();
            }

            var result = await JellyfinApiClient.Artists.GetAsync(param =>
            {
                param.QueryParameters = new ArtistsRequestBuilder.ArtistsRequestBuilderGetQueryParameters
                {
                    UserId = UserDto.Id,
                    ParentId = library().Id,
                    SortBy = [ItemSortBy.SortName],
                    SortOrder = [SortOrder.Ascending],
                    Fields = [ItemFields.PrimaryImageAspectRatio, ItemFields.SortName],
                    ImageTypeLimit = 1,
                    EnableImageTypes = [ImageType.Primary, ImageType.Backdrop, ImageType.Banner, ImageType.Thumb],
                    StartIndex = startIndex,
                    Limit = limit,
                };
            }, token).ConfigureAwait(false);
            return result?.Items;
        }

        public async Task<List<BaseItemDto>?> GetJellyfinVideoListAsync(Func<BaseItemDto> library, int startIndex = 0, int limit = 50, CancellationToken token = default)
        {
            if (!IsLogin)
            {
                var pw = Config.GetConfig<string>("JellyfinPassword");
                if (string.IsNullOrEmpty(pw))
                {
                    return null;
                }
                await JellyfinLoginAsync();
            }

            var result = await JellyfinApiClient.Items.GetAsync(param =>
            {
                param.QueryParameters = new ItemsRequestBuilder.ItemsRequestBuilderGetQueryParameters
                {
                    UserId = UserDto.Id,
                    ParentId = library().Id,
                    SortBy = [ItemSortBy.ProductionYear, ItemSortBy.PremiereDate, ItemSortBy.SortName],
                    SortOrder = [SortOrder.Descending],
                    Fields = [ItemFields.PrimaryImageAspectRatio, ItemFields.SortName, ItemFields.Path, ItemFields.ChildCount, ItemFields.MediaSourceCount,],
                    ImageTypeLimit = 1,
                    StartIndex = startIndex,
                    Limit = limit,
                };
            }, token).ConfigureAwait(false);
            return result?.Items;
        }

        private Uri? GetPrimaryJellyfinImageBase(BaseItemDto_ImageTags? tag, Guid? parentId, int widthHeigh, string tagStr = "Primary")
        {
            if (tag == null) return null;
            if (!tag.AdditionalData.TryGetValue(tagStr, out object? value)) return null;
            var requestInformation = JellyfinApiClient.Items[parentId!.Value].Images[ImageType.Primary.ToString()].ToGetRequestInformation(param =>
            {
                param.QueryParameters = new Jellyfin.Sdk.Generated.Items.Item.Images.Item.WithImageTypeItemRequestBuilder.WithImageTypeItemRequestBuilderGetQueryParameters
                {
                    Tag = value.ToString(),
                    FillHeight = widthHeigh,
                    FillWidth = widthHeigh,
                    Quality = 96
                };
            });
            var uri = JellyfinApiClient.BuildUri(requestInformation);
            return uri;
        }

        public Uri? GetPrimaryJellyfinImage(BaseItemDto_ImageTags? tag, Guid? parentId)
        {
            return GetPrimaryJellyfinImageBase(tag, parentId, 300);
        }

        public Uri? GetPrimaryJellyfinImageWidth(BaseItemDto_ImageTags? tag, Guid? parentId, int widthheight)
        {
            return GetPrimaryJellyfinImageBase(tag, parentId, widthheight);
        }

        public Uri? GetPrimaryJellyfinImageLarge(BaseItemDto_ImageTags tag, Guid? parentId)
        {
            return GetPrimaryJellyfinImageBase(tag, parentId, 600);
        }

        public Uri? GetPrimaryJellyfinImageSmall(BaseItemDto_ImageTags? tag, Guid? parentId)
        {
            return GetPrimaryJellyfinImageBase(tag, parentId, 100);
        }

        public Uri? GetPrimaryJellyfinImageVerySmall(BaseItemDto_ImageTags tag, Guid? parentId)
        {
            return GetPrimaryJellyfinImageBase(tag, parentId, 32);
        }

        public Uri? GetArtJellyfinImage(BaseItemDto_ImageTags tag, Guid? parentId)
        {
            return GetPrimaryJellyfinImageBase(tag, parentId, 300, "Art");
        }
        public Uri? GetBannerJellyfinImage(BaseItemDto_ImageTags tag, Guid? parentId)
        {
            return GetPrimaryJellyfinImageBase(tag, parentId, 300, "Banner");
        }

        public Uri? GetChapterImage(string? tag, int? index, Guid? parentId)
        {
            if (tag == null || index == null || parentId == null) return null;
            var requestInformation = JellyfinApiClient.Items[parentId.Value].Images["Chapter"][index.Value].ToGetRequestInformation(param =>
            {
                param.QueryParameters = new Jellyfin.Sdk.Generated.Items.Item.Images.Item.Item.WithImageIndexItemRequestBuilder.WithImageIndexItemRequestBuilderGetQueryParameters
                {
                    MaxWidth = 600,
                    Tag = tag,
                    Quality = 90
                };
            });
            var uri = JellyfinApiClient.BuildUri(requestInformation);
            return uri;
        }

        public Uri? GetBackdropJellyfinImage(List<string>? tag, Guid? parentId, int widthheight)
        {
            if (tag == null || tag.Count == 0) return null;
            var requestInformation = JellyfinApiClient.Items[parentId!.Value].Images[ImageType.Backdrop.ToString()].ToGetRequestInformation(param =>
            {
                param.QueryParameters = new Jellyfin.Sdk.Generated.Items.Item.Images.Item.WithImageTypeItemRequestBuilder.WithImageTypeItemRequestBuilderGetQueryParameters
                {
                    Tag = tag[0],
                    FillHeight = widthheight,
                    FillWidth = widthheight,
                    Quality = 96
                };
            });
            var uri = JellyfinApiClient.BuildUri(requestInformation);
            return uri;
        }

        public string? GetPrimaryJellyfinImageBlur(BaseItemDto_ImageBlurHashes blurs)
        {
            if (blurs.Primary == null)
            {
                return null;
            }
            if (blurs.Primary.AdditionalData == null)
            {
                return null;
            }
            return blurs.Primary.AdditionalData.First().Value.ToString();
        }

        public async Task JellyfinLoginAsync()
        {
            if (IsLogin)
            {
                return;
            }
            SystemInfo = (await GetSystemInfoPublicAsync())!;
            //await JellyfinApiClient.QuickConnect.Authorize.PostAsync(req =>
            //{
            //    req.QueryParameters = new Jellyfin.Sdk.Generated.QuickConnect.Authorize.AuthorizeRequestBuilder.AuthorizeRequestBuilderPostQueryParameters
            //    {
            //        Code = ""
            //    };
            //});
            // Authenticate user.
            var authenticationResult = await JellyfinApiClient.Users.AuthenticateByName.PostAsync(new AuthenticateUserByName
            {
                Username = Config.GetConfig<string>("JellyfinUserName"),
                Pw = Config.GetConfig<string>("JellyfinPassword")
            }).ConfigureAwait(false);

            SdkClientSettings.SetAccessToken(authenticationResult!.AccessToken);
            UserDto = authenticationResult.User!;

            await GetViews().ConfigureAwait(false);
            Session = authenticationResult.SessionInfo!;
            IsLogin = true;
        }

        public async Task<string?> QuickConnectInitiate(HttpClient http, string url)
        {
            var setting = new JellyfinSdkSettings();
            setting.Initialize(
                "HotPotPlayer",
                App.ApplicationVersion,
                Environment.MachineName,
                DevideId);
            setting.SetServerUrl(url);

            var auth = new JellyfinAuthenticationProvider(setting);
            var adapter = new JellyfinRequestAdapter(auth, setting, http);
            var client = new JellyfinApiClient(adapter);
            var init = await client.QuickConnect.Initiate.PostAsync();

            return init?.Code;
        }

        public async Task<PublicSystemInfo?> GetPublicSystemInfo()
        {
            if (!IsLogin)
            {
                var pw = Config.GetConfig<string>("JellyfinPassword");
                if (string.IsNullOrEmpty(pw))
                {
                    return null;
                }
                await JellyfinLoginAsync();
            }

            return SystemInfo;
        }

        public async Task<(bool success, string message)> TryLoginAsync(string url, string username, string password)
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("HotPotPlayer", App.ApplicationVersion));
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1.0));
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8));

            var setting = new JellyfinSdkSettings();
            setting.Initialize(
                "HotPotPlayer",
                App.ApplicationVersion,
                Environment.MachineName,
                DevideId);
            setting.SetServerUrl(url);

            var auth = new JellyfinAuthenticationProvider(setting);
            var adapter = new JellyfinRequestAdapter(auth, setting, http);
            var client = new JellyfinApiClient(adapter);
            var token = string.Empty;
            var message = string.Empty;
            try
            {
                var authenticationResult = await client.Users.AuthenticateByName.PostAsync(new AuthenticateUserByName
                {
                    Username = username,
                    Pw = password
                }).ConfigureAwait(false);
                token = authenticationResult!.AccessToken;
            }
            catch (Exception e)
            {
                message = e.Message;
            }
            finally
            {
                http.Dispose();
            }
            return (!string.IsNullOrEmpty(token), message);
        }

        public async Task<(JObject re, string msg)> TryGetSystemInfoPublicAsync(string url)
        {
            using var http = new HttpClient();
            var msg = string.Empty;
            var r = string.Empty;
            try
            {
                r = await http.GetStringAsync(url + "/System/Info/Public");
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return (JObject.Parse(r), msg);
        }
        public void Reset()
        {
            jellyfinAuthenticationProvider = null;
            jellyfinRequestAdapter?.Dispose();
            jellyfinApiClient?.Dispose();
            jellyfinRequestAdapter = null;
            jellyfinApiClient = null;
            httpClient?.Dispose();
            httpClient = null;

            IsMusicPageFirstNavigate = true;
            IsVideoPageFirstNavigate = true;
        }

        public async Task<List<BaseItemDto>?> GetAlbumMusicItemsAsync(BaseItemDto album)
        {
            var result = await JellyfinApiClient.Items.GetAsync(param =>
            {
                param.QueryParameters = new ItemsRequestBuilder.ItemsRequestBuilderGetQueryParameters
                {
                    UserId = UserDto.Id,
                    ParentId = album.Id,
                    Fields = [ItemFields.ItemCounts, ItemFields.PrimaryImageAspectRatio, ItemFields.CanDelete, ItemFields.MediaSourceCount],
                    SortBy = [ItemSortBy.ParentIndexNumber, ItemSortBy.IndexNumber, ItemSortBy.SortName],
                };
            }).ConfigureAwait(false);

            return result?.Items;
        }

        public async Task<List<BaseItemDto>?> GetPlayListMusicItemsAsync(BaseItemDto playlist)
        {
            var result = await JellyfinApiClient.Playlists[playlist.Id!.Value].Items.GetAsync(param =>
            {
                param.QueryParameters = new Jellyfin.Sdk.Generated.Playlists.Item.Items.ItemsRequestBuilder.ItemsRequestBuilderGetQueryParameters
                {
                    UserId = UserDto.Id,
                    Fields = [ItemFields.ItemCounts, ItemFields.PrimaryImageAspectRatio, ItemFields.CanDelete, ItemFields.MediaSourceCount],
                    EnableImageTypes = [ImageType.Primary, ImageType.Backdrop, ImageType.Banner, ImageType.Thumb]
                };
            }).ConfigureAwait(false);

            return result?.Items;
        }

        public async Task<BaseItemDto?> GetPlayListInfoAsync(BaseItemDto playlist)
        {
            var result = await JellyfinApiClient.Items[playlist.Id!.Value].GetAsync(param =>
            {
                param.QueryParameters = new Jellyfin.Sdk.Generated.Items.Item.WithItemItemRequestBuilder.WithItemItemRequestBuilderGetQueryParameters
                {
                    UserId = UserDto.Id,
                };
            }).ConfigureAwait(false);

            return result;
        }

        public async Task<BaseItemDto?> GetItemInfoAsync(BaseItemDto item)
        {
            var result = await JellyfinApiClient.Items[item.Id!.Value].GetAsync(param =>
            {
                param.QueryParameters = new Jellyfin.Sdk.Generated.Items.Item.WithItemItemRequestBuilder.WithItemItemRequestBuilderGetQueryParameters
                {
                    UserId = UserDto.Id,
                };
            }).ConfigureAwait(false);

            return result;
        }

        public async Task<BaseItemDto?> GetMusicInfoAsync(BaseItemDto music)
        {
            var result = await JellyfinApiClient.Items[music.Id!.Value].GetAsync(param =>
            {
                param.QueryParameters = new Jellyfin.Sdk.Generated.Items.Item.WithItemItemRequestBuilder.WithItemItemRequestBuilderGetQueryParameters
                {
                    UserId = UserDto.Id,
                };
            }).ConfigureAwait(false);

            return result;
        }

        public async Task<BaseItemDto?> GetAlbumInfoFromMusicAsync(BaseItemDto music)
        {
            var result = await JellyfinApiClient.Items[music.AlbumId!.Value].GetAsync(param =>
            {
                param.QueryParameters = new Jellyfin.Sdk.Generated.Items.Item.WithItemItemRequestBuilder.WithItemItemRequestBuilderGetQueryParameters
                {
                    UserId = UserDto.Id,
                };
            }).ConfigureAwait(false);

            return result;
        }

        public async Task<List<BaseItemDto>?> GetSimilarAlbumAsync(BaseItemDto music)
        {
            var result = await JellyfinApiClient.Items[music.Id!.Value].Similar.GetAsync(param =>
            {
                param.QueryParameters = new Jellyfin.Sdk.Generated.Items.Item.Similar.SimilarRequestBuilder.SimilarRequestBuilderGetQueryParameters
                {
                    UserId = UserDto.Id,
                    Limit = 7,
                    Fields = [ItemFields.PrimaryImageAspectRatio, ItemFields.CanDelete],
                    ExcludeArtistIds = [.. music.AlbumArtists!.Select(a => a.Id)]
                };
            }).ConfigureAwait(false);
            return result?.Items;
        }

        public async Task<BaseItemDto?> GetArtistAsync(Guid id)
        {
            var result = await JellyfinApiClient.Items[id].GetAsync(param =>
            {
                param.QueryParameters = new Jellyfin.Sdk.Generated.Items.Item.WithItemItemRequestBuilder.WithItemItemRequestBuilderGetQueryParameters
                {
                    UserId = UserDto.Id,
                };
            }).ConfigureAwait(false);
            return result;
        }

        public async Task<List<BaseItemDto>?> GetAlbumsFromArtistAsync(Guid id)
        {
            var result = await JellyfinApiClient.Items.GetAsync(param =>
            {
                param.QueryParameters = new ItemsRequestBuilder.ItemsRequestBuilderGetQueryParameters
                {
                    SortOrder = [SortOrder.Descending],
                    IncludeItemTypes = [BaseItemKind.MusicAlbum],
                    Recursive = true,
                    Fields = [ItemFields.ParentId, ItemFields.PrimaryImageAspectRatio],
                    Limit = 10,
                    StartIndex = 0,
                    CollapseBoxSetItems = false,
                    AlbumArtistIds = [id],
                    SortBy = [ItemSortBy.PremiereDate, ItemSortBy.ProductionYear, ItemSortBy.SortName]
                };
            }).ConfigureAwait(false);
            return result?.Items;
        }

        public string GetMusicStream(BaseItemDto music)
        {
            var req = JellyfinApiClient.Audio[music.Id!.Value].Universal.ToGetRequestInformation(param =>
            {
                param.QueryParameters = new Jellyfin.Sdk.Generated.Audio.Item.Universal.UniversalRequestBuilder.UniversalRequestBuilderGetQueryParameters
                {
                    UserId = UserDto.Id!.Value,
                    DeviceId = DevideId,
                    MaxStreamingBitrate = 876421732,
                    Container = ["opus", "webm|opus", "ts|mp3", "mp3", "aac", "m4a|aac", "m4b|aac", "flac", "webma", "webm|webma", "wav", "ogg"],
                    TranscodingContainer = "mp4",
                    TranscodingProtocol = Jellyfin.Sdk.Generated.Audio.Item.Universal.MediaStreamProtocol.Hls,
                    AudioCodec = "aac",
                    StartTimeTicks = 0,
                    EnableRedirection = false,
                    EnableRemoteMedia = false,
                    EnableAudioVbrEncoding = false,
                };
            });
            var uri = JellyfinApiClient.BuildUri(req);
            var url_temp = uri.ToString();
            url_temp = url_temp + "&api_key=" + SdkClientSettings.AccessToken;
            return url_temp;
        }

        public string GetVideoStream(BaseItemDto video)
        {
            var req = JellyfinApiClient.Videos[video.Id!.Value].Stream.ToGetRequestInformation(param =>
            {
                param.QueryParameters = new Jellyfin.Sdk.Generated.Videos.Item.StreamNamespace.StreamRequestBuilder.StreamRequestBuilderGetQueryParameters
                {
                    Static = true,
                    DeviceId = DevideId,
                    MediaSourceId = video.Id.Value.ToString()
                };
            });
            var uri = JellyfinApiClient.BuildUri(req);
            var url_temp = uri.ToString();
            url_temp = url_temp + "&api_key=" + SdkClientSettings.AccessToken;
            return url_temp;
        }
        public async Task<string?> GetPlayBackInfo(BaseItemDto video)
        {
            var result = await JellyfinApiClient.Items[video.Id!.Value].PlaybackInfo.PostAsync(new PlaybackInfoDto
            {
                AutoOpenLiveStream = true,
                EnableDirectPlay = true,
                EnableDirectStream = true,
                MediaSourceId = video.Id.Value.ToString(),
                MaxStreamingBitrate = 785555264,
                StartTimeTicks = 0,
                UserId = UserDto.Id!.Value,
                DeviceProfile = new DeviceProfile
                {
                    MaxStaticBitrate = 1000000000,
                    MaxStreamingBitrate = 1000000000,
                    DirectPlayProfiles =
                    [
                        new() {
                            Container = "mkv",
                            Type = DirectPlayProfile_Type.Video,
                            VideoCodec = "h264,hevc,vp9,av1",
                            AudioCodec = "aac,mp3,ac3,eac3,mp2,opus,flac,vorbis"
                        },
                        new() {
                            Container = "mp4,m4v",
                            Type = DirectPlayProfile_Type.Video,
                            VideoCodec = "h264,hevc,vp9,av1",
                            AudioCodec = "aac,mp3,ac3,eac3,mp2,opus,flac,vorbis"
                        },
                        new() {
                            Container = "flac",
                            Type = DirectPlayProfile_Type.Audio,
                        }
                    ]
                }
            });
            return result?.PlaySessionId;
        }

        public async Task<List<SessionInfoDto>?> GetSeesions()
        {
            var result = await JellyfinApiClient.Sessions.GetAsync(param =>
            {
                param.QueryParameters = new Jellyfin.Sdk.Generated.Sessions.SessionsRequestBuilder.SessionsRequestBuilderGetQueryParameters
                {
                    DeviceId = DevideId
                };
            }).ConfigureAwait(false);
            return result;
        }

        public async Task ReportProgress(BaseItemDto? video, long positionTicks, bool isPause, bool isMute = false)
        {
            if (video == null || video.Id == null)
            {
                return;
            }
            await JellyfinApiClient.Sessions.Playing.Progress.PostAsync(new PlaybackProgressInfo
            {
                CanSeek = true,
                IsPaused = isPause,
                IsMuted = isMute,
                ItemId = video.Id,
                MediaSourceId = video.Id.Value.ToString(),
                PlaySessionId = Session.Id,
                PlayMethod = PlaybackProgressInfo_PlayMethod.DirectStream,
                VolumeLevel = 100,
                PositionTicks = positionTicks
            });
        }

        public async void ReportStop(BaseItemDto? video, long positionTicks)
        {
            if (video == null || video.Id == null)
            {
                return;
            }
            await JellyfinApiClient.Sessions.Playing.Stopped.PostAsync(new PlaybackStopInfo
            {
                ItemId = video.Id,
                SessionId = Session.Id,
                MediaSourceId = video.Id.Value.ToString(),
                PositionTicks = positionTicks,
                PlaySessionId = Session.Id,
            });
        }

        public async Task<List<BaseItemDto>?> GetSeasonsAsync(BaseItemDto bangumi)
        {
            var result = await JellyfinApiClient.Shows[bangumi.Id!.Value].Seasons.GetAsync(param =>
            {
                param.QueryParameters = new Jellyfin.Sdk.Generated.Shows.Item.Seasons.SeasonsRequestBuilder.SeasonsRequestBuilderGetQueryParameters
                {
                    UserId = UserDto.Id,
                    Fields = [ItemFields.ItemCounts, ItemFields.PrimaryImageAspectRatio, ItemFields.CanDelete, ItemFields.MediaSourceCount]
                };
            }).ConfigureAwait(false);
            return result?.Items;
        }

        public async Task<List<BaseItemDto>?> GetEpisodes(BaseItemDto? season)
        {
            if (season == null) return null;
            var result = await JellyfinApiClient.Shows[season.Id!.Value].Episodes.GetAsync(param =>
            {
                param.QueryParameters = new Jellyfin.Sdk.Generated.Shows.Item.Episodes.EpisodesRequestBuilder.EpisodesRequestBuilderGetQueryParameters
                {
                    UserId = UserDto.Id,
                    SeasonId = season.Id,
                    Fields = [ItemFields.ItemCounts, ItemFields.PrimaryImageAspectRatio, ItemFields.CanDelete, ItemFields.MediaSourceCount, ItemFields.Overview]
                };
            }).ConfigureAwait(false);
            return result?.Items;
        }

        public async Task Logout()
        {
            if (!IsLogin) return;
            await JellyfinApiClient.Sessions.Logout.PostAsync().ConfigureAwait(false);
        }
    }
}
