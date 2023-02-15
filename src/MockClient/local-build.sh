docker run -v `pwd`:/code \
-v `pwd`/app:/app \
-w /code \
--rm \
mcr.microsoft.com/dotnet/sdk:7.0 \
dotnet publish -r linux-x64 -c Release -o /app