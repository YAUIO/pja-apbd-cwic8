namespace pja_apbd_cwic8.DTOs;

public class TripClientGet
{
    public int Id { set; get; }
    public string Name { set; get; }
    public string Description { set; get; }
    public DateTime DateFrom { set; get; }
    public DateTime DateTo { set; get; }
    public int MaxPeople { set; get; }
    public int RegisteredAt { set; get; }
    public int? PaymentDate { set; get; }
}