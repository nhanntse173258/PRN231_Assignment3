using Grpc.Core;
using GrpcServer.Models;
using GrpcServer.Repository;

namespace GrpcServer.Services
{
    public class ItemServiceImpl : ItemService.ItemServiceBase
    {
        private readonly ItemRepository _itemRepository;

        public ItemServiceImpl(ItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public override Task<ItemResponse> GetItem(GetItemRequest request, ServerCallContext context)
        {
            var item = _itemRepository.GetItem(request.Id);
            if (item == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Item not found."));
            }

            return Task.FromResult(new ItemResponse
            {
                Item = new Item
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description
                }
            });
        }

        public override Task<ListItemsResponse> ListItems(ListItemsRequest request, ServerCallContext context)
        {
            var items = _itemRepository.GetAllItems();
            var response = new ListItemsResponse();
            response.Items.AddRange(items.Select(item => new Item
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description
            }));

            return Task.FromResult(response);
        }

        public override Task<ItemResponse> CreateItem(CreateItemRequest request, ServerCallContext context)
        {
            var newItem = new Item
            {
                Name = request.Name,
                Description = request.Description
            };

            _itemRepository.AddItem(newItem);
            return Task.FromResult(new ItemResponse
            {
                Item = new Item
                {
                    Id = newItem.Id,
                    Name = newItem.Name,
                    Description = newItem.Description
                }
            });
        }

        public override Task<ItemResponse> UpdateItem(UpdateItemRequest request, ServerCallContext context)
        {
            var itemToUpdate = new Item
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description
            };

            _itemRepository.UpdateItem(itemToUpdate);
            return Task.FromResult(new ItemResponse
            {
                Item = itemToUpdate
            });
        }

        public override Task<DeleteItemResponse> DeleteItem(DeleteItemRequest request, ServerCallContext context)
        {
            _itemRepository.DeleteItem(request.Id);
            return Task.FromResult(new DeleteItemResponse { Success = true });
        }
    }
}

