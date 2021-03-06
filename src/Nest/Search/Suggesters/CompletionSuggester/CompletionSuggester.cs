﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nest
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof(ReadAsTypeJsonConverter<CompletionSuggester>))]
	public interface ICompletionSuggester : ISuggester
	{
		[JsonProperty(PropertyName = "fuzzy")]
		IFuzzySuggester Fuzzy { get; set; }

		[JsonProperty("context")]
		IDictionary<string, object> Context { get; set; }
	}

	public class CompletionSuggester : SuggesterBase, ICompletionSuggester
	{
		public IFuzzySuggester Fuzzy { get; set; }
		public IDictionary<string, object> Context { get; set; }
	}

	public class CompletionSuggesterDescriptor<T> : SuggestDescriptorBase<CompletionSuggesterDescriptor<T>, ICompletionSuggester, T>, ICompletionSuggester 
		where T : class
	{
		IFuzzySuggester ICompletionSuggester.Fuzzy { get; set; }

		IDictionary<string, object> ICompletionSuggester.Context { get; set; }

		public CompletionSuggesterDescriptor<T> Fuzzy(Func<FuzzySuggestDescriptor<T>, IFuzzySuggester> selector = null) =>
			Assign(a => a.Fuzzy = selector.InvokeOrDefault(new FuzzySuggestDescriptor<T>()));

		public CompletionSuggesterDescriptor<T> Context(Func<FluentDictionary<string, object>, FluentDictionary<string, object>> selector) =>
			Assign(a => a.Context = selector?.Invoke(new FluentDictionary<string, object>()));
		
	}
}
