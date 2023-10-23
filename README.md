## Aspnet Core Microservices:

**Development Environment:**
1. Using docker-compose
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --remove-orphans
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --remove-orphans --build
```
- Stop & Removing:
```Powershell
docker-compose down
```
##Docker application urrls - local ENV
- Portainer : http://localhost:9000/ - admin - Climax!@#
- Kibana http://localhost:5601 - elastic - admin
- RabbitMQ http://localhost:15672 - guest - guest

##Application urls - Local
- Product.API : http://localhost:6002/api/products


VS 2022
- ASPNETCORE_ENVIRONMENT=Production dotnet ef database update
- dotnet watch run --environment "Development"
- dotnet restore


--Code First Product DB :
dotnet ef migrations add "Init_ProductDB"
dotnet ef migrations add "Product_Set_No_IsUnique"
dotnet ef database update
--Gen ordering DB - Migration Commands - Clean Achitecture

CD Into Ordering Folder

dotnet ef migrations add  "Init_OrderDB" --project Ordering.Infrastructure --startup-project Ordering.API --output-dir Persistence/Migrations

dotnet ef migrations remove --project Ordering.Infrastructure --startup-project Ordering.API 

dotnet ef database update --project Ordering.Infrastructure --startup-project Ordering.API 


** Secret 
PASS GG Mail : itesydmowkobhakx folbjoonrforfabg