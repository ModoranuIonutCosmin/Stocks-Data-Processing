# Stocks research SaaS.

## Summary
A web software solution that gives users convenient ways of:
* Researching stock market opportunities using Machine Learning driven algorithms for technical analysis including prices prediction for next period.
* Tracking stock market information such as up-to-date current price data as well as historical data
* Simulating opening a virtual transaction and then checking it's profitability as well as other standard economic metric

## Demo
A *short* summary of all features is presented in the video below.
If you want to try the app yourself you can access [this](https://bit.ly/3tu3orN) deployed version which
may be slow at first operation due to it being hosted on a free development deployment slot,
app having to cold-start.

See the video below:

[![Demo](https://i.imgur.com/2qCXI3a.png)](https://bit.ly/3irf9sL "Stocks research")

Alt: [click](https://bit.ly/3irf9sL)

## Technologies

### Back end
- Web API using ASP .NET Core 6.
- SQL Server and Entity Framework 5 for Persistance Layer
- Scheduled tasks using Quartz.NET (locally) and Azure Functions for Production environment.
- ML.NET for taking advantage of SSA algorithm and also several regression algorithms by preproccesing time series datasets to tabular forms.
- ASP .NET Identity for Authentication and Authorization module.
- HtmlAgilityPack for web scraping.

### Front end
- Angular 13
- PWA for Angular
- Angular Material UI
- Apex Charts for financial charts

## Architecture
* WebAPI hosted on Azure App service.
* 3 Functions hosted on Azure Functions Apps.
* Azure SQL.
* Angular app on Azure Static Web app service with custom domain.

### Back end
Organized similar to N-Tier architecture. 
* Domain project (Stocks.General) 
  - entities.
  - DTOs.
  - exceptions.
  - helpers.

* Application layer (StocksFinalSolution.BusinessLogic):
  - services interfaces and their implementation.
  - exposes infrastracture contracts (email providers interfaces, persistance layer repositories interfaces).
  - AutoMapper for swapping contracts between DB / models.

* Infrastracture layer (StocksProccesing.Relational):
  - Repository pattern.
  - Persistance provider versioning
   - registering a new ORM / SQL driver other than EF & SQL Server requires just creating a new implementation for some basic CRUD operations.
  - Utilizes repository pattern for accesing resources grouped by the DB Relation it queries/modifies.
  - Implements email services using SendInBlue SMTP.

* API Presentation layer(StocksProcessing.API):
  - Uses JWT with Identity for Authorization / Authentication.
  - API Versioning.
  - ProblemDetails exceptions middleware.

* Scheduled tasks (Stocks-Data-Processing project):
  - Autofac Dependency Injection.
  - Quartz.NET (locally).
  - Azure functions for cron tasks (production).

### Tasks:
- A web scraper for updating stock data.
- A predictions generator using ML.NET
- A job that generates summaries of large amounts of prices data for fast API info access
- A job that monitors tasks that have a stop loss treshold and automatically closes those exceeding user-set bounds.
- A job that collects "taxes" for leveraged trades.

### Front end
Using a highly scalable standard folder structure.
* Core consists of Services, Interceptors, Directives, Pipes.
* Shared module for models and common components.
* All specific components are organized in modules.
  - lazy loading modules for reducing bundle size.
