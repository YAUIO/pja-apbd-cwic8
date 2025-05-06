namespace pja_apbd_cwic8.DTOs;

public class TripGet
{
    public int Id { set; get; }
    public string Name { set; get; }
    public string Description { set; get; }
    public DateTime DateFrom { set; get; }
    public DateTime DateTo { set; get; }
    public int MaxPeople { set; get; }
}