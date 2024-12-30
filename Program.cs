using System.Collections;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", (HttpRequest req) => Dump(req));

app.Run();


string Dump(HttpRequest req) {
	const string PREFIX = "    ";
	var sb = new StringBuilder();
	sb.Append("URL: ").Append(req.GetDisplayUrl()).AppendLine();
	sb.AppendLine();
	sb.AppendLine("== HEADERS ==").AppendLine();
	foreach (KeyValuePair<string, StringValues> header in req.Headers) {
		sb.AppendLine(header.Key);
		foreach (var value in header.Value) {
			sb.Append(PREFIX).AppendLine(value);
		}
	}

	sb.AppendLine();
	sb.AppendLine("== ENVIRONMENT VARIABLES ==");
	sb.AppendLine();
	foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables()) {
		sb.Append(entry.Key).AppendLine();
		sb.Append(PREFIX).Append(entry.Value).AppendLine();
	}
	return sb.ToString();

}


public class IgnoreErrorPropertiesResolver : DefaultContractResolver {
	private static string[] ignoreList = [
		"ReadTimeout"
	];

	protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
		var property = base.CreateProperty(member, memberSerialization);
		if (ignoreList.Contains(property.PropertyName)) property.Ignored = true;
		return property;
	}
}