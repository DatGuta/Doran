FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8000
ENV ASPNETCORE_URLS=http://+:8000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Api/DR.WebManApi/DR.WebManApi.csproj", "src/Api/DR.WebManApi/"]
COPY ["src/Business/DR.Application/DR.Application.csproj", "src/Business/DR.Application/"]
COPY ["src/CommonLibrary/DR.Database/DR.Database.csproj", "src/CommonLibrary/DR.Database/"]
COPY ["src/CommonLibrary/DR.Constant/DR.Constant.csproj", "src/CommonLibrary/DR.Constant/"]
COPY ["src/CommonLibrary/DR.Helper/DR.Helper.csproj", "src/CommonLibrary/DR.Helper/"]
COPY ["src/CommonLibrary/DR.Attributes/DR.Attributes.csproj", "src/CommonLibrary/DR.Attributes/"]
COPY ["src/CommonLibrary/DR.Configuration/DR.Configuration.csproj", "src/CommonLibrary/DR.Configuration/"]
COPY ["src/CommonLibrary/DR.Models/DR.Models.csproj", "src/CommonLibrary/DR.Models/"]
COPY ["src/CommonLibrary/DR.Message/DR.Message.csproj", "src/CommonLibrary/DR.Message/"]
COPY ["src/CommonLibrary/DR.Resource/DR.Resource.csproj", "src/CommonLibrary/DR.Resource/"]
COPY ["src/CommonLibrary/DR.Telegram/DR.Telegram.csproj", "src/CommonLibrary/DR.Telegram/"]
COPY ["src/CommonLibrary/DR.Common/DR.Common.csproj", "src/CommonLibrary/DR.Common/"]
COPY ["src/CommonLibrary/DR.Contexts/DR.Contexts.csproj", "src/CommonLibrary/DR.Contexts/"]
COPY ["src/Business/DR.Domain/DR.Domain.csproj", "src/Business/DR.Domain/"]
COPY ["src/CommonLibrary/DR.Redis/DR.Redis.csproj", "src/CommonLibrary/DR.Redis/"]
COPY ["src/Business/DR.Export/DR.Export.csproj", "src/Business/DR.Export/"]
RUN dotnet restore "src/Api/DR.WebManApi/DR.WebManApi.csproj"
COPY . .
WORKDIR "/src/src/Api/DR.WebManApi"
RUN dotnet build "DR.WebManApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DR.WebManApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DR.WebManApi.dll"]
