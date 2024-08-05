using AutoMapper;
using Infrastructure.Extensions;
using Inventory.Product.API.Entities;
using Inventory.Product.API.Extensions;
using Inventory.Product.API.Repositories.Abstraction;
using Inventory.Product.API.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.DTOs.Inventory;
using Shared.SeedWork;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Inventory.Product.API.Services
{
    public class InventoryService : MongoDbRepository<InventoryEntry>, IInventoryService
    {
        private readonly IMapper _mapper;
        public InventoryService ( IMongoClient client, MongoDbSettings settings, IMapper mapper ) : base(client, settings)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoAsync ( string itemNo )
        {
            var entities = await FindAll()
                 .Find(x => x.ItemNo.Equals(itemNo))
                 .ToListAsync();
            var result = _mapper.Map<IEnumerable<InventoryEntryDto>>(entities);

            return result;
        }

        public async Task<PagedList<InventoryEntryDto>> GetAllByItemNoPagingAsync ( GetInventoryPagingQuery query )
        {
            var filterSearchTerm = Builders<InventoryEntry>.Filter.Empty;
            var filterItemNo = Builders<InventoryEntry>.Filter.Eq(x => x.ItemNo, query.ItemNo());

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                filterSearchTerm = Builders<InventoryEntry>.Filter.Eq(x => x.DocumentNo, query.SearchTerm);
            }

            var andFilter = filterItemNo & filterSearchTerm;
            var pagedList = await Collection.PaginatedListAsync(andFilter, query.PageNumber, query.PageSize);

            var items = _mapper.Map<IEnumerable<InventoryEntryDto>>(pagedList);

            var result = new PagedList<InventoryEntryDto>(items, pagedList.GetMetaData().TotalItems, pageNumber: query.PageNumber, pageSize: query.PageSize);
            return result;
        }

        public async Task<InventoryEntryDto> GetByIdAsync ( string id )
        {
            FilterDefinition<InventoryEntry> filter = Builders<InventoryEntry>.Filter.Eq(x => x.Id, id);
            var entity = await FindAll().Find(filter).FirstOrDefaultAsync();

            var result = _mapper.Map<InventoryEntryDto>(entity);
            return result;
        }

        public async Task<InventoryEntryDto> PurchaseItemAsync ( string itemNo, PurchaseProductDto model )
        {
            var itemToAdd = new InventoryEntry(ObjectId.GenerateNewId().ToString())
            {
                ItemNo = itemNo,
                Quantity = model.Quantity,
                DocumentType = model.DocumentType
            };

            await CreateAsync(itemToAdd);
            var result = _mapper.Map<InventoryEntryDto>(itemToAdd);
            return result;
        }
    }
}
