using System.Collections.Generic;
using System.Threading.Tasks;
using WorkoutApp.Models;
using WorkoutApp.Repository;

namespace WorkoutApp.Service
{
    class OrderDetailService : IService<OrderDetail>
    {
        private readonly IRepository<OrderDetail> orderDetailRepository;

        public OrderDetailService(IRepository<OrderDetail> orderDetailRepository)
        {
            this.orderDetailRepository = orderDetailRepository;
        }

        public async Task<OrderDetail> CreateAsync(OrderDetail entity)
        {
            return await this.orderDetailRepository.CreateAsync(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await this.orderDetailRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<OrderDetail>> GetAllAsync()
        {
            return await this.orderDetailRepository.GetAllAsync();
        }

        public async Task<OrderDetail> GetByIdAsync(int id)
        {
            return await this.orderDetailRepository.GetByIdAsync(id);
        }

        public async Task<OrderDetail> UpdateAsync(OrderDetail entity)
        {
            return await this.orderDetailRepository.UpdateAsync(entity);
        }

        private async Task addOrderDetail(int OrderID, int ProductID, int Quantity, double Price)
        {
            //call creasteAsync(OrderDetail)
            OrderDetail newOrderDetail = new OrderDetail(0, OrderID, ProductID, Quantity, Price, true);
            await this.orderDetailRepository.CreateAsync(newOrderDetail);
        }
    }
}
