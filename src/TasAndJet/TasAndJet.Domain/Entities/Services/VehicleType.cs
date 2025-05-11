namespace TasAndJet.Domain.Entities.Services;

public enum VehicleType
{
    // Легковые автомобили
    Towtruck = 1, // Экакуатор

    // Грузовой транспорт
    Truck = 2, // Грузовик
    Pickup = 3, // Пикап
    Van = 4, // Фургон

    // Спецтехника
    Excavator = 10, // Экскаватор
    Bulldozer = 11, // Бульдозер
    Crane = 12, // Кран
    Forklift = 13, // Погрузчик
    ConcreteMixer = 14, // Бетономешалка
    DumpTruck = 15, // Самосвал
    RoadRoller = 16, // Каток
    Grader = 17, // Грейдер
    Tractor = 18, // Трактор
}
