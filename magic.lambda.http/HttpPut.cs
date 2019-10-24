﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019, thomas@gaiasoul.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using System.Threading.Tasks;
using magic.node;
using magic.http.contracts;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.lambda.http
{
    // TODO: Implement support for generic headers once a release of magic.http has been created.
    /// <summary>
    /// Invokes the HTTP PUT verb towards some resource.
    /// </summary>
    [Slot(Name = "http.put")]
    [Slot(Name = "wait.http.put")]
    public class HttpPut : ISlot, ISlotAsync
    {
        readonly IHttpClient _httpClient;

        /// <summary>
        /// Creates a new instance of your class.
        /// </summary>
        /// <param name="httpClient">HTTP client to use for invocation.</param>
        public HttpPut(IHttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Implementation of your slot.
        /// </summary>
        /// <param name="signaler">Signale rthat raised the signal.</param>
        /// <param name="input">Arguments to your slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            if (input.Children.Count() > 2 || input.Children.Any(x => x.Name != "token" && x.Name != "payload"))
                throw new ArgumentException("[http.put.json] can only handle one [token] and one [payload] child node");

            var url = input.GetEx<string>();
            var token = input.Children.FirstOrDefault(x => x.Name == "token")?.GetEx<string>();
            var payload = input.Children.FirstOrDefault(x => x.Name == "payload")?.GetEx<string>() ??
                throw new ArgumentException("No [payload] supplied to [http.put]");

            // Invoking endpoint, passing in payload, and returning result as value of root node.
            var response = token == null ?
                _httpClient.PutAsync<string, string>(url, payload).Result :
                _httpClient.PutAsync<string, string>(url, payload, token).Result;
            Common.CreateResponse(input, response);
        }

        /// <summary>
        /// Implementation of your slot.
        /// </summary>
        /// <param name="signaler">Signaler that raised the signal.</param>
        /// <param name="input">Arguments to your slot.</param>
        /// <returns>An awaitable task.</returns>
        public async Task SignalAsync(ISignaler signaler, Node input)
        {
            if (input.Children.Count() > 2 || input.Children.Any(x => x.Name != "token" && x.Name != "payload"))
                throw new ApplicationException("[http.put.json] can only handle one [token] and one [payload] child node");

            var url = input.GetEx<string>();
            var token = input.Children.FirstOrDefault(x => x.Name == "token")?.GetEx<string>();
            var payload = input.Children.FirstOrDefault(x => x.Name == "payload")?.GetEx<string>() ??
                throw new ArgumentException("No [payload] supplied to [http.put]");

            // Invoking endpoint, passing in payload, and returning result as value of root node.
            Response<string> response;
            if (token == null)
                response = _httpClient.GetAsync<string>(url).Result;
            else
                response = _httpClient.GetAsync<string>(url, token).Result;
            Common.CreateResponse(input, response);
        }
    }
}
