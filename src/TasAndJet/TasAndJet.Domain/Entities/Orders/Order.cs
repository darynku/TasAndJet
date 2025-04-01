using System.ComponentModel.DataAnnotations.Schema;
using TasAndJet.Domain.Entities.Account;
using TasAndJet.Domain.Entities.Enums;
using TasAndJet.Domain.Entities.Reviews;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Domain.Entities.Orders;

public class Order
{
    private Order() { }

    private Order(
        Guid id,
        Guid clientId,
        string description,
        string? pickupAddress,
        string? destinationAddress,
        DateTime orderDate,
        OrderStatus orderStatus,
        VehicleType vehicleType,
        KazakhstanCity city,
        OrderType orderType,
        string region,
        decimal totalPrice,
        DateTime? rentalStartDate,
        DateTime? rentalEndDate,
        decimal? cargoWeight,
        string? cargoType)
    {
        Id = id;
        ClientId = clientId;
        Description = description;
        PickupAddress = pickupAddress;
        DestinationAddress = destinationAddress;
        OrderDate = orderDate;
        Status = orderStatus;
        VehicleType = vehicleType;
        City = city;
        OrderType = orderType;
        Region = region;
        TotalPrice = totalPrice;

        RentalStartDate = rentalStartDate;
        RentalEndDate = rentalEndDate;
        CargoWeight = cargoWeight;
        CargoType = cargoType;
    }

    public Guid Id { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid? DriverId { get; private set; }

    public User Client { get; private set; } 
    public User? Driver { get; private set; }

    public string Description { get; private set; }
    public string? PickupAddress { get; private set; }
    public string? DestinationAddress { get; private set; }

    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public OrderType OrderType { get; private set; }
    public VehicleType VehicleType { get; private set; }
    public string Region { get; private set; }
    public decimal TotalPrice { get; private set; }

    public KazakhstanCity City { get; private set; }

    public Review? Review { get; private set; }

    // Freight-specific
    public decimal? CargoWeight { get; private set; }
    public string? CargoType { get; private set; }

    // Rental-specific
    public DateTime? RentalStartDate { get; private set; }
    public DateTime? RentalEndDate { get; private set; }
    
    public List<string> ImageKeys { get; private set; } = [];
    
    
    // ========== Фабрики ==========
    public static Order CreateFreightOrder(
        Guid id,
        Guid clientId,
        string description,
        string pickupAddress,
        string destinationAddress,
        decimal cargoWeight,
        string cargoType,
        decimal totalPrice,
        VehicleType vehicleType,
        KazakhstanCity city,
        string region)
    {
        return new Order(
            id,
            clientId,
            description,
            pickupAddress,
            destinationAddress,
            DateTime.UtcNow,
            OrderStatus.Created,
            vehicleType,
            city,
            OrderType.Freight,
            region,
            totalPrice,
            null,
            null,
            cargoWeight,
            cargoType
        );
    }

    public static Order CreateRentalOrder(
        Guid id,
        Guid clientId,
        string description,
        DateTime rentalStartDate,
        DateTime rentalEndDate,
        decimal totalPrice,
        VehicleType vehicleType,
        KazakhstanCity city,
        string region)
    {
        return new Order(
            id,
            clientId,
            description,
            null,
            null,
            DateTime.UtcNow,
            OrderStatus.Created,
            vehicleType,
            city,
            OrderType.Rental,
            region,
            totalPrice,
            rentalStartDate,
            rentalEndDate,
            null,
            null
        );
    }

    // ========== Методы состояния ==========
    public void AddReview(Review review) => Review = review;

    public void AssignDriver(Guid clientId, Guid driverId)
    {
        if (Status != OrderStatus.Created)
            throw new InvalidOperationException("Водителя можно назначить только на новый заказ.");

        ClientId = clientId;
        DriverId = driverId;
        Status = OrderStatus.Assigned;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Assigned)
            throw new InvalidOperationException("Заказ можно подтвердить только после назначения водителя.");

        Status = OrderStatus.Confirmed;
    }

    public void Complete()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Заказ можно завершить только после подтверждения.");

        Status = OrderStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("Нельзя отменить завершённый заказ.");

        Status = OrderStatus.Canceled;
    }

    // ========== Защищённый доступ к данным ==========
    public (string Pickup, string Destination, decimal Weight, string Type) GetFreightInfo()
    {
        if (OrderType != OrderType.Freight)
            throw new InvalidOperationException("Этот заказ не является грузоперевозкой");

        return (PickupAddress!, DestinationAddress!, CargoWeight!.Value, CargoType!);
    }

    public (DateTime Start, DateTime End) GetRentalInfo()
    {
        if (OrderType != OrderType.Rental)
            throw new InvalidOperationException("Этот заказ не является арендой");

        return (RentalStartDate!.Value, RentalEndDate!.Value);
    }
    
    public void AddImageKey(string key)
    {
        if (!ImageKeys.Contains(key))
            ImageKeys.Add(key);
    }

}
