namespace AppListWebAPI.Models
{
    public partial class BTAppLaunchRecord
    {
        public const int PLATFORM_UNKNOW = -1;
        public const int PLATFORM_IOS = 0;
        public const int PLATFORM_ANDROID = 1;
        public const int PLATFORM_WINDOWS = 2;
        public const int PLATFORM_MACOS = 3;
        public const int PLATFORM_LINUX = 4;
        public const string CHANNEL_UNKNOW = "UNKNOW";
        public const string CHANNEL_APP_STORE = "APPSTORE";
        public const string CHANNEL_GOOGLE_PLAY = "GOOGLEPLAY";
        public const string CHANNEL_MS_MARKET = "MSMARKET";
        public const string CHANNEL_TAP_TAP = "TAPTAP";
    }

    public partial class BTAppLaunchRecord
    {
        public long ID { get; set; }
        public string DeviceId { get; set; }
        public int Platform { get; set; }
        public string UniqueId { get; set; }
        public string Channel { get; set; }
        public double LaunchDateTs { get; set; }
        public string BundleId { get; set; }
        public string UrlSchemes { get; set; }
    }

    public partial class BTAppLaunchRecord
    {
        public static void OnDbContextModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BTAppLaunchRecord>(ac =>
            {
                ac.HasKey(e => e.ID);
                ac.Property(e => e.DeviceId);
                ac.Property(e => e.Platform);
                ac.Property(e => e.UniqueId);
                ac.Property(e => e.Channel);
                ac.Property(e => e.LaunchDateTs);
                ac.Property(e => e.BundleId);
                ac.Property(e => e.UrlSchemes);
            });
        }
    }
}