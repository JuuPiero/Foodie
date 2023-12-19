using Microsoft.EntityFrameworkCore;

using AppCore.Data;
using AppCore.Models;
using AppCore.Models.ViewModels;

namespace AppCore.App.Repositories;

public class OrderRepository : IOrderRepository {

    private readonly ApplicationDbContext _dbContext;

    public OrderRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<Order> GetOrderByIdAsync(int orderId) {
        return await _dbContext.Orders.FindAsync(orderId);
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _dbContext.Orders.ToListAsync();
    }
    public async Task<IEnumerable<OrderRequest>> GetAllOrdersJoinAsync() {
        var result = await (from order in _dbContext.Orders
                        join user in _dbContext.Users on order.UserId equals user.Id
                        join paymentMethod in _dbContext.PaymentMethods on order.PaymentMethodId equals paymentMethod.PaymentMethodId
                        select new OrderRequest {
                            OrderId = order.OrderId,
                            ReceiverName = order.ReceiverName,
                            ReceiverPhoneNumber = order.ReceiverPhoneNumber,
                            // UserName = user.UserName,
                            PaymentMethodName = paymentMethod.Name,
                            //Thêm các trường khác tùy thuộc vào yêu cầu của bạn
                        }).ToListAsync();
        return result;
    }
    public async Task AddOrderAsync(Order order) {
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateOrderAsync(Order order) {
        _dbContext.Entry(order).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteOrderAsync(int orderId) {
        var order = await _dbContext.Orders.FindAsync(orderId);
        if (order != null) {
            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }
 
    Task<IEnumerable<Order>> IOrderRepository.GetAllOrdersAsync() {
        throw new NotImplementedException();
    }

}