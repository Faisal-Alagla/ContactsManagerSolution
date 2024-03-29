﻿using ContactsManager.Core.ServiceContracts;
using CRUD_Example.Filters.ResultFilters;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using Services;
using ContactsManager.Core.ServiceContracts.PersonsServiceContracts;
using ContactsManager.Core.Services.PersonsServices;
using ContactsManager.Core.Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ContactsManager.UI.Filters.ActionFilters;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_Example
{
    public static class ConfigureServicesExtension
    {
        //extension method to extend the IServiceCollection type
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllersWithViews(options =>
            {
                //adding a global filter
                //if we're not supplying any parameters to the filter class, we can use this and provide the order as following
                //options.Filters.Add<ResponseHeaderActionFilter>(5);

                //var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

                //order = 2 (IOrderedFilter)
                options.Filters.Add(new ResponseHeaderActionFilter(/*logger,*/ "Some-Key", "Some-Value", 2));

                //Applying AntiForgeryToken globally for all post requests
                //note: use AutoValidateAntiforgeryTokenAttribute instead of ValidateAntiforgeryTokenAttribute to avoid using it on get requests
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            //adding services into IoC container
            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();
            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<IPersonsGetterService, PersonsGetterServiceWithFewerExcelFields>();
            //services.AddScoped<IPersonsGetterService, PersonsGetterServiceChild>();
            services.AddScoped<PersonsGetterService, PersonsGetterService>();
            services.AddScoped<IPersonsAdderService, PersonsAdderService>();
            services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
            services.AddScoped<IPersonsSorterService, PersonsSorterService>();
            services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
            services.AddTransient<PersonsListResultFilter>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            //In case of using ServiceFilter instead of TypeFilter
            //Here can decide transient / scoped / singleton.. TypeFilter is Transient by default
            //services.AddTransient<TokenResultFilter>();

            //Have to do this for IFilterFactory (check its file)

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredUniqueChars = 4;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
                .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

            services.AddAuthorization(options =>
            {
                //enforce authorization policy (user must be logged in) for all action methods
                options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

                //custom policy
                options.AddPolicy("NotAuthenticated", policy =>
                {
                    //when applied to a controller / action method, it allows non-logged in users only
                    //e.g. can be used for login / register
                    policy.RequireAssertion(context =>
                    {
                        //return true if the user is not logged in
                        return context.User.Identity.IsAuthenticated is false;
                    });
                });
            });
            //incase the user isn't authenticated, redirect to login:
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            });

            services.AddHttpLogging(options =>
            {
                //http logging options here
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties;
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
            });

            return services;
        }
    }
}
