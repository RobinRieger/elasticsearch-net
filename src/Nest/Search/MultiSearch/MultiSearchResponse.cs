﻿using System.Collections.Generic;
using System.Linq;
using Elasticsearch.Net;
using Newtonsoft.Json;

namespace Nest
{
	[JsonObject]
	[ContractJsonConverter(typeof(MultiSearchResponseJsonConverter))]
	public class MultiSearchResponse : BaseResponse, IMultiSearchResponse
	{
		public MultiSearchResponse()
		{
			this.Responses = new Dictionary<string, object>();
		}

		public override bool IsValid => base.IsValid && this.AllResponses.All(b => b.IsValid);

		[JsonConverter(typeof(VerbatimDictionaryKeysJsonConverter))]	
		internal IDictionary<string, object> Responses { get; set; }

		public int TotalResponses => this.Responses.HasAny() ? this.Responses.Count() : 0;

		private IEnumerable<T> _allResponses<T>() where T : class, IResponse, IBodyWithApiCallDetails
		{
			foreach (var r in this.Responses.Values.OfType<T>())
			{
				r.CallDetails = this.ApiCall;
				yield return r;
			}
		}

		public IEnumerable<IResponse> AllResponses => this._allResponses<IResponse>();

		public IEnumerable<IResponse> GetInvalidResponses() => this._allResponses<IResponse>().Where(r => !r.IsValid);

		public IEnumerable<SearchResponse<T>> GetResponses<T>() where T : class => this._allResponses<SearchResponse<T>>();

		public SearchResponse<T> GetResponse<T>(string name) where T : class
		{
			object response;
			this.Responses.TryGetValue(name, out response);
			var r = response as IBodyWithApiCallDetails;
			if (r != null)
				r.CallDetails = this.ApiCall;
			return response as SearchResponse<T>;
		}
	}
}
