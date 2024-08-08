using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using EventBus.Messages.IntegrationEvents.Events;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketsController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly StockItemGrpcService _stockItemGrpcService;

        public BasketsController ( IBasketRepository basketRepository, IMapper mapper = null, IPublishEndpoint publishEndpoint = null, StockItemGrpcService stockItemGrpcService = null )
        {
            _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            //_stockItemGrpcService = stockItemGrpcService ?? throw new ArgumentNullException(nameof(stockItemGrpcService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _stockItemGrpcService = stockItemGrpcService ?? throw new ArgumentNullException(nameof(stockItemGrpcService));
        }
        [HttpGet("{username}", Name = "GetBasket")]
        [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Cart>> GetBasketByUsername ( [Required] string username )
        {
            var result = await _basketRepository.GetBasketByUserName(username);
            return Ok(result ?? new Cart());
        }

        [HttpPost(Name = "UpdateBasket")]
        [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Cart>> UpdateBasket ( [FromBody] Cart cart )
        {
            //Communicate with Inventory.Grpc and check quantity avaiable of products
            foreach (var item in cart.Items)
            {
                var stock = await _stockItemGrpcService.GetStock(item.ItemNo);
                item.SetAvailableQuantity(stock.Quantity);
            }

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.UtcNow.AddHours(1))
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));
            var result = await _basketRepository.UpdateBasket(cart, options);
            return Ok(result);
        }

        [HttpDelete(Name = "DeleteBasket")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> DeleteBasket ( [Required] string username )
        {
            var result = await _basketRepository.DeleteBasketFromUserName(username);
            return Ok(result);
        }
        [Route("[action]/{username}")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout ( [Required] string username, [FromBody] BasketCheckout basketCheckout )
        {
            var basket = await _basketRepository.GetBasketByUserName(username);
            if (basket == null || !basket.Items.Any()) return NotFound();

            //publish checkout event to EventBus Message
            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMessage);
            await _basketRepository.DeleteBasketFromUserName(username);

            return Accepted();
        }
    }
}
