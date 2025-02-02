namespace TasAndJet.Api.Entities.Services;

/// <summary>
///Сущность услуги, которая будет в заказе
/// </summary>
public class Service
{
    private Service()
    {
    }

    private Service(Guid id, string title, decimal cost, Vehicle vehicle,ServiceType serviceType)
    {
        Id = id;
        Title = title;
        Cost = cost;
        Vehicle = vehicle;
        ServiceType = serviceType;
    }

    public Guid Id { get; set; }
    public string Title { get; set; } 
    public decimal Cost { get; set; }
    public Vehicle Vehicle { get; set; }
    public ServiceType ServiceType { get; set; }
    
    public static Service Create(Guid id, string title, decimal cost, Vehicle vehicle, ServiceType serviceType)
    {
        return new Service(id, title, cost, vehicle, serviceType);
    }
}