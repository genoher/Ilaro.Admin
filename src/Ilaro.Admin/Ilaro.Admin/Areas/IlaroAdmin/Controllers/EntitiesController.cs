﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.DataAnnotations;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Filters;
using Ilaro.Admin.Models;

namespace Ilaro.Admin.Areas.IlaroAdmin.Controllers
{
    [AuthorizeWrapper]
    public class EntitiesController : Controller
    {
        private readonly IIlaroAdmin _admin;
        private readonly Notificator _notificator;
        private readonly IFetchingRecords _entitiesSource;
        private readonly IConfiguration _configuration;
        private readonly IFilterFactory _filterFactory;

        public EntitiesController(
            IIlaroAdmin admin,
            Notificator notificator,
            IFetchingRecords entitiesSource,
            IConfiguration configuration,
            IFilterFactory filterFactory)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));
            if (notificator == null)
                throw new ArgumentNullException(nameof(notificator));
            if (entitiesSource == null)
                throw new ArgumentException(nameof(entitiesSource));
            if (configuration == null)
                throw new ArgumentException(nameof(configuration));
            if (filterFactory == null)
                throw new ArgumentException(nameof(filterFactory));

            _admin = admin;
            _notificator = notificator;
            _entitiesSource = entitiesSource;
            _configuration = configuration;
            _filterFactory = filterFactory;
        }

        public virtual ActionResult Index(string entityName, TableInfo tableInfo)
        {
            var entity = _admin.GetEntity(entityName);
            if (entity == null)
            {
                throw new NoNullAllowedException("entity is null");
            }
            var entityRecord = new EntityRecord(entity);
            entityRecord.Fill(Request);
            var filters = _filterFactory.BuildFilters(entityRecord).ToList();
            var pagedRecords = _entitiesSource.GetRecords(
                entity,
                filters,
                tableInfo.SearchQuery,
                tableInfo.Order,
                tableInfo.OrderDirection,
                false,
                tableInfo.Page,
                tableInfo.PerPage);
            if (pagedRecords.Records.IsNullOrEmpty() && tableInfo.Page > 1)
            {
                return RedirectToAction(
                    "Index",
                    PrepareRouteValues(
                        entityName,
                        "1",
                        filters,
                        tableInfo));
            }

            var url = Url.Action(
                "Index",
                PrepareRouteValues(
                    entityName,
                    "-page-",
                    filters,
                    tableInfo))
                .Replace("-page-", "{0}");

            var model = new EntitiesIndexModel
            {
                Data = pagedRecords.Records,
                Columns = entity.DisplayProperties
                    .Select(x => new Column(x, tableInfo.Order, tableInfo.OrderDirection)).ToList(),
                Entity = entity,
                Pager =
                    new PagerInfo(url, tableInfo.PerPage, tableInfo.Page, pagedRecords.TotalItems),
                Filters = filters.Where(x => x.DisplayInUI).ToList(),
                TableInfo = tableInfo,
                Configuration = _configuration
            };

            return View(model);
        }

        public virtual ActionResult Changes(string entityName, string key, TableInfo tableInfo)
        {
            var entityChangesFor = _admin.GetEntity(entityName);
            if (entityChangesFor == null)
            {
                throw new NoNullAllowedException("entity is null");
            }
            var changeEntity = _admin.ChangeEntity;
            var entityRecord = new EntityRecord(changeEntity);
            entityRecord.Fill(Request);
            var filters = _filterFactory.BuildFilters(entityRecord).ToList();
            if (key.IsNullOrWhiteSpace() == false)
            {
                filters.Add(new ForeignEntityFilter(changeEntity["EntityKey"], key));
            }
            filters.Add(new ChangeEntityFilter(changeEntity["EntityName"], entityName));
            var pagedRecords = _entitiesSource.GetRecords(
                changeEntity,
                filters,
                tableInfo.SearchQuery,
                tableInfo.Order,
                tableInfo.OrderDirection,
                false,
                tableInfo.Page,
                tableInfo.PerPage);
            if (pagedRecords.Records.IsNullOrEmpty() && tableInfo.Page > 1)
            {
                return RedirectToAction(
                    "Changes",
                    PrepareRouteValues(
                        entityName,
                        "1",
                        filters,
                        tableInfo));
            }

            var url = Url.Action(
                "Changes",
                PrepareRouteValues(
                    entityName,
                    "-page-",
                    filters,
                    tableInfo))
                .Replace("-page-", "{0}");
            var model = new EntitiesChangesModel
            {
                Data = pagedRecords.Records,
                Columns = changeEntity.DisplayProperties
                    .Select(x => new Column(x, tableInfo.Order, tableInfo.OrderDirection)).ToList(),
                Entity = changeEntity,
                EntityChangesFor = entityChangesFor,
                Pager =
                    new PagerInfo(url, tableInfo.PerPage, tableInfo.Page, pagedRecords.TotalItems),
                Filters = filters.Where(x => x.DisplayInUI).ToList(),
                TableInfo = tableInfo,
                Configuration = _configuration,
                Key = key
            };

            return View(model);
        }

        protected virtual RouteValueDictionary PrepareRouteValues(
            string entityName,
            string page,
            IEnumerable<BaseFilter> filters,
            TableInfo tableInfo)
        {
            var routeValues = new Dictionary<string, object>
            {
                { "entityName", entityName },
                { _configuration.PageRequestName, page },
                { _configuration.PerPageRequestName, tableInfo.PerPage }
            };

            if (!tableInfo.SearchQuery.IsNullOrEmpty())
            {
                routeValues.Add(_configuration.SearchQueryRequestName, tableInfo.SearchQuery);
            }

            if (!tableInfo.Order.IsNullOrEmpty() && !tableInfo.OrderDirection.IsNullOrEmpty())
            {
                routeValues.Add(_configuration.OrderRequestName, tableInfo.Order);
                routeValues.Add(_configuration.OrderDirectionRequestName, tableInfo.OrderDirection);
            }

            foreach (var filter in filters.Where(x => x.DisplayInUI && !x.Value.IsNullOrEmpty()))
            {
                routeValues.Add(filter.Property.Name, filter.Value);
            }

            return new RouteValueDictionary(routeValues);
        }
    }
}
