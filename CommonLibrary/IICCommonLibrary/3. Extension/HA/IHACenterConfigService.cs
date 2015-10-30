using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Google.ProtoBuf;

using Imps.Services.CommonV4;
using Imps.Services.CommonV4.Configuration;

namespace Imps.Services.HA
{
	[RpcService("HACenterConfigService")]
	public interface IHACenterConfigService
	{
		[RpcServiceMethod(ArgsType = typeof(HAGetConfigArgs), ResultType = typeof(HAServiceSettings))]
		void LoadServiceSettings(RpcServerContext context);

		//
		// 获取服务配置: 一对一 不用Like
		[RpcServiceMethod(ArgsType = typeof(HAGetConfigArgs), ResultType = typeof(IICConfigFieldBuffer))]
		void LoadConfigField(RpcServerContext context);

		//
		// 获取服务配置: 需要 ConfigPath like 'Key%'
		[RpcServiceMethod(ArgsType = typeof(HAGetConfigArgs), ResultType = typeof(List<IICConfigItemBuffer>))]
		void LoadConfigSection(RpcServerContext context);

		//
		// 获取服务配置: 需要 ConfigPath like 'Key%'
		[RpcServiceMethod(ArgsType = typeof(HAGetConfigArgs), ResultType = typeof(List<IICConfigItemBuffer>))]
		void LoadConfigItem(RpcServerContext context);

        //
        // 获取服务配置: 获取CodeTableBuffer
        [RpcServiceMethod(ArgsType = typeof(HAGetConfigArgs), ResultType = typeof(RpcClass<string>))]
        void LoadConfigText(RpcServerContext context);

		//
		// 获取服务配置: 获取CodeTableBuffer
		[RpcServiceMethod(ArgsType = typeof(HAGetConfigArgs), ResultType = typeof(IICConfigTableBuffer))]
		void LoadConfigTable(RpcServerContext context);

		//
		// 获取CodeTable版本号, 用于轮询
        [RpcServiceMethod(ArgsType = typeof(HAGetTableVersionArgs), ResultType = typeof(Dictionary<string, DateTime>))]
		void LoadCodeTableVersions(RpcServerContext context);

		//
		// 获取配置版本号
		[RpcServiceMethod(ArgsType = typeof(List<HAGetConfigVersionArgs>), ResultType = typeof(List<HAGetConfigVersionResults>))]
		void LoadConfigVersions(RpcServerContext context);
		//
		// 更新SoftBalance表, 用于CS更新自己的动态负载
		[RpcServiceMethod(ArgsType = typeof(UpdateSoftBalanceTableArgs), ResultType = null)]
		void UpdateSoftBalanceTable(RpcServerContext context);
	}
}
