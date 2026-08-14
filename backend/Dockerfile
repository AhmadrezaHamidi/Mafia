FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# csproj ها را جدا کپی می‌کنیم تا لایه‌ی restore کش شود
COPY ["Src/Host/Ahmad.Mafia.ServiceHost/Ahmad.Mafia.ServiceHost.csproj",                     "Src/Host/Ahmad.Mafia.ServiceHost/"]
COPY ["Src/Rest/Ahmad.Mafia.API.Rest/Ahmad.Mafia.API.Rest.csproj",                           "Src/Rest/Ahmad.Mafia.API.Rest/"]
COPY ["Src/Application/Ahmad.Mafia.Application/Ahmad.Mafia.Application.csproj",              "Src/Application/Ahmad.Mafia.Application/"]
COPY ["Src/Application/Ahmad.Mafia.Application.Contract/Ahmad.Mafia.Application.Contract.csproj", "Src/Application/Ahmad.Mafia.Application.Contract/"]
COPY ["Src/Application/Ahmad.Mafia.Application.Query/Ahmad.Mafia.Application.Query.csproj",  "Src/Application/Ahmad.Mafia.Application.Query/"]
COPY ["Src/Application/Ahmad.Mafia.Application.Query.Contract/Ahmad.Mafia.Application.Query.Contract.csproj", "Src/Application/Ahmad.Mafia.Application.Query.Contract/"]
COPY ["Src/Application/Ahmad.Mafia.Application.Validators/Ahmad.Mafia.Application.Validators.csproj", "Src/Application/Ahmad.Mafia.Application.Validators/"]
COPY ["Src/Application/Ahmad.Mafia.Event.Handler/Ahmad.Mafia.Event.Handler.csproj",          "Src/Application/Ahmad.Mafia.Event.Handler/"]
COPY ["Src/Domain/Ahmad.Mafia.Domain/Ahmad.Mafia.Domain.csproj",                             "Src/Domain/Ahmad.Mafia.Domain/"]
COPY ["Src/Domain/Ahmad.Mafia.Event/Ahmad.Mafia.Event.csproj",                               "Src/Domain/Ahmad.Mafia.Event/"]
COPY ["Src/Infrastructure/Ahmad.Mafia.API.Config/Ahmad.Mafia.API.Config.csproj",             "Src/Infrastructure/Ahmad.Mafia.API.Config/"]
COPY ["Src/Infrastructure/Ahmad.Mafia.Persistence.EF/Ahmad.Mafia.Persistence.EF.csproj",     "Src/Infrastructure/Ahmad.Mafia.Persistence.EF/"]
COPY ["Src/Infrastructure/Ahmad.Mafia.Read.Dapper/Ahmad.Mafia.Read.Dapper.csproj",           "Src/Infrastructure/Ahmad.Mafia.Read.Dapper/"]
COPY ["Src/Host/Ahmad.Mafia.Option/Ahmad.Mafia.Option.csproj",                               "Src/Host/Ahmad.Mafia.Option/"]

# پکیج‌های AhmadBase از فولدر لوکال می‌آیند، پس باید قبل از restore حاضر باشند
COPY ["nuget.config", "."]
COPY ["Nuget/",       "Nuget/"]

RUN dotnet restore "Src/Host/Ahmad.Mafia.ServiceHost/Ahmad.Mafia.ServiceHost.csproj"

COPY . .
RUN dotnet publish "Src/Host/Ahmad.Mafia.ServiceHost/Ahmad.Mafia.ServiceHost.csproj" \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Ahmad.Mafia.ServiceHost.dll"]
