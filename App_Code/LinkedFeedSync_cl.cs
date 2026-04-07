using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Threading;
using System.Configuration;
using System.Globalization;

public static class LinkedFeedSync_cl
{
    public sealed class FeedLog
    {
        public DateTime RanAt { get; set; }
        public string SourceKey { get; set; }
        public string SourceLabel { get; set; }
        public int Created { get; set; }
        public int Updated { get; set; }
        public int Expired { get; set; }
        public bool IsSuccess { get; set; }
        public string StatusText { get; set; }
    }

    private sealed class SourceConfig
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public string Url { get; set; }
        public string DetailToken { get; set; }
        public string SourceType { get; set; }
    }

    private sealed class ImageCandidate
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Score { get; set; }
    }

    private sealed class ParsedImages
    {
        public string ThumbnailUrl { get; set; }
        public List<string> GalleryUrls { get; set; }
    }

    private static readonly object _syncLock = new object();
    private static DateTime _lastAttemptAt = DateTime.MinValue;
    private static DateTime _lastSourceSyncAt = DateTime.MinValue;
    private const int AutoSyncMinutes = 15;
    private const int AutoSourceSyncHours = 6;
    private static int _backgroundSyncRunning = 0;
    private static int _backgroundSourceSyncRunning = 0;
    private static Timer _autoTimer;
    private static int _autoTimerStarted = 0;
    private static readonly Dictionary<string, string> ProvinceAliasMap = BuildProvinceAliasMap();

    public static string RunManualSync(string sourceKey)
    {
        return RunSyncInternal(sourceKey);
    }

    public static bool TryQueueManualSync(string sourceKey, out string message)
    {
        if (Interlocked.CompareExchange(ref _backgroundSyncRunning, 1, 0) != 0)
        {
            message = "Đang có một tiến trình cập nhật feed chạy nền. Hãy đợi vài phút rồi bấm tải lại danh sách.";
            return false;
        }

        string normalizedSource = (sourceKey ?? "").Trim();
        ThreadPool.QueueUserWorkItem(delegate
        {
            try
            {
                RunSyncInternal(normalizedSource);
            }
            catch (Exception ex)
            {
                SaveFailureLog("manual", normalizedSource, ex);
            }
            finally
            {
                Interlocked.Exchange(ref _backgroundSyncRunning, 0);
            }
        });

        message = string.IsNullOrWhiteSpace(normalizedSource)
            ? "Đã đưa job cập nhật tất cả nguồn xuống nền. Bạn có thể bấm 'Tải lại danh sách' sau 1-3 phút để xem log."
            : "Đã đưa job cập nhật nguồn '" + normalizedSource + "' xuống nền. Bạn có thể bấm 'Tải lại danh sách' sau 1-3 phút để xem log.";
        return true;
    }

    public static bool TryQueueSourceCatalogSync(out string message)
    {
        if (Interlocked.CompareExchange(ref _backgroundSourceSyncRunning, 1, 0) != 0)
        {
            message = "Đang có một tiến trình đồng bộ nguồn chạy nền. Hãy đợi vài phút rồi tải lại.";
            return false;
        }

        ThreadPool.QueueUserWorkItem(delegate
        {
            try
            {
                SyncAutoApprovedSources();
            }
            catch (Exception ex)
            {
                SaveFailureLog("source-sync", "", ex);
            }
            finally
            {
                Interlocked.Exchange(ref _backgroundSourceSyncRunning, 0);
            }
        });

        message = "Đã đưa job đồng bộ nguồn hợp lệ xuống nền. Bạn có thể bấm 'Tải lại danh sách' sau 1-3 phút.";
        return true;
    }

    public static void RunAutoSyncIfDue()
    {
        DateTime now = AhaTime_cl.Now;
        TryAutoSourceSyncIfDue(now);
        lock (_syncLock)
        {
            if (_lastAttemptAt != DateTime.MinValue && (now - _lastAttemptAt).TotalMinutes < AutoSyncMinutes)
                return;
            _lastAttemptAt = now;
        }

        try { RunSyncInternal(""); } catch { }
    }

    public static void TriggerAutoSyncInBackground()
    {
        if (!IsAutoSyncEnabled())
            return;

        DateTime now = AhaTime_cl.Now;
        bool due;
        lock (_syncLock)
        {
            due = _lastAttemptAt == DateTime.MinValue || (now - _lastAttemptAt).TotalMinutes >= AutoSyncMinutes;
            if (due)
                _lastAttemptAt = now;
        }
        if (!due)
            return;

        if (Interlocked.CompareExchange(ref _backgroundSyncRunning, 1, 0) != 0)
            return;

        ThreadPool.QueueUserWorkItem(delegate
        {
            try
            {
                TryAutoSourceSyncIfDue(AhaTime_cl.Now);
                RunSyncInternal("");
            }
            catch
            {
            }
            finally
            {
                Interlocked.Exchange(ref _backgroundSyncRunning, 0);
            }
        });
    }

    public static void EnsureAutoSchedulerStarted()
    {
        if (!IsAutoSyncEnabled())
            return;
        if (Interlocked.CompareExchange(ref _autoTimerStarted, 1, 0) != 0)
            return;

        _autoTimer = new Timer(delegate
        {
            try { TriggerAutoSyncInBackground(); } catch { }
        }, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5));
    }

    public static string SyncAutoApprovedSources()
    {
        List<SourceConfig> catalog = BuildAutoSourceCatalog();
        int approved = 0;
        int rejected = 0;
        var messages = new List<string>();

        using (dbDataContext db = new dbDataContext())
        {
            LinkedFeedStore_cl.EnsureSchema(db);
            for (int i = 0; i < catalog.Count; i++)
            {
                SourceConfig s = catalog[i];
                bool allowed = false;
                string reason = "";
                try
                {
                    if (!IsAllowedByRobots(s.Url))
                    {
                        reason = "robots disallow";
                    }
                    else
                    {
                        string html = DownloadHtml(s.Url);
                        var links = ExtractDetailUrls(html, s);
                        if (links.Count < 2)
                        {
                            reason = "no parseable listings";
                        }
                        else
                        {
                            allowed = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    reason = "error: " + ex.Message;
                }

                LinkedFeedStore_cl.UpsertSource(db, s.Key, s.Label, s.Url, s.DetailToken, (i + 1) * 10, allowed, allowed ? "robots-approved" : "rejected-auto", s.SourceType);
                if (allowed)
                {
                    approved++;
                }
                else
                {
                    rejected++;
                    messages.Add(s.Label + " (" + reason + ")");
                }
            }
        }

        string msg = "Approved=" + approved + ", Rejected=" + rejected;
        if (messages.Count > 0)
            msg += ". " + string.Join(" | ", messages.Take(5).ToArray());
        return msg;
    }

    public static List<FeedLog> GetRecentLogs(int take)
    {
        using (dbDataContext db = new dbDataContext())
        {
            return LinkedFeedStore_cl.GetRecentLogs(db, take);
        }
    }

    private static string RunSyncInternal(string sourceKey)
    {
        string normalizedSource = (sourceKey ?? "").Trim().ToLowerInvariant();
        List<SourceConfig> sources = BuildSourceConfigsFromRegistry();
        if (normalizedSource != "")
            sources = sources.Where(x => x.Key == normalizedSource).ToList();

        var allItems = new List<LinkedFeedStore_cl.UpsertItem>();
        var messages = new List<string>();

        foreach (SourceConfig source in sources)
        {
            int linksFound = 0;
            int detailAccepted = 0;
            try
            {
                if (!IsAllowedByRobots(source.Url))
                {
                    messages.Add(source.Label + ": robots không cho phép.");
                    continue;
                }

                var detailUrls = new List<string>();
                bool isMuaban = IsMuabanSource(source);
                bool isDongNaiFocus = IsDongNaiFocusSource(source);
                List<string> listingPages = BuildListingPageUrls(source, isDongNaiFocus ? 40 : (isMuaban ? 30 : 16));
                for (int p = 0; p < listingPages.Count; p++)
                {
                    string listingHtml = DownloadHtml(listingPages[p]);
                    if (string.IsNullOrWhiteSpace(listingHtml))
                        continue;
                    detailUrls.AddRange(ExtractDetailUrls(listingHtml, source));
                }
                detailUrls = detailUrls.Distinct().ToList();
                linksFound = detailUrls.Count;
                if (linksFound == 0)
                {
                    messages.Add(source.Label + ": không có URL chi tiết.");
                    continue;
                }

                int take = Math.Min(isDongNaiFocus ? 360 : (isMuaban ? 260 : 120), detailUrls.Count);
                for (int i = 0; i < take; i++)
                {
                    LinkedFeedStore_cl.UpsertItem item = ParseDetailPage(source, detailUrls[i]);
                    if (item == null)
                        continue;
                    item = NormalizeParsedItem(source, item);
                    if (item == null)
                        continue;
                    allItems.Add(item);
                    detailAccepted++;
                }

                messages.Add(source.Label + ": link=" + linksFound + ", hợp lệ=" + detailAccepted);
            }
            catch (Exception ex)
            {
                messages.Add(source.Label + ": lỗi " + ex.Message);
            }
        }

        allItems = allItems
            .Where(x => x != null && !string.IsNullOrWhiteSpace(x.SourceUrl))
            .GroupBy(x => x.SourceUrl)
            .Select(g => g.First())
            .ToList();

        int strictDropped = 0;
        var strictItems = new List<LinkedFeedStore_cl.UpsertItem>();
        for (int i = 0; i < allItems.Count; i++)
        {
            if (IsStrictListingQualified(allItems[i]))
                strictItems.Add(allItems[i]);
            else
                strictDropped++;
        }
        allItems = strictItems;

        int created = 0;
        int updated = 0;
        int expired = 0;
        int deactivatedNoLocation = 0;
        using (dbDataContext db = new dbDataContext())
        {
            LinkedFeedStore_cl.EnsureSchema(db);
            LinkedFeedStore_cl.UpsertPosts(db, allItems, out created, out updated);
            deactivatedNoLocation = LinkedFeedStore_cl.DeactivateMissingLocationPosts(db);
            expired = LinkedFeedStore_cl.ExpireOldPosts(db, LinkedFeedStore_cl.ResolveLinkedTtlDays());
        }

        FeedLog log = new FeedLog
        {
            RanAt = AhaTime_cl.Now,
            SourceKey = normalizedSource == "" ? "all" : normalizedSource,
            SourceLabel = normalizedSource == "" ? "Tất cả nguồn" : ResolveSourceLabel(normalizedSource),
            Created = created,
            Updated = updated,
            Expired = expired,
            IsSuccess = true,
            StatusText = "Created=" + created + ", Updated=" + updated + ", Dropped(quality)=" + strictDropped + ", Deactivated(no-location)=" + deactivatedNoLocation + ". " + string.Join(" | ", messages.ToArray())
        };
        LinkedFeedStore_cl.SaveLog(log);
        return log.StatusText;
    }

    private static void SaveFailureLog(string sourceKey, string sourceLabel, Exception ex)
    {
        try
        {
            LinkedFeedStore_cl.SaveLog(new FeedLog
            {
                RanAt = AhaTime_cl.Now,
                SourceKey = string.IsNullOrWhiteSpace(sourceKey) ? "error" : sourceKey,
                SourceLabel = string.IsNullOrWhiteSpace(sourceLabel) ? "Lỗi cập nhật feed" : sourceLabel,
                Created = 0,
                Updated = 0,
                Expired = 0,
                IsSuccess = false,
                StatusText = ex == null ? "Unknown background sync error." : ex.Message
            });
        }
        catch
        {
        }
    }

    private static List<SourceConfig> BuildSourceConfigs()
    {
        return new List<SourceConfig>
        {
            new SourceConfig { Key = "muaban-thue-bien-hoa", Label = "Muaban thuê Biên Hòa", Url = "https://muaban.net/bat-dong-san/cho-thue-nha-dat-tp-bien-hoa-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-thue-dong-nai", Label = "Muaban thuê Đồng Nai", Url = "https://muaban.net/bat-dong-san/cho-thue-nha-dat-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-thue-nha-bien-hoa", Label = "Muaban thuê nhà Biên Hòa", Url = "https://muaban.net/bat-dong-san/cho-thue-nha-tp-bien-hoa-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-thue-can-ho-bien-hoa", Label = "Muaban thuê căn hộ Biên Hòa", Url = "https://muaban.net/bat-dong-san/cho-thue-can-ho-chung-cu-tp-bien-hoa-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-thue-van-phong-bien-hoa", Label = "Muaban thuê văn phòng Biên Hòa", Url = "https://muaban.net/bat-dong-san/cho-thue-van-phong-mat-bang-tp-bien-hoa-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-thue-kho-bien-hoa", Label = "Muaban thuê kho/xưởng Biên Hòa", Url = "https://muaban.net/bat-dong-san/cho-thue-nha-xuong-nha-kho-tp-bien-hoa-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-ban-bien-hoa", Label = "Muaban bán Biên Hòa", Url = "https://muaban.net/bat-dong-san/mua-ban-nha-dat-tp-bien-hoa-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-ban-dong-nai", Label = "Muaban bán Đồng Nai", Url = "https://muaban.net/bat-dong-san/mua-ban-nha-dat-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-ban-nha-bien-hoa", Label = "Muaban bán nhà Biên Hòa", Url = "https://muaban.net/bat-dong-san/ban-nha-tp-bien-hoa-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-ban-can-ho-bien-hoa", Label = "Muaban bán căn hộ Biên Hòa", Url = "https://muaban.net/bat-dong-san/ban-can-ho-chung-cu-tp-bien-hoa-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-ban-can-ho-bien-hoa-v2", Label = "Muaban bán căn hộ Biên Hòa 2", Url = "https://muaban.net/bat-dong-san/ban-can-ho-tp-bien-hoa-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-ban-dat-bien-hoa", Label = "Muaban bán đất Biên Hòa", Url = "https://muaban.net/bat-dong-san/ban-dat-tho-cu-tp-bien-hoa-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "batdongsan-ban-bien-hoa", Label = "Batdongsan bán Biên Hòa", Url = "https://batdongsan.com.vn/nha-dat-ban-bien-hoa-dna", DetailToken = "-pr", SourceType = "html" },
            new SourceConfig { Key = "batdongsan-ban-dat-bien-hoa", Label = "Batdongsan bán đất Biên Hòa", Url = "https://batdongsan.com.vn/ban-dat-bien-hoa-dna", DetailToken = "-pr", SourceType = "html" },
            new SourceConfig { Key = "batdongsan-thue-bien-hoa", Label = "Batdongsan thuê Biên Hòa", Url = "https://batdongsan.com.vn/cho-thue-nha-dat-bien-hoa-dna", DetailToken = "-pr", SourceType = "html" },
            new SourceConfig { Key = "batdongsan-thue-nha-bien-hoa", Label = "Batdongsan thuê nhà Biên Hòa", Url = "https://batdongsan.com.vn/cho-thue-nha-rieng-bien-hoa-dna", DetailToken = "-pr", SourceType = "html" },
            new SourceConfig { Key = "alonhadat", Label = "Alonhadat", Url = "https://alonhadat.com.vn/nha-dat/can-ban/nha-dat-1.html", DetailToken = ".html", SourceType = "html" },
            new SourceConfig { Key = "rongbay", Label = "Rongbay", Url = "https://rongbay.com/nha-dat.html", DetailToken = ".html", SourceType = "html" },
            new SourceConfig { Key = "raovat-net", Label = "Raovat.net", Url = "https://raovat.net/", DetailToken = "/xem/", SourceType = "html" },
            new SourceConfig { Key = "muaban", Label = "Muaban", Url = "https://muaban.net/bat-dong-san", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "homedy", Label = "Homedy", Url = "https://homedy.com/ban-nha-dat", DetailToken = ".html", SourceType = "html" },
            new SourceConfig { Key = "nhadat247", Label = "Nhadat247", Url = "https://nhadat247.com.vn/ban-nha-dat", DetailToken = ".html", SourceType = "html" },
            new SourceConfig { Key = "dothi", Label = "Dothi", Url = "https://dothi.net/ban-nha-rieng.htm", DetailToken = ".htm", SourceType = "html" },
            new SourceConfig { Key = "dothi-thue", Label = "Dothi thuê", Url = "https://dothi.net/cho-thue-nha-rieng.htm", DetailToken = ".htm", SourceType = "html" },
            new SourceConfig { Key = "nhadatcanban", Label = "Nhadatcanban", Url = "https://nhadatcanban.com.vn/nha-dat-can-ban", DetailToken = ".html", SourceType = "html" },
            new SourceConfig { Key = "batdongsanviet", Label = "Batdongsanviet", Url = "https://batdongsanviet.com.vn/ban-nha", DetailToken = ".html", SourceType = "html" },
            new SourceConfig { Key = "mogi", Label = "Mogi", Url = "https://mogi.vn/mua-ban-nha-dat", DetailToken = "/", SourceType = "html" },
            new SourceConfig { Key = "bds123-ban", Label = "BDS123 bán", Url = "https://bds123.vn/ban-nha-dat.html", DetailToken = ".html", SourceType = "html" },
            new SourceConfig { Key = "bds123-thue", Label = "BDS123 thuê", Url = "https://bds123.vn/cho-thue-nha-dat.html", DetailToken = ".html", SourceType = "html" },
            new SourceConfig { Key = "phongtro123", Label = "Phongtro123", Url = "https://phongtro123.com/cho-thue-nha-nguyen-can", DetailToken = ".html", SourceType = "html" },
            new SourceConfig { Key = "thuecanho123", Label = "Thuecanho123", Url = "https://thuecanho123.com", DetailToken = ".html", SourceType = "html" },
            new SourceConfig { Key = "nhadatso", Label = "Nhadatso", Url = "https://nhadatso.com/ban-nha-dat", DetailToken = ".html", SourceType = "html" }
        };
    }

    private static List<SourceConfig> BuildAutoSourceCatalog()
    {
        return new List<SourceConfig>
        {
            new SourceConfig { Key = "muaban-thue-bien-hoa", Label = "Muaban thuê Biên Hòa", Url = "https://muaban.net/bat-dong-san/cho-thue-nha-dat-tp-bien-hoa-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-thue-dong-nai", Label = "Muaban thuê Đồng Nai", Url = "https://muaban.net/bat-dong-san/cho-thue-nha-dat-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-thue-nha-bien-hoa", Label = "Muaban thuê nhà Biên Hòa", Url = "https://muaban.net/bat-dong-san/cho-thue-nha-tp-bien-hoa-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-thue-can-ho-bien-hoa", Label = "Muaban thuê căn hộ Biên Hòa", Url = "https://muaban.net/bat-dong-san/cho-thue-can-ho-chung-cu-tp-bien-hoa-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-thue-van-phong-bien-hoa", Label = "Muaban thuê văn phòng Biên Hòa", Url = "https://muaban.net/bat-dong-san/cho-thue-van-phong-mat-bang-tp-bien-hoa-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-thue-kho-bien-hoa", Label = "Muaban thuê kho/xưởng Biên Hòa", Url = "https://muaban.net/bat-dong-san/cho-thue-nha-xuong-nha-kho-tp-bien-hoa-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-ban-bien-hoa", Label = "Muaban bán Biên Hòa", Url = "https://muaban.net/bat-dong-san/mua-ban-nha-dat-tp-bien-hoa-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-ban-dong-nai", Label = "Muaban bán Đồng Nai", Url = "https://muaban.net/bat-dong-san/mua-ban-nha-dat-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-ban-nha-bien-hoa", Label = "Muaban bán nhà Biên Hòa", Url = "https://muaban.net/bat-dong-san/ban-nha-tp-bien-hoa-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-ban-can-ho-bien-hoa", Label = "Muaban bán căn hộ Biên Hòa", Url = "https://muaban.net/bat-dong-san/ban-can-ho-chung-cu-tp-bien-hoa-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-ban-can-ho-bien-hoa-v2", Label = "Muaban bán căn hộ Biên Hòa 2", Url = "https://muaban.net/bat-dong-san/ban-can-ho-tp-bien-hoa-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "muaban-ban-dat-bien-hoa", Label = "Muaban bán đất Biên Hòa", Url = "https://muaban.net/bat-dong-san/ban-dat-tho-cu-tp-bien-hoa-dong-nai", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "batdongsan-ban-bien-hoa", Label = "Batdongsan bán Biên Hòa", Url = "https://batdongsan.com.vn/nha-dat-ban-bien-hoa-dna", DetailToken = "-pr", SourceType = "html" },
            new SourceConfig { Key = "batdongsan-ban-dat-bien-hoa", Label = "Batdongsan bán đất Biên Hòa", Url = "https://batdongsan.com.vn/ban-dat-bien-hoa-dna", DetailToken = "-pr", SourceType = "html" },
            new SourceConfig { Key = "batdongsan-thue-bien-hoa", Label = "Batdongsan thuê Biên Hòa", Url = "https://batdongsan.com.vn/cho-thue-nha-dat-bien-hoa-dna", DetailToken = "-pr", SourceType = "html" },
            new SourceConfig { Key = "batdongsan-thue-nha-bien-hoa", Label = "Batdongsan thuê nhà Biên Hòa", Url = "https://batdongsan.com.vn/cho-thue-nha-rieng-bien-hoa-dna", DetailToken = "-pr", SourceType = "html" },
            new SourceConfig { Key = "alonhadat", Label = "Alonhadat", Url = "https://alonhadat.com.vn/nha-dat/can-ban/nha-dat-1.html", DetailToken = ".html", SourceType = "html" },
            new SourceConfig { Key = "rongbay", Label = "Rongbay", Url = "https://rongbay.com/nha-dat.html", DetailToken = ".html", SourceType = "html" },
            new SourceConfig { Key = "raovat-net", Label = "Raovat.net", Url = "https://raovat.net/", DetailToken = "/xem/", SourceType = "html" },
            new SourceConfig { Key = "muaban", Label = "Muaban", Url = "https://muaban.net/bat-dong-san", DetailToken = "/bat-dong-san/", SourceType = "nextjson" },
            new SourceConfig { Key = "homedy", Label = "Homedy", Url = "https://homedy.com/ban-nha-dat", DetailToken = ".html", SourceType = "html" },
            new SourceConfig { Key = "nhadat247", Label = "Nhadat247", Url = "https://nhadat247.com.vn/ban-nha-dat", DetailToken = ".html", SourceType = "html" },
            new SourceConfig { Key = "dothi", Label = "Dothi", Url = "https://dothi.net/ban-nha-rieng.htm", DetailToken = ".htm", SourceType = "html" },
            new SourceConfig { Key = "dothi-thue", Label = "Dothi thuê", Url = "https://dothi.net/cho-thue-nha-rieng.htm", DetailToken = ".htm", SourceType = "html" },
            new SourceConfig { Key = "nhadatcanban", Label = "Nhadatcanban", Url = "https://nhadatcanban.com.vn/nha-dat-can-ban", DetailToken = ".html", SourceType = "html" },
            new SourceConfig { Key = "batdongsanviet", Label = "Batdongsanviet", Url = "https://batdongsanviet.com.vn/ban-nha", DetailToken = ".html", SourceType = "html" },
            new SourceConfig { Key = "mogi", Label = "Mogi", Url = "https://mogi.vn/mua-ban-nha-dat", DetailToken = "/", SourceType = "html" },
            new SourceConfig { Key = "bds123-ban", Label = "BDS123 bán", Url = "https://bds123.vn/ban-nha-dat.html", DetailToken = ".html", SourceType = "html" },
            new SourceConfig { Key = "bds123-thue", Label = "BDS123 thuê", Url = "https://bds123.vn/cho-thue-nha-dat.html", DetailToken = ".html", SourceType = "html" },
            new SourceConfig { Key = "phongtro123", Label = "Phongtro123", Url = "https://phongtro123.com/cho-thue-nha-nguyen-can", DetailToken = ".html", SourceType = "html" },
            new SourceConfig { Key = "thuecanho123", Label = "Thuecanho123", Url = "https://thuecanho123.com", DetailToken = ".html", SourceType = "html" },
            new SourceConfig { Key = "nhadatso", Label = "Nhadatso", Url = "https://nhadatso.com/ban-nha-dat", DetailToken = ".html", SourceType = "html" }
        };
    }

    private static void TryAutoSourceSyncIfDue(DateTime now)
    {
        lock (_syncLock)
        {
            if (_lastSourceSyncAt != DateTime.MinValue && (now - _lastSourceSyncAt).TotalHours < AutoSourceSyncHours)
                return;
            _lastSourceSyncAt = now;
        }
        try { SyncAutoApprovedSources(); } catch { }
    }

    private static bool IsAutoSyncEnabled()
    {
        try
        {
            string raw = ConfigurationManager.AppSettings["BDS.LinkedAutoSyncEnabled"];
            if (string.IsNullOrWhiteSpace(raw))
                return true;
            bool parsed;
            if (bool.TryParse(raw.Trim(), out parsed))
                return parsed;
            return true;
        }
        catch
        {
            return true;
        }
    }

    private static List<SourceConfig> BuildSourceConfigsFromRegistry()
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                LinkedFeedStore_cl.EnsureSchema(db);
                var rows = LinkedFeedStore_cl.GetActiveSources(db);
                var list = rows
                    .Where(x => x != null && !string.IsNullOrWhiteSpace(x.SourceKey) && !string.IsNullOrWhiteSpace(x.ListingUrl))
                    .Select(x => new SourceConfig
                    {
                        Key = (x.SourceKey ?? "").Trim().ToLowerInvariant(),
                        Label = string.IsNullOrWhiteSpace(x.SourceLabel) ? (x.SourceKey ?? "") : x.SourceLabel.Trim(),
                        Url = x.ListingUrl.Trim(),
                        DetailToken = (x.DetailToken ?? "").Trim(),
                        SourceType = string.IsNullOrWhiteSpace(x.SourceType) ? "html" : x.SourceType.Trim().ToLowerInvariant()
                    })
                    .ToList();

                var defaults = BuildSourceConfigs();
                for (int i = 0; i < defaults.Count; i++)
                {
                    SourceConfig d = defaults[i];
                    if (list.Any(x => string.Equals(x.Key, d.Key, StringComparison.OrdinalIgnoreCase)))
                        continue;
                    if (IsTrustedFilteredDongNaiSource(d))
                        list.Insert(0, d);
                    else
                        list.Add(d);
                }

                list = list
                    .GroupBy(x => (x.Key ?? "").Trim().ToLowerInvariant())
                    .Select(g => g.First())
                    .OrderBy(x =>
                    {
                        int idx = defaults.FindIndex(d => string.Equals(d.Key, x.Key, StringComparison.OrdinalIgnoreCase));
                        return idx < 0 ? 9999 : idx;
                    })
                    .ThenBy(x => x.Key)
                    .ToList();

                if (list.Count > 0)
                    return list;
            }
        }
        catch { }

        return BuildSourceConfigs();
    }

    private static string ResolveSourceLabel(string key)
    {
        try
        {
            using (dbDataContext db = new dbDataContext())
            {
                LinkedFeedStore_cl.EnsureSchema(db);
                var row = LinkedFeedStore_cl.GetActiveSources(db)
                    .FirstOrDefault(x => string.Equals((x.SourceKey ?? "").Trim(), (key ?? "").Trim(), StringComparison.OrdinalIgnoreCase));
                if (row != null && !string.IsNullOrWhiteSpace(row.SourceLabel))
                    return row.SourceLabel.Trim();
            }
        }
        catch { }

        switch ((key ?? "").Trim().ToLowerInvariant())
        {
            case "alonhadat": return "Alonhadat";
            case "rongbay": return "Rongbay";
            case "raovat-net": return "Raovat.net";
            case "muaban": return "Muaban";
            case "muaban-thue-bien-hoa": return "Muaban thuê Biên Hòa";
            case "muaban-thue-dong-nai": return "Muaban thuê Đồng Nai";
            case "muaban-thue-nha-bien-hoa": return "Muaban thuê nhà Biên Hòa";
            case "muaban-thue-can-ho-bien-hoa": return "Muaban thuê căn hộ Biên Hòa";
            case "muaban-thue-van-phong-bien-hoa": return "Muaban thuê văn phòng Biên Hòa";
            case "muaban-thue-kho-bien-hoa": return "Muaban thuê kho/xưởng Biên Hòa";
            case "muaban-ban-bien-hoa": return "Muaban bán Biên Hòa";
            case "muaban-ban-dong-nai": return "Muaban bán Đồng Nai";
            case "muaban-ban-nha-bien-hoa": return "Muaban bán nhà Biên Hòa";
            case "muaban-ban-can-ho-bien-hoa": return "Muaban bán căn hộ Biên Hòa";
            case "muaban-ban-can-ho-bien-hoa-v2": return "Muaban bán căn hộ Biên Hòa 2";
            case "muaban-ban-dat-bien-hoa": return "Muaban bán đất Biên Hòa";
            case "batdongsan-ban-bien-hoa": return "Batdongsan bán Biên Hòa";
            case "batdongsan-ban-dat-bien-hoa": return "Batdongsan bán đất Biên Hòa";
            case "batdongsan-thue-bien-hoa": return "Batdongsan thuê Biên Hòa";
            case "batdongsan-thue-nha-bien-hoa": return "Batdongsan thuê nhà Biên Hòa";
            default: return key;
        }
    }

    private static bool IsAllowedByRobots(string url)
    {
        try
        {
            Uri uri = new Uri(url);
            string robots = DownloadHtml(uri.Scheme + "://" + uri.Host + "/robots.txt");
            if (string.IsNullOrWhiteSpace(robots))
                return false;

            string path = uri.AbsolutePath.ToLowerInvariant();
            bool inRelevantGroup = false;
            var matchedRules = new List<Tuple<bool, string>>();
            string[] lines = robots.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string line = (lines[i] ?? "").Trim();
                if (line == "" || line.StartsWith("#"))
                    continue;

                string lower = line.ToLowerInvariant();
                if (lower.StartsWith("user-agent:"))
                {
                    string agent = lower.Substring("user-agent:".Length).Trim();
                    inRelevantGroup = agent == "*" || agent.Contains("ahasalebot");
                    continue;
                }
                if (!inRelevantGroup)
                    continue;

                if (lower.StartsWith("disallow:"))
                {
                    string rule = lower.Substring("disallow:".Length).Trim();
                    if (rule != "" && IsRobotsRuleMatch(path, rule))
                        matchedRules.Add(Tuple.Create(false, rule));
                    continue;
                }

                if (lower.StartsWith("allow:"))
                {
                    string rule = lower.Substring("allow:".Length).Trim();
                    if (rule != "" && IsRobotsRuleMatch(path, rule))
                        matchedRules.Add(Tuple.Create(true, rule));
                }
            }

            if (matchedRules.Count == 0)
                return true;

            var best = matchedRules
                .OrderByDescending(x => x.Item2.Length)
                .ThenByDescending(x => x.Item1 ? 1 : 0)
                .FirstOrDefault();
            if (best != null)
                return best.Item1;

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsRobotsRuleMatch(string path, string rule)
    {
        string normalizedPath = (path ?? "").Trim().ToLowerInvariant();
        string normalizedRule = (rule ?? "").Trim().ToLowerInvariant();
        if (normalizedRule == "" || normalizedRule == "/")
            return normalizedPath.StartsWith("/");
        if (normalizedRule.EndsWith("$"))
            normalizedRule = normalizedRule.TrimEnd('$');
        if (normalizedRule.Contains("*"))
        {
            string regex = "^" + Regex.Escape(normalizedRule).Replace("\\*", ".*");
            return Regex.IsMatch(normalizedPath, regex, RegexOptions.IgnoreCase);
        }
        return normalizedPath.StartsWith(normalizedRule);
    }

    private static string DownloadHtml(string url)
    {
        using (WebClient client = new WebClient())
        {
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 Safari/537.36 AhaSaleBot/1.0");
            client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            client.Headers.Add("Accept-Language", "vi-VN,vi;q=0.9,en-US;q=0.8,en;q=0.7");
            client.Encoding = System.Text.Encoding.UTF8;
            try
            {
                return client.DownloadString(url);
            }
            catch (WebException ex)
            {
                HttpWebResponse resp = ex.Response as HttpWebResponse;
                if (resp != null && (int)resp.StatusCode == 403)
                    throw new Exception("403 Cloudflare challenge/forbidden");
                throw;
            }
        }
    }

    private static List<string> ExtractDetailUrls(string listingHtml, SourceConfig source)
    {
        var urls = new List<string>();
        if (source != null && string.Equals((source.SourceType ?? "").Trim(), "nextjson", StringComparison.OrdinalIgnoreCase))
            urls.AddRange(ExtractDetailUrlsFromEmbeddedJson(listingHtml, source.Url, source));

        MatchCollection links = Regex.Matches(
            listingHtml,
            "<a[^>]*href\\s*=\\s*[\"'](?<href>[^\"']+)[\"'][^>]*>(?<text>.*?)</a>",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        for (int i = 0; i < links.Count; i++)
        {
            string href = HtmlDecode(links[i].Groups["href"].Value);
            string title = NormalizeWhiteSpace(StripHtml(links[i].Groups["text"].Value));
            if (!IsMuabanSource(source) && title.Length < 8)
                continue;

            string abs = ToAbsoluteUrl(source.Url, href);
            if (abs == "")
                continue;
            if (!LooksLikeDetailUrl(source, abs))
                continue;
            urls.Add(abs);
        }

        return urls.Distinct().Take(IsMuabanSource(source) ? 520 : 180).ToList();
    }

    private static List<string> BuildListingPageUrls(SourceConfig source, int maxPages)
    {
        var urls = new List<string>();
        string baseUrl = source == null ? "" : (source.Url ?? "").Trim();
        if (baseUrl == "")
            return urls;

        urls.Add(baseUrl);
        int safeMax = maxPages <= 1 ? 1 : (maxPages > 40 ? 40 : maxPages);
        for (int i = 2; i <= safeMax; i++)
        {
            if (baseUrl.Contains("?"))
            {
                urls.Add(baseUrl + "&page=" + i.ToString());
                urls.Add(baseUrl + "&p=" + i.ToString());
                if (IsMuabanSource(source))
                {
                    urls.Add(baseUrl + "&cp=" + i.ToString());
                    urls.Add(baseUrl + "&pi=" + i.ToString());
                }
            }
            else
            {
                urls.Add(baseUrl + "?page=" + i.ToString());
                urls.Add(baseUrl + "?p=" + i.ToString());
                if (IsMuabanSource(source))
                {
                    urls.Add(baseUrl + "?cp=" + i.ToString());
                    urls.Add(baseUrl + "?pi=" + i.ToString());
                }
            }
            if (IsMuabanSource(source))
                urls.Add(baseUrl.TrimEnd('/') + "/?page=" + i.ToString());
        }

        return urls.Distinct().ToList();
    }

    private static List<string> ExtractDetailUrlsFromEmbeddedJson(string html, string baseUrl, SourceConfig source)
    {
        var urls = new List<string>();
        string input = html ?? "";
        if (input == "")
            return urls;

        MatchCollection m1 = Regex.Matches(input, "\"url\"\\s*:\\s*\"(?<u>https?:\\\\/\\\\/[^\"\\\\]+(?:\\\\/[^\"\\\\]*)*)\"", RegexOptions.IgnoreCase);
        for (int i = 0; i < m1.Count; i++)
        {
            string raw = (m1[i].Groups["u"].Value ?? "").Replace("\\/", "/");
            string abs = ToAbsoluteUrl(baseUrl, raw);
            if (abs == "" || !LooksLikeDetailUrl(source, abs))
                continue;
            urls.Add(abs);
        }

        MatchCollection m2 = Regex.Matches(input, "\"href\"\\s*:\\s*\"(?<u>/bat-dong-san/[^\"]+)\"", RegexOptions.IgnoreCase);
        for (int i = 0; i < m2.Count; i++)
        {
            string raw = (m2[i].Groups["u"].Value ?? "").Replace("\\/", "/");
            string abs = ToAbsoluteUrl(baseUrl, raw);
            if (abs == "" || !LooksLikeDetailUrl(source, abs))
                continue;
            urls.Add(abs);
        }

        MatchCollection m3 = Regex.Matches(input, "\"url\"\\s*:\\s*\"(?<u>https?://[^\"\\\\]+)\"", RegexOptions.IgnoreCase);
        for (int i = 0; i < m3.Count; i++)
        {
            string raw = (m3[i].Groups["u"].Value ?? "").Trim();
            string abs = ToAbsoluteUrl(baseUrl, raw);
            if (abs == "" || !LooksLikeDetailUrl(source, abs))
                continue;
            urls.Add(abs);
        }

        MatchCollection m4 = Regex.Matches(input, "\"(?:href|url)\"\\s*:\\s*\"(?<u>\\\\/bat-dong-san\\\\/[^\"]+)\"", RegexOptions.IgnoreCase);
        for (int i = 0; i < m4.Count; i++)
        {
            string raw = (m4[i].Groups["u"].Value ?? "").Replace("\\/", "/");
            string abs = ToAbsoluteUrl(baseUrl, raw);
            if (abs == "" || !LooksLikeDetailUrl(source, abs))
                continue;
            urls.Add(abs);
        }

        return urls.Distinct().Take(IsMuabanSource(source) ? 900 : 260).ToList();
    }

    private static bool LooksLikeDetailUrl(SourceConfig source, string url)
    {
        string lower = (url ?? "").ToLowerInvariant();
        if (lower == "")
            return false;

        if (source.Key == "raovat-net")
            return lower.Contains("/xem/") && lower.Contains(".html");

        if (source.Key == "alonhadat")
            return lower.Contains(".html")
                && !lower.Contains("/nha-dat/can-ban/")
                && !lower.Contains("/nha-dat/cho-thue/");

        if (source.Key == "rongbay")
            return lower.Contains("rongbay.com") && lower.Contains(".html");

        if (IsMuabanSource(source))
        {
            if (!lower.Contains("muaban.net/bat-dong-san/"))
                return false;
            if (lower.Contains("?page=") || lower.Contains("&page=") || lower.Contains("?p=") || lower.Contains("&p="))
                return false;
            if (lower.EndsWith("/bat-dong-san") || lower.EndsWith("/bat-dong-san/"))
                return false;
            return Regex.IsMatch(lower, @"\d{5,}");
        }

        if (IsBatdongsanSource(source))
        {
            if (!lower.Contains("batdongsan.com.vn/"))
                return false;
            if (lower.Contains("?page=") || lower.Contains("&page="))
                return false;
            if (lower.EndsWith("-dna") || lower.EndsWith("-dna/"))
                return false;
            return lower.Contains("-pr");
        }

        if (source.Key == "dothi" || source.Key == "dothi-thue")
            return lower.Contains("dothi.net") && (lower.Contains(".htm") || lower.Contains(".html")) && !lower.Contains("default.aspx");

        if (source.Key == "mogi")
            return lower.Contains("mogi.vn/") && (lower.Contains("mua-ban") || lower.Contains("cho-thue"));

        if (source.Key == "phongtro123" || source.Key == "thuecanho123" || source.Key == "bds123-ban" || source.Key == "bds123-thue" || source.Key == "nhadatso")
            return (lower.Contains(".html") || lower.Contains(".htm")) && !lower.Contains("?page=");

        return lower.Contains(source.DetailToken ?? "");
    }

    private static LinkedFeedStore_cl.UpsertItem ParseDetailPage(SourceConfig source, string detailUrl)
    {
        try
        {
            string html = DownloadHtml(detailUrl);
            if (string.IsNullOrWhiteSpace(html))
                return null;

            string title = ExtractMeta(html, "og:title");
            if (title == "")
                title = ExtractTag(html, "title");
            if (title == "")
                title = ExtractFieldFromEmbeddedJson(html, "title");
            title = NormalizeWhiteSpace(StripHtml(title));
            if (title.Length < 10)
                return null;

            string desc = ExtractRichDescription(html);
            if (desc == "")
            {
                desc = ExtractMeta(html, "og:description");
                if (desc == "")
                    desc = ExtractFirstParagraph(html);
                if (desc == "")
                    desc = ExtractFieldFromEmbeddedJson(html, "description");
            }
            desc = CleanSummary(desc);

            string sample = title + " " + desc + " " + StripHtml(html.Substring(0, Math.Min(3000, html.Length)));
            if (!LooksCommercial(sample))
                return null;

            ParsedImages images = ParseAndCacheImages(html, detailUrl, source.Key);
            string locationSample = string.Join(" ",
                sample,
                ExtractMeta(html, "place:location:latitude"),
                ExtractMeta(html, "place:location:longitude"),
                ExtractFieldFromEmbeddedJson(html, "address"),
                ExtractFieldFromEmbeddedJson(html, "location"),
                ExtractFieldFromEmbeddedJson(html, "ward"),
                ExtractFieldFromEmbeddedJson(html, "district"),
                ExtractFieldFromEmbeddedJson(html, "city"),
                detailUrl
            );

            string district = ExtractDistrict(locationSample);
            string province = ExtractLocation(locationSample);
            if (string.IsNullOrWhiteSpace(province))
                province = InferProvinceFromSource(source);
            if (string.IsNullOrWhiteSpace(district))
                district = InferDistrictFromSource(source);

            return new LinkedFeedStore_cl.UpsertItem
            {
                Title = title,
                Summary = desc == "" ? title : desc,
                Source = source.Key,
                SourceUrl = detailUrl,
                Purpose = InferPurpose(sample),
                PropertyType = InferPropertyType(sample),
                PriceText = ExtractPriceText(sample),
                AreaText = ExtractAreaText(sample),
                ThumbnailUrl = images == null ? "" : images.ThumbnailUrl,
                GalleryCsv = images == null ? "" : string.Join("|", (images.GalleryUrls ?? new List<string>()).ToArray()),
                PublishedAt = AhaTime_cl.Now,
                Province = province,
                District = district
            };
        }
        catch
        {
            return null;
        }
    }

    private static LinkedFeedStore_cl.UpsertItem NormalizeParsedItem(SourceConfig source, LinkedFeedStore_cl.UpsertItem item)
    {
        if (item == null)
            return null;

        string sourceKey = (source == null ? item.Source : source.Key) ?? item.Source ?? "";
        sourceKey = sourceKey.Trim().ToLowerInvariant();

        string province = NormalizeProvinceName(item.Province);
        string district = NormalizeDistrictName(item.District);

        string inferredProvince = NormalizeProvinceName(InferProvinceFromSource(source));
        string inferredDistrict = NormalizeDistrictName(InferDistrictFromSource(source));

        if (district == "Biên Hòa")
            province = "Đồng Nai";

        if (IsDongNaiDistrict(district) && province != "Đồng Nai")
            province = "Đồng Nai";

        if (IsTrustedFilteredDongNaiSource(source))
        {
            province = "Đồng Nai";
            if (inferredDistrict != "")
                district = inferredDistrict;
            else if (district != "" && !IsDongNaiDistrict(district))
                district = "";
        }
        else
        {
            if (province == "")
                province = inferredProvince;
            if (district == "")
                district = inferredDistrict;

            if (province == "Đồng Nai" && district != "" && !IsDongNaiDistrict(district))
                district = inferredDistrict;
        }

        item.Source = sourceKey;
        item.Title = NormalizeWhiteSpace(item.Title);
        item.Summary = CleanSummary(item.Summary);
        item.Province = province;
        item.District = district;
        return item;
    }

    private static string ExtractMeta(string html, string propertyName)
    {
        Match m = Regex.Match(
            html ?? "",
            "<meta[^>]+(?:property|name)=[\"']" + Regex.Escape(propertyName) + "[\"'][^>]+content=[\"'](?<v>[^\"']+)[\"'][^>]*>",
            RegexOptions.IgnoreCase);
        return m.Success ? HtmlDecode(m.Groups["v"].Value) : "";
    }

    private static string ExtractTag(string html, string tagName)
    {
        Match m = Regex.Match(
            html ?? "",
            "<" + tagName + "[^>]*>(?<v>.*?)</" + tagName + ">",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        return m.Success ? HtmlDecode(m.Groups["v"].Value) : "";
    }

    private static string ExtractFirstParagraph(string html)
    {
        Match m = Regex.Match(
            html ?? "",
            "<p[^>]*>(?<v>.*?)</p>",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        return m.Success ? HtmlDecode(m.Groups["v"].Value) : "";
    }

    private static ParsedImages ParseAndCacheImages(string html, string detailUrl, string sourceKey)
    {
        List<string> rawUrls = new List<string>();

        string og = ExtractMeta(html, "og:image");
        if (!string.IsNullOrWhiteSpace(og))
        {
            string ogAbs = ToAbsoluteUrl(detailUrl, og);
            if (LooksLikeGoodThumbnail(ogAbs) && IsLikelyListingPhotoUrl(ogAbs))
                rawUrls.Add(ogAbs);
        }

        List<ImageCandidate> images = ExtractImageCandidates(html, detailUrl);
        rawUrls.AddRange(images
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.Width * x.Height)
            .Select(x => x.Url)
            .ToList());

        rawUrls.AddRange(ExtractImageUrlsFromEmbeddedJson(html, detailUrl));

        List<string> uniqueRaw = rawUrls
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .Take(10)
            .ToList();

        var cached = new List<string>();
        for (int i = 0; i < uniqueRaw.Count; i++)
        {
            string local = CacheImage(uniqueRaw[i], sourceKey);
            if (string.IsNullOrWhiteSpace(local))
                continue;
            if (!cached.Contains(local))
                cached.Add(local);
            if (cached.Count >= 8)
                break;
        }

        if (cached.Count == 0)
        {
            var fallbackRemote = uniqueRaw
                .Where(x => LooksLikeGoodThumbnail(x) && IsLikelyListingPhotoUrl(x))
                .Distinct()
                .Take(8)
                .ToList();

            if (fallbackRemote.Count > 0)
            {
                return new ParsedImages
                {
                    ThumbnailUrl = fallbackRemote[0],
                    GalleryUrls = fallbackRemote
                };
            }

            return new ParsedImages
            {
                ThumbnailUrl = "",
                GalleryUrls = new List<string>()
            };
        }

        return new ParsedImages
        {
            ThumbnailUrl = cached[0],
            GalleryUrls = cached
        };
    }

    private static List<string> ExtractImageUrlsFromEmbeddedJson(string html, string detailUrl)
    {
        var urls = new List<string>();
        MatchCollection m = Regex.Matches(html ?? "", "\"(?:image|thumbnail|photo|cover|src)\"\\s*:\\s*\"(?<u>https?:\\\\/\\\\/[^\"\\\\]+(?:\\\\/[^\"\\\\]*)*)\"", RegexOptions.IgnoreCase);
        for (int i = 0; i < m.Count; i++)
        {
            string raw = (m[i].Groups["u"].Value ?? "").Replace("\\/", "/");
            string abs = ToAbsoluteUrl(detailUrl, raw);
            if (abs == "" || !LooksLikeImage(abs))
                continue;
            if (!LooksLikeGoodThumbnail(abs) || !IsLikelyListingPhotoUrl(abs))
                continue;
            if (!urls.Contains(abs))
                urls.Add(abs);
            if (urls.Count >= 12)
                break;
        }
        return urls;
    }

    private static bool LooksLikeImage(string url)
    {
        string lower = (url ?? "").ToLowerInvariant();
        return lower.Contains(".jpg") || lower.Contains(".jpeg") || lower.Contains(".png") || lower.Contains(".webp");
    }

    private static bool LooksLikeGoodThumbnail(string url)
    {
        string lower = (url ?? "").ToLowerInvariant();
        if (lower == "")
            return false;
        if (lower.Contains("logo") || lower.Contains("icon") || lower.Contains("sprite") || lower.Contains("blank"))
            return false;
        if (lower.Contains("avatar") || lower.Contains("profile") || lower.Contains("user") || lower.Contains("no-avatar") || lower.Contains("default-user"))
            return false;
        if (lower.Contains("placeholder") || lower.Contains("default") || lower.Contains("anonymous") || lower.Contains("person"))
            return false;
        if (IsLikelyBrandingUrl(lower))
            return false;
        return LooksLikeImage(lower);
    }

    private static List<ImageCandidate> ExtractImageCandidates(string html, string detailUrl)
    {
        var output = new List<ImageCandidate>();
        MatchCollection matches = Regex.Matches(html ?? "", "<img[^>]*>", RegexOptions.IgnoreCase);
        for (int i = 0; i < matches.Count; i++)
        {
            string tag = matches[i].Value;
            string rawSrc = ResolveImageSource(tag);
            string abs = ToAbsoluteUrl(detailUrl, HtmlDecode(rawSrc));
            if (string.IsNullOrWhiteSpace(abs))
                continue;
            if (!LooksLikeImage(abs) || !LooksLikeGoodThumbnail(abs) || !IsLikelyListingPhotoUrl(abs))
                continue;

            int w = ParseInt(ExtractAttr(tag, "width"));
            int h = ParseInt(ExtractAttr(tag, "height"));
            int score = ScoreImage(abs, w, h);
            if (score < 1)
                continue;

            output.Add(new ImageCandidate
            {
                Url = abs,
                Width = w,
                Height = h,
                Score = score
            });
        }

        return output
            .GroupBy(x => x.Url)
            .Select(g => g.OrderByDescending(x => x.Score).First())
            .ToList();
    }

    private static int ScoreImage(string url, int width, int height)
    {
        int score = 0;
        int area = width * height;

        if (area >= 160000) score += 80; // >= 400x400
        else if (area >= 90000) score += 50;
        else if (area >= 40000) score += 20;
        else if (area > 0) score -= 20;

        string lower = (url ?? "").ToLowerInvariant();
        if (lower.Contains("thumb") || lower.Contains("thumbnail")) score -= 10;
        if (lower.Contains("small") || lower.Contains("icon") || lower.Contains("logo")) score -= 40;
        if (lower.Contains("avatar") || lower.Contains("profile") || lower.Contains("user") || lower.Contains("no-avatar") || lower.Contains("default-user")) score -= 80;
        if (lower.Contains("placeholder") || lower.Contains("default") || lower.Contains("anonymous") || lower.Contains("person")) score -= 120;
        if (IsLikelyBrandingUrl(lower)) score -= 200;
        if (lower.Contains("uploads") || lower.Contains("images")) score += 10;

        return score;
    }

    private static bool IsLikelyBrandingUrl(string url)
    {
        string lower = (url ?? "").ToLowerInvariant();
        if (lower == "")
            return true;

        string[] blocked = new[]
        {
            "logo", "icon", "sprite", "favicon", "brand", "banner",
            "placeholder", "default", "avatar", "profile", "anonymous", "person", "blank"
        };
        for (int i = 0; i < blocked.Length; i++)
        {
            if (lower.Contains(blocked[i]))
                return true;
        }
        return false;
    }

    private static bool IsLikelyListingPhotoUrl(string url)
    {
        string lower = (url ?? "").ToLowerInvariant();
        if (lower == "")
            return false;
        if (IsLikelyBrandingUrl(lower))
            return false;

        bool hasPhotoDir =
            lower.Contains("/upload/") ||
            lower.Contains("/uploads/") ||
            lower.Contains("/media/") ||
            lower.Contains("/photo/") ||
            lower.Contains("/photos/") ||
            lower.Contains("/images/news/") ||
            lower.Contains("/images/listing/") ||
            lower.Contains("/images/product/") ||
            lower.Contains("cdn.") ||
            lower.Contains("muaban.net/images/");

        string fileName = lower;
        int slash = lower.LastIndexOf('/');
        if (slash >= 0 && slash < lower.Length - 1)
            fileName = lower.Substring(slash + 1);

        bool hasDigit = Regex.IsMatch(fileName, @"\d{3,}");
        bool hasExt = LooksLikeImage(lower);
        return hasExt && (hasPhotoDir || hasDigit);
    }

    private static string ResolveImageSource(string imgTag)
    {
        string srcset = ExtractAttr(imgTag, "srcset");
        if (!string.IsNullOrWhiteSpace(srcset))
        {
            string best = PickLargestFromSrcSet(srcset);
            if (!string.IsNullOrWhiteSpace(best))
                return best;
        }

        string[] attrs = new[] { "data-src", "data-original", "data-lazy-src", "data-echo", "src" };
        for (int i = 0; i < attrs.Length; i++)
        {
            string v = ExtractAttr(imgTag, attrs[i]);
            if (!string.IsNullOrWhiteSpace(v))
                return v;
        }

        return "";
    }

    private static string PickLargestFromSrcSet(string srcset)
    {
        string[] parts = (srcset ?? "").Split(',');
        string bestUrl = "";
        int bestWidth = 0;
        for (int i = 0; i < parts.Length; i++)
        {
            string part = (parts[i] ?? "").Trim();
            if (part == "")
                continue;

            string[] tokens = part.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
                continue;

            string candidateUrl = tokens[0];
            int width = 0;
            if (tokens.Length > 1)
            {
                Match m = Regex.Match(tokens[1], @"(\d+)\s*w", RegexOptions.IgnoreCase);
                if (m.Success)
                    int.TryParse(m.Groups[1].Value, out width);
            }

            if (bestUrl == "" || width > bestWidth)
            {
                bestUrl = candidateUrl;
                bestWidth = width;
            }
        }

        return bestUrl;
    }

    private static string ExtractAttr(string tag, string attrName)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return "";
        Match m = Regex.Match(tag, attrName + "\\s*=\\s*[\"'](?<v>[^\"']+)[\"']", RegexOptions.IgnoreCase);
        if (m.Success)
            return m.Groups["v"].Value;
        Match m2 = Regex.Match(tag, attrName + "\\s*=\\s*(?<v>[^\\s>]+)", RegexOptions.IgnoreCase);
        return m2.Success ? m2.Groups["v"].Value : "";
    }

    private static int ParseInt(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0;
        string digits = Regex.Replace(value, @"[^\d]", "");
        int n;
        if (int.TryParse(digits, out n))
            return n;
        return 0;
    }

    private static string CacheImage(string remoteUrl, string sourceKey)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(remoteUrl))
                return "";
            if (IsLikelyBrandingUrl(remoteUrl))
                return "";
            if (!IsLikelyListingPhotoUrl(remoteUrl))
                return "";

            Uri uri;
            if (!Uri.TryCreate(remoteUrl, UriKind.Absolute, out uri))
                return "";

            string ext = Path.GetExtension(uri.AbsolutePath);
            if (string.IsNullOrWhiteSpace(ext) || ext.Length > 6)
                ext = ".jpg";
            ext = ext.ToLowerInvariant();

            string hash = Md5(remoteUrl);
            string fileName = (sourceKey ?? "linked") + "-" + hash + ext;
            string relative = "/uploads/images/linked-bds/" + fileName;
            string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads", "images", "linked-bds", fileName);
            if (File.Exists(localPath))
                return relative;

            string dir = Path.GetDirectoryName(localPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent", "AhaSaleBot/1.0");
                client.Headers.Add("Referer", uri.Scheme + "://" + uri.Host + "/");
                client.DownloadFile(remoteUrl, localPath);
            }

            if (!File.Exists(localPath))
                return "";

            FileInfo fi = new FileInfo(localPath);
            if (fi.Length < 12000)
            {
                try { File.Delete(localPath); } catch { }
                return "";
            }

            return relative;
        }
        catch
        {
            return "";
        }
    }

    private static string Md5(string value)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] input = Encoding.UTF8.GetBytes(value ?? "");
            byte[] hash = md5.ComputeHash(input);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("x2"));
            return sb.ToString();
        }
    }

    private static string CleanSummary(string value)
    {
        return BatDongSanService_cl.SanitizeLinkedText(value, 1600, true);
    }

    private static string ExtractRichDescription(string html)
    {
        string source = html ?? "";
        if (source == "")
            return "";

        MatchCollection blocks = Regex.Matches(source,
            "<p[^>]*>(?<v>.*?)</p>|<li[^>]*>(?<v2>.*?)</li>",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        List<string> lines = new List<string>();
        for (int i = 0; i < blocks.Count; i++)
        {
            string raw = blocks[i].Groups["v"].Success ? blocks[i].Groups["v"].Value : blocks[i].Groups["v2"].Value;
            string text = NormalizeWhiteSpace(StripHtml(raw));
            if (text.Length < 24)
                continue;
            if (!lines.Contains(text))
                lines.Add(text);
            if (lines.Count >= 8)
                break;
        }

        if (lines.Count == 0)
            return "";

        return string.Join("\n", lines.ToArray());
    }

    private static string ExtractFieldFromEmbeddedJson(string html, string fieldName)
    {
        Match m = Regex.Match(html ?? "", "\"" + Regex.Escape(fieldName) + "\"\\s*:\\s*\"(?<v>[^\"\\\\]*(?:\\\\.[^\"\\\\]*)*)\"", RegexOptions.IgnoreCase);
        if (!m.Success)
            return "";
        string raw = (m.Groups["v"].Value ?? "").Replace("\\n", " ").Replace("\\r", " ").Replace("\\/", "/").Replace("\\\"", "\"");
        return HtmlDecode(raw);
    }

    private static string ExtractPriceText(string text)
    {
        Match m = Regex.Match(text ?? "", @"(\d+[.,]?\d*)\s*(tỷ|ty|triệu|trieu|nghìn|nghin|đ|vnd)", RegexOptions.IgnoreCase);
        return m.Success ? NormalizeWhiteSpace(m.Value) : "";
    }

    private static string ExtractAreaText(string text)
    {
        Match m = Regex.Match(text ?? "", @"(\d+[.,]?\d*)\s*(m2|m²)", RegexOptions.IgnoreCase);
        return m.Success ? NormalizeWhiteSpace(m.Value) : "";
    }

    private static string InferPurpose(string text)
    {
        string lower = (text ?? "").ToLowerInvariant();
        if (lower.Contains("cho thuê") || lower.Contains("thuê"))
            return "rent";
        return "sale";
    }

    private static string InferPropertyType(string text)
    {
        string lower = (text ?? "").ToLowerInvariant();
        if (lower.Contains("căn hộ") || lower.Contains("chung cư")) return "apartment";
        if (lower.Contains("đất") || lower.Contains("dat")) return "land";
        if (lower.Contains("văn phòng") || lower.Contains("van phong")) return "office";
        if (lower.Contains("mặt bằng") || lower.Contains("mat bang")) return "business-premises";
        if (lower.Contains("nhà") || lower.Contains("nha")) return "house";
        return "apartment";
    }

    private static string ExtractLocation(string text)
    {
        string normalized = NormalizeLocationText(text);
        if (normalized == "")
            return "";
        if (normalized.Contains("toan quoc"))
            return "";

        foreach (var kv in ProvinceAliasMap)
        {
            if (ContainsLocationToken(normalized, kv.Key))
                return kv.Value;
        }

        return "";
    }

    private static string ExtractDistrict(string text)
    {
        string lower = NormalizeLocationText(text);
        if (lower == "")
            return "";

        Match q = Regex.Match(lower, @"\b(quận|quan)\s*\d+\b", RegexOptions.IgnoreCase);
        if (q.Success)
            return NormalizeWhiteSpace(q.Value).Replace("quan", "Quận");

        Match h = Regex.Match(lower, @"\b(huyện|huyen)\s+[a-z0-9à-ỹ\-\s]{2,40}(?=,|\.|;|\||$)", RegexOptions.IgnoreCase);
        if (h.Success)
            return ToVietnameseTitle(h.Value);

        Match tp = Regex.Match(lower, @"\b(thanh pho|tp)\s+[a-z0-9à-ỹ\-\s]{2,40}(?=,|\.|;|\||$)", RegexOptions.IgnoreCase);
        if (tp.Success)
            return ToVietnameseTitle(tp.Value);

        Match tx = Regex.Match(lower, @"\b(thi xa|thị xã)\s+[a-z0-9à-ỹ\-\s]{2,40}(?=,|\.|;|\||$)", RegexOptions.IgnoreCase);
        if (tx.Success)
            return ToVietnameseTitle(tx.Value);

        string[] known = new[]
        {
            "thủ đức", "nhà bè", "long thành", "biên hòa", "thuận an", "dĩ an", "thanh xuân",
            "gò vấp", "bình thạnh", "tân bình", "phú nhuận", "quận 7", "quận 9", "quận 12"
        };
        for (int i = 0; i < known.Length; i++)
        {
            if (lower.Contains(known[i]))
                return ToVietnameseTitle(known[i]);
        }

        return "";
    }

    private static bool LooksCommercial(string text)
    {
        string lower = (text ?? "").ToLowerInvariant();
        bool action = lower.Contains("bán") || lower.Contains("ban ") || lower.Contains("cho thuê") || lower.Contains("thuê");
        bool category = lower.Contains("nhà") || lower.Contains("nha") || lower.Contains("đất") || lower.Contains("dat")
            || lower.Contains("căn hộ") || lower.Contains("chung cư") || lower.Contains("mặt bằng") || lower.Contains("văn phòng");
        return action && category;
    }

    private static string ToAbsoluteUrl(string baseUrl, string href)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(href))
                return "";
            Uri baseUri = new Uri(baseUrl);
            Uri abs = new Uri(baseUri, href);
            return abs.ToString();
        }
        catch
        {
            return "";
        }
    }

    private static string StripHtml(string value)
    {
        string noTag = Regex.Replace(value ?? "", "<.*?>", " ");
        return HtmlDecode(noTag);
    }

    private static string HtmlDecode(string value)
    {
        return HttpUtility.HtmlDecode(value ?? "");
    }

    private static string NormalizeWhiteSpace(string value)
    {
        return Regex.Replace(value ?? "", "\\s+", " ").Trim();
    }

    private static string TrimTo(string value, int max)
    {
        string v = NormalizeWhiteSpace(value);
        if (v.Length <= max)
            return v;
        return v.Substring(0, max).Trim() + "...";
    }

    private static bool IsStrictListingQualified(LinkedFeedStore_cl.UpsertItem item)
    {
        if (item == null)
            return false;

        string title = (item.Title ?? "").Trim();
        string summary = (item.Summary ?? "").Trim();
        string sourceUrl = (item.SourceUrl ?? "").Trim();
        string thumb = (item.ThumbnailUrl ?? "").Trim();
        string gallery = (item.GalleryCsv ?? "").Trim();
        string province = (item.Province ?? "").Trim();

        // Lọc tối thiểu: giữ được lượng tin, phần chất lượng hiển thị chấm điểm ở tầng UI.
        if (title == "" || sourceUrl == "")
            return false;
        if (thumb == "" && gallery == "")
            return false;
        if (province == "")
            return false;

        string inspect = (title + " " + summary).ToLowerInvariant();
        if (inspect.Contains("toàn quốc") || inspect.Contains("toan quoc"))
            return false;

        return true;
    }

    public static bool IsStrictLinkedPost(LinkedFeedStore_cl.LinkedPost post)
    {
        if (post == null)
            return false;

        return IsStrictListingQualified(new LinkedFeedStore_cl.UpsertItem
        {
            Title = post.Title,
            Summary = post.Summary,
            Source = post.Source,
            SourceUrl = post.SourceUrl,
            Purpose = post.Purpose,
            PropertyType = post.PropertyType,
            PriceText = post.PriceText,
            AreaText = post.AreaText,
            ThumbnailUrl = post.ThumbnailUrl,
            GalleryCsv = post.GalleryCsv,
            PublishedAt = post.PublishedAt,
            Province = post.Province,
            District = post.District
        });
    }

    private static string ToVietnameseTitle(string input)
    {
        string v = NormalizeWhiteSpace(input);
        if (v == "")
            return "";
        string[] parts = v.Split(' ');
        for (int i = 0; i < parts.Length; i++)
        {
            string p = parts[i];
            if (p.Length <= 1)
                continue;
            parts[i] = char.ToUpperInvariant(p[0]) + p.Substring(1);
        }
        return string.Join(" ", parts).Replace("Quan", "Quận").Replace("Huyen", "Huyện");
    }

    private static string NormalizeProvinceName(string input)
    {
        string normalized = NormalizeLocationText(input);
        if (normalized == "")
            return "";

        if (normalized == "dong nai")
            return "Đồng Nai";
        if (normalized == "nghe an")
            return "Nghệ An";
        if (normalized == "ha noi" || normalized == "hanoi")
            return "Hà Nội";
        if (normalized == "da nang" || normalized == "danang")
            return "Đà Nẵng";
        if (normalized == "ho chi minh" || normalized == "tp hcm" || normalized == "tphcm" || normalized == "sai gon" || normalized == "hcm")
            return "TP.HCM";

        string mapped;
        if (ProvinceAliasMap.TryGetValue(normalized, out mapped))
            return mapped;

        return ToVietnameseTitle(input);
    }

    private static string NormalizeDistrictName(string input)
    {
        string normalized = NormalizeLocationText(input);
        if (normalized == "")
            return "";

        normalized = Regex.Replace(normalized, @"^(thanh pho|tp|quan|quận|huyen|huyện|thi xa|thị xã)\s+", "").Trim();

        switch (normalized)
        {
            case "bien hoa":
                return "Biên Hòa";
            case "long thanh":
                return "Long Thành";
            case "nhon trach":
                return "Nhơn Trạch";
            case "trang bom":
                return "Trảng Bom";
            case "vinh cuu":
                return "Vĩnh Cửu";
            case "cam my":
                return "Cẩm Mỹ";
            case "xuan loc":
                return "Xuân Lộc";
            case "dinh quan":
                return "Định Quán";
            case "tan phu":
                return "Tân Phú";
            case "thong nhat":
                return "Thống Nhất";
            case "long khanh":
                return "Long Khánh";
            case "vinh":
                return "Vinh";
            default:
                return ToVietnameseTitle(normalized);
        }
    }

    private static bool IsDongNaiDistrict(string district)
    {
        switch (NormalizeDistrictName(district))
        {
            case "Biên Hòa":
            case "Long Thành":
            case "Nhơn Trạch":
            case "Trảng Bom":
            case "Vĩnh Cửu":
            case "Cẩm Mỹ":
            case "Xuân Lộc":
            case "Định Quán":
            case "Tân Phú":
            case "Thống Nhất":
            case "Long Khánh":
                return true;
            default:
                return false;
        }
    }

    private static string NormalizeLocationText(string input)
    {
        string v = NormalizeWhiteSpace(input);
        if (v == "")
            return "";
        v = RemoveDiacritics(v).ToLowerInvariant();
        v = v.Replace("-", " ").Replace("/", " ").Replace("_", " ");
        v = Regex.Replace(v, @"\s+", " ").Trim();
        return v;
    }

    private static bool ContainsLocationToken(string normalizedText, string alias)
    {
        string a = (alias ?? "").Trim().ToLowerInvariant();
        if (a == "")
            return false;
        return normalizedText.Contains(" " + a + " ")
            || normalizedText.StartsWith(a + " ")
            || normalizedText.EndsWith(" " + a)
            || normalizedText.Equals(a, StringComparison.OrdinalIgnoreCase);
    }

    private static string RemoveDiacritics(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";
        string normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        for (int i = 0; i < normalized.Length; i++)
        {
            UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(normalized[i]);
            if (uc != UnicodeCategory.NonSpacingMark)
                sb.Append(normalized[i]);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC).Replace('đ', 'd').Replace('Đ', 'D');
    }

    private static Dictionary<string, string> BuildProvinceAliasMap()
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Action<string, string[]> add = (name, aliases) =>
        {
            for (int i = 0; i < aliases.Length; i++)
            {
                string k = NormalizeLocationText(aliases[i]);
                if (k == "")
                    continue;
                if (!map.ContainsKey(k))
                    map[k] = name;
            }
        };

        add("TP.HCM", new[] { "tp hcm", "tphcm", "ho chi minh", "sai gon", "hcm" });
        add("Hà Nội", new[] { "ha noi", "hanoi" });
        add("Đà Nẵng", new[] { "da nang", "danang" });
        add("Cần Thơ", new[] { "can tho" });
        add("Bình Dương", new[] { "binh duong" });
        add("Đồng Nai", new[] { "dong nai" });
        add("Bà Rịa - Vũng Tàu", new[] { "ba ria", "vung tau", "ba ria vung tau" });
        add("Long An", new[] { "long an" });
        add("Bình Phước", new[] { "binh phuoc" });
        add("Tây Ninh", new[] { "tay ninh" });
        add("Khánh Hòa", new[] { "khanh hoa", "nha trang" });
        add("Lâm Đồng", new[] { "lam dong", "da lat", "dalat" });
        add("Quảng Ninh", new[] { "quang ninh", "ha long", "halong" });
        add("Hải Phòng", new[] { "hai phong", "haiphong" });
        add("Bắc Ninh", new[] { "bac ninh" });
        add("Hưng Yên", new[] { "hung yen" });
        add("Vĩnh Phúc", new[] { "vinh phuc" });
        add("Thừa Thiên Huế", new[] { "hue", "thua thien hue" });
        add("Quảng Nam", new[] { "quang nam", "hoi an" });
        add("Quảng Ngãi", new[] { "quang ngai" });
        add("Bình Định", new[] { "binh dinh", "quy nhon" });
        add("Phú Yên", new[] { "phu yen" });
        add("Nghệ An", new[] { "nghe an", "vinh" });
        add("Thanh Hóa", new[] { "thanh hoa" });
        add("Ninh Bình", new[] { "ninh binh" });
        add("Hải Dương", new[] { "hai duong" });
        add("Nam Định", new[] { "nam dinh" });
        add("Thái Bình", new[] { "thai binh" });
        add("Bắc Giang", new[] { "bac giang" });
        add("Thái Nguyên", new[] { "thai nguyen" });

        return map;
    }

    private static bool IsMuabanSource(SourceConfig source)
    {
        string key = source == null ? "" : (source.Key ?? "").Trim().ToLowerInvariant();
        return key.StartsWith("muaban");
    }

    private static bool IsBatdongsanSource(SourceConfig source)
    {
        string key = source == null ? "" : (source.Key ?? "").Trim().ToLowerInvariant();
        return key.StartsWith("batdongsan");
    }

    private static bool IsDongNaiFocusSource(SourceConfig source)
    {
        string key = source == null ? "" : (source.Key ?? "").Trim().ToLowerInvariant();
        return key.Contains("dong-nai") || key.Contains("bien-hoa");
    }

    private static bool IsTrustedFilteredDongNaiSource(SourceConfig source)
    {
        return source != null
            && IsDongNaiFocusSource(source)
            && (IsMuabanSource(source) || IsBatdongsanSource(source));
    }

    private static string InferProvinceFromSource(SourceConfig source)
    {
        string key = source == null ? "" : (source.Key ?? "").Trim().ToLowerInvariant();
        if (key.Contains("dong-nai") || key.Contains("bien-hoa"))
            return "Đồng Nai";
        return "";
    }

    private static string InferDistrictFromSource(SourceConfig source)
    {
        string key = source == null ? "" : (source.Key ?? "").Trim().ToLowerInvariant();
        if (key.Contains("bien-hoa"))
            return "Biên Hòa";
        return "";
    }
}
