﻿using CSharpFunctionalExtensions;
using SharedKernel.Common;
using TasAndJet.Api.Entities.Orders;

namespace TasAndJet.Api.Entities.Account;

public class User 
{
    private User()
    {
    }
    private User(
        Guid id, 
        string firstName, 
        string lastName,
        string email,
        string passwordHash,
        string phoneNumber, 
        string region,
        string address, 
        Role role)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        PhoneNumber = phoneNumber;
        Region = region;
        Address = address;
        Role = role;
    } 

    public Guid Id { get; set; }
    public string FirstName { get; set; } 
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string PhoneNumber { get; set; }
    public string Region { get; set; }
    public string Address { get; set; }
    public Role Role { get; set; }
    
    private readonly List<Order> _clientOrders = [];
    private readonly List<Order> _driverOrders = [];
    public IReadOnlyCollection<Order> ClientOrders => _clientOrders.AsReadOnly();
    public IReadOnlyCollection<Order> DriverOrders => _driverOrders.AsReadOnly();
    public static User CreateUser(
        Guid id,
        string firstName, 
        string lastName,
        string email,
        string password,
        string phoneNumber,
        string region,
        string address,
        Role role)
    {
        
        return new User(id, firstName, lastName, email, password, phoneNumber, region, address, role);
    }

    public void AddClientOrder(Order order)
    {
        _clientOrders.Add(order);
    }

    public void AddDriverOrder(Order order)
    {
        _driverOrders.Add(order);
    }
}