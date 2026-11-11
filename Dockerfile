# =====================================================
# Stage 1: build — نبني المشروع بصورة فيها .NET SDK الكامل
# =====================================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# ننسخ ملفات المشاريع أول، عشان Docker يحفظ نتيجة restore
# وما يعيد تنزيل الحزم كل مرة بتغيّر فيها الكود
COPY HeritageAI.API/HeritageAI.API.csproj HeritageAI.API/
COPY HeritageAI.Application/HeritageAI.Application.csproj HeritageAI.Application/
COPY HeritageAI.Domain/HeritageAI.Domain.csproj HeritageAI.Domain/
COPY HeritageAI.Infrastructure/HeritageAI.Infrastructure.csproj HeritageAI.Infrastructure/
COPY HeritageAI.Persistence/HeritageAI.Persistence.csproj HeritageAI.Persistence/
RUN dotnet restore HeritageAI.API/HeritageAI.API.csproj

# بعدين ننسخ باقي الكود ونبني نسخة Release
COPY . .
RUN dotnet publish HeritageAI.API/HeritageAI.API.csproj -c Release -o /app/publish --no-restore

# =====================================================
# Stage 2: runtime — صورة صغيرة فيها بس اللي بيحتاجه التشغيل
# =====================================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# مجلد رفع صور المنتجات، وصلاحية الكتابة عليه للمستخدم app
RUN mkdir -p /app/wwwroot/uploads/products && chown -R $APP_UID /app/wwwroot/uploads

# نشغّل التطبيق كمستخدم عادي، مش root (أأمن)
USER $APP_UID

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "HeritageAI.API.dll"]
