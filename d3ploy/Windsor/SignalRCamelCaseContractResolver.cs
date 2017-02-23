using Microsoft.AspNet.SignalR.Infrastructure;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace d3ploy.Windsor
{
	public class SignalRCamelCaseContractResolver : IContractResolver
	{
		private readonly Assembly assembly;
		private readonly IContractResolver camelCaseContractResolver;
		private readonly IContractResolver defaultContractSerializer;

		public SignalRCamelCaseContractResolver()
		{
			defaultContractSerializer = new DefaultContractResolver();
			camelCaseContractResolver = new CamelCasePropertyNamesContractResolver();
			assembly = typeof(Connection).Assembly;
		}

		public JsonContract ResolveContract(Type type)
		{
			if (type.Assembly.Equals(assembly))
			{
				return defaultContractSerializer.ResolveContract(type);
			}

			return camelCaseContractResolver.ResolveContract(type);
		}

	}
}