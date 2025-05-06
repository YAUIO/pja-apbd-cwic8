namespace pja_apbd_cwic8.Models;

public class Trip
{
    public int Id { set; get; }
    public string Name { set; get; }
    public string Description { set; get; }
    public DateTime DateFrom { set; get; }
    public DateTime DateTo { set; get; }
    public int MaxPeople { set; get; }
    public List<Country> ToCountry { set; get; }
}