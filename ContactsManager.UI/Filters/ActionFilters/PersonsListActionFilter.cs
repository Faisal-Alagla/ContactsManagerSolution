using CRUD_Example.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ContactsManager.UI.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;

        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuted));

            PersonsController personsController = (PersonsController)context.Controller;

            if (context.HttpContext.Items["arguments"] is IDictionary<string, object?> parameters)
            {
                if (parameters.TryGetValue("CurrentSearchBy", out var parameter))
                {
                    personsController.ViewBag["CurrentSearchBy"] = Convert.ToString(parameter);
                }

                if (parameters.TryGetValue("CurrentSearchString", out var parameter1))
                {
                    personsController.ViewBag["CurrentSearchString"] = Convert.ToString(parameter1);
                }

                if (parameters.TryGetValue("CurrentSortBy", out var parameter2))
                {
                    personsController.ViewBag["CurrentSortBy"] = Convert.ToString(parameter2);
                }
                else
                {
                    personsController.ViewBag["CurrentSortBy"] = nameof(PersonResponse.PersonName);
                }

                if (parameters.TryGetValue("CurrentSortOrder", out var parameter3))
                {
                    personsController.ViewBag["CurrentSortOrder"] = Convert.ToString(parameter3);
                }
                else
                {
                    personsController.ViewBag["CurrentSortOrder"] = nameof(SortOrderOptions.ASC);
                }
            }

            personsController.ViewData["SearchFields"] = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryID), "Country" },
                { nameof(PersonResponse.Address), "Address" },
            };
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items["arguments"] = context.ActionArguments;

            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuting));

            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

                if (!string.IsNullOrEmpty(searchBy))
                {
                    var searchByOptions = new List<string>()
                    {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.CountryID),
                        nameof(PersonResponse.Address),
                    };

                    if (!searchByOptions.Any(temp => temp == searchBy))
                    {
                        _logger.LogInformation("searchBy actual value {searchBy}", searchBy);

                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);

                        _logger.LogInformation("searchBy updated value {searchBy}", context.ActionArguments["searchBy"]);
                    }
                }
            }
        }
    }
}
