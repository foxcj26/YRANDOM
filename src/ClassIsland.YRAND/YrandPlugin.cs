using System;
using System.Reflection;
using System.Runtime.Loader;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.YRAND.Services;
using ClassIsland.YRAND.Views.SettingsPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClassIsland.YRAND;

public class YrandPlugin : PluginBase
{
	private static readonly Assembly SelfAssembly = typeof(YrandPlugin).Assembly;

	public override void Initialize(HostBuilderContext context, IServiceCollection services)
	{
		EnsureAssemblyResolvable();
		NotificationProviderRegistryExtensions.AddNotificationProvider<YrandNotificationProvider>(services);
		string configFolder = ((PluginBase)this).PluginConfigFolder;
		services.AddHostedService((IServiceProvider _) => new YrandHostService(configFolder));
		SettingsWindowRegistryExtensions.AddSettingsPage<YrandSettingsPage>(services);
	}

	private static void EnsureAssemblyResolvable()
	{
		string selfName = SelfAssembly.GetName().Name;
		AssemblyLoadContext.Default.Resolving += (AssemblyLoadContext _, AssemblyName requested) => (!(requested.Name == selfName)) ? null : SelfAssembly;
	}
}
