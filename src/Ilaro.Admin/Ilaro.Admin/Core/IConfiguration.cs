﻿namespace Ilaro.Admin.Core
{
    public interface IConfiguration
    {
        int ItemsQuantityPerPage { get; }

        string PageRequestName { get; }
        string SearchQueryRequestName { get; }
        string PerPageRequestName { get; }
        string OrderRequestName { get; }
        string OrderDirectionRequestName { get; }
    }
}