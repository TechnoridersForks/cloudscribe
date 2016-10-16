﻿// Copyright (c) Source Tree Solutions, LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0
// Author:					Joe Audette
// Created:					2016-10-14
// Last Modified:			2016-10-14
// 

using cloudscribe.Core.Models;
using cloudscribe.Core.Models.Generic;
using cloudscribe.Core.IdentityServerIntegration.StorageModels;
using IdentityServer4.Models;
using NoDb;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.Core.IdentityServer.NoDb
{
    public class ScopeQueries : IScopeQueries
    {
        public ScopeQueries(
           // SiteContext site,
            IBasicQueries<Scope> queries
            )
        {
          //  _siteId = site.Id.ToString();
            _queries = queries;
        }

        private IBasicQueries<Scope> _queries;
        //private string _siteId;

        private async Task<List<Scope>> GetAllScopes(string siteId)
        {
            //TODO: cache
            var allScopes = await _queries.GetAllAsync(siteId).ConfigureAwait(false);

            return allScopes.ToList();
        }

        public async Task<bool> ScopeExists(string siteId, string scopeName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var scope = await FetchScope(siteId, scopeName, cancellationToken).ConfigureAwait(false);
            return (scope != null);
        }

        public async Task<Scope> FetchScope(string siteId, string scopeName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _queries.FetchAsync(siteId, scopeName, cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> CountScopes(string siteId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var allScopes = await GetAllScopes(siteId).ConfigureAwait(false);
            return allScopes.Count;
        }

        public async Task<PagedResult<Scope>> GetScopes(
            string siteId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var all = await GetAllScopes(siteId).ConfigureAwait(false);

            int offset = (pageSize * pageNumber) - pageSize;

            var result = new PagedResult<Scope>();
            result.TotalItems = all.Count;
            result.Data = all
                .OrderBy(x => x.Type).ThenBy(x => x.Name)
                .Skip(offset)
                .Take(pageSize).ToList();

            return result;

        }
    }
}
