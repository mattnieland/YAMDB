﻿using System.Linq.Dynamic.Core;
using YAMDB.Models;

// ReSharper disable PossibleMultipleEnumeration

namespace YAMDB.Extensions;

/// <summary>
///     Credit to https://hodo.dev/posts/post-03-dynamic-filter-ef
///     For original concept
/// </summary>
public static class QueryableExtensions
{
    private static readonly IDictionary<string, string>
        Operators = new Dictionary<string, string>
        {
            {"eq", "=="},
            {"neq", "!="},
            {"lt", "<"},
            {"lte", "<="},
            {"gt", ">"},
            {"gte", ">="},
            {"startswith", "StartsWith"},
            {"endswith", "EndsWith"},
            {"contains", "Contains"},
            {"doesnotcontain", "Contains"}
        };

    public static IList<Filter> GetAllFilters(Filter filter)
    {
        var filters = new List<Filter>();
        GetFilters(filter, filters);
        return filters;
    }

    public static IQueryable<T> ToFilterView<T>(
        this IQueryable<T> query, DynamicSearch filter)
    {
        // filter
        if (filter.Filter != null)
        {
            query = Filter(query, filter.Filter);
        }

        //sort
        if (filter.Sort != null)
        {
            query = Sort(query, filter.Sort);
        }

        if (filter.Limit != null || filter.Offset != null)
        {
            // EF does not apply skip and take without order
            query = Limit(query, filter.Limit ?? 0, filter.Offset ?? 0);
        }

        // return the final query
        return query;
    }

    public static string Transform(Filter filter, IList<Filter> filters)
    {
        //if (filter.Filters != null && filter.Filters.Any())
        //{
        //    return "(" + string.Join(" " + filter.Logic + " ",
        //        filter.Filters.Select(f => Transform(f, filters)).ToArray()) + ")";
        //}

        var index = filters.IndexOf(filter);
        var comparison = Operators[filter.Operator];
        if (filter.Operator == "doesnotcontain")
        {
            return string.Format("({0} != null && !{0}.{1}(@{2}))",
                filter.Field, comparison, index);
        }

        if (comparison is "StartsWith" or "EndsWith" or "Contains")
        {
            return string.Format("({0} != null && {0}.{1}(@{2}))",
                filter.Field, comparison, index);
        }

        return $"{filter.Field} {comparison} @{index}";
    }

    private static IQueryable<T> Filter<T>(
        IQueryable<T> queryable, Filter filter)
    {
        //if (filter is {Logic: { }})
        //{
        var filters = GetAllFilters(filter);
        var values = filters.Select(f => f.Value).ToArray();
        var where = Transform(filter, filters);
        // ReSharper disable once CoVariantArrayConversion
        queryable = queryable.Where(where, values);
        //}
        return queryable;
    }

    private static void GetFilters(Filter filter, IList<Filter> filters)
    {
        //if (filter.Filters != null && filter.Filters.Any())
        //{
        //    foreach (var item in filter.Filters)
        //    {
        //        GetFilters(item, filters);
        //    }
        //    filters.Add(filter);
        //}
        //else
        //{
        filters.Add(filter);
        //}
    }

    private static IQueryable<T> Limit<T>(IQueryable<T> queryable, int limit, int offset)
    {
        return queryable.Skip(offset).Take(limit);
    }

    private static IQueryable<T> Sort<T>(
        IQueryable<T> queryable, IEnumerable<Sort>? sort)
    {
        if (sort != null && sort.Any())
        {
            var ordering = string.Join(",", sort.Select(s => $"{s.Field} {s.Dir}"));
            return queryable.OrderBy(ordering);
        }

        return queryable;
    }
}