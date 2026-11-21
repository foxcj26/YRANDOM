using ClassIsland.Core.Abstractions.Services.NotificationProviders;
using ClassIsland.Core.Attributes;

namespace ClassIsland.YRAND.Services;

[NotificationProviderInfo("6C2E8F41-9A73-4B05-8D2E-3F6C1A94B7D8", "YRANDOM", "\uEAFC", "YRANDOM 随机点名：抽到人之后在主界面上显示名字。")]
public class YrandNotificationProvider : NotificationProviderBase
{
}
